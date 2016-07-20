using Kinematics;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for Kinematics.xaml
    /// </summary>
    public partial class IKSolver : PluginBase
    {
        public ObservableCollection<Type> KinematicTypes { get; set; }
        public Thread solverThread;
        private Kinematic model;

        private Type selectedKinematic;
        public Type SelectedKinematic
        {
            get
            {
                return selectedKinematic;
            }
            set
            {
                selectedKinematic = value;
                LoadModel();
            }
        }

        public bool InvertX { get; set; }
        public bool InvertY { get; set; }
        public bool InvertZ { get; set; }

        private double x, y, z, roll, pitch, yaw;
        private double ix, iy, iz, iroll, ipitch, iyaw;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["X"].UniqueID, (message) =>
            {
                x = message.Value;
                if (!solverThread.IsAlive && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    solverThread = new Thread(new ThreadStart(UpdateOutput));
                    solverThread.Start();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Y"].UniqueID, (message) =>
            {
                y = message.Value;
                if (!solverThread.IsAlive && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    solverThread = new Thread(new ThreadStart(UpdateOutput));
                    solverThread.Start();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Z"].UniqueID, (message) =>
            {
                z = message.Value;
                if (!solverThread.IsAlive && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    solverThread = new Thread(new ThreadStart(UpdateOutput));
                    solverThread.Start();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Roll"].UniqueID, (message) =>
            {
                roll = message.Value;
                if (!solverThread.IsAlive && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    solverThread = new Thread(new ThreadStart(UpdateOutput));
                    solverThread.Start();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Pitch"].UniqueID, (message) =>
            {
                pitch = message.Value;
                if (!solverThread.IsAlive && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    solverThread = new Thread(new ThreadStart(UpdateOutput));
                    solverThread.Start();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Yaw"].UniqueID, (message) =>
            {
                yaw = message.Value;
                if (!solverThread.IsAlive && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    solverThread = new Thread(new ThreadStart(UpdateOutput));
                    solverThread.Start();
                }
            });

            base.PostLoadSetup();
        }

        public IKSolver()
        {
            TypeName = "IK Solver";

            var ListOfKinematicModels = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                         from lType in lAssembly.GetTypes()
                                         where typeof(Kinematic).IsAssignableFrom(lType)
                                         select lType).ToArray();

            while (ListOfKinematicModels.Length == 0)
            {
                ListOfKinematicModels = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                         from lType in lAssembly.GetTypes()
                                         where typeof(Kinematic).IsAssignableFrom(lType)
                                         select lType).ToArray();
            }

            KinematicTypes = new ObservableCollection<Type>(ListOfKinematicModels);

            // get directory where robot models are located
            string currentDir = Directory.GetCurrentDirectory();
            DirectoryInfo dirInfo = Directory.GetParent(currentDir);
            dirInfo = Directory.GetParent(dirInfo.FullName);
            dirInfo = Directory.GetParent(dirInfo.FullName);
            FileInfo[] robotInfoList;
            ObservableCollection<string> robotNames;
            // get names and trim of the file extension
            string robotDir = dirInfo.FullName + "\\Kinematics\\Robots\\IKSolver";
            robotInfoList = new DirectoryInfo(robotDir).GetFiles("*.cs");
            robotNames = new ObservableCollection<string>();
            foreach (var robotName in robotInfoList)
            {
                robotNames.Add(robotName.Name.TrimEnd('.', 'c', 's'));
            }
            // remove kinematic models from list
            ObservableCollection<Type> dummyTypes = new ObservableCollection<Type>(ListOfKinematicModels);
            string dummyName;
            bool isRobot = false;
            foreach (var kineType in dummyTypes)
            {
                dummyName = kineType.FullName;
                dummyName = dummyName.Remove(0, 11);
                if (dummyName[0] == ('R') && dummyName.Contains('.'))
                    dummyName = dummyName.Remove(0, 7);
                foreach (var robotName in robotNames)
                {
                    if (dummyName == robotName)
                    {
                        isRobot = true;
                        break;
                    }
                }
                if (!isRobot)
                    KinematicTypes.Remove(kineType);
                isRobot = false;
            }
            
            Inputs.Add("X", new ViewModel.InputSignalViewModel("X", this.InstanceName));
            Inputs.Add("Y", new ViewModel.InputSignalViewModel("Y", this.InstanceName));
            Inputs.Add("Z", new ViewModel.InputSignalViewModel("Z", this.InstanceName));
            Inputs.Add("Pitch", new ViewModel.InputSignalViewModel("Pitch", this.InstanceName));
            Inputs.Add("Yaw", new ViewModel.InputSignalViewModel("Yaw", this.InstanceName));
            Inputs.Add("Roll", new ViewModel.InputSignalViewModel("Roll", this.InstanceName));

            InitializeComponent();
            solverThread = new Thread(new ThreadStart(UpdateOutput));

            PostLoadSetup();
        }

        public void UpdateOutput()
        {
            // Only update the output if we've set our kinematic model
            if (model == null)
                return;
            Point3D point = new Point3D();
            point.X = InvertX ? -ix : ix;
            point.Y = InvertY ? -iy : iy;
            point.Z = InvertZ ? -iz : iz;
            Point3D orient = new Point3D();
            orient.X = iroll;
            orient.Y = ipitch;
            orient.Z = iyaw;
            double[] angles = model.GetJointAngles(point, orient);

            for (int i = 0; i < angles.Length; i++)
            {
                Outputs[model.OutputNames[i]].Value = angles[i];
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void LoadModel()
        {
            model = (Kinematic)Activator.CreateInstance(selectedKinematic);
            foreach (string output in model.OutputNames)
            {
                if (!Outputs.ContainsKey(output))
                    Outputs.Add(output, new ViewModel.OutputSignalViewModel(output));
            }
        }
    }
}

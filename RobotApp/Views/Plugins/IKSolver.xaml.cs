using Kinematics;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for Kinematics.xaml
    /// </summary>
    public partial class IKSolver : PluginBase
    {
        public ObservableCollection<Type> KinematicTypes { get; set; }
        private Kinematic model;

        public BackgroundWorker workerThread;

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
        private double[,] rotm = new double[3, 3];
        private double[,] irotm = new double[3, 3];

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["X"].UniqueID, (message) =>
            {
                x = message.Value;

                if (!workerThread.IsBusy && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    irotm = rotm;
                    workerThread.RunWorkerAsync();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Y"].UniqueID, (message) =>
            {
                y = message.Value;

                if (!workerThread.IsBusy && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    irotm = rotm;
                    workerThread.RunWorkerAsync();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Z"].UniqueID, (message) =>
            {
                z = message.Value;

                if (!workerThread.IsBusy && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    irotm = rotm;
                    workerThread.RunWorkerAsync();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Roll"].UniqueID, (message) =>
            {
                roll = message.Value;

                if (!workerThread.IsBusy && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    irotm = rotm;
                    workerThread.RunWorkerAsync();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Pitch"].UniqueID, (message) =>
            {
                pitch = message.Value;

                if (!workerThread.IsBusy && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    irotm = rotm;
                    workerThread.RunWorkerAsync();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Yaw"].UniqueID, (message) =>
            {
                yaw = message.Value;

                if (!workerThread.IsBusy && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    irotm = rotm;
                    workerThread.RunWorkerAsync();
                }
            }); 
            
            Messenger.Default.Register<Messages.Signal>(this, Inputs["R00"].UniqueID, (message) =>
            {
                rotm[0,0] = message.Value;

                if (!workerThread.IsBusy && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    irotm = rotm;
                    workerThread.RunWorkerAsync();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R01"].UniqueID, (message) =>
            {
                rotm[0, 1] = message.Value;

                if (!workerThread.IsBusy && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    irotm = rotm;
                    workerThread.RunWorkerAsync();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R02"].UniqueID, (message) =>
            {
                rotm[0, 2] = message.Value;

                if (!workerThread.IsBusy && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    irotm = rotm;
                    workerThread.RunWorkerAsync();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R10"].UniqueID, (message) =>
            {
                rotm[1, 0] = message.Value;

                if (!workerThread.IsBusy && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    irotm = rotm;
                    workerThread.RunWorkerAsync();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R11"].UniqueID, (message) =>
            {
                rotm[1, 1] = message.Value;

                if (!workerThread.IsBusy && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    irotm = rotm;
                    workerThread.RunWorkerAsync();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R12"].UniqueID, (message) =>
            {
                rotm[1, 2] = message.Value;

                if (!workerThread.IsBusy && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    irotm = rotm;
                    workerThread.RunWorkerAsync();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R20"].UniqueID, (message) =>
            {
                rotm[2, 0] = message.Value;
                
                if (!workerThread.IsBusy && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    irotm = rotm;
                    workerThread.RunWorkerAsync();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R21"].UniqueID, (message) =>
            {
                rotm[2, 1] = message.Value;

                if (!workerThread.IsBusy && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    irotm = rotm;
                    workerThread.RunWorkerAsync();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R22"].UniqueID, (message) =>
            {
                rotm[2, 2] = message.Value;

                if (!workerThread.IsBusy && model != null)
                {
                    ix = x;
                    iy = y;
                    iz = z;
                    ipitch = pitch;
                    iroll = roll;
                    iyaw = yaw;
                    irotm = rotm;
                    workerThread.RunWorkerAsync();
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
            Inputs.Add("R00", new ViewModel.InputSignalViewModel("R00", this.InstanceName));
            Inputs.Add("R01", new ViewModel.InputSignalViewModel("R01", this.InstanceName));
            Inputs.Add("R02", new ViewModel.InputSignalViewModel("R02", this.InstanceName));
            Inputs.Add("R10", new ViewModel.InputSignalViewModel("R10", this.InstanceName));
            Inputs.Add("R11", new ViewModel.InputSignalViewModel("R11", this.InstanceName));
            Inputs.Add("R12", new ViewModel.InputSignalViewModel("R12", this.InstanceName));
            Inputs.Add("R20", new ViewModel.InputSignalViewModel("R20", this.InstanceName));
            Inputs.Add("R21", new ViewModel.InputSignalViewModel("R21", this.InstanceName));
            Inputs.Add("R22", new ViewModel.InputSignalViewModel("R22", this.InstanceName));

            InitializeComponent();
            workerThread = new BackgroundWorker();
            workerThread.DoWork += workerThread_DoWork;

            PostLoadSetup();
        }

        void workerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            UpdateOutput();
        }

        public void UpdateOutput()
        {
            // Only update the output if we've set our kinematic model
            if (model == null)
                return;
            Vector3D point = new Vector3D();
            point.X = InvertX ? -(float)ix : (float)ix;
            point.Y = InvertY ? -(float)iy : (float)iy;
            point.Z = InvertZ ? -(float)iz : (float)iz;
            Vector3D orient = new Vector3D();
            orient.X = (float)iroll;
            orient.Y = (float)ipitch;
            orient.Z = (float)iyaw;
            float[,] frotm = new float[3, 3];
            frotm[0, 0] = (float)irotm[0, 0];
            frotm[0, 1] = (float)irotm[0, 1];
            frotm[0, 2] = (float)irotm[0, 2];
            frotm[1, 0] = (float)irotm[1, 0];
            frotm[1, 1] = (float)irotm[1, 1];
            frotm[1, 2] = (float)irotm[1, 2];
            frotm[2, 0] = (float)irotm[2, 0];
            frotm[2, 1] = (float)irotm[2, 1];
            frotm[2, 2] = (float)irotm[2, 2];
            //double[] angles = model.GetJointAngles(point, orient);
            //double[] angles = model.GetJointAngles(point, orient, frotm);
            double[] angles = model.GetJointAngles(point, orient, irotm);
            RobotApp.App.Current.Dispatcher.BeginInvoke((Action)delegate() 
            {
                for (int i = 0; i < angles.Length; i++)
                {
                    Outputs[model.OutputNames[i]].Value = angles[i];
                }
            });
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

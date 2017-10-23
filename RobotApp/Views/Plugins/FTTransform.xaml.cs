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
    public partial class FTTransform : PluginBase
    {
        private double[,] DHParameters;

        private Matrix4x4[] Transformations;

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

        private double fx, fy, fz, tx, ty, tz;
        

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Fx"].UniqueID, (message) =>
            {
                fx = message.Value;

            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Fy"].UniqueID, (message) =>
            {
                fy = message.Value;

            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Fz"].UniqueID, (message) =>
            {
                fz = message.Value;
              
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Tx"].UniqueID, (message) =>
            {
                tx = message.Value;

            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Ty"].UniqueID, (message) =>
            {
                ty = message.Value;

            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Tz"].UniqueID, (message) =>
            {
                tz = message.Value;

            });                 

            base.PostLoadSetup();
        }

        public FTTransform()
        {
            TypeName = "Force Torque Transform";

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
            
            Inputs.Add("Fx", new ViewModel.InputSignalViewModel("R10", this.InstanceName));
            Inputs.Add("Fy", new ViewModel.InputSignalViewModel("R11", this.InstanceName));
            Inputs.Add("Fz", new ViewModel.InputSignalViewModel("R12", this.InstanceName));
            Inputs.Add("Tx", new ViewModel.InputSignalViewModel("R20", this.InstanceName));
            Inputs.Add("Ty", new ViewModel.InputSignalViewModel("R21", this.InstanceName));
            Inputs.Add("Tz", new ViewModel.InputSignalViewModel("R22", this.InstanceName));

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
            
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void LoadModel()
        {
            model = (Kinematic)Activator.CreateInstance(selectedKinematic);
            DHParameters = model.JointParams;
            Transformations = new Matrix4x4[DHParameters.GetLength(0)];

            double alphai, ai, di, thetai;

            for(int i =0; i < DHParameters.GetLength(0); i++) //loop through rows
            {
                    alphai = DHParameters[i, 0];
                    ai = DHParameters[i, 1];
                    di = DHParameters[i, 2];
                    thetai = DHParameters[i, 3];

                    //DH parameters alpha, a, d, theta, joint type
                    Transformations[i] = new Matrix4x4((float)Math.Cos(thetai * Math.PI/180), (float)(-1 * Math.Sin(thetai * Math.PI / 180)* Math.Cos(alphai * Math.PI / 180)), //0.5
                        (float)(Math.Sin(thetai * Math.PI / 180) * Math.Sin(alphai * Math.PI / 180)), (float)(ai * Math.Cos(thetai * Math.PI / 180)), //1
                        (float)Math.Sin(thetai * Math.PI / 180), (float)(Math.Cos(thetai * Math.PI / 180) * Math.Cos(alphai * Math.PI / 180)), //1.5
                        (float)(-1 * Math.Cos(thetai * Math.PI / 180) * Math.Sin(alphai * Math.PI / 180)), (float)(ai * Math.Sin(thetai * Math.PI / 180)), //2.0
                        0, (float)Math.Sin(alphai * Math.PI / 180), (float)Math.Cos(alphai * Math.PI / 180), (float)di, //3
                        0, 0, 0, 1); //4
            }

            Console.WriteLine(Transformations[3].ToString());
            Matrix4x4 test = Matrix4x4.Transpose(Transformations[3]);
            Console.WriteLine(test.ToString());
            Console.WriteLine(Matrix4x4.Multiply(Transformations[3], test));
        }
    }
}

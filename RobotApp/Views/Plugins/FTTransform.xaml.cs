using Kinematics;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
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

        //private Matrix4x4[] Transformations;
        private Matrix<float> [] Transformations;
        
        //private Matrix4x4 BaseTransform;
        private Matrix<float> BaseForceTransform = Matrix<float>.Build.Dense(6, 6);



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

        public bool LHR { get; set; }

        private double fx, fy, fz, tx, ty, tz, ja1, ja2, ja3, ja4;
        private double[] JointAnlges;

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

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint1"].UniqueID, (message) =>
            {
                ja1 = message.Value;

            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint2"].UniqueID, (message) =>
            {
                ja2 = message.Value;

            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint3"].UniqueID, (message) =>
            {
                ja3 = message.Value;

            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint4"].UniqueID, (message) =>
            {
                ja4 = message.Value;

            });

            JointAnlges = new double[] { ja1, ja2, ja3, ja4 };

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
            
            Inputs.Add("Fx", new ViewModel.InputSignalViewModel("Fx", this.InstanceName));
            Inputs.Add("Fy", new ViewModel.InputSignalViewModel("Fy", this.InstanceName));
            Inputs.Add("Fz", new ViewModel.InputSignalViewModel("Fz", this.InstanceName));
            Inputs.Add("Tx", new ViewModel.InputSignalViewModel("Tx", this.InstanceName));
            Inputs.Add("Ty", new ViewModel.InputSignalViewModel("Ty", this.InstanceName));
            Inputs.Add("Tz", new ViewModel.InputSignalViewModel("Tz", this.InstanceName));
            Inputs.Add("Joint1", new ViewModel.InputSignalViewModel("Joint1", this.InstanceName));
            Inputs.Add("Joint2", new ViewModel.InputSignalViewModel("Joint2", this.InstanceName));
            Inputs.Add("Joint3", new ViewModel.InputSignalViewModel("Joint3", this.InstanceName));
            Inputs.Add("Joint4", new ViewModel.InputSignalViewModel("Joint4", this.InstanceName));


            InitializeComponent();
            workerThread = new BackgroundWorker();
            workerThread.DoWork += workerThread_DoWork;

            PostLoadSetup();
        }

        //Calculates coordinate transform matrix to transform from endpoint frame to base frame
        public void CalculateTransformations()
        {

            Transformations = new Matrix<float>[DHParameters.GetLength(0)];
            Matrix<float> BaseTransform = Matrix<float>.Build.Dense(4, 4);
            Matrix<float> BaseForceTransform = Matrix<float>.Build.DenseIdentity(6);
            Matrix<float> BaseRotation = Matrix<float>.Build.Dense(3, 3);
            Matrix<float> BasePosition = Matrix<float>.Build.Dense(3,3);
            Matrix<float> ToolSensorRotation = Matrix<float>.Build.Dense(3, 3);
            Matrix<float> ToolSensorTransform;
            Matrix<float> ToolSensorPosition;
            Vector<float> ForceTorque;

            double alphai, ai, di, thetai, jai;

            for (int i = 0; i < DHParameters.GetLength(0); i++) //loop through rows
            {
                alphai = DHParameters[i, 0];
                ai = DHParameters[i, 1];
                di = DHParameters[i, 2];
                thetai = DHParameters[i, 3];
                jai = JointAnlges[i];

                //DH parameters alpha, a, d, theta, joint type

                float[,] t = { { (float)Math.Cos((thetai + jai) * Math.PI / 180), (float)(-1 * Math.Sin((thetai + jai) * Math.PI / 180) * Math.Cos(alphai * Math.PI / 180)), //0.5
                    (float)(Math.Sin((thetai + jai) * Math.PI / 180) * Math.Sin(alphai * Math.PI / 180)), (float)(ai * Math.Cos((thetai + jai) * Math.PI / 180))}, //1
                    { (float)Math.Sin((thetai + jai) * Math.PI / 180), (float)(Math.Cos((thetai + jai) * Math.PI / 180) * Math.Cos(alphai * Math.PI / 180)), //1.5
                    (float)(-1 * Math.Cos((thetai + jai) * Math.PI / 180) * Math.Sin(alphai * Math.PI / 180)), (float)(ai * Math.Sin((thetai + jai) * Math.PI / 180))}, //2.0
                    { 0, (float)Math.Sin(alphai * Math.PI / 180), (float)Math.Cos(alphai * Math.PI / 180), (float)di}, //3
                    { 0, 0, 0, 1} }; //4

                Transformations[i] = Matrix<float>.Build.DenseOfArray(t);
                Transformations[i].CoerceZero(1e-14);
            }

            BaseTransform = Transformations[0];
            for (int i = 1; i < Transformations.Length; i++)
            {
                BaseTransform = BaseTransform.Multiply(Transformations[i]);
            }

            //Creation of transformation matrix to transform wrist torques into base frame
            BaseRotation = BaseTransform.SubMatrix(0, 2, 0, 2);

            var bp = BaseTransform.RemoveRow(3).Column(3);

            float[,] bst = {{0, -bp[2], bp[1] },
                            { bp[2], 0, -bp[0] },
                             {-bp[1], bp[0], 0 }};

            BasePosition = Matrix<float>.Build.DenseOfArray(bst);
            BasePosition = BasePosition.Multiply(BaseRotation);


            /*[ R wrist to base         0]
             *[ P wtb * R wtb       R wtb]
             * */
            BaseForceTransform = BaseRotation.Stack(BasePosition);
            BaseForceTransform = BaseForceTransform.Append(Matrix<float>.Build.Dense(3, 3).Stack(BaseRotation));
            
            //Creation of transformation matrix to transform sensor forces into wrist frame
            //ToolSensorRotation = Transformations[3].SubMatrix(0, 2, 0, 2);  //Assume sensor and wrist axises are aligned

            var tsp = Transformations[3].RemoveRow(3).Column(3);

			float[,] tst = {{0, tsp[2], -tsp[1] },
			                { -tsp[2], 0, tsp[0] },
			                 {tsp[1], -tsp[0], 0 }};

            ToolSensorPosition = Matrix<float>.Build.DenseOfArray(tst);


            /*[I        0]
             *[ P stw   I]
             * */
            ToolSensorTransform = Matrix<float>.Build.DenseIdentity(3).Stack(ToolSensorPosition);
            ToolSensorTransform = ToolSensorTransform.Append(Matrix<float>.Build.Dense(3, 3).Stack(Matrix<float>.Build.DenseIdentity(3)));

            float[] f = { (float)fx, (float)fy, (float)fz, (float)tx, (float)ty, (float)tz };
            ForceTorque = Vector<float>.Build.DenseOfArray(f);

            //Final Force torque transformation from sensor frame to base frame
            ForceTorque = BaseTransform.Multiply(ToolSensorTransform.Multiply(ForceTorque));

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

            /*
            foreach(var name in model.OutputNames)
            {
                if(name.Contains("Joint"))
                {
                    Inputs.Add(name, new ViewModel.InputSignalViewModel(name, this.InstanceName));
                }
            }
            */

            DHParameters = model.JointParams;

            CalculateTransformations();
            
        }
    }
}

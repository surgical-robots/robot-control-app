using Kinematics;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using RobotApp.ViewModel;
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
        private double ifx, ify, ifz, itx, ity, itz, ija1, ija2, ija3, ija4;
        private double[] JointAnlges;

        private double forceConversionFactor = 4.4482;      // convert lbf to N
        private double torqueConversionFactor = 112.98;     // convert lbf-in to N-mm
        private double armWeight = .24516;                    // Newtons

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Fx"].UniqueID, (message) =>
            {
                ifx = message.Value;
                //if (!workerThread.IsBusy && model != null)
                //{
                //    fx = ifx;
                //    fy = ify;
                //    fz = ifz;
                //    tx = itx;
                //    ty = ity;
                //    tz = itz;
                //    ja1 = ija1;
                //    ja2 = ija2;
                //    ja3 = ija3;
                //    ja4 = ija4;
                //    workerThread.RunWorkerAsync();
                //}
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Fy"].UniqueID, (message) =>
            {
                ify = message.Value;
                //if (!workerThread.IsBusy && model != null)
                //{
                //    fx = ifx;
                //    fy = ify;
                //    fz = ifz;
                //    tx = itx;
                //    ty = ity;
                //    tz = itz;
                //    ja1 = ija1;
                //    ja2 = ija2;
                //    ja3 = ija3;
                //    ja4 = ija4;
                //    workerThread.RunWorkerAsync();
                //}
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Fz"].UniqueID, (message) =>
            {
                ifz = message.Value;
                //if (!workerThread.IsBusy && model != null)
                //{
                //    fx = ifx;
                //    fy = ify;
                //    fz = ifz;
                //    tx = itx;
                //    ty = ity;
                //    tz = itz;
                //    ja1 = ija1;
                //    ja2 = ija2;
                //    ja3 = ija3;
                //    ja4 = ija4;
                //    workerThread.RunWorkerAsync();
                //}
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Tx"].UniqueID, (message) =>
            {
                itx = message.Value;
                //if (!workerThread.IsBusy && model != null)
                //{
                //    fx = ifx;
                //    fy = ify;
                //    fz = ifz;
                //    tx = itx;
                //    ty = ity;
                //    tz = itz;
                //    ja1 = ija1;
                //    ja2 = ija2;
                //    ja3 = ija3;
                //    ja4 = ija4;
                //    workerThread.RunWorkerAsync();
                //}
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Ty"].UniqueID, (message) =>
            {
                ity = message.Value;
                //if (!workerThread.IsBusy && model != null)
                //{
                //    fx = ifx;
                //    fy = ify;
                //    fz = ifz;
                //    tx = itx;
                //    ty = ity;
                //    tz = itz;
                //    ja1 = ija1;
                //    ja2 = ija2;
                //    ja3 = ija3;
                //    ja4 = ija4;
                //    workerThread.RunWorkerAsync();
                //}
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Tz"].UniqueID, (message) =>
            {
                itz = message.Value;
                //if (!workerThread.IsBusy && model != null)
                //{
                //    fx = ifx;
                //    fy = ify;
                //    fz = ifz;
                //    tx = itx;
                //    ty = ity;
                //    tz = itz;
                //    ja1 = ija1;
                //    ja2 = ija2;
                //    ja3 = ija3;
                //    ja4 = ija4;
                //    workerThread.RunWorkerAsync();
                //}
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint1"].UniqueID, (message) =>
            {
                ija1 = message.Value;
                if (!workerThread.IsBusy && model != null)
                {
                    fx = ifx;
                    fy = ify;
                    fz = ifz;
                    tx = itx;
                    ty = ity;
                    tz = itz;
                    ja1 = ija1;
                    ja2 = ija2;
                    ja3 = ija3;
                    ja4 = ija4;
                    workerThread.RunWorkerAsync();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint2"].UniqueID, (message) =>
            {
                ija2 = message.Value;
                if (!workerThread.IsBusy && model != null)
                {
                    fx = ifx;
                    fy = ify;
                    fz = ifz;
                    tx = itx;
                    ty = ity;
                    tz = itz;
                    ja1 = ija1;
                    ja2 = ija2;
                    ja3 = ija3;
                    ja4 = ija4;
                    workerThread.RunWorkerAsync();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint3"].UniqueID, (message) =>
            {
                ija3 = message.Value;
                if (!workerThread.IsBusy && model != null)
                {
                    fx = ifx;
                    fy = ify;
                    fz = ifz;
                    tx = itx;
                    ty = ity;
                    tz = itz;
                    ja1 = ija1;
                    ja2 = ija2;
                    ja3 = ija3;
                    ja4 = ija4;
                    workerThread.RunWorkerAsync();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint4"].UniqueID, (message) =>
            {
                ija4 = message.Value;
                if (!workerThread.IsBusy && model != null)
                {
                    fx = ifx;
                    fy = ify;
                    fz = ifz;
                    tx = itx;
                    ty = ity;
                    tz = itz;
                    ja1 = ija1;
                    ja2 = ija2;
                    ja3 = ija3;
                    ja4 = ija4;
                    workerThread.RunWorkerAsync();
                }
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

            Outputs.Add("Fx", new OutputSignalViewModel("Force X"));
            Outputs.Add("Fy", new OutputSignalViewModel("Force Y"));
            Outputs.Add("Fz", new OutputSignalViewModel("Force Z"));
            Outputs.Add("Tx", new OutputSignalViewModel("Torque X"));
            Outputs.Add("Ty", new OutputSignalViewModel("Torque Y"));
            Outputs.Add("Tz", new OutputSignalViewModel("Torque Z"));

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
            Matrix<float> BaseSensorTransform = Matrix<float>.Build.Dense(4, 4);
            Matrix<float> BaseSensorRotation = Matrix<float>.Build.Dense(3, 3);
            Matrix<float> BaseForceTransform = Matrix<float>.Build.DenseIdentity(6);
            Matrix<float> BaseRotation = Matrix<float>.Build.Dense(3, 3);
            Matrix<float> BasePosition = Matrix<float>.Build.Dense(3, 3);
            Matrix<float> ToolSensorRotation = Matrix<float>.Build.Dense(3, 3);
            Matrix<float> ToolSensorTransform;
            Matrix<float> ToolSensorPosition;
            Vector<float> ForceTorque;
            Vector<float> sensorArmForce;
            Vector<float> armLength;
            Vector<float> sensorArmTorque;


            JointAnlges = new double[] {0, ja1, ja2, ja3, ja4 };

            double alphai, ai, di, thetai, jai;

            for (int i = 0; i < DHParameters.GetLength(0); i++) // loop through rows
            {
                alphai = DHParameters[i, 0];
                ai = DHParameters[i, 1];
                di = DHParameters[i, 2];
                thetai = DHParameters[i, 3];

                if (i < 5)
                {
                    jai = JointAnlges[i];
                } else
                {
                    jai = 0;
                }

                // DH parameters alpha, a, d, theta, joint type

                float[,] t = { { (float)Math.Cos((thetai + jai) * Math.PI / 180), (float)(-1 * Math.Sin((thetai + jai) * Math.PI / 180) * Math.Cos(alphai * Math.PI / 180)), //0.5
                    (float)(Math.Sin((thetai + jai) * Math.PI / 180) * Math.Sin(alphai * Math.PI / 180)), (float)(ai * Math.Cos((thetai + jai) * Math.PI / 180))}, //1
                    { (float)Math.Sin((thetai + jai) * Math.PI / 180), (float)(Math.Cos((thetai + jai) * Math.PI / 180) * Math.Cos(alphai * Math.PI / 180)), //1.5
                    (float)(-1 * Math.Cos((thetai + jai) * Math.PI / 180) * Math.Sin(alphai * Math.PI / 180)), (float)(ai * Math.Sin((thetai + jai) * Math.PI / 180))}, //2.0
                    { 0, (float)Math.Sin(alphai * Math.PI / 180), (float)Math.Cos(alphai * Math.PI / 180), (float)di}, //3
                    { 0, 0, 0, 1} }; //4

                Transformations[i] = Matrix<float>.Build.DenseOfArray(t);
                // Transformations[i].CoerceZero(1e-14);
            }

            BaseTransform = Transformations[0];
            for (int i = 1; i < Transformations.Length; i++)
            {
                BaseTransform = BaseTransform.Multiply(Transformations[i]);

                if (i == 4)
                {
                    BaseSensorTransform = BaseTransform;
                }
            }

            //Creation of transformation matrix to transform tool torques into base frame
            BaseRotation = BaseTransform.SubMatrix(0, 3, 0, 3);
            BaseSensorRotation = BaseSensorTransform.SubMatrix(0, 3, 0, 3);

            var bp = BaseTransform.RemoveRow(3).Column(3);

            float[,] bst = {{0, -bp[2], bp[1] },
                            { bp[2], 0, -bp[0] },
                             {-bp[1], bp[0], 0 }};

            BasePosition = Matrix<float>.Build.DenseOfArray(bst);
            BasePosition = BasePosition.Multiply(BaseRotation);


            /*[ R tool to base         0]
             *[ P ttb * R wtb       R ttb]
             * */
            BaseForceTransform = BaseRotation.Stack(BasePosition);
            BaseForceTransform = BaseForceTransform.Append(Matrix<float>.Build.Dense(3, 3).Stack(BaseRotation));

            // Creation of transformation matrix to transform sensor forces into tool frame
            ToolSensorRotation = Transformations[5].SubMatrix(0, 3, 0, 3);

            // Transform tool sensor position to tool frame
            var tsp = Transformations[5].RemoveRow(3).Column(3);
            armLength = tsp / 2;
            tsp = ToolSensorRotation.Multiply(tsp);

            // Sign inverted cross product operator
            float[,] tst = {{0, tsp[2], -tsp[1] },
			                { -tsp[2], 0, tsp[0] },
			                 {tsp[1], -tsp[0], 0 }};

            ToolSensorPosition = Matrix<float>.Build.DenseOfArray(tst);
            ToolSensorPosition = ToolSensorPosition.Multiply(ToolSensorRotation);

            /*[R sensor to tool        0]
             *[ P tts * R stt      R stt]
             */
            ToolSensorTransform = ToolSensorRotation.Stack(ToolSensorPosition);
            ToolSensorTransform = ToolSensorTransform.Append(Matrix<float>.Build.Dense(3, 3).Stack(ToolSensorRotation));

            Vector<float> gForce = Vector<float>.Build.Dense(3);


            float[] f = { (float)(fx * forceConversionFactor), (float)(fy * forceConversionFactor), (float)(fz * forceConversionFactor), (float)(tx * torqueConversionFactor)
                    , (float)(ty * torqueConversionFactor), (float)(tz * torqueConversionFactor) };
            ForceTorque = Vector<float>.Build.DenseOfArray(f);

            // Removal of the arm mass and torque from sensor readings
            float[] am = { 0, 0, (float)armWeight };
            sensorArmForce = Vector<float>.Build.DenseOfArray(am);
            sensorArmForce = BaseSensorRotation.Transpose() * sensorArmForce;

            sensorArmTorque = Vector<float>.Build.Dense(3);
            sensorArmTorque[0] = armLength[1] * sensorArmForce[2] - armLength[2] * sensorArmForce[1];
            sensorArmTorque[1] = -armLength[0] * sensorArmForce[2] + armLength[2] * sensorArmForce[0];
            sensorArmTorque[2] = armLength[0] * sensorArmForce[1] - armLength[1] * sensorArmForce[0];

            float[] ft = { sensorArmForce[0], sensorArmForce[1], sensorArmForce[2], sensorArmTorque[0], sensorArmTorque[1], sensorArmTorque[2] };

            ForceTorque = ForceTorque + Vector<float>.Build.DenseOfArray(ft);


            // sensorArmForce = BaseSensorRotation.Transpose() * 



            // Final Force torque transformation from sensor frame to base frame
            var tmp = ToolSensorTransform.Multiply(ForceTorque);

            ForceTorque = BaseForceTransform.Multiply(tmp);

            /*
            float forceGain = 4;
            for(int i = 0; i < 6; i++)
            {
                ForceTorque[i] = ForceTorque[i] * forceGain;
                if (ForceTorque[i] > 5)
                    ForceTorque[i] = 5;
            }
            */

            RobotApp.App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            {
                Outputs["Fx"].Value = -ForceTorque[0];
                Outputs["Fy"].Value = -ForceTorque[1];
                Outputs["Fz"].Value = ForceTorque[2];
                Outputs["Tx"].Value = ForceTorque[3];
                Outputs["Ty"].Value = ForceTorque[4];
                Outputs["Tz"].Value = ForceTorque[5];
            });

        }

        void workerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            CalculateTransformations();
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

            DHParameters = model.JointParms;

            CalculateTransformations();
        }
    }
}

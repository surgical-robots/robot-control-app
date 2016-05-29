using Kinematics;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["X"].UniqueID, (message) =>
            {
                x = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Y"].UniqueID, (message) =>
            {
                y = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Z"].UniqueID, (message) =>
            {
                z = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Roll"].UniqueID, (message) =>
            {
                roll = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Pitch"].UniqueID, (message) =>
            {
                pitch = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Yaw"].UniqueID, (message) =>
            {
                yaw = message.Value;
                UpdateOutput();
            });

            base.PostLoadSetup();
        }

        public IKSolver()
        {
            TypeName = "IK Solver";

            Kinematic dummyVariable;

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

            Inputs.Add("X", new ViewModel.InputSignalViewModel("X", this.InstanceName));
            Inputs.Add("Y", new ViewModel.InputSignalViewModel("Y", this.InstanceName));
            Inputs.Add("Z", new ViewModel.InputSignalViewModel("Z", this.InstanceName));
            Inputs.Add("Pitch", new ViewModel.InputSignalViewModel("Pitch", this.InstanceName));
            Inputs.Add("Yaw", new ViewModel.InputSignalViewModel("Yaw", this.InstanceName));
            Inputs.Add("Roll", new ViewModel.InputSignalViewModel("Roll", this.InstanceName));

            InitializeComponent();

            PostLoadSetup();
        }

        public void UpdateOutput()
        {
            // Only update the output if we've set our kinematic model
            if (model == null)
                return;
            Point3D point = new Point3D();
            point.X = InvertX ? -x : x;
            point.Y = InvertY ? -y : y;
            point.Z = InvertZ ? -z : z;
            Point3D orient = new Point3D();
            orient.X = roll;
            orient.Y = pitch;
            orient.Z = yaw;
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

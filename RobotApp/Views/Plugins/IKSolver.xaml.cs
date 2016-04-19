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

        public bool InvertXL { get; set; }
        public bool InvertYL { get; set; }
        public bool InvertZL { get; set; }
        public bool InvertXR { get; set; }
        public bool InvertYR { get; set; }
        public bool InvertZR { get; set; }

        private double xL, yL, zL, xR, yR, zR;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["XL"].UniqueID, (message) =>
            {
                xL = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["YL"].UniqueID, (message) =>
            {
                yL = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["ZL"].UniqueID, (message) =>
            {
                zL = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["XR"].UniqueID, (message) =>
            {
                xR = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["YR"].UniqueID, (message) =>
            {
                yR = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["ZR"].UniqueID, (message) =>
            {
                zR = message.Value;
                UpdateOutput();
            });

            base.PostLoadSetup();
        }

        public IKSolver()
        {
            TypeName = "Kinematics";

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

            Inputs.Add("XL", new ViewModel.InputSignalViewModel("XL", this.InstanceName));
            Inputs.Add("YL", new ViewModel.InputSignalViewModel("YL", this.InstanceName));
            Inputs.Add("ZL", new ViewModel.InputSignalViewModel("ZL", this.InstanceName));
            Inputs.Add("ZL", new ViewModel.InputSignalViewModel("ZL", this.InstanceName));
            Inputs.Add("ZL", new ViewModel.InputSignalViewModel("ZL", this.InstanceName));
            Inputs.Add("ZL", new ViewModel.InputSignalViewModel("ZL", this.InstanceName));
            Inputs.Add("XR", new ViewModel.InputSignalViewModel("XR", this.InstanceName));
            Inputs.Add("YR", new ViewModel.InputSignalViewModel("YR", this.InstanceName));
            Inputs.Add("ZR", new ViewModel.InputSignalViewModel("ZR", this.InstanceName));

            InitializeComponent();

            PostLoadSetup();
        }

        public void UpdateOutput()
        {
            // Only update the output if we've set our kinematic model
            if (model == null)
                return;
            Point3D pointL = new Point3D();
            pointL.X = InvertXL ? -xL : xL;
            pointL.Y = InvertYL ? -yL : yL;
            pointL.Z = InvertZL ? -zL : zL;
            Point3D pointR = new Point3D();
            pointR.X = InvertXR ? -xR : xR;
            pointR.Y = InvertYR ? -yR : yR;
            pointR.Z = InvertZR ? -zR : zR;
            double[] angles = model.GetJointAngles(pointL, pointR);

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

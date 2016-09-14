using System;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for Kinematics.xaml
    /// </summary>
    public partial class ForwardKinematics : PluginBase
    {

        public bool InvertX { get; set; }
        public bool InvertY { get; set; }
        public bool InvertZ { get; set; }

        private double x, y, z;
        private double ub, lb, el;
        private double theta1, theta2, theta3;

        private double LengthUpperArm = 68.58;
        private double LengthForearm = 96.393;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["UpperBevel"].UniqueID, (message) =>
            {
                ub = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LowerBevel"].UniqueID, (message) =>
            {
                lb = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Elbow"].UniqueID, (message) =>
            {
                el = message.Value;
                UpdateOutput();
            });

            base.PostLoadSetup();
        }

        public ForwardKinematics()
        {
            TypeName = "Forward Kinematics";

            Inputs.Add("UpperBevel", new ViewModel.InputSignalViewModel("Upper Bevel", this.InstanceName));
            Inputs.Add("LowerBevel", new ViewModel.InputSignalViewModel("Lower Bevel", this.InstanceName));
            Inputs.Add("Elbow", new ViewModel.InputSignalViewModel("Elbow", this.InstanceName));

            Outputs.Add("X", new ViewModel.OutputSignalViewModel("X"));
            Outputs.Add("Y", new ViewModel.OutputSignalViewModel("Y"));
            Outputs.Add("Z", new ViewModel.OutputSignalViewModel("Z"));

            InitializeComponent();
            PostLoadSetup();
        }

        public void UpdateOutput()
        {
            theta1 = ((ub + lb) / 2) * Math.PI / 180;
            theta2 = ((ub - lb) / 2) * Math.PI / 180;
            theta3 = el * Math.PI / 180;

            // calculate forward kinematics and haptic forces
            double kineZ = LengthUpperArm * Math.Cos(theta1) * Math.Cos(theta2) - LengthForearm * (Math.Sin(theta1) * Math.Sin(theta3) - Math.Cos(theta1) * Math.Cos(theta2) * Math.Cos(theta3));
            double kineY = LengthUpperArm * Math.Sin(theta2) + LengthForearm * Math.Sin(theta2) * Math.Cos(theta3);
            double kineX = LengthUpperArm * Math.Sin(theta1) * Math.Cos(theta2) + LengthForearm * (Math.Cos(theta1) * Math.Sin(theta3) + Math.Sin(theta1) * Math.Cos(theta2) * Math.Cos(theta3));

            Outputs["X"].Value = InvertX ? -kineX : kineX;
            Outputs["Y"].Value = InvertY ? -kineY : kineY;
            Outputs["Z"].Value = InvertZ ? -kineZ : kineZ;
        }

    }
}
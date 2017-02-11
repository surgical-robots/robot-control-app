using System;
using System.Windows;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for Clutch.xaml
    /// </summary>
    public partial class AlphaAngle : PluginBase
    {
        Vector3D Position, Rx, Ry, Rz;
        double[,] rotm = new double[3, 3];

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Alpha"].UniqueID, (message) =>
            {
                Alpha = message.Value;
                RotateAlpha();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["X"].UniqueID, (message) =>
            {
                Position.X = message.Value;
                RotateAlpha();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Y"].UniqueID, (message) =>
            {
                Position.Y = message.Value;
                RotateAlpha();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Z"].UniqueID, (message) =>
            {
                Position.Z = message.Value;
                RotateAlpha();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R00"].UniqueID, (message) =>
            {
                Rx.X = message.Value;
                RotateAlpha();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R01"].UniqueID, (message) =>
            {
                Rx.Y = message.Value;
                RotateAlpha();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R02"].UniqueID, (message) =>
            {
                Rx.Z = message.Value;
                RotateAlpha();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R10"].UniqueID, (message) =>
            {
                Ry.X = message.Value;
                RotateAlpha();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R11"].UniqueID, (message) =>
            {
                Ry.Y = message.Value;
                RotateAlpha();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R12"].UniqueID, (message) =>
            {
                Ry.Z = message.Value;
                RotateAlpha();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R20"].UniqueID, (message) =>
            {
                Rz.X = message.Value;
                RotateAlpha();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R21"].UniqueID, (message) =>
            {
                Rz.Y = message.Value;
                RotateAlpha();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["R22"].UniqueID, (message) =>
            {
                Rz.Z = message.Value;
                RotateAlpha();
            });

            base.PostLoadSetup();
        }

        public AlphaAngle()
        {
            this.TypeName = "Alpha Angle";
            InitializeComponent();

            // OUTPUTS
            Outputs.Add("X", new ViewModel.OutputSignalViewModel("X"));
            Outputs.Add("Y", new ViewModel.OutputSignalViewModel("Y"));
            Outputs.Add("Z", new ViewModel.OutputSignalViewModel("Z"));
            Outputs.Add("R00", new ViewModel.OutputSignalViewModel("R00"));
            Outputs.Add("R01", new ViewModel.OutputSignalViewModel("R01"));
            Outputs.Add("R02", new ViewModel.OutputSignalViewModel("R02"));
            Outputs.Add("R10", new ViewModel.OutputSignalViewModel("R10"));
            Outputs.Add("R11", new ViewModel.OutputSignalViewModel("R11"));
            Outputs.Add("R12", new ViewModel.OutputSignalViewModel("R12"));
            Outputs.Add("R20", new ViewModel.OutputSignalViewModel("R20"));
            Outputs.Add("R21", new ViewModel.OutputSignalViewModel("R21"));
            Outputs.Add("R22", new ViewModel.OutputSignalViewModel("R22"));

            // INPUTS
            Inputs.Add("Alpha", new ViewModel.InputSignalViewModel("Alpha", this.InstanceName));
            Inputs.Add("X", new ViewModel.InputSignalViewModel("X", this.InstanceName));
            Inputs.Add("Y", new ViewModel.InputSignalViewModel("Y", this.InstanceName));
            Inputs.Add("Z", new ViewModel.InputSignalViewModel("Z", this.InstanceName));
            Inputs.Add("R00", new ViewModel.InputSignalViewModel("R00", this.InstanceName));
            Inputs.Add("R01", new ViewModel.InputSignalViewModel("R01", this.InstanceName));
            Inputs.Add("R02", new ViewModel.InputSignalViewModel("R02", this.InstanceName));
            Inputs.Add("R10", new ViewModel.InputSignalViewModel("R10", this.InstanceName));
            Inputs.Add("R11", new ViewModel.InputSignalViewModel("R11", this.InstanceName));
            Inputs.Add("R12", new ViewModel.InputSignalViewModel("R12", this.InstanceName));
            Inputs.Add("R20", new ViewModel.InputSignalViewModel("R20", this.InstanceName));
            Inputs.Add("R21", new ViewModel.InputSignalViewModel("R21", this.InstanceName));
            Inputs.Add("R22", new ViewModel.InputSignalViewModel("R22", this.InstanceName));

            rotm.Initialize();
            rotm[1, 1] = 1;

            PostLoadSetup();
        }

        public void RotateAlpha()
        {
            rotm[0, 0] = Math.Cos((90 - Alpha) * Math.PI / 180);
            rotm[0, 2] = Math.Sin((90 - Alpha) * Math.PI / 180);
            rotm[2, 0] = -Math.Sin((90 - Alpha) * Math.PI / 180);
            rotm[2, 2] = Math.Cos((90 - Alpha) * Math.PI / 180);

            Outputs["X"].Value = rotm[0, 0] * Position.X + rotm[0, 2] * Position.Z;
            Outputs["Y"].Value = Position.Y;
            Outputs["Z"].Value = rotm[2, 0] * Position.X + rotm[2, 2] * Position.Z;

            Outputs["R00"].Value = rotm[0, 0] * Rx.X + rotm[0, 2] * Rx.Z;
            Outputs["R01"].Value = Rx.Y;
            Outputs["R02"].Value = rotm[2, 0] * Rx.X + rotm[2, 2] * Rx.Z;

            Outputs["R10"].Value = rotm[0, 0] * Ry.X + rotm[0, 2] * Ry.Z;
            Outputs["R11"].Value = Ry.Y;
            Outputs["R12"].Value = rotm[2, 0] * Ry.X + rotm[2, 2] * Ry.Z;

            Outputs["R20"].Value = rotm[0, 0] * Rz.X + rotm[0, 2] * Rz.Z;
            Outputs["R21"].Value = Rz.Y;
            Outputs["R22"].Value = rotm[2, 0] * Rz.X + rotm[2, 2] * Rz.Z;
        }

        /// <summary>
        /// The <see cref="Alpha" /> property's name.
        /// </summary>
        public const string AlphaPropertyName = "Alpha";

        private double alpha = 0;

        /// <summary>
        /// Sets and gets the Alpha property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Alpha
        {
            get
            {
                return alpha;
            }

            set
            {
                if (alpha == value)
                {
                    return;
                }

                alpha = value;
                RotateAlpha();
                RaisePropertyChanged(AlphaPropertyName);
            }
        }
    }
}

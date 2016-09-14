using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for Clutch.xaml
    /// </summary>
    public partial class Clutch : PluginBase
    {
        double x, y, z, pitch, yaw, roll;
        double offsetX, offsetY, offsetZ, offsetPitch, offsetYaw, offsetRoll;
        double clutchStartPositionX, clutchStartPositionY, clutchStartPositionZ, clutchStartPitch, clutchStartYaw, clutchStartRoll;
        bool clutchIsEnabled;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["X"].UniqueID, (message) =>
            {
                x = message.Value;
                if (!ClutchIsEnabled)
                    Outputs["X"].Value = x + offsetX;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Y"].UniqueID, (message) =>
            {
                y = message.Value;
                if (!ClutchIsEnabled)
                    Outputs["Y"].Value = y + offsetY;

            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Z"].UniqueID, (message) =>
            {
                z = message.Value;
                if (!ClutchIsEnabled)
                    Outputs["Z"].Value = z + offsetZ;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Pitch"].UniqueID, (message) =>
            {
                pitch = message.Value;
                if (!ClutchIsEnabled)
                    Outputs["Pitch"].Value = pitch + offsetPitch;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Yaw"].UniqueID, (message) =>
            {
                yaw = message.Value;
                if (!ClutchIsEnabled)
                    Outputs["Yaw"].Value = yaw + offsetYaw;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Roll"].UniqueID, (message) =>
            {
                roll = message.Value;
                if (!ClutchIsEnabled)
                    Outputs["Roll"].Value = roll + offsetRoll;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Clutch"].UniqueID, (message) =>
            {
                ClutchIsEnabled = message.Value > 0.5 ? true : false;
            });

            base.PostLoadSetup();
        }

        private RelayCommand resetOffsetsCommand;

        /// <summary>
        /// Gets the ResetOffsetsCommand.
        /// </summary>
        public RelayCommand ResetOffsetsCommand
        {
            get
            {
                return resetOffsetsCommand
                    ?? (resetOffsetsCommand = new RelayCommand(
                    () =>
                    {
                        offsetX = 0;
                        offsetY = 0;
                        offsetZ = 0;
                        offsetPitch = 0;
                        offsetYaw = 0;
                        offsetRoll = 0;
                    }));
            }
        }

        public bool ClutchIsEnabled 
        {
            get { return clutchIsEnabled; }
            set {
                if (value == clutchIsEnabled)
                    return;

                clutchIsEnabled = value;

                if(clutchIsEnabled == true)
                {
                    clutchStartPositionX = x;
                    clutchStartPositionY = y;
                    clutchStartPositionZ = z;
                    clutchStartPitch = pitch;
                    clutchStartYaw = yaw;
                    clutchStartRoll = roll;
                }
                if(clutchIsEnabled == false)
                {
                    // We transitioned from off to on. Look at the last input and set
                    // that as our offset.
                    offsetX -= x - clutchStartPositionX;
                    offsetY -= y - clutchStartPositionY;
                    offsetZ -= z - clutchStartPositionZ;
                    offsetPitch -= pitch - clutchStartPitch;
                    offsetYaw -= yaw - clutchStartYaw;
                    offsetRoll -= roll - clutchStartRoll;
                }
                this.RaisePropertyChanged("ClutchIsEnabled");
            } 
        }

        public Clutch()
        {
            this.TypeName = "Clutch";
            this.PluginInfo = "Allows user to clutch in and out of a 3D workspace. When INPUT:Clutch > 0.5, the clutch is enabled and disables the outputs. The input positions are offset to adjust the clutch-in position = clutch-out position.\n\nINPUTS: X, Y, Z, Pitch, Yaw, Roll, Clutch\nOUTPUTS: X, Y, Z, Pitch, Yaw, Roll\n";
            InitializeComponent();
            clutchIsEnabled = false;

            // OUTPUTS
            Outputs.Add("X", new ViewModel.OutputSignalViewModel("X"));
            Outputs.Add("Y", new ViewModel.OutputSignalViewModel("Y"));
            Outputs.Add("Z", new ViewModel.OutputSignalViewModel("Z"));
            Outputs.Add("Pitch", new ViewModel.OutputSignalViewModel("Pitch"));
            Outputs.Add("Yaw", new ViewModel.OutputSignalViewModel("Yaw"));
            Outputs.Add("Roll", new ViewModel.OutputSignalViewModel("Roll"));

            // INPUTS
            Inputs.Add("X", new ViewModel.InputSignalViewModel("X", this.InstanceName));
            Inputs.Add("Y", new ViewModel.InputSignalViewModel("Y", this.InstanceName));
            Inputs.Add("Z", new ViewModel.InputSignalViewModel("Z", this.InstanceName));
            Inputs.Add("Pitch", new ViewModel.InputSignalViewModel("Pitch", this.InstanceName));
            Inputs.Add("Yaw", new ViewModel.InputSignalViewModel("Yaw", this.InstanceName));
            Inputs.Add("Roll", new ViewModel.InputSignalViewModel("Roll", this.InstanceName));
            Inputs.Add("Clutch", new ViewModel.InputSignalViewModel("Clutch", this.InstanceName));

            PostLoadSetup();
        }
    }
}

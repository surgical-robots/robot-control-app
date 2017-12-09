using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for Clutch.xaml
    /// </summary>
    public partial class FrankenShoulder : PluginBase
    {
        double pitch, yaw, roll;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Yaw"].UniqueID, (message) =>
            {
                yaw = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Pitch"].UniqueID, (message) =>
            {
                pitch = message.Value;
                UpdateOutput();
            });

            //Messenger.Default.Register<Messages.Signal>(this, Inputs["Roll"].UniqueID, (message) =>
            //{
            //    roll = message.Value;
            //    UpdateOutput();
            //});

            base.PostLoadSetup();
        }

        void UpdateOutput()
        {
            //// Joey's shoulder
            //Outputs["Motor1"].Value = yaw + pitch;
            //Outputs["Motor2"].Value = roll + yaw - pitch;
            //Outputs["Motor3"].Value = yaw;

            // Franken-bot
            Outputs["Motor1"].Value = yaw;
            Outputs["Motor2"].Value = pitch + (yaw / 2.868);
        }

        public FrankenShoulder()
        {
            this.TypeName = "FrankenShoulder";
            InitializeComponent();

            // OUTPUTS
            Outputs.Add("Motor1", new ViewModel.OutputSignalViewModel("Motor1"));
            Outputs.Add("Motor2", new ViewModel.OutputSignalViewModel("Motor2"));
            //Outputs.Add("Motor3", new ViewModel.OutputSignalViewModel("Motor3"));

            // INPUTS
            Inputs.Add("Yaw", new ViewModel.InputSignalViewModel("Yaw", this.InstanceName));
            Inputs.Add("Pitch", new ViewModel.InputSignalViewModel("Pitch", this.InstanceName));
            //Inputs.Add("Roll", new ViewModel.InputSignalViewModel("Roll", this.InstanceName));

            PostLoadSetup();
        }
    }
}

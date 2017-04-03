using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for LouShoulder.xaml
    /// </summary>
    public partial class LouShoulder : PluginBase
    {
        double pitch, yaw, roll;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Yaw"].UniqueID, (message) =>
            {
                yaw = message.Value;
                UpdateOutputs();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Pitch"].UniqueID, (message) =>
            {
                pitch = message.Value;
                UpdateOutputs();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Roll"].UniqueID, (message) =>
            {
                roll = message.Value;
                UpdateOutputs();
            });

            base.PostLoadSetup();
        }

        public LouShoulder()
        {
            this.TypeName = "Lou Shoulder";
            InitializeComponent();

            // OUTPUTS
            Outputs.Add("Motor1", new ViewModel.OutputSignalViewModel("Motor1"));
            Outputs.Add("Motor2", new ViewModel.OutputSignalViewModel("Motor2"));
            Outputs.Add("Motor3", new ViewModel.OutputSignalViewModel("Motor3"));

            // INPUTS
            Inputs.Add("Yaw", new ViewModel.InputSignalViewModel("Yaw", this.InstanceName));
            Inputs.Add("Pitch", new ViewModel.InputSignalViewModel("Pitch", this.InstanceName));
            Inputs.Add("Roll", new ViewModel.InputSignalViewModel("Roll", this.InstanceName));

            PostLoadSetup();
        }

        void UpdateOutputs()
        {
            Outputs["Motor1"].Value = yaw - pitch;
            Outputs["Motor2"].Value = yaw + pitch;
            Outputs["Motor3"].Value = roll - yaw + pitch;
        }
    }
}

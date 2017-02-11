using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for Clutch.xaml
    /// </summary>
    public partial class FrankenShoulder : PluginBase
    {
        double pitch, yaw;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Yaw"].UniqueID, (message) =>
            {
                yaw = message.Value;
                Outputs["Motor1"].Value = yaw;
                Outputs["Motor2"].Value = pitch - (yaw / 2.868);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Pitch"].UniqueID, (message) =>
            {
                pitch = message.Value;
                Outputs["Motor1"].Value = yaw;
                Outputs["Motor2"].Value = pitch - (yaw / 2.868);
            });

            base.PostLoadSetup();
        }

        public FrankenShoulder()
        {
            this.TypeName = "FrankenShoulder";
            InitializeComponent();

            // OUTPUTS
            Outputs.Add("Motor1", new ViewModel.OutputSignalViewModel("Motor1"));
            Outputs.Add("Motor2", new ViewModel.OutputSignalViewModel("Motor2"));

            // INPUTS
            Inputs.Add("Yaw", new ViewModel.InputSignalViewModel("Yaw", this.InstanceName));
            Inputs.Add("Pitch", new ViewModel.InputSignalViewModel("Pitch", this.InstanceName));

            PostLoadSetup();
        }
    }
}

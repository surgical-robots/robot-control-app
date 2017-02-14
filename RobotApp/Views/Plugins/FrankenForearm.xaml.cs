using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for Clutch.xaml
    /// </summary>
    public partial class FrankenForearm : PluginBase
    {
        double act, roll;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Actuation"].UniqueID, (message) =>
            {
                act = message.Value * 1200 / 30;
                Outputs["Actuation"].Value = act - roll;
                Outputs["Roll"].Value = roll;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Roll"].UniqueID, (message) =>
            {
                roll = message.Value;
                Outputs["Actuation"].Value = act - roll;
                Outputs["Roll"].Value = roll;
            });

            base.PostLoadSetup();
        }

        public FrankenForearm()
        {
            this.TypeName = "FrankenForearm";
            InitializeComponent();

            // OUTPUTS
            Outputs.Add("Actuation", new ViewModel.OutputSignalViewModel("Actuation"));
            Outputs.Add("Roll", new ViewModel.OutputSignalViewModel("Roll"));

            // INPUTS
            Inputs.Add("Actuation", new ViewModel.InputSignalViewModel("Actuation", this.InstanceName));
            Inputs.Add("Roll", new ViewModel.InputSignalViewModel("Roll", this.InstanceName));

            PostLoadSetup();
        }

    }
}

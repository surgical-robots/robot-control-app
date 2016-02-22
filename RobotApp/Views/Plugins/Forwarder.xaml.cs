using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Description for Scale.
    /// </summary>
    public partial class Forwarder : PluginBase
    {
        public ObservableCollection<double> scaleLevels { get; set; }
        
        private double xL, yL, zL, xR, yR, zR;

        public int scale { get; set; }

       

        /// <summary>
        /// Initializes a new instance of the Scale class.
        /// </summary>
        public Forwarder()
        {
            TypeName = "Forwarder (Loopback Tester)";
            InstanceName = "Forwarder";

            Outputs.Add("Output", new ViewModel.OutputSignalViewModel("Output"));

            Inputs.Add("Input", new ViewModel.InputSignalViewModel("Input", this.InstanceName));

            InitializeComponent();

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Input"].UniqueID, (message) =>
            {
                Outputs["Output"].Value = message.Value;
            });

        }

    }
}
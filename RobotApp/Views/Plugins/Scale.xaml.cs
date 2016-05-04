using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Description for Scale.
    /// </summary>
    public partial class Scale : PluginBase
    {
        public ObservableCollection<double> scaleLevels { get; set; }
        
        private double xL, yL, zL, xR, yR, zR;

        public int scale { get; set; }

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["X-Left"].UniqueID, (message) =>
            {
                xL = scale / 4.0 * message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Y-Left"].UniqueID, (message) =>
            {
                yL = scale / 4.0 * message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Z-Left"].UniqueID, (message) =>
            {
                zL = scale / 4.0 * message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["X-Right"].UniqueID, (message) =>
            {
                xR = scale / 4.0 * message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Y-Right"].UniqueID, (message) =>
            {
                yR = scale / 4.0 * message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Z-Right"].UniqueID, (message) =>
            {
                zR = scale / 4.0 * message.Value;
                UpdateOutput();
            });

            base.PostLoadSetup();
        }

        /// <summary>
        /// Initializes a new instance of the Scale class.
        /// </summary>
        public Scale()
        {
            TypeName = "Two Armed Scaling";
            InstanceName = "Scaling";

            Outputs.Add("X-Left", new ViewModel.OutputSignalViewModel("Scaled - X-Left"));
            Outputs.Add("Y-Left", new ViewModel.OutputSignalViewModel("Scaled - Y-Left"));
            Outputs.Add("Z-Left", new ViewModel.OutputSignalViewModel("Scaled - Z-Left"));
            Outputs.Add("X-Right", new ViewModel.OutputSignalViewModel("Scaled - X-Right"));
            Outputs.Add("Y-Right", new ViewModel.OutputSignalViewModel("Scaled - Y-Right"));
            Outputs.Add("Z-Right", new ViewModel.OutputSignalViewModel("Scaled - Z-Right"));
            Outputs.Add("Scale", new ViewModel.OutputSignalViewModel("Scale Level"));

            Inputs.Add("X-Left", new ViewModel.InputSignalViewModel("X-Left", this.InstanceName));
            Inputs.Add("Y-Left", new ViewModel.InputSignalViewModel("Y-Left", this.InstanceName));
            Inputs.Add("Z-Left", new ViewModel.InputSignalViewModel("Z-Left", this.InstanceName));
            Inputs.Add("X-Right", new ViewModel.InputSignalViewModel("X-Right", this.InstanceName));
            Inputs.Add("Y-Right", new ViewModel.InputSignalViewModel("Y-Right", this.InstanceName));
            Inputs.Add("Z-Right", new ViewModel.InputSignalViewModel("Z-Right", this.InstanceName));

            InitializeComponent();
            ScaleLevelIndex = 4;

            PostLoadSetup();
        }
        public void UpdateOutput()
        {
            Outputs["X-Left"].Value = xL;
            Outputs["Y-Left"].Value = yL;
            Outputs["Z-Left"].Value = zL;
            Outputs["X-Right"].Value = xR;
            Outputs["Y-Right"].Value = yR;
            Outputs["Z-Right"].Value = zR;
            Outputs["Scale"].Value = scale/4.0;
        }
        public int ScaleLevelIndex
        {
            get
            {
                return scale;
            }
            set
            {
                scale = (value);
                UpdateOutput();
                this.RaisePropertyChanged("ScaleLevelIndex");
            }
        }

    }
}
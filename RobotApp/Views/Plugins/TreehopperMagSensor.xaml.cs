using System.ComponentModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using RobotApp.ViewModel;
using Treehopper.Desktop;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for ButtonInterface.xaml
    /// </summary>
    public partial class TreehopperMagSensor : PluginBase
    {
        public BackgroundWorker workerThread;

        public TreehopperMagSensor()
        {
            this.TypeName = "Treehopper Mag Sensor";
            InitializeComponent();

            Outputs.Add("SenseX", new OutputSignalViewModel("Sense X"));
            Outputs.Add("SenseY", new OutputSignalViewModel("Sense Y"));
            Outputs.Add("SenseZ", new OutputSignalViewModel("Sense Z"));

            workerThread = new BackgroundWorker();
            workerThread.WorkerSupportsCancellation = true;
            workerThread.DoWork += workerThread_DoWork;
        }

        async void workerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            await RunSensor();
        }

        async Task RunSensor()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();

            var sensor = new Treehopper.Libraries.Sensors.Inertial.Tlv493d(board.I2c);

            while(true)
            {
                var reading = sensor.MagneticFlux; // one I2c fetch
                Process(reading.X, reading.Y, reading.Z);
            }
        }

        void Process(float X, float Y, float Z)
        {
            Outputs["SenseX"].Value = X;
            Outputs["SenseY"].Value = Y;
            Outputs["SenseZ"].Value = Z;
        }

        private RelayCommand<string> connectCommand;

        /// <summary>
        /// Gets the ConnectCommand.
        /// </summary>
        public RelayCommand<string> ConnectCommand
        {
            get
            {
                return connectCommand
                    ?? (connectCommand = new RelayCommand<string>(
                    p =>
                    {
                        if (!workerThread.IsBusy)
                            workerThread.RunWorkerAsync();
                    }));
            }
        }

        /// <summary>
        /// The <see cref="ConnectText" /> property's name.
        /// </summary>
        public const string ConnectTextPropertyName = "ConnectText";

        private string connectText = "Connect to Selected Device";

        /// <summary>
        /// Sets and gets the ConnectText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ConnectText
        {
            get
            {
                return connectText;
            }

            set
            {
                if (connectText == value)
                {
                    return;
                }

                connectText = value;
                RaisePropertyChanged(ConnectTextPropertyName);
            }
        }
    }
}

using System.ComponentModel;
using System.Numerics;
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

            //RunSensor();
            //workerThread.WorkerSupportsCancellation = true;
            //workerThread.DoWork += workerThread_DoWork;
        }

        async void workerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            await RunSensor();
        }

        async Task RunSensor()
        {
            int i = 0;
            Vector3 avg = new Vector3();

            //var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            //await board.ConnectAsync();

            //var sensor = new Treehopper.Libraries.Sensors.Inertial.Tlv493d(board.I2c);
            //sensor.AutoUpdateWhenPropertyRead = false;
            //while(true)
            //{
            //    await sensor.Update();
            //    //Vector3 reading = sensor.MagneticFlux; // one I2c fetch
            //    //avg += reading;
            //    i++;

            //    if (i > 9)
            //    {
            //        avg = avg / 10;
            //        Process(avg.X, avg.Y, avg.Z);
            //        await Task.Delay(10);
            //        i = 0;
            //        avg = new Vector3(0, 0, 0);
            //    }
            //}
        }

        void Process(float X, float Y, float Z)
        {
            //RobotApp.App.Current.Dispatcher.BeginInvoke((Action)delegate ()
            //{
                Outputs["SenseX"].Value = X;
                Outputs["SenseY"].Value = Y;
                Outputs["SenseZ"].Value = Z;
            //});
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
                        RunSensor();
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

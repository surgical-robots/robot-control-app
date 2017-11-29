using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using Saleae.SocketApi;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for PrintData.xaml
    /// </summary>
    public partial class SaleaeData : PluginBase
    {
        private SaleaeClient client;
        private List<ConnectedDevice> connectedDevices;
        private ConnectedDevice activeDevice;
        private double captureTime = 3;
        private bool connected = false;

        private double input1 = 0;
        private double input2 = 0;
        private double input3 = 0;
        private double input4 = 0;
        private double input5 = 0;
        private double input6 = 0;
        private double input7 = 0;
        private double input8 = 0;

        private bool printing = false;
        private string path = System.IO.Directory.GetCurrentDirectory();
        private Stopwatch dataTimer = new Stopwatch();
        System.Windows.Forms.Timer writeTimer = new System.Windows.Forms.Timer();
        private long millisTime;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Input1"].UniqueID, (message) =>
            {
                if ((input1 != message.Value))
                {
                    input1 = message.Value;
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Input2"].UniqueID, (message) =>
            {
                if (input2 != message.Value)
                {
                    input2 = message.Value;
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Input3"].UniqueID, (message) =>
            {
                if (input3 != message.Value)
                {
                    input3 = message.Value;
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Input4"].UniqueID, (message) =>
            {
                if (input4 != message.Value)
                {
                    input4 = message.Value;
                }
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Input5"].UniqueID, (message) =>
            {
                if (input5 != message.Value)
                {
                    input5 = message.Value;
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Input6"].UniqueID, (message) =>
            {
                if (input6 != message.Value)
                {
                    input6 = message.Value;
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Input7"].UniqueID, (message) =>
            {
                if (input7 != message.Value)
                {
                    input7 = message.Value;
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Input8"].UniqueID, (message) =>
            {
                if (input8 != message.Value)
                {
                    input8 = message.Value;
                }
            });

            base.PostLoadSetup();
        }

        public SaleaeData()
        {
            Inputs.Add("Input1", new ViewModel.InputSignalViewModel("Input 1", this.InstanceName));
            Inputs.Add("Input2", new ViewModel.InputSignalViewModel("Input 2", this.InstanceName));
            Inputs.Add("Input3", new ViewModel.InputSignalViewModel("Input 3", this.InstanceName));
            Inputs.Add("Input4", new ViewModel.InputSignalViewModel("Input 4", this.InstanceName));
            Inputs.Add("Input5", new ViewModel.InputSignalViewModel("Input 5", this.InstanceName));
            Inputs.Add("Input6", new ViewModel.InputSignalViewModel("Input 6", this.InstanceName));
            Inputs.Add("Input7", new ViewModel.InputSignalViewModel("Input 7", this.InstanceName));
            Inputs.Add("Input8", new ViewModel.InputSignalViewModel("Input 8", this.InstanceName));

            this.TypeName = "Print Data";
            InitializeComponent();

            writeTimer.Interval = 15;
            writeTimer.Tick += writeTimer_Tick;

            PostLoadSetup();
        }

        public void writeTimer_Tick(object sender, EventArgs e)
        {
            if (printing)
                PrintFile();
        }

        public void PrintFile()
        {
            string fullPath = path + OutputFileName + ".txt";
            if (printing == true)
            {
                if(dataTimer.ElapsedMilliseconds > captureTime*1000)
                {
                    printing = false;
                    ButtonText = "Start Capturing Data";
                    dataTimer.Stop();
                    writeTimer.Stop();
                    return;
                }
                if (!File.Exists(fullPath))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = File.CreateText(fullPath))
                    {
                        sw.Write(OutputFileName);
                        sw.Write("\t");
                        DateTime millisTime = DateTime.Now;
                        sw.WriteLine(millisTime);
                    }
                }
                using (StreamWriter sw = File.AppendText(fullPath))
                {
                    millisTime = dataTimer.ElapsedMilliseconds;
                    sw.Write(millisTime);
                    sw.Write("\t");
                    sw.Write("\t");
                    sw.Write(input1);
                    sw.Write("\t");
                    sw.Write(input2);
                    sw.Write("\t");
                    sw.Write(input3);
                    sw.Write("\t");
                    sw.Write(input4);
                    sw.Write("\t");
                    sw.Write(input5);
                    sw.Write("\t");
                    sw.Write(input6);
                    sw.Write("\t");
                    sw.Write(input7);
                    sw.Write("\t");
                    sw.WriteLine(input8);
                }
            }
        }

        /// <summary>
        /// The <see cref="OutputFileName" /> property's name.
        /// </summary>
        public const string OutputFileNamePropertyName = "OutputFileName";

        private string outputFileName = "file_name";

        /// <summary>
        /// Sets and gets the OutputFileName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string OutputFileName
        {
            get
            {
                return outputFileName;
            }

            set
            {
                if (outputFileName == value)
                {
                    return;
                }
                outputFileName = value;
                RaisePropertyChanged(OutputFileNamePropertyName);

                string fullPath = path + outputFileName;
                if (File.Exists(fullPath))
                    ButtonText = OutputFileName + " Already Exists. Click to Overwrite";
            }
        }

        /// <summary>
        /// The <see cref="ConnectButtonText" /> property's name.
        /// </summary>
        public const string ConnectButtonTextPropertyName = "ConnectButtonText";

        private string connectButtonText = "Connect to Saleae";

        /// <summary>
        /// Sets and gets the ButtonText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ConnectButtonText
        {
            get
            {
                return connectButtonText;
            }

            set
            {
                if (connectButtonText == value)
                {
                    return;
                }

                connectButtonText = value;
                RaisePropertyChanged(ConnectButtonTextPropertyName);
            }
        }

        private RelayCommand connectCommand;

        /// <summary>
        /// Gets the PrintCommand.
        /// </summary>
        public RelayCommand ConnectCommand
        {
            get
            {
                return connectCommand
                    ?? (connectCommand = new RelayCommand(
                    () =>
                    {
                        if (connected == false)
                        {
                            connected = true;
                            try
                            {
                                client = new SaleaeClient();
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Error while connecting: " + ex.Message);
                                return;
                            }
                            if (connectedDevices != null)
                                connectedDevices.Clear();

                            connectedDevices = client.GetConnectedDevices();
                            activeDevice = connectedDevices.Single(x => x.IsActive == true);

                            client.SetCaptureSeconds(captureTime);

                            switch (activeDevice.DeviceType)
                            {
                                case DeviceType.Logic:
                                    ConnectButtonText = " Connected to: " + DeviceType.Logic.GetDescription();
                                    break;
                                case DeviceType.Logic16:
                                    ConnectButtonText = " Connected to: " + DeviceType.Logic16.GetDescription();
                                    break;
                                case DeviceType.Logic4:
                                    ConnectButtonText = " Connected to: " + DeviceType.Logic4.GetDescription();
                                    break;
                                case DeviceType.Logic8:
                                    ConnectButtonText = " Connected to: " + DeviceType.Logic8.GetDescription();
                                    break;
                                case DeviceType.LogicPro16:
                                    ConnectButtonText = " Connected to: " + DeviceType.LogicPro16.GetDescription();
                                    break;
                                case DeviceType.LogicPro8:
                                    ConnectButtonText = " Connected to: " + DeviceType.LogicPro8.GetDescription();
                                    break;
                            }
                        }
                        else
                        {
                            client.StopCapture();
                            connected = false;
                            ButtonText = "Connect to device";
                        }

                    }));
            }
        }

        /// <summary>
        /// The <see cref="ButtonText" /> property's name.
        /// </summary>
        public const string ButtonTextPropertyName = "ButtonText";

        private string buttonText = "Start Capturing Data";

        /// <summary>
        /// Sets and gets the ButtonText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ButtonText
        {
            get
            {
                return buttonText;
            }

            set
            {
                if (buttonText == value)
                {
                    return;
                }

                buttonText = value;
                RaisePropertyChanged(ButtonTextPropertyName);
            }
        }

        private RelayCommand printCommand;

        /// <summary>
        /// Gets the PrintCommand.
        /// </summary>
        public RelayCommand PrintCommand
        {
            get
            {
                return printCommand
                    ?? (printCommand = new RelayCommand(
                    () =>
                    {
                        if (printing == false)
                        {
                            printing = true;
                            string fullPath = path + OutputFileName + ".logicdata";
                            Task t = Task.Run(() => { client.CaptureToFile(fullPath); });
                            Thread.Sleep(400);
                            ButtonText = "Capturing Data...";
                            dataTimer.Restart();
                            writeTimer.Start();
                        }
                        else
                        {
                            printing = false;
                            ButtonText = "Start Capturing Data";
                            dataTimer.Stop();
                            writeTimer.Stop();
                        }
                    }));
            }
        }

    }
}

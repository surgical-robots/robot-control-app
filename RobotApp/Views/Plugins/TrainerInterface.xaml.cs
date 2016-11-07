using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Threading;
using GalaSoft.MvvmLight.Command;
using RobotApp.ViewModel;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for TrainerInterface.xaml
    /// </summary>
    public partial class TrainerInterface : PluginBase
    {
        public Thread ListenThread;
        public SerialPort connectPort;
        public int baudRate = 57600;

        public bool connected = false;
        public bool listen = false;
        public int portCount = 0;
        public int[] buttonNum = new int[6] { 0, 0, 0, 0, 0, 0};
        public int outputNum;

        public int EndByte;

        public TrainerInterface()
        {
            this.TypeName = "Trainer Interface";
            InitializeComponent();

            Outputs.Add("Roll", new OutputSignalViewModel("Roll"));
            Outputs.Add("Pitch", new OutputSignalViewModel("Pitch"));
            Outputs.Add("Yaw", new OutputSignalViewModel("Yaw"));
            Outputs.Add("Depth", new OutputSignalViewModel("Depth"));
     
            FindPorts();
        }

        public void FindPorts()
        {
            bool[] removeIndex = new bool[5] { false, false, false, false, false };
            int removeCount = 0;
            int i = 0;
            SerialPort currentPort;
            string[] ports = SerialPort.GetPortNames();
            PortList = new List<string>(ports);
            foreach (string port in PortList)
            {
                currentPort = new SerialPort(port, baudRate);
                if (DetectButtons(currentPort))
                {
                    portCount++;
                }
                else
                    removeIndex[i] = true;
                i++;
            }
            for(int j = 0; j < i; j++)
            {
                if (removeIndex[j])
                {
                    PortList.RemoveAt(j - removeCount);
                    removeCount++;
                }
            }
        }

        public bool DetectButtons(SerialPort port)
        {
            try
            {
                //The below setting are for the Hello handshake
                byte[] buffer = new byte[2];
                buffer[0] = Convert.ToByte(16);
                buffer[1] = Convert.ToByte(128);
                
                int intReturnASCII = 0;
                char charReturnValue = (Char)intReturnASCII;
                if(!port.IsOpen)
                    port.Open();
                port.Write(buffer, 0, 2);
                Thread.Sleep(500);
                int count = port.BytesToRead;
                string returnMessage = "";
                while (count > 0)
                {
                    intReturnASCII = port.ReadByte();
                    returnMessage = returnMessage + Convert.ToChar(intReturnASCII);
                    count--;
                }
                //   	        ComPort.name = returnMessage;
                port.Close();
                if (returnMessage.Contains("FOOT"))
                {
                    buttonNum[portCount] = (int)Char.GetNumericValue(returnMessage[4]);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private RelayCommand<string> detectCOMsCommand;

        /// <summary>
        /// Gets the DetectCOMsCommand.
        /// </summary>
        public RelayCommand<string> DetectCOMsCommand
        {
            get
            {
                return detectCOMsCommand
                    ?? (detectCOMsCommand = new RelayCommand<string>(
                    p =>
                    {
                        FindPorts();
                    }));
            }
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
                        if(!connected)
                        {
                            if (!connectPort.IsOpen)
                                connectPort.Open();
                            ConnectText = "Connected... Click to Disconnect";
                            connected = true;
                            listen = true;
                            ListenThread = new Thread(new ThreadStart(Listen));
                            ListenThread.Start();
                        }
                        else
                        {
                            listen = false;
                            ListenThread.Abort();
                            if (connectPort.IsOpen)
                                connectPort.Close();
                            ConnectText = "Connect to Selected Device";
                            connected = false;
                        }
                    }));
            }
        }

        public void Listen()
        {
            int[] ReadByte = new int[5];
            int count = outputNum;

            while(listen)
            {

                if (connectPort.BytesToRead >= outputNum)
                {
                    string returnMessage = "";
                    count = outputNum;

                        for (int j = 0; j < 5; j++)
                        {
                          ReadByte[j] = connectPort.ReadByte();
                            if (ReadByte[j] == 255)
                            {
                            EndByte = j;
                            }
                        }

                    if (EndByte == 0)
                    {
                        Outputs["Roll"].Value = ((double)ReadByte[1] / 256) * 360;
                        Outputs["Pitch"].Value = ((double)ReadByte[2] / 256) * (70) - 35;
                        Outputs["Yaw"].Value = ((double)ReadByte[3] / 256) * (70) - 35;
                        Outputs["Depth"].Value = 185 - (((double)ReadByte[4] - 112) / 134) * 185;
                    }
                    else if (EndByte == 1)
                    {
                        Outputs["Roll"].Value = ((double)ReadByte[2] / 256) * 360;
                        Outputs["Pitch"].Value = ((double)ReadByte[3] / 256) * (70) - 35;
                        Outputs["Yaw"].Value = ((double)ReadByte[4] / 256) * (70) - 35;
                        Outputs["Depth"].Value = 185 - (((double)ReadByte[0] - 112) / 134) * 185;

                    }
                    else if (EndByte == 2)
                    {
                        Outputs["Roll"].Value = ((double)ReadByte[3] / 256) * 360;
                        Outputs["Pitch"].Value = ((double)ReadByte[4] / 256) * (70) - 35;
                        Outputs["Yaw"].Value = ((double)ReadByte[0] / 256) * (70) - 35;
                        Outputs["Depth"].Value = 185 - (((double)ReadByte[1] - 112) / 134) * 185;
                    }
                    else if (EndByte == 3)
                    {
                        Outputs["Roll"].Value = ((double)ReadByte[4] / 256) * 360;
                        Outputs["Pitch"].Value = ((double)ReadByte[0] / 256) * (70) - 35;
                        Outputs["Yaw"].Value = ((double)ReadByte[1] / 256) * (70) - 35;
                        Outputs["Depth"].Value = 185 - (((double)ReadByte[2] - 112) / 134) * 185;
                    }
                    else if (EndByte == 4)
                    {
                        Outputs["Roll"].Value = ((double)ReadByte[0] / 256) * 360;
                        Outputs["Pitch"].Value = ((double)ReadByte[1] / 256) * (70) - 35;
                        Outputs["Yaw"].Value = ((double)ReadByte[2] / 256) * (70) - 35;
                        Outputs["Depth"].Value = 185 - (((double)ReadByte[3] - 112) / 134) * 185;
                    }
                }
            }
        }

        /// <summary>
        /// The <see cref="PortList" /> property's name.
        /// </summary>
        public const string PortListPropertyName = "PortList";

        private List<string> portList = null;

        /// <summary>
        /// Sets and gets the PortList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public List<string> PortList
        {
            get
            {
                return portList;
            }

            set
            {
                if (portList == value)
                {
                    return;
                }

                portList = value;
                RaisePropertyChanged(PortListPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SelectedPort" /> property's name.
        /// </summary>
        public const string SelectedPortPropertyName = "SelectedPort";

        private string selectedPort = null;

        /// <summary>
        /// Sets and gets the SelectedPort property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SelectedPort
        {
            get
            {
                return selectedPort;
            }

            set
            {
                if (selectedPort == value)
                {
                    return;
                }
                
                selectedPort = value;

                if(selectedPort != null)
                {
                    outputNum = buttonNum[PortList.IndexOf(selectedPort)];

                    connectPort = new SerialPort(selectedPort, baudRate);
                    connectPort.WriteTimeout = 100;
                    //Outputs.Clear();
                    //if(outputNum == 1)
                    //    Outputs.Add("Button1", new OutputSignalViewModel("Button 1"));
                    //else if( outputNum == 2)
                    //{
                    //    Outputs.Add("Button1", new OutputSignalViewModel("Button 1"));
                    //    Outputs.Add("Button2", new OutputSignalViewModel("Button 2"));
                    //}
                    //else if (outputNum > 2)
                    //{
                    //    Outputs.Add("Button1", new OutputSignalViewModel("Button 1"));
                    //    Outputs.Add("Button2", new OutputSignalViewModel("Button 2"));
                    //    Outputs.Add("Button3", new OutputSignalViewModel("Button 3"));
                    //}
                    RaisePropertyChanged(SelectedPortPropertyName);
                }
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

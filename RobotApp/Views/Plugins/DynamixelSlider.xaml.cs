using System;
using System.IO.Ports;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Timers;

using ROBOTIS;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for DynamixelSlider.xaml
    /// </summary>
    public partial class DynamixelSlider : PluginBase
    {
        // Control table address (for DXL Pro)
        public const int P_TORQUE_ENABLE = 562;
        public const int P_GOAL_POSITION_L = 596;
        public const int P_PRESENT_POSITION_L = 611;
        public const int P_GOAL_VELOCITY_L = 600;
        public const int P_PRESENT_VELOCITY_L = 615;
        public const int P_MOVING = 610;
        public const int P_GOAL_ACCEL_L = 606;
        public const int P_GOAL_TORQUE_L = 30;
        public const int P_LED_RED = 563;
        public const int P_LED_GREEN = 564;
        public const int P_LED_BLUE = 565;
        public int PortNumber = 0;

        public SerialPort connectPort;
        public bool connected = false;
        
        int CommStatus;

        // Default settings
    //    public const int DEFAULT_PORT = 13;    // COM13
        public const int DEFAULT_BAUD = 57600;

        Timer errorTimer = new Timer(5000);

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Theta1"].UniqueID, (message) =>
            {
                Theta1Value = message.Value;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Theta2"].UniqueID, (message) =>
            {
                Theta2Value = message.Value;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Theta3"].UniqueID, (message) =>
            {
                Theta3Value = -message.Value;
            });

            base.PostLoadSetup();
        }

        public DynamixelSlider()
        {
            this.DataContext = this;
            this.TypeName = "Dynamixel Slider";

            Inputs.Add("Theta1", new ViewModel.InputSignalViewModel("Theta 1", this.InstanceName));
            Inputs.Add("Theta2", new ViewModel.InputSignalViewModel("Theta 2", this.InstanceName));
            Inputs.Add("Theta3", new ViewModel.InputSignalViewModel("Theta 3", this.InstanceName));

            PostLoadSetup();

            InitializeComponent();
            errorTimer.Elapsed += ErrorTimer_Elapsed;
            errorTimer.Start();
            string[] ports = SerialPort.GetPortNames();
            PortList = new List<string>(ports);
        }

        private void ErrorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ErrorText = "";
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
                RaisePropertyChanged(SelectedPortPropertyName);
                if (selectedPort != null)
                {
                    PortNumber = Convert.ToInt32(selectedPort.Remove(0, 3));

                }
                else
                {
                    PortNumber = 0;
                }
               
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
                        PortList = null;
                        string[] ports = SerialPort.GetPortNames();
                        PortList = new List<string>(ports);
                    }));
            }
        }

        private RelayCommand connectCommand;

        /// <summary>
        /// Gets the ResetHomePositionCommand.
        /// </summary>
        public RelayCommand ConnectCommand
        {
            get
            {
                return connectCommand
                    ?? (connectCommand = new RelayCommand(
                    () =>
                    {
                        if (!ConnectCommand.CanExecute(null))
                        {
                            return;
                        }
                        if (PortNumber == 0)
                        {
                            ErrorText = "No Port Selected";
                            ButtonText = "Connect to Dynamixel Pro";
                            return;
                        }
                        if (dynamixel.dxl_initialize(PortNumber, DEFAULT_BAUD) == 0)
                        {
                            ErrorText = "Failed to open USB2Dynamixel!";
                            return;
                        }
                        else
                        {
                            connected = true;
                            ButtonText = "Connected, Press to Home";
                        }
                        for (byte index = 1; index <= 3; index++)
                        {
                            dynamixel.dxl2_write_byte(index, P_TORQUE_ENABLE, 1);
                            CommStatus = dynamixel.dxl_get_comm_result();
                            if (CommStatus != dynamixel.COMM_RXSUCCESS)
                            {
                                ErrorText = "Failed to enable torque!";
                            }
                            dynamixel.dxl2_write_dword(index, P_GOAL_POSITION_L, 0);
                            dynamixel.dxl2_write_dword(index, P_GOAL_ACCEL_L, 10); 
                            dynamixel.dxl2_write_byte(index, P_LED_BLUE, 255);
                            CommStatus = dynamixel.dxl_get_comm_result();
                            if (CommStatus != dynamixel.COMM_RXSUCCESS)
                            {
                                ErrorText = "Failed to connect to Dynamixel Pro";
                                ButtonText = "Connect to Dynamixel Pro";
                            }
                            Theta1Value = 0;
                            Theta2Value = 0;
                            Theta3Value = 0;
                        }
                    },
                    () => true));
            }
        }

        /// <summary>
        /// The <see cref="ButtonText" /> property's name.
        /// </summary>
        public const string ButtonTextPropertyName = "ButtonText";

        private string buttonText = "Connect to Dynamixel Pro";

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

        /// <summary>
        /// The <see cref="ErrorText" /> property's name.
        /// </summary>
        public const string ErrorTextPropertyName = "ErrorText";

        private string errorText = "";

        /// <summary>
        /// Sets and gets the ErrorText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ErrorText
        {
            get
            {
                return errorText;
            }

            set
            {
                if (errorText == value)
                {
                    return;
                }

                errorText = value;
                RaisePropertyChanged(ErrorTextPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Theta1Value" /> property's name.
        /// </summary>
        public const string Theta1ValuePropertyName = "Theta1Value";

        private double theta1Value = 0;

        /// <summary>
        /// Sets and gets the Slider1Value property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Theta1Value
        {
            get
            {
                return theta1Value;
            }

            set
            {
                if (theta1Value == value)
                {
                    return;
                }

                theta1Value = (value);
                RaisePropertyChanged(Theta1ValuePropertyName);

                if (connected)
                {
                    int intVal = (int)Math.Round(theta1Value * 251000 / 180);
                    dynamixel.dxl2_write_dword(1, P_GOAL_POSITION_L, (UInt32)intVal);
                    CommStatus = dynamixel.dxl_get_comm_result();
                    if (CommStatus != dynamixel.COMM_RXSUCCESS)
                    {
                        ErrorText = "Failed to send Motor1 setpoint!";
                    }
                }
            }
        }

        /// <summary>
        /// The <see cref="Theta2Value" /> property's name.
        /// </summary>
        public const string Theta2ValuePropertyName = "Theta2Value";

        private double theta2Value = 0;

        /// <summary>
        /// Sets and gets the Slider1Value property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Theta2Value
        {
            get
            {
                return theta2Value;
            }

            set
            {
                if (theta2Value == value)
                {
                    return;
                }

                theta2Value = (value);
                RaisePropertyChanged(Theta2ValuePropertyName);

                if (connected)
                {
                    int intVal = (int)Math.Round(theta2Value * 151875 / 180);
                    dynamixel.dxl2_write_dword(2, P_GOAL_POSITION_L, (UInt32)intVal);
                    CommStatus = dynamixel.dxl_get_comm_result();
                    if (CommStatus != dynamixel.COMM_RXSUCCESS)
                    {
                        ErrorText = "Failed to send Motor2 setpoint!";
                    }
                }
            }
        }

        /// <summary>
        /// The <see cref="Theta3Value" /> property's name.
        /// </summary>
        public const string Theta3ValuePropertyName = "Theta3Value";

        private double theta3Value = 0;

        /// <summary>
        /// Sets and gets the Slider1Value property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Theta3Value
        {
            get
            {
                return theta3Value;
            }

            set
            {
                if (theta3Value == value)
                {
                    return;
                }

                theta3Value = (value);
                RaisePropertyChanged(Theta3ValuePropertyName);

                if (connected)
                {
                    int intVal = (int)Math.Round(theta3Value * 151875 / 180);
                    dynamixel.dxl2_write_dword(3, P_GOAL_POSITION_L, (UInt32)intVal);
                    CommStatus = dynamixel.dxl_get_comm_result();
                    if (CommStatus != dynamixel.COMM_RXSUCCESS)
                    {
                        ErrorText = "Failed to send Motor3 setpoint!";
                    }
                }
            }
        }

        /// <summary>
        /// The <see cref="Torque1" /> property's name.
        /// </summary>
        public const string Torque1PropertyName = "Torque1";

        private bool torque1 = true;

        /// <summary>
        /// Sets and gets the Torque1 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool Torque1
        {
            get
            {
                return torque1;
            }

            set
            {
                if (torque1 == value)
                {
                    return;
                }

                torque1 = value;
                RaisePropertyChanged(Torque1PropertyName);
                dynamixel.dxl2_write_byte(1, P_TORQUE_ENABLE, Convert.ToByte(torque1));
                CommStatus = dynamixel.dxl_get_comm_result();
                if (CommStatus != dynamixel.COMM_RXSUCCESS)
                {
                    ErrorText = "Failed to disable torque1!";
                    Torque1 = true;
                }
            }
        }
        /// <summary>
        /// The <see cref="Torque2" /> property's name.
        /// </summary>
        public const string Torque2PropertyName = "Torque2";

        private bool torque2 = true;

        /// <summary>
        /// Sets and gets the Torque2 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool Torque2
        {
            get
            {
                return torque2;
            }

            set
            {
                if (torque2 == value)
                {
                    return;
                }

                torque2 = value;
                RaisePropertyChanged(Torque2PropertyName);
                dynamixel.dxl2_write_byte(2, P_TORQUE_ENABLE, Convert.ToByte(torque2));
                CommStatus = dynamixel.dxl_get_comm_result();
                if (CommStatus != dynamixel.COMM_RXSUCCESS)
                {
                    ErrorText = "Failed to disable torque2!";
                    Torque2 = true;
                }
            }
        }
        /// <summary>
        /// The <see cref="Torque3" /> property's name.
        /// </summary>
        public const string Torque3PropertyName = "Torque3";

        private bool torque3 = true;

        /// <summary>
        /// Sets and gets the Torque3 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool Torque3
        {
            get
            {
                return torque3;
            }

            set
            {
                if (torque3 == value)
                {
                    return;
                }

                torque3 = value;
                RaisePropertyChanged(Torque3PropertyName);
                dynamixel.dxl2_write_byte(3, P_TORQUE_ENABLE, Convert.ToByte(torque3));
                CommStatus = dynamixel.dxl_get_comm_result();
                if (CommStatus != dynamixel.COMM_RXSUCCESS)
                {
                    ErrorText = "Failed to disable torque3!";
                    Torque3 = true;
                }
            }
        }

    }
}

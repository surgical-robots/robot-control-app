using System;
using System.IO.Ports;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
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

        public DynamixelSlider()
        {
            this.DataContext = this;
            this.TypeName = "Dynamixel Slider";
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
                            dynamixel.dxl2_write_dword(index, P_GOAL_ACCEL_L, 127); 
                            dynamixel.dxl2_write_byte(index, P_LED_BLUE, 255);
                            CommStatus = dynamixel.dxl_get_comm_result();
                            if (CommStatus != dynamixel.COMM_RXSUCCESS)
                            {
                                ErrorText = "Failed to connect to Dynamixel Pro";
                                ButtonText = "Connect to Dynamixel Pro";
                            }
                            Slider1Value = 0;
                            Slider2Value = 0;
                            Slider3Value = 0;
                            Slider4Value = 127;

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
        /// The <see cref="Slider1Value" /> property's name.
        /// </summary>
        public const string Slider1ValuePropertyName = "Slider1Value";

        private double slider1Value = 0;

        /// <summary>
        /// Sets and gets the Slider1Value property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Slider1Value
        {
            get
            {
                return slider1Value;
            }

            set
            {
                if (slider1Value == value)
                {
                    return;
                }

                slider1Value = (value);
                RaisePropertyChanged(Slider1ValuePropertyName);

                if (connected)
                {
                    int intVal = (int)Math.Round(slider1Value * 251000 / 180);
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
        /// The <see cref="Slider2Value" /> property's name.
        /// </summary>
        public const string Slider2ValuePropertyName = "Slider2Value";

        private double slider2Value = 0;

        /// <summary>
        /// Sets and gets the Slider1Value property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Slider2Value
        {
            get
            {
                return slider2Value;
            }

            set
            {
                if (slider2Value == value)
                {
                    return;
                }

                slider2Value = (value);
                RaisePropertyChanged(Slider2ValuePropertyName);

                if (connected)
                {
                    int intVal = (int)Math.Round(slider2Value*151875/180);
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
        /// The <see cref="Slider3Value" /> property's name.
        /// </summary>
        public const string Slider3ValuePropertyName = "Slider3Value";

        private double slider3Value = 0;

        /// <summary>
        /// Sets and gets the Slider1Value property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Slider3Value
        {
            get
            {
                return slider3Value;
            }

            set
            {
                if (slider3Value == value)
                {
                    return;
                }

                slider3Value = (value);
                RaisePropertyChanged(Slider3ValuePropertyName);

                if (connected)
                {
                    int intVal = (int)Math.Round(slider3Value*151875/180);
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
        /// The <see cref="Slider4Value" /> property's name.
        /// </summary>
        public const string Slider4ValuePropertyName = "Slider4Value";

        private int slider4Value = 0;

        /// <summary>
        /// Sets and gets the Slider4Value property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Slider4Value
        {
            get
            {
                return slider4Value;
            }

            set
            {
                if (slider4Value == value)
                {
                    return;
                }

                slider4Value = value;
                RaisePropertyChanged(Slider4ValuePropertyName);
                for (byte index = 1; index <= 3; index++)
                {
                    if (connected)
                    {
                   //     int intVal = (int)Math.Round(slider4Value);
                        dynamixel.dxl2_write_dword(index, P_GOAL_ACCEL_L, (UInt32)slider4Value);
                    }
                }
            }
        }


        ///// <summary>
        ///// The <see cref="Slider5Value" /> property's name.
        ///// </summary>
        //public const string Slider5ValuePropertyName = "Slider5Value";

        //private int slider5Value = 0;

        ///// <summary>
        ///// Sets and gets the Slider5Value property.
        ///// Changes to that property's value raise the PropertyChanged event. 
        ///// </summary>
        //public int Slider5Value
        //{
        //    get
        //    {
        //        return slider5Value;
        //    }

        //    set
        //    {
        //        if (slider5Value == value)
        //        {
        //            return;
        //        }

        //        slider5Value = value;
        //        RaisePropertyChanged(Slider5ValuePropertyName);
        //        if (connected)
        //        {
        //            int int1Val = (int)Math.Round(slider5Value * 9.3);
        //            int int2Val = (int)Math.Round(slider5Value * 13.95);
        //            dynamixel.dxl2_write_dword(1, P_GOAL_TORQUE_L, (UInt32)int1Val);
        //            dynamixel.dxl2_write_dword(2, P_GOAL_TORQUE_L, (UInt32)int2Val);
        //            dynamixel.dxl2_write_dword(3, P_GOAL_TORQUE_L, (UInt32)int2Val);
        //            CommStatus = dynamixel.dxl_get_comm_result();
        //            if (CommStatus != dynamixel.COMM_RXSUCCESS)
        //            {
        //                ErrorText = "Failed to Change Torque limit";
        //            }
        //        }
        //    }
        //}
        

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

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotControl;
using System.Diagnostics;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using RobotApp.Views;

namespace RobotApp.ViewModel
{
    [DataContract]
    [Serializable]
    public class MotorViewModel : INotifyPropertyChanged, IDisposable
    {
        [DataMember]
        public ObservableDictionary<string, OutputSignalViewModel> Outputs { get; set; }

        /// <summary>
        /// The jointItem vm that this motor belongs to.
        /// </summary>
        /// <summary>
        /// The <see cref="Controller" /> property's name.
        /// </summary>
        public const string JointItemPropertyName = "JointItem";

        [DataMember]
        private Controller controller = null;

        public MotorViewModel()
        {
            Motor = new Motor();
            Sinks = new ObservableDictionary<string, InputSignalViewModel>();
            JogSpeed = 32;
            MainViewModel.Instance.Robot.Motors.Add(Motor);

            // ObservableDictionary to hold all of the signal sources that our plugin might have
            Outputs = new ObservableDictionary<string, OutputSignalViewModel>();
            // Add outputs to ObservableDictionary
            Outputs.Add("Current", new ViewModel.OutputSignalViewModel("Current"));
            Outputs.Add("MotorPositon", new ViewModel.OutputSignalViewModel("MotorPosition"));

            CreateInputs();
            SetupMessenger();

            Kp = kp;
            SpeedMax = speedMax;
            CurrentMax = currentMax;
            PotZero = potZero;
        }

        /// <summary>
        /// The <see cref="AngleSetpoint" /> property's name.
        /// </summary>
        public const string AngleSetpointPropertyName = "AngleSetpoint";
        
        private double angleSetpoint = 0;

        /// <summary>
        /// Sets and gets the AngleSetpoint property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double AngleSetpoint
        {
            get
            {
                return angleSetpoint;
            }

            set
            {
                if (angleSetpoint == value)
                {
                    return;
                }
                angleSetpoint = value;
                Motor.Angle = value;
                if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(AngleSetpointPropertyName));
            }
        }

        /// <summary>
        /// The <see cref="Kp" /> property's name.
        /// </summary>
        public const string KpPropertyName = "Kp";
        [DataMember]
        private float kp = 1;

        /// <summary>
        /// Sets and gets the Kp property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public float Kp
        {
            get
            {
                return kp;
            }

            set
            {
                if (Motor.Kp == value)
                {
                    return;
                }
                if (value < 0)
                    kp = 0;
                else
                    kp = value;
                Motor.Kp = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(KpPropertyName));
            }
        }

        /// <summary>
        /// Sets and gets the JointItem property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Controller Controller
        {
            get
            {
                return controller;
            }

            set
            {
                if (controller == value)
                {
                    return;
                }

                if(value != null)
                {
                    //value.Motor1CurrentChanged -= 
                }

                controller = value;
                controller.Motor1CurrentChanged += controller_Motor1CurrentChanged;
                controller.Motor1CounterChanged += controller_Motor1CounterChanged;
                controller.Motor2CurrentChanged += controller_Motor2CurrentChanged;
                controller.Motor2CounterChanged += controller_Motor2CounterChanged;
                controller.Motor1PotChanged += controller_Motor1PotChanged;
                controller.Motor2PotChanged += controller_Motor2PotChanged;
                    this.Motor.Controller = controller;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(JointItemPropertyName));
            }
        }

        void controller_Motor2CounterChanged(int newValue)
        {
            if (Id != 1) return;
            ShaftCounter = newValue;
            Outputs["MotorPositon"].Value = (newValue/EncoderCountsPerRevolution) * 360;
        }

        void controller_Motor1CounterChanged(int newValue)
        {
            if (Id != 0) return;
            ShaftCounter = newValue;
            Outputs["MotorPositon"].Value = (newValue / EncoderCountsPerRevolution) * 360;
        }

        void controller_Motor1PotChanged(int newValue)
        {
            if (Id != 0) return;
            Pot = newValue;
        }

        void controller_Motor2PotChanged(int newValue)
        {
            if (Id != 1) return;
            Pot = newValue;
        }

        /// <summary>
        /// The <see cref="ShaftCounter" /> property's name.
        /// </summary>
        public const string ShaftCounterPropertyName = "ShaftCounter";
        [IgnoreDataMember]
        [NonSerialized]
        private int shaftCounter = 0;

        /// <summary>
        /// Sets and gets the ShaftCounter property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int ShaftCounter
        {
            get
            {
                return shaftCounter;
            }

            set
            {
                if (shaftCounter == value)
                {
                    return;
                }

                shaftCounter = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(ShaftCounterPropertyName));
            }
        }

        /// <summary>
        /// The <see cref="Current" /> property's name.
        /// </summary>
        public const string CurrentPropertyName = "Current";
        [IgnoreDataMember]
        [NonSerialized]
        private int current = 0;

        /// <summary>
        /// Sets and gets the Current property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Current
        {
            get
            {
                return current;
            }

            set
            {
                if (current == value)
                {
                    return;
                }

                current = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(CurrentPropertyName));
            }
        }

        /// <summary>
        /// The <see cref="Pot" /> property's name.
        /// </summary>
        public const string PotPropertyName = "Pot";
        [IgnoreDataMember]
        [NonSerialized]
        private int pot = 0;

        /// <summary>
        /// Sets and gets the Pot property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Pot
        {
            get
            {
                return pot;
            }

            set
            {
                if (pot == value)
                {
                    return;
                }

                pot = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(PotPropertyName));
            }
        }

        void controller_Motor2CurrentChanged(int newValue)
        {
            if (Id != 1) return;
            Current = newValue;
            Outputs["Current"].Value = (double)newValue * (3.3 / Math.Pow(2, 16)) / 0.25 * 1000;
        }

        void controller_Motor1CurrentChanged(int newValue)
        {
            if (Id != 0) return;
            Current = newValue;
            Outputs["Current"].Value = (double)newValue * (3.3 / Math.Pow(2, 16)) / 0.25 * 1000;
        }

        [DataMember]
        public Motor Motor { get; set; }
        
        [DataMember]
        public ObservableDictionary<string, InputSignalViewModel> Sinks { get; set; }

        [NonSerialized]
        private RelayCommand jogForwardUp;

        /// <summary>
        /// Gets the JogForwardUp.
        /// </summary>
        public RelayCommand JogForwardUp
        {
            get
            {
                return jogForwardUp
                    ?? (jogForwardUp = new RelayCommand(
                    () =>
                    {
                        JogDirection = true; JogEnabled = false;
                    }));
            }
        }
        [NonSerialized]
        private RelayCommand jogForwardDown;

        /// <summary>
        /// Gets the MyCommand.
        /// </summary>
        public RelayCommand JogForwardDown
        {
            get
            {
                return jogForwardDown
                    ?? (jogForwardDown = new RelayCommand(
                    () =>
                    {
                        JogDirection = true; JogEnabled = true;
                    }));
            }
        }
        [NonSerialized]
        private RelayCommand jogReverseUp;

        /// <summary>
        /// Gets the JogReverseUp.
        /// </summary>
        public RelayCommand JogReverseUp
        {
            get
            {
                return jogReverseUp
                    ?? (jogReverseUp = new RelayCommand(
                    () =>
                    {
                        JogDirection = false; JogEnabled = false;
                    }));
            }
        }
        [NonSerialized]
        private RelayCommand jogReverseDown;

        /// <summary>
        /// Gets the JogReverseDown.
        /// </summary>
        public RelayCommand JogReverseDown
        {
            get
            {
                return jogReverseDown
                    ?? (jogReverseDown = new RelayCommand(
                    () =>
                    {
                        JogDirection = false; JogEnabled = true; 
                    }));
            }
        }

        public void CreateInputs()
        {
            Sinks.Add("AngleSetpoint", new InputSignalViewModel("AngleSetpoint", FriendlyName));
            Sinks.Add("JogForward", new InputSignalViewModel("JogForward", FriendlyName));
            Sinks.Add("JogReverse", new InputSignalViewModel("JogReverse", FriendlyName));
            Sinks.Add("JogSpeed", new InputSignalViewModel("JogSpeed", FriendlyName));
        }

        public void SetupMessenger()
        {
            Messenger.Default.Register<Messages.Signal>(this, Sinks["AngleSetpoint"].UniqueID, (msg) =>
            {
                AngleSetpoint = msg.Value;
            });

            
            Messenger.Default.Register<Messages.Signal>(this, Sinks["JogForward"].UniqueID, (msg) =>
            {
                if (msg.Value > 0.5)
                {
                    JogDirection = true;
                    JogEnabled = true;
                }

                else
                    JogEnabled = false;
            });

            
            Messenger.Default.Register<Messages.Signal>(this, Sinks["JogReverse"].UniqueID, (msg) =>
            {

                if (msg.Value > 0.5)
                {
                    JogDirection = false;
                    JogEnabled = true;
                }

                else
                    JogEnabled = false;

            });

            
            Messenger.Default.Register<Messages.Signal>(this, Sinks["JogSpeed"].UniqueID, (msg) =>
            {
                int speed = 0;
                if (msg.Value > 0)
                {
                    if (msg.Value < 255.0)
                    {
                        speed = (int)Math.Round(msg.Value);
                    }
                }

                JogSpeed = speed;
            });
        }



        public string DisplayName
        {
            get
            {
                return "Motor " + Id + " (" + FriendlyName + ")";
            }
        }

        /// <summary>
        /// The <see cref="FriendlyName" /> property's name.
        /// </summary>
        public const string FriendlyNamePropertyName = "FriendlyName";

        [DataMember]
        private string friendlyName = "";

        /// <summary>
        /// Sets and gets the FriendlyName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string FriendlyName
        {
            get
            {
                return friendlyName;
            }

            set
            {
                if (friendlyName == value)
                {
                    return;
                }

                friendlyName = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(FriendlyNamePropertyName));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DisplayName"));

                // We have to tell our signal manager what our new name is
                foreach(var sink in Sinks)
                {
                    sink.Value.ParentInstanceName = FriendlyName;
                }
            }
        }

        private void UpdateJogState()
        {
            if(JogEnabled)
            {
                Motor.Jog(JogSpeed, JogDirection);
            }
            else
            {
                Motor.Jog(0, false);
            }
        }

        [DataMember]
        private int jogSpeed;
        public int JogSpeed 
        { 
            get
            {
             return jogSpeed;   
            }
            set
            {
                if (value == jogSpeed)
                    return;
                jogSpeed = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("JogSpeed"));
                UpdateJogState();
            }
        }

        /// <summary>
        /// Sets and gets the EncoderCountsPerRevolution property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [DataMember]
        private double encoderCountsPerRevolution = 1;

        public double EncoderCountsPerRevolution
        {
            get
            {
                return encoderCountsPerRevolution;
            }

            set
            {
                if (encoderCountsPerRevolution == value)
                {
                    return;
                }
                encoderCountsPerRevolution = value;
                Motor.EncoderClicksPerRevolution = encoderCountsPerRevolution;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("EncoderCountsPerRevolution"));
            }
        }
        
        /// <summary>
        /// The <see cref="JogDirection" /> property's name.
        /// </summary>
        public const string JogDirectionPropertyName = "JogDirection";

        private bool jogDirection = false;

        /// <summary>
        /// Sets and gets the JogDirection property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool JogDirection
        {
            get
            {
                return jogDirection;
            }

            set
            {
                if (jogDirection == value)
                {
                    return;
                }

                jogDirection = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(JogDirectionPropertyName));
                UpdateJogState();
            }
        }
        
        /// <summary>
        /// The <see cref="JogEnabled" /> property's name.
        /// </summary>
        public const string JogEnabledPropertyName = "JogEnabled";

        private bool jogEnabled = false;

        /// <summary>
        /// Sets and gets the JogEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool JogEnabled
        {
            get
            {
                return jogEnabled;
            }

            set
            {
                if (jogEnabled == value)
                {
                    return;
                }

                jogEnabled = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(JogEnabledPropertyName));
                UpdateJogState();
            }
        }

        /// <summary>
        /// The <see cref="PrintData" /> property's name.
        /// </summary>
        public const string PrintDataPropertyName = "PrintData";

        private bool printData = false;

        /// <summary>
        /// Sets and gets the JogEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool PrintData
        {
            get
            {
                return printData;
            }

            set
            {
                if (printData == value)
                {
                    return;
                }

                printData = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PrintDataPropertyName));
                //if (printData == true)
                //    PrintMotorData();
            }
        }

        /// <summary>
        /// The <see cref="SpeedMax" /> property's name.
        /// </summary>
        public const string SpeedMaxPropertyName = "SpeedMax";
        [DataMember]
        private byte speedMax = 255;

        /// <summary>
        /// Sets and gets the JogEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public byte SpeedMax
        {
            get
            {
                return speedMax;
            }

            set
            {
                if (Motor.SpeedMax == value)
                {
                    return;
                }
                speedMax = value;
                Motor.SpeedMax = speedMax;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(SpeedMaxPropertyName));
            }
        }

        /// <summary>
        /// The <see cref="CurrentMax" /> property's name.
        /// </summary>
        public const string CurrentMaxPropertyName = "CurrentMax";
        [DataMember]
        private double currentMax = 1000;

        /// <summary>
        /// Sets and gets the JogEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double CurrentMax
        {
            get
            {
                return currentMax;
            }

            set
            {
                if (Motor.CurrentMax == value)
                {
                    return;
                }
                if (value < 0)
                    currentMax = 0;
                else
                    currentMax = value;
                double bitCurrent = Math.Round(currentMax * Math.Pow(2, 16) / 3.3 * 0.25 / 1000);
                if (bitCurrent > 65535) bitCurrent = 65535;
                Motor.CurrentMax = Convert.ToUInt16(bitCurrent);
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(CurrentMaxPropertyName));
            }
        }

        /// <summary>
        /// The <see cref="PotZero" /> property's name.
        /// </summary>
        public const string PotZeroPropertyName = "PotZero";
        [DataMember]
        private UInt16 potZero = 0;

        /// <summary>
        /// Sets and gets the JogEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public UInt16 PotZero
        {
            get
            {
                return potZero;
            }

            set
            {
                if (Motor.PotZero == value)
                {
                    return;
                }
                potZero = value;
                Motor.PotZero = potZero;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PotZeroPropertyName));
            }
        }

        [DataMember]
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value;
            Motor.Index = id;
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Id"));
            }
        }

        [DataMember]
        private int controlModeIndex;

        public int ControlModeIndex
        {
            get
            {
                return controlModeIndex;
            }
            set
            {
                controlModeIndex = value;
                Motor.ControlMode = (ControlMode)(controlModeIndex);
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ControlModeIndex"));
            }
        }

        public void Dispose()
        {
            // Let the SignalSinkRegistry know that we're going bye-bye!
            foreach(var item in Sinks)
            {
                Messenger.Default.Send<Messages.UnregisterSignalSink>(new Messages.UnregisterSignalSink() { Sink = item.Value });
            }
            
        }
        [field: NonSerializedAttribute()] 
        public event PropertyChangedEventHandler PropertyChanged;

        [NonSerialized]
        private RelayCommand readPots;

        public RelayCommand ReadPots
        {
            get
            {
                return readPots
                    ?? (readPots = new RelayCommand(
                    () =>
                    {
                        if (!readPots.CanExecute(null))
                        {
                            return;
                        }

                        this.controller.Robot.SendCommand(JointCommands.GetPots, controller);
                    },
                    () => true));
            }
        }

        [NonSerialized]
        private RelayCommand homeJoint;

        public RelayCommand HomeJoint
        {
            get
            {
                return homeJoint
                    ?? (homeJoint = new RelayCommand(
                    () =>
                    {
                        if (!homeJoint.CanExecute(null))
                        {
                            return;
                        }

                        this.controller.Robot.SendCommand(JointCommands.ResetCounters, controller, new byte[] { (byte)id, (byte)0x01 });
                    },
                    () => true));
            }
        }
    }
}

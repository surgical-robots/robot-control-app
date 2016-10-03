using GalaSoft.MvvmLight.Command;
using RobotApp.ViewModel;
using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using TelSurge;

namespace RobotApp.Views.Plugins
{

    /// <summary>
    /// Interaction logic for GeomagicTouchConfigurationView.xaml
    /// </summary>
    public partial class TelSurgeView : PluginBase
    {
        TelSurgeMain telSurge;

        public System.Windows.Forms.Timer UpdateTimer = new System.Windows.Forms.Timer();

        public double forceLX = 0;
        public double forceLY = 0;
        public double forceLZ = 0;
        public double forceRX = 0;
        public double forceRY = 0;
        public double forceRZ = 0;
        public bool hapticEnable = false;
        private double lx, ly, lz, rx, ry, rz;

        /// <summary>
        /// This function is manually called at the end of the constructor (below) as well as automatically getting called after deserialization.
        /// Place any code in here that you want executed after deserialization or construction.
        /// </summary>
         public override void PostLoadSetup()
         {
             Messenger.Default.Register<Messages.Signal>(this, Inputs["EmergencySwitch"].UniqueID, (message) =>
             {
                 if (message.Value.Equals(0))
                     telSurge.emergencySwitchControl();
             });

             Messenger.Default.Register<Messages.Signal>(this, Inputs["Freeze"].UniqueID, (message) =>
             {
                 if (message.Value.Equals(0))
                 {
                    //telSurge.freezeCommandReceived();
                 }
             });

             Messenger.Default.Register<Messages.Signal>(this, Inputs["LX"].UniqueID, (message) =>
             {
                 lx = message.Value;
                 //telSurge.setPosition(lx, ly, lz, rx, ry, rz);
             });

             Messenger.Default.Register<Messages.Signal>(this, Inputs["LY"].UniqueID, (message) =>
             {
                 ly = message.Value;
                 //telSurge.setPosition(lx, ly, lz, rx, ry, rz);
             });

             Messenger.Default.Register<Messages.Signal>(this, Inputs["LZ"].UniqueID, (message) =>
             {
                 lz = message.Value;
                // telSurge.setPosition(lx, ly, lz, rx, ry, rz);
             });

             Messenger.Default.Register<Messages.Signal>(this, Inputs["RX"].UniqueID, (message) =>
             {
                 rx = message.Value;
                // telSurge.setPosition(lx, ly, lz, rx, ry, rz);
             });

             Messenger.Default.Register<Messages.Signal>(this, Inputs["RY"].UniqueID, (message) =>
             {
                 ry = message.Value;
                // telSurge.setPosition(lx, ly, lz, rx, ry, rz);
             });

             Messenger.Default.Register<Messages.Signal>(this, Inputs["RZ"].UniqueID, (message) =>
             {
                 rz = message.Value;
                // telSurge.setPosition(lx, ly, lz, rx, ry, rz);
             });

             Messenger.Default.Register<Messages.Signal>(this, Inputs["ForceLX"].UniqueID, (message) =>
             {
                 forceLX = message.Value;
               //  if (hapticEnable)
                //    telSurge.setForces(forceLX, forceLY, forceLZ, forceRX, forceRY, forceRZ);
             });

             Messenger.Default.Register<Messages.Signal>(this, Inputs["ForceLY"].UniqueID, (message) =>
             {
                 forceLY = message.Value;
                // if (hapticEnable)
                //    telSurge.setForces(forceLX, forceLY, forceLZ, forceRX, forceRY, forceRZ);
             });

             Messenger.Default.Register<Messages.Signal>(this, Inputs["ForceLZ"].UniqueID, (message) =>
             {
                 forceLZ = message.Value;
                // if (hapticEnable)
                  //  telSurge.setForces(forceLX, forceLY, forceLZ, forceRX, forceRY, forceRZ);
             });

             Messenger.Default.Register<Messages.Signal>(this, Inputs["ForceRX"].UniqueID, (message) =>
             {
                 forceRX = message.Value;
              //   if (hapticEnable)
                  //  telSurge.setForces(forceLX, forceLY, forceLZ, forceRX, forceRY, forceRZ);
             });

             Messenger.Default.Register<Messages.Signal>(this, Inputs["ForceRY"].UniqueID, (message) =>
             {
                 forceRY = message.Value;
               //  if (hapticEnable)
                 //   telSurge.setForces(forceLX, forceLY, forceLZ, forceRX, forceRY, forceRZ);
             });

             Messenger.Default.Register<Messages.Signal>(this, Inputs["ForceRZ"].UniqueID, (message) =>
             {
                 forceRZ = message.Value;
                // if(hapticEnable)
                   // telSurge.setForces(forceLX, forceLY, forceLZ, forceRX, forceRY, forceRZ);
             });

             Messenger.Default.Register<Messages.Signal>(this, Inputs["HapticEnable"].UniqueID, (message) =>
             {
                 if (message.Value > 0.5)
                     hapticEnable = true;
                 else
                     hapticEnable = false;
             });

             base.PostLoadSetup();
         }

        /// <summary>
        /// Create a new GeomagicTouch. 
        /// 
        /// This function only runs when creating new instances of this class -- it does *not* run when instances of this class are deserialized.
        /// </summary>
        public TelSurgeView()
        {
            this.DataContext = this;
            InitializeComponent();

            Outputs.Add("LX", new OutputSignalViewModel("Left X Position"));
            Outputs.Add("LY", new OutputSignalViewModel("Left Y Position"));
            Outputs.Add("LZ", new OutputSignalViewModel("Left Z Position"));
            Outputs.Add("LTheta1", new OutputSignalViewModel("Left Gimbal Theta 1"));
            Outputs.Add("LTheta2", new OutputSignalViewModel("Left Gimbal Theta 2"));
            Outputs.Add("LTheta3", new OutputSignalViewModel("Left Gimbal Theta 3"));
            Outputs.Add("LInkwell", new OutputSignalViewModel("Left Inkwell Switch"));
            Outputs.Add("LButton1", new OutputSignalViewModel("Left Button 1"));
            Outputs.Add("LButton2", new OutputSignalViewModel("Left Button 2"));

            Outputs.Add("RX", new OutputSignalViewModel("Right X Position"));
            Outputs.Add("RY", new OutputSignalViewModel("Right Y Position"));
            Outputs.Add("RZ", new OutputSignalViewModel("Right Z Position"));
            Outputs.Add("RTheta1", new OutputSignalViewModel("Right Gimbal Theta 1"));
            Outputs.Add("RTheta2", new OutputSignalViewModel("Right Gimbal Theta 2"));
            Outputs.Add("RTheta3", new OutputSignalViewModel("Right Gimbal Theta 3"));
            Outputs.Add("RInkwell", new OutputSignalViewModel("Right Inkwell Switch"));
            Outputs.Add("RButton1", new OutputSignalViewModel("Right Button 1"));
            Outputs.Add("RButton2", new OutputSignalViewModel("Right Button 2"));

            Outputs.Add("ExButton1", new OutputSignalViewModel("ExButton1"));
            Outputs.Add("ExButton2", new OutputSignalViewModel("ExButton2"));
            Outputs.Add("ExButton3", new OutputSignalViewModel("ExButton3"));
            Outputs.Add("ExButton4", new OutputSignalViewModel("ExButton4"));
            Outputs.Add("ExButton5", new OutputSignalViewModel("ExButton5"));
            Outputs.Add("ExButton6", new OutputSignalViewModel("ExButton6"));
            TypeName = "TelSurge";
            Outputs.Add("MessageCount", new OutputSignalViewModel("Message Count"));

            Inputs.Add("LX", new ViewModel.InputSignalViewModel("LX", this.InstanceName));
            Inputs.Add("LY", new ViewModel.InputSignalViewModel("LY", this.InstanceName));
            Inputs.Add("LZ", new ViewModel.InputSignalViewModel("LZ", this.InstanceName));

            Inputs.Add("RX", new ViewModel.InputSignalViewModel("RX", this.InstanceName));
            Inputs.Add("RY", new ViewModel.InputSignalViewModel("RY", this.InstanceName));
            Inputs.Add("RZ", new ViewModel.InputSignalViewModel("RZ", this.InstanceName));

            Inputs.Add("ForceLX", new ViewModel.InputSignalViewModel("ForceLX", this.InstanceName));
            Inputs.Add("ForceLY", new ViewModel.InputSignalViewModel("ForceLY", this.InstanceName));
            Inputs.Add("ForceLZ", new ViewModel.InputSignalViewModel("ForceLZ", this.InstanceName));

            Inputs.Add("ForceRX", new ViewModel.InputSignalViewModel("ForceRX", this.InstanceName));
            Inputs.Add("ForceRY", new ViewModel.InputSignalViewModel("ForceRY", this.InstanceName));
            Inputs.Add("ForceRZ", new ViewModel.InputSignalViewModel("ForceRZ", this.InstanceName));

            Inputs.Add("HapticEnable", new ViewModel.InputSignalViewModel("HapticEnable", this.InstanceName));
            Inputs.Add("Freeze", new ViewModel.InputSignalViewModel("Freeze", this.InstanceName));
            Inputs.Add("EmergencySwitch", new ViewModel.InputSignalViewModel("EmergencySwitch", this.InstanceName));


//            UpdateTimer = new System.Timers.Timer();
            UpdateTimer.Tick += UpdateTimer_Tick;
            UpdateTimer.Interval = UpdatePeriod;

            // Call any additional setup work that needs to happen in either constructor's case, or loading the plugin from deserialization.
            PostLoadSetup();
        }


        void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (telSurge != null)
            {
                Outputs["LX"].Value = telSurge.OutputPosition.LeftX;
                Outputs["LY"].Value = telSurge.OutputPosition.LeftY;
                Outputs["LZ"].Value = telSurge.OutputPosition.LeftZ;
                Outputs["LTheta1"].Value = telSurge.OutputPosition.Gimbal1Left;
                Outputs["LTheta2"].Value = telSurge.OutputPosition.Gimbal2Left;
                Outputs["LTheta3"].Value = telSurge.OutputPosition.Gimbal3Left;
                Outputs["LInkwell"].Value = telSurge.OutputPosition.InkwellLeft;
                Outputs["LButton1"].Value = Convert.ToDouble(telSurge.OutputPosition.ButtonsLeft.Equals(1));
                Outputs["LButton2"].Value = Convert.ToDouble(telSurge.OutputPosition.ButtonsLeft.Equals(2));

                Outputs["RX"].Value = telSurge.OutputPosition.RightX;
                Outputs["RY"].Value = telSurge.OutputPosition.RightY;
                Outputs["RZ"].Value = telSurge.OutputPosition.RightZ;
                Outputs["RTheta1"].Value = telSurge.OutputPosition.Gimbal1Right;
                Outputs["RTheta2"].Value = telSurge.OutputPosition.Gimbal2Right;
                Outputs["RTheta3"].Value = telSurge.OutputPosition.Gimbal3Right;
                Outputs["RInkwell"].Value = telSurge.OutputPosition.InkwellRight;
                Outputs["RButton1"].Value = Convert.ToDouble(telSurge.OutputPosition.ButtonsRight.Equals(1));
                Outputs["RButton2"].Value = Convert.ToDouble(telSurge.OutputPosition.ButtonsRight.Equals(2));
                Outputs["MessageCount"].Value = Convert.ToDouble(telSurge.messageCount);

                //Update output for any external buttons
                for (int i = 1; i <= telSurge.OutputPosition.ExtraButtons.Length; i++)
                {
                    string outputName = "ExButton" + i.ToString();
                    Outputs[outputName].Value = Convert.ToDouble(telSurge.OutputPosition.ExtraButtons[i-1]);
                }
                

                //bool forcesSet = false;
                //if (!Inputs["ForceLX"].Value.Equals(0) || !Inputs["ForceLY"].Value.Equals(0) || !Inputs["ForceLZ"].Value.Equals(0) || !Inputs["ForceRX"].Value.Equals(0) || !Inputs["ForceRY"].Value.Equals(0) || !Inputs["ForceRZ"].Value.Equals(0))
                //{
                //    forcesSet = telSurge.setForces(Inputs["ForceLX"].Value, Inputs["ForceLY"].Value, Inputs["ForceLZ"].Value, Inputs["ForceRX"].Value, Inputs["ForceRY"].Value, Inputs["ForceRZ"].Value);
                //}
            }
        }

        private RelayCommand startCommand;

        /// <summary>
        /// Gets the StartCommand.
        /// </summary>
        public RelayCommand StartCommand
        {
            get
            {
                return startCommand
                    ?? (startCommand = new RelayCommand(
                    () =>
                    {
                        //if (!StartCommand.CanExecute(null))
                        //{
                        //    return;
                        //}

                        //Device.Start();
                        UpdateTimer.Start();
                    },
                    () => true));
            }
        }

        //public override void Dispose()
        //{
        //    if(Device != null)
        //        Device.Stop();
        //}

        bool visualsAreSetup = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!visualsAreSetup)
            {
                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
                visualsAreSetup = true;
            }
            telSurge = new TelSurgeMain();
            telSurge.Show();
            //StartCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// The <see cref="UpdatePeriod" /> property's name.
        /// </summary>
        public const string UpdatePeriodPropertyName = "UpdatePeriod";

        private int updatePeriod = 15;

        /// <summary>
        /// Sets and gets the UpdatePeriod property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int UpdatePeriod
        {
            get
            {
                return updatePeriod;
            }

            set
            {
                if (updatePeriod == value)
                {
                    return;
                }

                updatePeriod = value;
                UpdateTimer.Interval = updatePeriod;
                RaisePropertyChanged(UpdatePeriodPropertyName);
            }
        }

    }
}

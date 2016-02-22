using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for MotorClutch.xaml
    /// </summary>
    public partial class MotorClutch : PluginBase
    {
        public double current = 0;
        public double kineSetpoint = 0;
        public double motorPosition = 0;
        public double[] oldCurrent = new double[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        public double[] oldPosition = new double[3] { 0, 0, 0 };

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Current"].UniqueID, (message) =>
            {
                if(message.Value < 2000)
                {
                    current = message.Value;
                    SetClutch();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["MotorPosition"].UniqueID, (message) =>
            {
                if(message.Value < 10000)
                {
                    motorPosition = message.Value;
                    SetClutch();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["KineSetpoint"].UniqueID, (message) =>
            {
                kineSetpoint = message.Value;
                SetClutch();
            });

            base.PostLoadSetup();
        }

        public MotorClutch()
        {
            this.TypeName = "Motor Clutch";
            InitializeComponent();

            Outputs.Add("ClutchSetpoint", new ViewModel.OutputSignalViewModel("ClutchSetpoint"));

            Inputs.Add("Current", new ViewModel.InputSignalViewModel("Current", this.InstanceName));
            Inputs.Add("MotorPosition", new ViewModel.InputSignalViewModel("MotorPosition", this.InstanceName));
            Inputs.Add("KineSetpoint", new ViewModel.InputSignalViewModel("KineSetpoint", this.InstanceName));

            PostLoadSetup();
        }

        public void SetClutch()
        {
            double avgCurrent = 0;
            //double agedCurrent = 0;
            double error;
            double motorSet;
            
            if(clutchEnabled)
            {
                //for (int i = 3; i < 8; i++)
                //{
                //    agedCurrent += oldCurrent[i];
                //}
                //agedCurrent = agedCurrent / 5;
                //avgCurrent += agedCurrent;
                //agedCurrent = 0;
                //for (int i = 0; i < 3; i++)
                //{
                //    agedCurrent += oldCurrent[i];
                //}
                //agedCurrent = agedCurrent / 3;
                //avgCurrent = (agedCurrent + avgCurrent + current) / 3;

                for (int i = 0; i < 8; i++)
                {
                    avgCurrent += oldCurrent[i];
                }
                avgCurrent = (current + avgCurrent) / 9;

                if (avgCurrent > slipTorque)
                {
                    error = avgCurrent - slipTorque;
                    if (motorPosition > kineSetpoint)
                    {
                        motorSet = motorPosition - (kp / error);
                        if (motorSet < kineSetpoint)
                            Outputs["ClutchSetpoint"].Value = kineSetpoint;
                        else
                            Outputs["ClutchSetpoint"].Value = motorSet;
                    }
                    else
                    {
                        motorSet = motorPosition + (kp / error);
                        if (motorPosition > kineSetpoint)
                            Outputs["ClutchSetpoint"].Value = kineSetpoint;
                        else
                            Outputs["ClutchSetpoint"].Value = motorSet;
                    }
                }
                else
                    Outputs["ClutchSetpoint"].Value = kineSetpoint;
            }
            else
                Outputs["ClutchSetpoint"].Value = kineSetpoint;
            
            oldCurrent[7] = oldCurrent[6];
            oldCurrent[6] = oldCurrent[5];
            oldCurrent[5] = oldCurrent[4];
            oldCurrent[4] = oldCurrent[3];
            oldCurrent[3] = oldCurrent[2];
            oldCurrent[2] = oldCurrent[1];
            oldCurrent[1] = oldCurrent[0];
            oldCurrent[0] = current;

            oldPosition[0] = motorPosition;
        }

        /// <summary>
        /// The <see cref="SlipTorque" /> property's name.
        /// </summary>
        public const string SlipTorquePropertyName = "SlipTorque";

        private double slipTorque = 800;

        /// <summary>
        /// Sets and gets the SlipTorque property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double SlipTorque
        {
            get
            {
                return slipTorque;
            }

            set
            {
                if (slipTorque == value)
                {
                    return;
                }

                slipTorque = value;
                RaisePropertyChanged(SlipTorquePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Kp" /> property's name.
        /// </summary>
        public const string KpPropertyName = "Kp";

        private double kp = 1;

        /// <summary>
        /// Sets and gets the Kp property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Kp
        {
            get
            {
                return kp;
            }

            set
            {
                if (kp == value)
                {
                    return;
                }

                kp = value;
                RaisePropertyChanged(KpPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ClutchEnabled" /> property's name.
        /// </summary>
        public const string ClutchEnabledPropertyName = "ClutchEnabled";

        private bool clutchEnabled = false;

        /// <summary>
        /// Sets and gets the EnableClutch property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool ClutchEnabled
        {
            get
            {
                return clutchEnabled;
            }

            set
            {
                if (clutchEnabled == value)
                {
                    return;
                }

                clutchEnabled = value;
                RaisePropertyChanged(ClutchEnabledPropertyName);
            }
        }
    }
}

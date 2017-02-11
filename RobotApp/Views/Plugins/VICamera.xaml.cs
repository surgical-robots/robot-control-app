using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Timers;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for Clutch.xaml
    /// </summary>
    public partial class VICamera : PluginBase
    {
        double thetaX, thetaY, mag;
        double ThetaX, ThetaY;
        System.Windows.Forms.Timer stepTimer = new System.Windows.Forms.Timer();
        bool looping = false;
        bool pause = false;
        double frameCount = 0;
        double maxTheta = 90;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["ThetaX"].UniqueID, (message) =>
            {
                thetaX = message.Value;
                mag = Math.Sqrt(Math.Pow(thetaX, 2) + Math.Pow(thetaY, 2));
                if(mag > 1)
                {
                    ThetaX = thetaX / mag * maxTheta;
                    ThetaY = thetaY / mag * maxTheta;
                }
                else
                {
                    ThetaX = thetaX * maxTheta;
                    ThetaY = thetaY * maxTheta;
                }
                if (!looping)
                {
                    Outputs["ThetaX"].Value = ThetaX;
                    Outputs["ThetaY"].Value = ThetaY;
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["ThetaY"].UniqueID, (message) =>
            {
                thetaY = message.Value;
                mag = Math.Sqrt(Math.Pow(thetaX, 2) + Math.Pow(thetaY, 2));
                if (mag > 1)
                {
                    ThetaX = thetaX / mag * maxTheta;
                    ThetaY = thetaY / mag * maxTheta;
                }
                else
                {
                    ThetaX = thetaX * maxTheta;
                    ThetaY = thetaY * maxTheta;
                }
                if(!looping)
                {
                    Outputs["ThetaX"].Value = ThetaX;
                    Outputs["ThetaY"].Value = ThetaY;
                }
            });

            base.PostLoadSetup();
        }

        public VICamera()
        {
            this.TypeName = "VIC Camera";
            InitializeComponent();

            // OUTPUTS
            Outputs.Add("ThetaX", new ViewModel.OutputSignalViewModel("Theta X"));
            Outputs.Add("ThetaY", new ViewModel.OutputSignalViewModel("Theta Y"));

            // INPUTS
            Inputs.Add("ThetaX", new ViewModel.InputSignalViewModel("Theta X", this.InstanceName));
            Inputs.Add("ThetaY", new ViewModel.InputSignalViewModel("Theta Y", this.InstanceName));

            stepTimer.Interval = timerInterval;
            stepTimer.Tick += stepTimer_Tick;

            PostLoadSetup();
        }

        void stepTimer_Tick(object sender, EventArgs e)
        {
            updatePosition();
        }

        void updatePosition()
        {
            Outputs["ThetaX"].Value = WaveAmplitude * Math.Sin(2 * Math.PI * (frameCount / 300));
            Outputs["ThetaY"].Value = WaveAmplitude * Math.Cos(2 * Math.PI * (frameCount / 300));
            frameCount++;
        }

        /// <summary>
        /// The <see cref="TimerInterval" /> property's name.
        /// </summary>
        public const string TimerIntervalPropertyName = "TimerInterval";

        private int timerInterval = 15;

        /// <summary>
        /// Sets and gets the TimerInterval property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int TimerInterval
        {
            get
            {
                return timerInterval;
            }

            set
            {
                if (timerInterval == value)
                {
                    return;
                }

                timerInterval = value;
                if (timerInterval < 15)
                    stepTimer.Interval = 15;
                else
                    stepTimer.Interval = timerInterval;
                RaisePropertyChanged(TimerIntervalPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="WaveAmplitude" /> property's name.
        /// </summary>
        public const string WaveAmplitudePropertyName = "WaveAmplitude";

        private int waveAmplitude = 45;

        /// <summary>
        /// Sets and gets the WaveAmplitude property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int WaveAmplitude
        {
            get
            {
                return waveAmplitude;
            }

            set
            {
                if (waveAmplitude == value)
                {
                    return;
                }
                
                if (waveAmplitude > 90)
                    waveAmplitude = 90;
                else
                    waveAmplitude = value;
                RaisePropertyChanged(WaveAmplitudePropertyName);
            }
        }

        private RelayCommand<string> pathCommand;

        /// <summary>
        /// Gets the StartCommand.
        /// </summary>
        public RelayCommand<string> PathCommand
        {
            get
            {
                return pathCommand
                    ?? (pathCommand = new RelayCommand<string>(
                    p =>
                    {
                        if (!looping)
                        {
                            stepTimer.Start();
                            looping = true;
                            pause = false;
                            ButtonText = "Pause Loop";
                        }
                        else if (pause == true)
                        {
                            stepTimer.Start();
                            pause = false;
                            ButtonText = "Pause Loop";
                        }
                        else
                        {
                            stepTimer.Stop();
                            pause = true;
                            looping = false;
                            ButtonText = "Resume Loop";
                        }
                    }));
            }
        }

        /// <summary>
        /// The <see cref="ButtonText" /> property's name.
        /// </summary>
        public const string ButtonTextPropertyName = "ButtonText";

        private string buttonText = "Start Camera Loop";

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
    }
}

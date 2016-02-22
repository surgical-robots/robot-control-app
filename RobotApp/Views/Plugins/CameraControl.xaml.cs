using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for HapticWorkspace.xaml
    /// </summary>
    public partial class CameraControl : PluginBase
    {
        private double XR, YR, ZR, XL, YL, ZL;
        private double PI1, YA1;
        private double pitch, yaw;
        private bool wasMovingR = false;
        private bool wasMovingL = false;
        private Queue<double> oldRX = new Queue<double>(200);
        private Queue<double> oldRY = new Queue<double>(200);
        private Queue<double> oldRZ = new Queue<double>(200);
        private Queue<double> oldLX = new Queue<double>(200);
        private Queue<double> oldLY = new Queue<double>(200);
        private Queue<double> oldLZ = new Queue<double>(200);

        // private bool pause = false;
        //private bool clutch = false;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["XR"].UniqueID, (message) =>
            {
                XR = message.Value;
                if (Toggle == 1)
                {
                    Tracking();
                    UpdateOutput();
                }
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["YR"].UniqueID, (message) =>
            {
                YR = message.Value;
                if (Toggle == 1)
                {
                    Tracking();
                    UpdateOutput();
                }
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["ZR"].UniqueID, (message) =>
            {
                ZR = message.Value;
                if (Toggle == 1)
                {
                    Tracking();
                    UpdateOutput();
                }
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["XL"].UniqueID, (message) =>
            {
                XL = message.Value;
                if (Toggle == 1)
                {
                    Tracking();
                    UpdateOutput();
                }
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["YL"].UniqueID, (message) =>
            {
                YL = message.Value;
                if (Toggle == 1)
                {
                    Tracking();
                    UpdateOutput();
                }
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["ZL"].UniqueID, (message) =>
            {
                ZL = message.Value;
                if (Toggle == 1)
                {
                    Tracking();
                    UpdateOutput();
                }
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Pitch"].UniqueID, (message) =>
            {
                PI1 = message.Value;
                //if (Toggle == 2)
                //{
                //    ManualMode();
                //    UpdateOutput();
                //}
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Yaw"].UniqueID, (message) =>
            {
                YA1 = message.Value;
                //if (Toggle == 2)
                //{
                //    ManualMode();
                //    UpdateOutput();
                //}
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["ModeButton"].UniqueID, (message) =>
            {
                if(message.Value > 0.5)
                {
                    if (Toggle == 1)
                        Toggle = 2;
                    else
                        Toggle = 1;
                }
            });
            base.PostLoadSetup();
        }

        public CameraControl()
        {
            this.TypeName = "Camera Control";
            InitializeComponent();

            Outputs.Add("Pitch", new ViewModel.OutputSignalViewModel("Pitch"));
            Outputs.Add("Yaw", new ViewModel.OutputSignalViewModel("Yaw"));

            Inputs.Add("XR", new ViewModel.InputSignalViewModel("XR", this.InstanceName));
            Inputs.Add("YR", new ViewModel.InputSignalViewModel("YR", this.InstanceName));
            Inputs.Add("ZR", new ViewModel.InputSignalViewModel("ZR", this.InstanceName));
            Inputs.Add("XL", new ViewModel.InputSignalViewModel("XL", this.InstanceName));
            Inputs.Add("YL", new ViewModel.InputSignalViewModel("YL", this.InstanceName));
            Inputs.Add("ZL", new ViewModel.InputSignalViewModel("ZL", this.InstanceName));
            Inputs.Add("Pitch", new ViewModel.InputSignalViewModel("Pitch", this.InstanceName));
            Inputs.Add("Yaw", new ViewModel.InputSignalViewModel("Yaw", this.InstanceName));
            Inputs.Add("ModeButton", new ViewModel.InputSignalViewModel("Mode Button", this.InstanceName));

            PostLoadSetup();
            Tracking();
            UpdateOutput();
        }

        public void UpdateOutput()
        {
            Outputs["Pitch"].Value = pitch;
            Outputs["Yaw"].Value = yaw;
        }

        public void Tracking()
        {
            double Xout, Yout, Zout;
            double avgXL, avgYL, avgZL, avgXR, avgYR, avgZR;
            double rightR, leftR;
            bool movingR, movingL;

            // Keep track of old position in queue
            oldLX.Enqueue(XL);
            oldLY.Enqueue(YL);
            oldLZ.Enqueue(ZL);
            oldRX.Enqueue(XR);
            oldRY.Enqueue(YR);
            oldRZ.Enqueue(ZR);
            // Average old positions
            avgXL = oldLX.Average();
            avgYL = oldLY.Average();
            avgZL = oldLZ.Average();
            avgXR = oldRX.Average();
            avgYR = oldRY.Average();
            avgZR = oldRZ.Average();
            // Only keep track of 200 points
            if (oldLX.Count > 200)
            {
                oldLX.Dequeue();
                oldLY.Dequeue();
                oldLZ.Dequeue();
                oldRX.Dequeue();
                oldRY.Dequeue();
                oldRZ.Dequeue();
            }
            // Find distance from average to actual position
            rightR = Math.Sqrt(Math.Pow((avgXR - XR), 2) + Math.Pow((avgYR - YR), 2) + Math.Pow((avgZR - ZR), 2));
            leftR = Math.Sqrt(Math.Pow((avgXL - XL), 2) + Math.Pow((avgYL - YL), 2) + Math.Pow((avgZL - ZL), 2));
            // Determine if movement should be tracked
            if (rightR > MoveLimit)
                movingR = true;
            else
                movingR = false;
            if (leftR > MoveLimit)
                movingL = true;
            else
                movingL = false;
            // Output based on what is moving
            if(movingL && !movingR)
            {
                Xout = -XL;
                Yout = YL;
                Zout = ZL;
                wasMovingL = true;
                wasMovingR = false;
            }
            else if( movingR && !movingL)
            {
                Xout = XR;
                Yout = YR;
                Zout = ZR;
                wasMovingR = true;
                wasMovingL = false;
            }
            else if(!movingL && !movingR && wasMovingL)
            {
                Xout = -XL;
                Yout = YL;
                Zout = ZL;
            }
            else if (!movingR && !movingL && wasMovingR)
            {
                Xout = XR;
                Yout = YR;
                Zout = ZR;
            }
            else
            {
                Xout = (-XL + XR) / 2;
                Yout = (YL + YR) / 2;
                Zout = (ZL + ZR) / 2;
                wasMovingR = false;
                wasMovingL = false;
            }
            Kinematics(Xout, Yout, Zout);
        }

        public void Kinematics(double Xin, double Yin, double Zin)
        {
            double Length;
            double c1 = 35;
            double c2 = 90;

            // AUTOMATIC TOOL TRACKING
            if (toggle == 1)
            {
                Length = Math.Sqrt(Math.Pow(Xin, 2) + Math.Pow((Yin - shaftLength), 2) + Math.Pow(Zin, 2));
                double theta1 = (Yin - shaftLength) / Length;
                theta1 = Math.Asin(theta1) * (180 / Math.PI) + 90;
                theta1 = theta1 * c1 / c2;
                if (theta1 > 30)
                    theta1 = 30;
                if (theta1 < 0)
                    theta1 = 0;
                pitch = theta1;

                double theta2 = Xin / (Math.Sqrt(Math.Pow(Xin, 2) + Math.Pow(Zin, 2)));
                theta2 = -Math.Asin(theta2) * (180 / Math.PI);
                theta2 = theta2 * c1 / c2;
                if (theta2 > 15)
                    theta2 = 15;
                if (theta2 < -15)
                    theta2 = -15;
                yaw = theta2;
            }
            else
                return;
        }

        public void ManualMode()
        {
            // MANUAL TOOL TRACKING
            if (toggle == 2)
            {
                if (PI1 < 30)
                    if (PI1 > 0)
                    {
                        pitch = PI1;
                    }
                if (YA1 < 15)
                    if (YA1 > -15)
                    {
                        yaw = YA1;
                    }
            }
            else
                return;
        }

        /// <summary>
        /// The <see cref="toggle" /> property's name.
        /// </summary>
        public const string TogglePropertyName = "toggle";

        private int toggle = 2;

        /// <summary>
        /// Sets and gets the ArmSide property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int Toggle
        {
            get
            {
                return toggle;
            }

            set
            {
                if (toggle == value)
                {
                    return;
                }

                toggle = value;
                RaisePropertyChanged(TogglePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="shaftLength" /> property's name.
        /// </summary>
        public const string ShaftLengthPropertyName = "shaftLength";

        private int shaftLength = 30;

        /// <summary>
        /// Sets and gets the ArmSide property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int ShaftLength
        {
            get
            {
                return shaftLength;
            }

            set
            {
                if (shaftLength == value)
                {
                    return;
                }

                shaftLength = value;
                RaisePropertyChanged(ShaftLengthPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="MoveLimit" /> property's name.
        /// </summary>
        public const string MoveLimitPropertyName = "MoveLimit";

        private double moveLimit = 10;

        /// <summary>
        /// Sets and gets the MoveLimit property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double MoveLimit
        {
            get
            {
                return moveLimit;
            }

            set
            {
                if (moveLimit == value)
                {
                    return;
                }

                moveLimit = value;
                RaisePropertyChanged(MoveLimitPropertyName);
            }
        }

    }
}

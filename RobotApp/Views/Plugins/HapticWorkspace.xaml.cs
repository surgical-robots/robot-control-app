using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;


namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for HapticWorkspace.xaml
    /// </summary>
    public partial class HapticWorkspace : PluginBase
    {
        private double x, y, z;
        private double avgX, avgY, avgZ;
        private double upperBevel, lowerBevel, elbow;
        private double setX, setY, setZ;
        private double pauseX, pauseY, pauseZ;
        private double forceX, forceY, forceZ;
        double actualX, actualY, actualZ;
        private bool pause = false;
        private Queue<double> oldX = new Queue<double>(200);
        private Queue<double> oldY = new Queue<double>(200);
        private Queue<double> oldZ = new Queue<double>(200);
        private Queue<double> oldSetX = new Queue<double>(200);
        private Queue<double> oldSetY = new Queue<double>(200);
        private Queue<double> oldSetZ = new Queue<double>(200);
        private Queue<double> oldForceX = new Queue<double>(200);
        private Queue<double> oldForceY = new Queue<double>(200);
        private Queue<double> oldForceZ = new Queue<double>(200);
        public System.Timers.Timer VibrationTimer = new System.Timers.Timer();
        private bool vibrateDir = false;
        private double grasperCurrent = 0;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["X"].UniqueID, (message) =>
            {
                x = message.Value;
                //if (oldX == 0)
                //    oldX = x;
                if (armSide != 0)
                {
                    Barriers();
                    UpdateOutput();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Y"].UniqueID, (message) =>
            {
                y = message.Value;
                //if (oldY == 0)
                //    oldY = y;
                if (armSide != 0)
                {
                    Barriers();
                    UpdateOutput();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Z"].UniqueID, (message) =>
            {
                z = message.Value;
                //if (oldZ == 0)
                //    oldZ = z;
                if (armSide != 0)
                {
                    Barriers();
                    UpdateOutput();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["UpperBevel"].UniqueID, (message) =>
            {
                upperBevel = message.Value;
                if (armSide != 0)
                {
                    ForwardKine();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LowerBevel"].UniqueID, (message) =>
            {
                lowerBevel = message.Value;
                if (armSide != 0)
                {
                    ForwardKine();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Elbow"].UniqueID, (message) =>
            {
                elbow = message.Value;
                if (armSide != 0)
                {
                    ForwardKine();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["HapticsPause"].UniqueID, (message) =>
            {
                if ((pause == true) && (message.Value == 1))
                    pause = false;
                else if ((pause == false) && (message.Value == 1))
                {
                    pause = true;
                    pauseX = setX;
                    pauseY = setY;
                    pauseZ = setZ;
                }
                if (armSide != 0)
                {
                    Barriers();
                    UpdateOutput();
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["GrasperCurrent"].UniqueID, (message) =>
            {
                grasperCurrent = message.Value;
                if (grasperCurrent > 300 && !VibrationTimer.Enabled)
                  VibrationTimer.Start();
                else if (grasperCurrent <= 300 && VibrationTimer.Enabled)
                //else
                  VibrationTimer.Stop();
            });

            base.PostLoadSetup();
        }

        public HapticWorkspace()
        {
            this.TypeName = "Haptic Workspace";
            InitializeComponent();

            Outputs.Add("SetX", new ViewModel.OutputSignalViewModel("SetX"));
            Outputs.Add("SetY", new ViewModel.OutputSignalViewModel("SetY"));
            Outputs.Add("SetZ", new ViewModel.OutputSignalViewModel("SetZ"));
            Outputs.Add("ForceX", new ViewModel.OutputSignalViewModel("ForceX"));
            Outputs.Add("ForceY", new ViewModel.OutputSignalViewModel("ForceY"));
            Outputs.Add("ForceZ", new ViewModel.OutputSignalViewModel("ForceZ"));
            Outputs.Add("EnableHaptics", new ViewModel.OutputSignalViewModel("Enable Haptics"));

            Inputs.Add("X", new ViewModel.InputSignalViewModel("X", this.InstanceName));
            Inputs.Add("Y", new ViewModel.InputSignalViewModel("Y", this.InstanceName));
            Inputs.Add("Z", new ViewModel.InputSignalViewModel("Z", this.InstanceName));
            Inputs.Add("UpperBevel", new ViewModel.InputSignalViewModel("Upper Bevel", this.InstanceName));
            Inputs.Add("LowerBevel", new ViewModel.InputSignalViewModel("Lower Bevel", this.InstanceName));
            Inputs.Add("Elbow", new ViewModel.InputSignalViewModel("Elbow", this.InstanceName));
            Inputs.Add("HapticsPause", new ViewModel.InputSignalViewModel("Haptics Pause", this.InstanceName));
            Inputs.Add("GrasperCurrent", new ViewModel.InputSignalViewModel("Grasper Current", this.InstanceName));

            VibrationTimer.Interval = 5;
            VibrationTimer.Elapsed += VibrationTimer_Elapsed;
//            VibrationTimer.Start();

            PostLoadSetup();
        }

        void VibrationTimer_Elapsed(object sender, EventArgs e)
        {
            if (vibrateDir)
            {
                Outputs["ForceX"].Value = forceX + (grasperCurrent - 300) / 50;
                vibrateDir = false;
            }
            else
            {
                Outputs["ForceX"].Value = forceX - (grasperCurrent - 300) / 50;
                vibrateDir = true;
            }
        }

        public void UpdateOutput()
        {
            Outputs["SetX"].Value = setX;
            Outputs["SetY"].Value = setY;
            Outputs["SetZ"].Value = setZ;

            Outputs["ForceX"].Value = forceX;
            Outputs["ForceY"].Value = forceY;
            Outputs["ForceZ"].Value = forceZ;
        }

        public void ForwardKine()
        {
            double[] theta = new double [3] {0, 0, 0};
            theta[0] = Math.PI * (upperBevel + lowerBevel) / 360;
            theta[1] = -Math.PI * (upperBevel - lowerBevel) / 360;
            theta[2] = Math.PI * elbow / 180;

            actualZ = upperLength * Math.Cos(theta[0]) * Math.Cos(theta[1]) - foreLength * (Math.Sin(theta[0]) * Math.Sin(theta[2]) - Math.Cos(theta[0]) * Math.Cos(theta[1]) * Math.Cos(theta[2]));
            actualY = upperLength * Math.Sin(theta[1]) + foreLength * Math.Sin(theta[1]) * Math.Cos(theta[2]);
            actualX = upperLength * Math.Sin(theta[0]) * Math.Cos(theta[1]) + foreLength * (Math.Cos(theta[0]) * Math.Sin(theta[2]) + Math.Sin(theta[0]) * Math.Cos(theta[1]) * Math.Cos(theta[2]));

            oldX.Enqueue(actualX);
            oldY.Enqueue(actualY);
            oldZ.Enqueue(actualZ);

            avgX = oldX.Average();
            avgY = oldY.Average();
            avgZ = oldZ.Average();

            if(oldX.Count > 15)
            {
                oldX.Dequeue();
                oldY.Dequeue();
                oldZ.Dequeue();
            }
        }

        public void Barriers()
        {
            double Lmax = (upperLength + foreLength) * 0.999;
            double minShoulderRadians = minShoulderTheta * Math.PI / 180;
            double maxShoulderRadians = maxShoulderTheta * Math.PI / 180;
            double minElbowRadians = minElbowTheta * Math.PI / 180;
            double maxElbowRadians = maxElbowTheta * Math.PI / 180;
            double radius;
            double setScale;
            double c = upperLength * Math.Sin(maxShoulderRadians);
            double xShiftOne = upperLength * Math.Cos(maxShoulderRadians);
            double torusOneTheta = Math.Atan(y / z);
            double yShiftOne = c * Math.Sin(torusOneTheta);
            double zShiftOne = c * Math.Cos(torusOneTheta);
            double torusTwoTheta = Math.Atan(y / x);
            double xShiftTwo = upperLength * Math.Cos(torusTwoTheta);
            double yShiftTwo = upperLength * Math.Sin(torusTwoTheta);
            double a = foreLength;
            double torusOneR, torusTwoR;
            double xMin = upperLength * Math.Sin(maxElbowRadians) - foreLength;
            double sphereOneLimit = Lmax * Math.Sin(maxShoulderRadians);
            double sphereThreeX = upperLength + Math.Cos(maxElbowRadians) * foreLength;
            double sphereThreeY = Math.Sin(maxElbowRadians) * foreLength;
            double sphereTwoR = Math.Sqrt(Math.Pow(sphereThreeX, 2) + Math.Pow(sphereThreeY, 2));
            double phi = Math.Tan(z / x);
            //double viscousGain = 1.0;
            //double velX = 0; double velY = 0; double velZ = 0;
            //double avgSetX = 0;
            //double avgSetY = 0;
            //double avgSetZ = 0;
            double deltaX, deltaY, deltaZ;
            double avgFX = 0;
            double avgFY = 0;
            double avgFZ = 0;
            double fMax = 5;
            double smoothLimit = 2;

            // Setpoints start on input coordinates
            setX = x;
            setY = y;
            setZ = z;

            radius = Math.Sqrt(Math.Pow(setX, 2) + Math.Pow(setY, 2) + Math.Pow(setZ, 2));
            torusOneR = Math.Sqrt(Math.Pow(c - Math.Sqrt(Math.Pow(setY, 2) + Math.Pow(setZ, 2)), 2) + Math.Pow((setX - xShiftOne), 2));
            torusTwoR = Math.Sqrt(Math.Pow(upperLength - Math.Sqrt(Math.Pow(setX, 2) + Math.Pow(setY, 2)), 2) + Math.Pow(setZ, 2));
            // Outside barriers
            if ((radius > Lmax) && (x <= sphereOneLimit))   // Sphere 1 
            {
                setScale = Lmax / radius;
                setX = setX * setScale;
                setY = setY * setScale;
                setZ = setZ * setScale;
            }
            if ((torusOneR > a) && (x > sphereOneLimit) && (radius > sphereTwoR))  // Torus 1
            {
                setScale = a / torusOneR;
                setX = ((setX + xShiftOne) * setScale) - xShiftOne;
                setY = ((setY - yShiftOne) * setScale) + yShiftOne;
                setZ = ((setZ - zShiftOne) * setScale) + zShiftOne;
            }
            //if ((torusTwoR < a) && (x < 0) && (x > -25))
            //    setX = 0;
            if ((torusTwoR < a)) // && (x <= -25))  // Torus 2
            {
                setScale = a / torusTwoR;
                setX = ((setX + xShiftTwo) * setScale) - xShiftTwo;
                setY = ((setY + yShiftTwo) * setScale) - yShiftTwo;
                setZ = setZ * setScale;
            }
            // Sphere 3 force inside outward
            radius = Math.Sqrt(Math.Pow(x, 2) + Math.Pow((y + upperLength), 2) + Math.Pow(z, 2));
            if ((radius < foreLength) && (y < -35.56)) // && (x > -25))
            {
                setScale = foreLength / radius;
                setX = x * setScale;
                setY = ((y + upperLength) * setScale) - upperLength;
                setZ = z * setScale;
            }
            // Shpere 2, force inside outward
            radius = Math.Sqrt(Math.Pow(setX, 2) + Math.Pow(setY, 2) + Math.Pow(setZ, 2));
            if ((radius < sphereTwoR) && (y >= -35.56)) // && (x > -25))
            {
                setScale = sphereTwoR / radius;
                setX = x * setScale;
                setY = y * setScale;
                setZ = z * setScale;
            }
            // Ceiling
            if (setY > 0)
                setY = 0;
            // Back Wall
            if (setZ < 0)
                setZ = 0;

            if(pause == true)
            {
                setX = pauseX;
                setY = pauseY;
                setZ = pauseZ;
            }
            // Boundary Forces
            if (armSide == 2)
            {
                forceX = (setX - x) * barrierGain + (actualX - x) * feelGain; // -(velX * viscousGain);
            }
            else
                forceX = (setX - x) * -barrierGain - (actualX - x) * feelGain; // +(velX * viscousGain);
            forceY = (setY - y) * barrierGain + (actualY - y) * feelGain; // -(velY * viscousGain);
            forceZ = (setZ - z) * -barrierGain - (actualZ - z) * feelGain; // -(velX * viscousGain);
            // Haptic Forces
            //deltaX = actualX - x;
            //deltaY = actualY - y;
            //deltaZ = actualZ - z;
            deltaX = avgX - x;
            deltaY = avgY - y;
            deltaZ = avgZ - z;
            double deltaR = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2) + Math.Pow(deltaZ, 2));
            if(deltaR > HapticThreshold)
            {
                double scaleFactor = HapticThreshold / deltaR;
                int power = 3;
                double errorX = deltaX * (1 - scaleFactor);
                if(errorX < 0)
                    errorX = -Math.Pow(errorX, power);
                else
                    errorX = Math.Pow(errorX, power);
                double errorY = deltaY * (1 - scaleFactor);
                if (errorY < 0)
                    errorY = -Math.Pow(errorY, power);
                else
                    errorY = Math.Pow(errorY, power);
                double errorZ = deltaZ * (1 - scaleFactor);
                if (errorZ < 0)
                    errorZ = -Math.Pow(errorZ, power);
                else
                    errorZ = Math.Pow(errorZ, power);
                if (armSide == 2)
                {
                    forceX += Math.Pow((deltaX * feelGain), 3);
                }
                else
                    forceX -= Math.Pow((deltaX * feelGain), 3);
                forceY += Math.Pow((deltaY * feelGain), 3);
                forceZ -= Math.Pow((deltaZ * feelGain), 3);
            }
            // Force Limits
            if (forceX > ForceMax)
                forceX = ForceMax;
            else if (forceX < -ForceMax)
                forceX = -ForceMax;
            if (forceY > ForceMax)
                forceY = ForceMax;
            else if (forceY < -ForceMax)
                forceY = -ForceMax;
            if (forceZ > ForceMax)
                forceZ = ForceMax;
            else if (forceZ < -ForceMax)
                forceZ = -ForceMax;

            //oldForceX.Enqueue(forceX);
            //oldForceY.Enqueue(forceY);
            //oldForceZ.Enqueue(forceZ);

            //avgFX = oldForceX.Average();
            //avgFY = oldForceY.Average();
            //avgFZ = oldForceZ.Average();

            //double setDiff = Math.Sqrt(Math.Pow((avgFX - forceX), 2) + Math.Pow((avgFY - forceY), 2) + Math.Pow((avgFZ - forceZ), 2));
            //if(setDiff > smoothLimit)
            //{
            //    forceX = avgFX;
            //    forceY = avgFY;
            //    forceZ = avgFZ;
            //}

            //// cycle out old data
            //if (oldForceX.Count > 10)
            //{
            //    oldForceX.Dequeue();
            //    oldForceY.Dequeue();
            //    oldForceZ.Dequeue();
            //}
        }

        /// <summary>
        /// The <see cref="ArmSide" /> property's name.
        /// </summary>
        public const string ArmSidePropertyName = "ArmSide";

        private int armSide = 0;

        /// <summary>
        /// Sets and gets the ArmSide property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int ArmSide
        {
            get
            {
                return armSide;
            }

            set
            {
                if (armSide == value)
                {
                    return;
                }

                armSide = value;
                RaisePropertyChanged(ArmSidePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="UpperLength" /> property's name.
        /// </summary>
        public const string UpperLengthPropertyName = "UpperLength";

        private double upperLength = 68.58;

        /// <summary>
        /// Sets and gets the UpperLength property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double UpperLength
        {
            get
            {
                return upperLength;
            }

            set
            {
                if (upperLength == value)
                {
                    return;
                }

                upperLength = value;
                RaisePropertyChanged(UpperLengthPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ForeLength" /> property's name.
        /// </summary>
        public const string ForeLengthPropertyName = "ForeLength";

        private double foreLength = 96.393;

        /// <summary>
        /// Sets and gets the ForeLength property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double ForeLength
        {
            get
            {
                return foreLength;
            }

            set
            {
                if (foreLength == value)
                {
                    return;
                }

                foreLength = value;
                RaisePropertyChanged(ForeLengthPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="MinShoulderTheta" /> property's name.
        /// </summary>
        public const string MinShoulderThetaPropertyName = "MinShoulderTheta";

        private double minShoulderTheta = 0;

        /// <summary>
        /// Sets and gets the MinShoulderTheta property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double MinShoulderTheta
        {
            get
            {
                return minShoulderTheta;
            }

            set
            {
                if (minShoulderTheta == value)
                {
                    return;
                }

                minShoulderTheta = value;
                RaisePropertyChanged(MinShoulderThetaPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="MaxShoulderTheta" /> property's name.
        /// </summary>
        public const string MaxShoulderThetaPropertyName = "MaxShoulderTheta";

        private double maxShoulderTheta = 90;

        /// <summary>
        /// Sets and gets the MaxShoulderTheta property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double MaxShoulderTheta
        {
            get
            {
                return maxShoulderTheta;
            }

            set
            {
                if (maxShoulderTheta == value)
                {
                    return;
                }

                maxShoulderTheta = value;
                RaisePropertyChanged(MaxShoulderThetaPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="MinElbowTheta" /> property's name.
        /// </summary>
        public const string MinElbowThetaPropertyName = "MinElbowTheta";

        private double minElbowTheta = 0;

        /// <summary>
        /// Sets and gets the MinElbowTheta property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double MinElbowTheta
        {
            get
            {
                return minElbowTheta;
            }

            set
            {
                if (minElbowTheta == value)
                {
                    return;
                }

                minElbowTheta = value;
                RaisePropertyChanged(MinElbowThetaPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="MaxElbowTheta" /> property's name.
        /// </summary>
        public const string MaxElbowThetaPropertyName = "MaxElbowTheta";

        private double maxElbowTheta = 100;

        /// <summary>
        /// Sets and gets the MaxElbowTheta property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double MaxElbowTheta
        {
            get
            {
                return maxElbowTheta;
            }

            set
            {
                if (maxElbowTheta == value)
                {
                    return;
                }

                maxElbowTheta = value;
                RaisePropertyChanged(MaxElbowThetaPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="HapticsEnabled" /> property's name.
        /// </summary>
        public const string HapticsEnabledPropertyName = "HapticsEnabled";

        private bool hapticsEnabled = false;

        /// <summary>
        /// Sets and gets the HapticsEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool HapticsEnabled
        {
            get
            {
                return hapticsEnabled;
            }

            set
            {
                if (hapticsEnabled == value)
                {
                    return;
                }

                hapticsEnabled = value;

                if (hapticsEnabled == true)
                    Outputs["EnableHaptics"].Value = 1;
                else
                    Outputs["EnableHaptics"].Value = 0;

                RaisePropertyChanged(HapticsEnabledPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="BarrierGain" /> property's name.
        /// </summary>
        public const string BarrierGainPropertyName = "BarrierGain";

        private double barrierGain = 0.1;

        /// <summary>
        /// Sets and gets the ForceGain property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double BarrierGain
        {
            get
            {
                return barrierGain;
            }

            set
            {
                if (barrierGain == value)
                {
                    return;
                }

                barrierGain = Math.Round( value, 3 );
                RaisePropertyChanged(BarrierGainPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="FeelGain" /> property's name.
        /// </summary>
        public const string FeelGainPropertyName = "FeelGain";

        private double feelGain = 0;

        /// <summary>
        /// Sets and gets the FeelGain property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double FeelGain
        {
            get
            {
                return feelGain;
            }

            set
            {
                if (feelGain == value)
                {
                    return;
                }

                feelGain = Math.Round(value, 3);
                RaisePropertyChanged(FeelGainPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="HapticThreshold" /> property's name.
        /// </summary>
        public const string HapticThresholdPropertyName = "HapticThreshold";

        private double hapticThreshold = 3;

        /// <summary>
        /// Sets and gets the HapticThreshold property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double HapticThreshold
        {
            get
            {
                return hapticThreshold;
            }

            set
            {
                if (hapticThreshold == value)
                {
                    return;
                }

                hapticThreshold = Math.Round(value, 3);
                RaisePropertyChanged(HapticThresholdPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ForceMax" /> property's name.
        /// </summary>
        public const string ForceMaxPropertyName = "ForceMax";

        private double forceMax = 4;

        /// <summary>
        /// Sets and gets the ForceMax property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double ForceMax
        {
            get
            {
                return forceMax;
            }

            set
            {
                if (forceMax == value)
                {
                    return;
                }

                forceMax = Math.Round(value, 3);
                RaisePropertyChanged(ForceMaxPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="RobotSelector" /> property's name.
        /// </summary>
        public const string RobotSelectorPropertyName = "RobotSelector";

        private int robotSelector = 0;

        /// <summary>
        /// Sets and gets the RobotSelector property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int RobotSelector
        {
            get
            {
                return robotSelector;
            }

            set
            {
                if (robotSelector == value)
                {
                    return;
                }

                robotSelector = value;
                RaisePropertyChanged(RobotSelectorPropertyName);

                if(robotSelector == 1)
                {
                    UpperLength = 68.58;
                    ForeLength = 96.393;
                }
                else if(robotSelector == 2)
                {
                    UpperLength = 54;
                    ForeLength = 63;
                }
            }
        }

    }
}

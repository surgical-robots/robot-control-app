using GalaSoft.MvvmLight.Command;
using RobotApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using GeomagicTouch;
using GalaSoft.MvvmLight.Messaging;


namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for HapticWorkspace.xaml
    /// </summary>
    public partial class HapticsOneDOF : PluginBase
    {
        private double angleOutput;
        private double x, y, z;
        private double motorAngle;
        private double forceX, forceY, forceZ;
        private bool pause = false;
        private Queue<double> oldZ = new Queue<double>(200);
        private Queue<double> oldSetZ = new Queue<double>(200);
        private Queue<double> oldForceX = new Queue<double>(200);
        private Queue<double> oldForceY = new Queue<double>(200);
        private Queue<double> oldForceZ = new Queue<double>(200);

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["X"].UniqueID, (message) =>
            {
                x = message.Value;
                    Barriers();
                    UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Y"].UniqueID, (message) =>
            {
                y = message.Value;
                    Barriers();
                    UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Z"].UniqueID, (message) =>
            {
                z = message.Value;
                    Barriers();
                    UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["MotorAngle"].UniqueID, (message) =>
            {
                if(motorAngle != message.Value)
                {
                    motorAngle = message.Value;
                    ForwardKine();
                }
            });

            base.PostLoadSetup();
        }

        public HapticsOneDOF()
        {
            this.TypeName = "One DOF Haptics";
            InitializeComponent();

            Outputs.Add("ForceX", new ViewModel.OutputSignalViewModel("ForceX"));
            Outputs.Add("ForceY", new ViewModel.OutputSignalViewModel("ForceY"));
            Outputs.Add("ForceZ", new ViewModel.OutputSignalViewModel("ForceZ"));
            Outputs.Add("EnableHaptics", new ViewModel.OutputSignalViewModel("Enable Haptics"));
            Outputs.Add("AngleOutput", new ViewModel.OutputSignalViewModel("Angle Output"));

            Inputs.Add("X", new ViewModel.InputSignalViewModel("X", this.InstanceName));
            Inputs.Add("Y", new ViewModel.InputSignalViewModel("Y", this.InstanceName));
            Inputs.Add("Z", new ViewModel.InputSignalViewModel("Z", this.InstanceName));
            Inputs.Add("MotorAngle", new ViewModel.InputSignalViewModel("Motor Angle", this.InstanceName));

            PostLoadSetup();
        }

        public void UpdateOutput()
        {
            Outputs["AngleOutput"].Value = angleOutput;

            Outputs["ForceX"].Value = forceX;
            Outputs["ForceY"].Value = forceY;
            Outputs["ForceZ"].Value = forceZ;
        }

        public void ForwardKine()
        {
        }

        public void Barriers()
        {

        }

        /// <summary>
        /// The <see cref="ArmLength" /> property's name.
        /// </summary>
        public const string ArmLengthPropertyName = "ArmLength";

        private double armLength = 68.58;

        /// <summary>
        /// Sets and gets the UpperLength property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double ArmLength
        {
            get
            {
                return armLength;
            }

            set
            {
                if (armLength == value)
                {
                    return;
                }

                armLength = value;
                RaisePropertyChanged(ArmLengthPropertyName);
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

                barrierGain = value;
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

                feelGain = value;
                RaisePropertyChanged(FeelGainPropertyName);
            }
        }

    }
}

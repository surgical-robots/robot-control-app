using Kinematics;
using System;
using System.Windows.Media;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for Kinematics.xaml
    /// </summary>
    public partial class TwoArmKinematics : PluginBase
    {
        public ObservableCollection<Type> KinematicTypes { get; set; }

        private Kinematic model;

        private Type selectedKinematic;
        public Type SelectedKinematic
        {
            get
            {
                return selectedKinematic;
            }
            set
            {
                selectedKinematic = value;
                LoadModel();
            }
        }

        public bool InvertXL { get; set; }
        public bool InvertYL { get; set; }
        public bool InvertZL { get; set; }
        public bool InvertXR { get; set; }
        public bool InvertYR { get; set; }
        public bool InvertZR { get; set; }

        private double xL, yL, zL, xR, yR, zR;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["XL"].UniqueID, (message) =>
            {
                xL = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["YL"].UniqueID, (message) =>
            {
                yL = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["ZL"].UniqueID, (message) =>
            {
                zL = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["XR"].UniqueID, (message) =>
            {
                xR = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["YR"].UniqueID, (message) =>
            {
                yR = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["ZR"].UniqueID, (message) =>
            {
                zR = message.Value;
                UpdateOutput();
            });

            base.PostLoadSetup();
        }

        public TwoArmKinematics()
        {
            TypeName = "Kinematics";

            var ListOfKinematicModels = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                         from lType in lAssembly.GetTypes()
                                         where typeof(Kinematic).IsAssignableFrom(lType)
                                         select lType).ToArray();

            while (ListOfKinematicModels.Length == 0)
            {
                ListOfKinematicModels = (from lAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                         from lType in lAssembly.GetTypes()
                                         where typeof(Kinematic).IsAssignableFrom(lType)
                                         select lType).ToArray();
            }

            KinematicTypes = new ObservableCollection<Type>(ListOfKinematicModels);

            // get directory where robot models are located
            string currentDir = Directory.GetCurrentDirectory();
            DirectoryInfo dirInfo = Directory.GetParent(currentDir);
            dirInfo = Directory.GetParent(dirInfo.FullName);
            dirInfo = Directory.GetParent(dirInfo.FullName);
            FileInfo[] robotInfoList;
            ObservableCollection<string> robotNames;
            // get names and trim of the file extension
            string robotDir = dirInfo.FullName + "\\Kinematics\\Robots\\TwoArm";
            robotInfoList = new DirectoryInfo(robotDir).GetFiles("*.cs");
            robotNames = new ObservableCollection<string>();
            foreach (var robotName in robotInfoList)
            {
                robotNames.Add(robotName.Name.TrimEnd('.', 'c', 's'));
            }
            // remove kinematic models from list
            ObservableCollection<Type> dummyTypes = new ObservableCollection<Type>(ListOfKinematicModels);
            string dummyName;
            bool isRobot = false;
            foreach (var kineType in dummyTypes)
            {
                dummyName = kineType.FullName;
                dummyName = dummyName.Remove(0, 11);
                if (dummyName[0] == ('R') && dummyName.Contains('.'))
                    dummyName = dummyName.Remove(0, 7);
                foreach (var robotName in robotNames)
                {
                    if (dummyName == robotName)
                    {
                        isRobot = true;
                        break;
                    }
                }
                if (!isRobot)
                    KinematicTypes.Remove(kineType);
                isRobot = false;
            }

            Inputs.Add("XL", new ViewModel.InputSignalViewModel("XL", this.InstanceName));
            Inputs.Add("YL", new ViewModel.InputSignalViewModel("YL", this.InstanceName));
            Inputs.Add("ZL", new ViewModel.InputSignalViewModel("ZL", this.InstanceName));
            Inputs.Add("XR", new ViewModel.InputSignalViewModel("XR", this.InstanceName));
            Inputs.Add("YR", new ViewModel.InputSignalViewModel("YR", this.InstanceName));
            Inputs.Add("ZR", new ViewModel.InputSignalViewModel("ZR", this.InstanceName));

            InitializeComponent();
            HapticsButton.Visibility = System.Windows.Visibility.Hidden;
            PostLoadSetup();
        }

        public void UpdateOutput()
        {
            // Only update the output if we've set our kinematic model
            if (model == null)
                return;
            Point3D pointL = new Point3D();
            pointL.X = InvertXL ? -xL : xL;
            pointL.Y = InvertYL ? -yL : yL;
            pointL.Z = InvertZL ? -zL : zL;
            Point3D pointR = new Point3D();
            pointR.X = InvertXR ? -xR : xR;
            pointR.Y = InvertYR ? -yR : yR;
            pointR.Z = InvertZR ? -zR : zR;
            double[] angles = model.GetJointAngles(pointL, pointR);

            for (int i = 0; i < angles.Length; i++)
            {
                Outputs[model.OutputNames[i]].Value = angles[i];
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void LoadModel()
        {
            model = (Kinematic)Activator.CreateInstance(selectedKinematic);
            foreach (string output in model.OutputNames)
            {
                if (!Outputs.ContainsKey(output))
                    Outputs.Add(output, new ViewModel.OutputSignalViewModel(output));
            }
            Outputs.Add("EnableHaptics", new ViewModel.OutputSignalViewModel("Enable Haptics"));
            Outputs["EnableHaptics"].Value = 0;
            HapticsButton.Visibility = System.Windows.Visibility.Visible;
        }
        
        private RelayCommand enableHapticsCommand;

        /// <summary>
        /// Gets the StartCommand.
        /// </summary>
        public RelayCommand EnableHapticsCommand
        {
            get
            {
                return enableHapticsCommand
                    ?? (enableHapticsCommand = new RelayCommand(
                    () =>
                    {
                        if (Outputs["EnableHaptics"].Value == 0)
                        {
                            Outputs["EnableHaptics"].Value = 1;
                            HapticsText = "Haptic feedback enabled. Click to disable...";
                        }
                        else
                        {
                            Outputs["EnableHaptics"].Value = 0;
                            HapticsText = "Haptic feedback disabled. Click to enable...";
                        }

                    }));
            }
        }

        /// <summary>
        /// The <see cref="HapticsText" /> property's name.
        /// </summary>
        public const string HapticsTextPropertyName = "HapticsText";

        private string hapticsText = "Click to enable haptic feedback";

        /// <summary>
        /// Sets and gets the HapticText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string HapticsText
        {
            get
            {
                return hapticsText;
            }

            set
            {
                if (hapticsText == value)
                {
                    return;
                }

                hapticsText = value;
                RaisePropertyChanged(HapticsTextPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="HapticColor" /> property's name.
        /// </summary>
        public const string HapticColorPropertyName = "HapticColor";

        private Brush hapticColor = new SolidColorBrush(Color.FromRgb(50, 50, 50));

        /// <summary>
        /// Sets and gets the HapticColor property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Brush HapticColor
        {
            get
            {
                return hapticColor;
            }

            set
            {
                if (hapticColor == value)
                {
                    return;
                }

                hapticColor = value;
                RaisePropertyChanged(HapticColorPropertyName);
            }
        }

        byte map(double value, double from1, double to1, double from2, double to2)
        {
            return Convert.ToByte(from2 + (value - from1) * (to2 - from2) / (to1 - from1));
        }
    }
}

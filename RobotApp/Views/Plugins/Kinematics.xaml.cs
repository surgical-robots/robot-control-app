using Kinematics;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using System.Windows.Media;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for Kinematics.xaml
    /// </summary>
    public partial class Kinematics : PluginBase
    {
        public ObservableCollection<Type> KinematicTypes { get; set; }

        private Kinematic model;

        private Type selectedKinematic;
        public Type SelectedKinematic
        {
            get { 
                return selectedKinematic; 
            }
            set { 
                selectedKinematic = value;
                LoadModel();
            }
        }

        public bool InvertX { get; set; }
        public bool InvertY { get; set; }
        public bool InvertZ { get; set; }
        public int ArmSide { get; set; }

        private double x, y, z;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["X"].UniqueID, (message) =>
            {
                x = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Y"].UniqueID, (message) =>
            {
                y = message.Value;
                UpdateOutput();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Z"].UniqueID, (message) =>
            {
                z = message.Value;
                UpdateOutput();
            });

            base.PostLoadSetup();
        }

        public Kinematics()
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
            string robotDir = dirInfo.FullName + "\\Kinematics\\Robots\\SingleArm";
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
                if(!isRobot)
                    KinematicTypes.Remove(kineType);
                isRobot = false;
            }

            Inputs.Add("X", new ViewModel.InputSignalViewModel("X", this.InstanceName));
            Inputs.Add("Y", new ViewModel.InputSignalViewModel("Y", this.InstanceName));
            Inputs.Add("Z", new ViewModel.InputSignalViewModel("Z", this.InstanceName));

            InitializeComponent();
            HapticsButton.Visibility = System.Windows.Visibility.Hidden;
            PostLoadSetup();
        }

        public void UpdateOutput()
        {
            // Only update the output if we've set our kinematic model
            if (model == null)
                return;
            Point3D point = new Point3D();
            point.X = InvertX ? -x : x;
            point.Y = InvertY ? -y : y;
            point.Z = InvertZ ? -z : z;
            double[] angles = model.GetJointAngles(point);
            for(int i = 0; i< angles.Length; i++)
            {
                if(ArmSide == 1 && i == 3)
                {
                    Outputs[model.OutputNames[i]].Value = -angles[i];
                }
                else
                    Outputs[model.OutputNames[i]].Value = angles[i];
            }
            byte r = map(Math.Pow(angles[angles.Length - 3], 2) + Math.Pow(angles[angles.Length - 2], 2) + Math.Pow(angles[angles.Length - 1], 2), 0, 48, 50, 255);
            HapticColor = new SolidColorBrush(Color.FromRgb(r, 50, 50));
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void LoadModel()
        {
            model = (Kinematic)Activator.CreateInstance(selectedKinematic);
            foreach(string output in model.OutputNames)
            {
                if(!Outputs.ContainsKey(output))
                Outputs.Add(output, new ViewModel.OutputSignalViewModel(output));
            }
            if (!Outputs.ContainsKey("EnableHaptics"))
            {
                Outputs.Add("EnableHaptics", new ViewModel.OutputSignalViewModel("Enable Haptics"));
            }
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

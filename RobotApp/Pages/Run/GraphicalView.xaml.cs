using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using HelixToolkit.Wpf;
using RobotApp.ViewModel;
using AForge.Video.DirectShow;

namespace RobotApp.Pages
{
    /// <summary>
    /// Interaction logic for GraphicalViewPage.xaml
    /// </summary>
    public partial class GraphicalView : UserControl, INotifyPropertyChanged
    {
        private readonly Dispatcher dispatcher;

        public ObservableDictionary<string, InputSignalViewModel> Sinks { get; set; }

        public RotateTransform3D rsyTransform = new RotateTransform3D();
        public AxisAngleRotation3D rightShoulderRotY = new AxisAngleRotation3D();
        public RotateTransform3D rsxTransform = new RotateTransform3D();
        public AxisAngleRotation3D rightShoulderRotX = new AxisAngleRotation3D();
        public RotateTransform3D relTransform = new RotateTransform3D();
        public AxisAngleRotation3D rightElbow = new AxisAngleRotation3D();
        public RotateTransform3D rTipTransform = new RotateTransform3D();
        public AxisAngleRotation3D rightTip = new AxisAngleRotation3D();
        public RotateTransform3D lsyTransform = new RotateTransform3D();
        public AxisAngleRotation3D leftShoulderRotY = new AxisAngleRotation3D();
        public RotateTransform3D lsxTransform = new RotateTransform3D();
        public AxisAngleRotation3D leftShoulderRotX = new AxisAngleRotation3D();
        public RotateTransform3D lelTransform = new RotateTransform3D();
        public AxisAngleRotation3D leftElbow = new AxisAngleRotation3D();
        public RotateTransform3D graspTransform = new RotateTransform3D();
        public AxisAngleRotation3D graspOrient1 = new AxisAngleRotation3D();
        public RotateTransform3D jaw1Transform = new RotateTransform3D();
        public AxisAngleRotation3D jawAngle1 = new AxisAngleRotation3D();
        public RotateTransform3D jaw2Transform = new RotateTransform3D();
        public AxisAngleRotation3D jawAngle2 = new AxisAngleRotation3D();

        public ModelVisual3D wholeModel = new ModelVisual3D();
        public ModelVisual3D rightVisual = new ModelVisual3D();
        public ModelVisual3D rightUpperVisual = new ModelVisual3D();
        public ModelVisual3D rightForeVisual = new ModelVisual3D();
        public ModelVisual3D rightForeBodyVisual = new ModelVisual3D();
        public ModelVisual3D rightTipVisual = new ModelVisual3D();
        public ModelVisual3D leftVisual = new ModelVisual3D();
        public ModelVisual3D leftUpperVisual = new ModelVisual3D();
        public ModelVisual3D leftForeVisual = new ModelVisual3D();
        public ModelVisual3D leftForeBodyVisual = new ModelVisual3D();
        public ModelVisual3D grasperVisual = new ModelVisual3D();
        public ModelVisual3D jawOneVisual = new ModelVisual3D();
        public ModelVisual3D jawTwoVisual = new ModelVisual3D();
        public ModelVisual3D yolkVisual = new ModelVisual3D();
        public ModelVisual3D rightSpaceVisual = new ModelVisual3D();
        public ModelVisual3D leftSpaceVisual = new ModelVisual3D();
        public ModelVisual3D staticVisual = new ModelVisual3D();

        // local variables for input angles
        public double rub = 0;
        public double rlb = 0;
        public double rel = 0;
        public double lub = 0;
        public double llb = 0;
        public double lel = 0;
        public double jOpen = 0;
        public double jTwist = 0;
        public double cTwist = 0;

        // video stream objects

        //public ObservableCollection<string> DeviceNames { get; set; }
        /// <summary>
        /// The <see cref="DeviceNames" /> property's name.
        /// </summary>
        public const string DeviceNamesPropertyName = "DeviceNames";

        private ObservableCollection<string> deviceNames = null;

        /// <summary>
        /// Sets and gets the DeviceNames property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<string> DeviceNames
        {
            get
            {
                return deviceNames;
            }

            set
            {
                if (deviceNames == value)
                {
                    return;
                }

                deviceNames = value;
                RaisePropertyChanged(DeviceNamesPropertyName);
            }
        }

        //public ObservableCollection<string> SettingNames { get; set; }
        /// <summary>
        /// The <see cref="SettingNames" /> property's name.
        /// </summary>
        public const string SettingNamesPropertyName = "SettingNames";

        private ObservableCollection<string> settingNames = null;

        /// <summary>
        /// Sets and gets the SettingNames property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<string> SettingNames
        {
            get
            {
                return settingNames;
            }

            set
            {
                if (settingNames == value)
                {
                    return;
                }

                settingNames = value;
                RaisePropertyChanged(SettingNamesPropertyName);
            }
        }

        public VideoCaptureDevice CaptureDevice;
        private FilterInfoCollection _deviceList;
        private VideoCapabilities[] _deviceCapabilites;
        private bool _wasRunning = false;

        public void SetupMessenger()
        {
            Messenger.Default.Register<Messages.Signal>(this, Sinks["RightUpperBevel"].UniqueID, (message) =>
            {
                rub = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Sinks["RightLowerBevel"].UniqueID, (message) =>
            {
                rlb = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Sinks["RightElbow"].UniqueID, (message) =>
            {
                rel = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Sinks["LeftUpperBevel"].UniqueID, (message) =>
            {
                lub = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Sinks["LeftLowerBevel"].UniqueID, (message) =>
            {
                llb = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Sinks["LeftElbow"].UniqueID, (message) =>
            {
                lel = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Sinks["GrasperOpen"].UniqueID, (message) =>
            {
                jOpen = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Sinks["GrasperTwist"].UniqueID, (message) =>
            {
                jTwist = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Sinks["CauteryTwist"].UniqueID, (message) =>
            {
                cTwist = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Sinks["LeftGrasperForce"].UniqueID, (message) =>
            {
                GrasperForceL = message.Value;
            });
        }

        public GraphicalView()
        {
            this.DataContext = this;
            InitializeComponent();
            Sinks = new ObservableDictionary<string, InputSignalViewModel>();

            Sinks.Add("RightUpperBevel", new InputSignalViewModel("RightUpperBevel", "GraphicalView"));
            Sinks.Add("RightLowerBevel", new InputSignalViewModel("RightLowerBevel", "GraphicalView"));
            Sinks.Add("RightElbow", new InputSignalViewModel("RightElbow", "GraphicalView"));
            Sinks.Add("LeftUpperBevel", new InputSignalViewModel("LeftUpperBevel", "GraphicalView"));
            Sinks.Add("LeftLowerBevel", new InputSignalViewModel("LeftLowerBevel", "GraphicalView"));
            Sinks.Add("LeftElbow", new InputSignalViewModel("LeftElbow", "GraphicalView"));
            Sinks.Add("GrasperOpen", new InputSignalViewModel("GrasperOpen", "GraphicalView"));
            Sinks.Add("GrasperTwist", new InputSignalViewModel("GrasperTwist", "GraphicalView"));
            Sinks.Add("CauteryTwist", new InputSignalViewModel("CauteryTwist", "GraphicalView"));
            Sinks.Add("LeftGrasperForce", new InputSignalViewModel("LeftGrasperForce", "GraphicalView"));

            SetupMessenger();

            DeviceNames = new ObservableCollection<string>();
            SettingNames = new ObservableCollection<string>();
            _deviceList = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            // Get a list of all video capture source names
            for (int i = 0; i < _deviceList.Count; i++)
            {
                DeviceNames.Add(_deviceList[i].Name);
            }

            // paths to *.stl files
            string startupPath = System.IO.Directory.GetCurrentDirectory();
            startupPath = startupPath + "\\3D Models\\";
            this.dispatcher = Dispatcher.CurrentDispatcher;

            string shoulderPath = startupPath + "Shoulder.stl";
            string upperRightPath = startupPath + "upperRight.stl";
            string upperLeftPath = startupPath + "upperLeft.stl";
            string foreRightPath = startupPath + "cauteryFore.stl";
            string rightTipPath = startupPath + "cauteryHook.stl";
            string foreLeftPath = startupPath + "grasperFore.stl";
            string yolkPath = startupPath + "grasperYolk.stl";
            string jaw1Path = startupPath + "grasperJaw1.stl";
            string jaw2Path = startupPath + "grasperJaw2.stl";
            string rightSpacePath = startupPath + "RightWorkspace.stl";
            string leftSpacePath = startupPath + "LeftWorkspace.stl";

            // Import *.stl files
            var up = new ModelImporter();
            var shoulder = up.Load(shoulderPath, this.dispatcher);
            var upperRightArm = up.Load(upperRightPath, this.dispatcher);
            var upperLeftArm = up.Load(upperLeftPath, this.dispatcher);
            var foreRightArm = up.Load(foreRightPath, this.dispatcher);
            var tipRightArm = up.Load(rightTipPath, this.dispatcher);
            var foreLeftArm = up.Load(foreLeftPath, this.dispatcher);
            var grasperYolk = up.Load(yolkPath, this.dispatcher);
            var grasperJaw1 = up.Load(jaw1Path, this.dispatcher);
            var grasperJaw2 = up.Load(jaw2Path, this.dispatcher);
            var rightSpace = up.Load(rightSpacePath, this.dispatcher);
            var leftSpace = up.Load(leftSpacePath, this.dispatcher);

            // Convert to GeometryModel3d so we can rotate models
            GeometryModel3D SHmodel = shoulder.Children[0] as GeometryModel3D;
            GeometryModel3D URmodel = upperRightArm.Children[0] as GeometryModel3D;
            GeometryModel3D ULmodel = upperLeftArm.Children[0] as GeometryModel3D;
            GeometryModel3D FRmodel = foreRightArm.Children[0] as GeometryModel3D;
            GeometryModel3D FLmodel = foreLeftArm.Children[0] as GeometryModel3D;
            GeometryModel3D TRmodel = tipRightArm.Children[0] as GeometryModel3D;
            GeometryModel3D GYmodel = grasperYolk.Children[0] as GeometryModel3D;
            GeometryModel3D GJ1model = grasperJaw1.Children[0] as GeometryModel3D;
            GeometryModel3D GJ2model = grasperJaw2.Children[0] as GeometryModel3D;
            GeometryModel3D RWSmodel = rightSpace.Children[0] as GeometryModel3D;
            GeometryModel3D LWSmodel = leftSpace.Children[0] as GeometryModel3D;

            // GHOST WHITE // Set model color
            DiffuseMaterial material = new DiffuseMaterial(new SolidColorBrush(Colors.GhostWhite));
            SHmodel.Material = material;
            SHmodel.BackMaterial = material;
            // RED // Set model color
            material = new DiffuseMaterial(new SolidColorBrush(Colors.Red));
            FRmodel.Material = material;
            FRmodel.BackMaterial = material;
            TRmodel.Material = material;
            TRmodel.BackMaterial = material;
            // DARK RED // Set model color
            material = new DiffuseMaterial(new SolidColorBrush(Colors.DarkRed));
            URmodel.Material = material;
            URmodel.BackMaterial = material;
            // GREEN // Set model color
            material = new DiffuseMaterial(new SolidColorBrush(Colors.Green));
            // BLUE // Set model color
            material = new DiffuseMaterial(new SolidColorBrush(Colors.Blue));
            GYmodel.Material = material;
            GYmodel.BackMaterial = material;
            GJ1model.Material = material;
            GJ1model.BackMaterial = material;
            GJ2model.Material = material;
            GJ2model.BackMaterial = material;
            FLmodel.Material = material;
            FLmodel.BackMaterial = material;
            // DARK BLUE // Set model color
            material = new DiffuseMaterial(new SolidColorBrush(Colors.DarkBlue));
            ULmodel.Material = material;
            ULmodel.BackMaterial = material;
            // BLACK // Set model color
            material = new DiffuseMaterial(new SolidColorBrush(Colors.Black));
            // TRANSPARENT RED // Set model color
            SolidColorBrush brushOne = new SolidColorBrush();
            brushOne.Opacity = 0.50;
            brushOne.Color = Colors.Red;
            material = new DiffuseMaterial(brushOne);
            RWSmodel.Material = material;
            RWSmodel.BackMaterial = material;
            // TRANSPARENT BLUE // Set model color
            SolidColorBrush brushTwo = new SolidColorBrush();
            brushTwo.Opacity = 0.50;
            brushTwo.Color = Colors.Blue;
            material = new DiffuseMaterial(brushTwo);
            LWSmodel.Material = material;
            LWSmodel.BackMaterial = material;

            // right shoulder rotations
            rsyTransform.CenterX = 0;
            rsyTransform.CenterY = 0;
            rsyTransform.CenterZ = 0;
            rightShoulderRotY.Axis = new Vector3D(0, 1, 0);
            rsxTransform.CenterX = 0;
            rsxTransform.CenterY = 0;
            rsxTransform.CenterZ = 0;
            rightShoulderRotX.Axis = new Vector3D(1, 0, 0);
            // right elbow rotations
            relTransform.CenterX = -13.386;
            relTransform.CenterY = 5;
            relTransform.CenterZ = 74.684;
            rightElbow.Axis = new Vector3D(0, 1, 0);
            // right tip rotations
            rTipTransform.CenterX = -15.111;
            rTipTransform.CenterY = 9.515;
            rTipTransform.CenterZ = 0;
            rightTip.Axis = new Vector3D(0, 0, 1);
            //left shoulder rotations
            lsyTransform.CenterX = 14.478;
            lsyTransform.CenterY = 0;
            lsyTransform.CenterZ = 0;
            leftShoulderRotY.Axis = new Vector3D(0, 1, 0);
            lsxTransform.CenterX = 0;
            lsxTransform.CenterY = 0;
            lsxTransform.CenterZ = 0;
            leftShoulderRotX.Axis = new Vector3D(1, 0, 0);
            // left elbow rotations
            lelTransform.CenterX = 27.864;
            lelTransform.CenterY = 0;
            lelTransform.CenterZ = 74.684;
            leftElbow.Axis = new Vector3D(0, 1, 0);
            // grasper rotations
            graspTransform.CenterX = 27.864;
            graspTransform.CenterY = 13.624;
            graspTransform.CenterZ = 0;
            graspOrient1.Axis = new Vector3D(0, 0, 1);
            jaw1Transform.CenterX = 27.864;
            jaw1Transform.CenterZ = 160.284;
            jawAngle1.Axis = new Vector3D(0, 1, 0);
            jaw2Transform.CenterX = 27.864;
            jaw2Transform.CenterZ = 160.284;
            jawAngle2.Axis = new Vector3D(0, 1, 0);

            // Define cautery tip
            rightTipVisual.Content = TRmodel;
            // Define right forearm group
            rightForeBodyVisual.Content = FRmodel;
            rightForeVisual.Children.Clear();
            rightForeVisual.Children.Add(rightForeBodyVisual);
            rightForeVisual.Children.Add(rightTipVisual);
            // Define right arm group
            rightUpperVisual.Content = URmodel;
            rightVisual.Children.Clear();
            rightVisual.Children.Add(rightUpperVisual);
            rightVisual.Children.Add(rightForeVisual);
            // Left grasper open/close
            jawOneVisual.Content = GJ1model;
            jawTwoVisual.Content = GJ2model;
            // Define grasper group
            yolkVisual.Content = GYmodel;
            grasperVisual.Children.Clear();
            grasperVisual.Children.Add(yolkVisual);
            grasperVisual.Children.Add(jawOneVisual);
            grasperVisual.Children.Add(jawTwoVisual);
            // Define left forearm group
            leftForeBodyVisual.Content = FLmodel;
            leftForeVisual.Children.Clear();
            leftForeVisual.Children.Add(leftForeBodyVisual);
            leftForeVisual.Children.Add(grasperVisual);
            // Define left arm group
            leftUpperVisual.Content = ULmodel;
            leftVisual.Children.Clear();
            leftVisual.Children.Add(leftUpperVisual);
            leftVisual.Children.Add(leftForeVisual);
            // workspace
            rightSpaceVisual.Content = RWSmodel;
            leftSpaceVisual.Content = LWSmodel;
            staticVisual.Children.Clear();
            staticVisual.Children.Add(rightSpaceVisual);
            staticVisual.Children.Add(leftSpaceVisual);

            // Add left and right arms to full model
            wholeModel.Content = SHmodel;
            wholeModel.Children.Clear();
            wholeModel.Children.Add(rightVisual);
            wholeModel.Children.Add(leftVisual);
            //            wholeModel.Children.Add(staticVisual);
            DisplayModel();
        }

        public void DisplayModel()
        // DisplayModel -----> Performs Rotations and translations and sends models to VirtualRobotWindow
        {
            this.dispatcher.Invoke((Action)(() =>
            {
                // Convert bevel angles to x-y rotations
                double rxTheta = (rub - rlb) / 2;
                double ryTheta = (rub + rlb) / 2;
                double lxTheta = -(lub - llb) / 2;
                double lyTheta = (lub + llb) / 2;

                // Define rotations and translations

                // right shoulder rotations
                rightShoulderRotY.Angle = ryTheta;
                rsyTransform.Rotation = rightShoulderRotY;
                rightShoulderRotX.Angle = rxTheta;
                rsxTransform.Rotation = rightShoulderRotX;
                // right elbow rotations
                rightElbow.Angle = rel;
                relTransform.Rotation = rightElbow;
                // right tip rotations
                rightTip.Angle = cTwist;
                rTipTransform.Rotation = rightTip;
                //left shoulder rotations
                leftShoulderRotY.Angle = -lyTheta;
                lsyTransform.Rotation = leftShoulderRotY;
                leftShoulderRotX.Angle = -lxTheta;
                lsxTransform.Rotation = leftShoulderRotX;
                // left elbow rotations
                leftElbow.Angle = -lel;
                lelTransform.Rotation = leftElbow;
                // grasper rotations
                graspOrient1.Angle = jTwist;
                graspTransform.Rotation = graspOrient1;

                jawAngle1.Angle = jOpen;
                jaw1Transform.Rotation = jawAngle1;
                jawAngle2.Angle = -jOpen;
                jaw2Transform.Rotation = jawAngle2;

                //// whole model rotations
                RotateTransform3D modelXTransform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), -90));
                RotateTransform3D modelYTransform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 180));

                // Cautery Tip Roatations
                rightTipVisual.Transform = rTipTransform;
                // Right elbow rotation
                rightForeVisual.Transform = relTransform;
                // Right shoulder rotations
                Transform3DGroup rightArmTransform = new Transform3DGroup();
                rightArmTransform.Children.Add(rsxTransform);
                rightArmTransform.Children.Add(rsyTransform);
                rightVisual.Transform = rightArmTransform;
                // Left grasper open/close
                jawOneVisual.Transform = jaw1Transform;
                jawTwoVisual.Transform = jaw2Transform;
                // Left grasper rotations
                grasperVisual.Transform = graspTransform;
                // Left elbow rotaions
                leftForeVisual.Transform = lelTransform;
                // Left shoulder rotations
                Transform3DGroup leftArmTransform = new Transform3DGroup();
                leftArmTransform.Children.Add(lsxTransform);
                leftArmTransform.Children.Add(lsyTransform);
                leftVisual.Transform = leftArmTransform;
                // Whole model transform
                Transform3DGroup modelTransform = new Transform3DGroup();
                modelTransform.Children.Add(modelXTransform);
                modelTransform.Children.Add(modelYTransform);
                wholeModel.Transform = modelTransform;
                // Add content to HelixViewport3D in VirtualRobotWindow.xaml

                FullModel = wholeModel;
            }));

        }

        private ModelVisual3D fullModel = null;
        /// <summary>
        /// Sets and gets the ModelGroup property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ModelVisual3D FullModel
        {
            get
            {
                return fullModel;
            }
            set
            {
                if (fullModel == value)
                    return;
                fullModel = value;
                robotModel.Children.Clear();
                robotModel.Children.Add(fullModel);
            }
        }

        /// <summary>
        /// The <see cref="SelectedDeviceName" /> property's name.
        /// </summary>
        public const string SelectedDeviceNamePropertyName = "SelectedDeviceName";

        private string selectedDevice = "";

        /// <summary>
        /// Sets and gets the SelectedDeviceName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SelectedDeviceName
        {
            get
            {
                return selectedDevice;
            }

            set
            {
                if (selectedDevice == value)
                {
                    return;
                }

                selectedDevice = value;
                RaisePropertyChanged(SelectedDeviceNamePropertyName);
                if (CaptureDevice != null && CaptureDevice.IsRunning)
                    CaptureDevice.SignalToStop();
                if (_wasRunning)
                    CaptureDevice.NewFrame -= CaptureDevice_NewFrame;

                for (int i = 0; i < _deviceList.Count; i++)
                {
                    if (_deviceList[i].Name == selectedDevice)
                    {
                        CaptureDevice = new VideoCaptureDevice(_deviceList[i].MonikerString);
                        _deviceCapabilites = CaptureDevice.VideoCapabilities;
                        SelectedSetting = 0;
                        CreateCapabilityList();
                        CaptureDevice.NewFrame += CaptureDevice_NewFrame;
                        _wasRunning = true;
                    }
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        void CaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            Bitmap frame = eventArgs.Frame;
            this.Dispatcher.Invoke((Action)(() =>
            {
                IntPtr hBitmap = frame.GetHbitmap();
                try
                {
                    VideoImage.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                finally
                {
                    DeleteObject(hBitmap);
                    frame.Dispose();
                }
            }));
        }

        void CreateCapabilityList()
        {
            string dummyString = "";
            SettingNames.Clear();
            foreach (VideoCapabilities settings in _deviceCapabilites)
            {
                dummyString = "";
                dummyString = settings.FrameSize.Width + "x" + settings.FrameSize.Height + " " + settings.AverageFrameRate + "FPS";
                SettingNames.Add(dummyString);
            }
        }

        /// <summary>
        /// The <see cref="SelectedSetting" /> property's name.
        /// </summary>
        public const string SelectedSettingPropertyName = "SelectedSetting";

        private int selectedSetting = 0;

        /// <summary>
        /// Sets and gets the SelectedSetting property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int SelectedSetting
        {
            get
            {
                return selectedSetting;
            }

            set
            {
                if (selectedSetting == value)
                {
                    return;
                }

                selectedSetting = value;
                RaisePropertyChanged(SelectedSettingPropertyName);
                if (selectedSetting != -1)
                    CaptureDevice.VideoResolution = _deviceCapabilites[selectedSetting];
            }
        }

        /// <summary>
        /// The <see cref="ConnectButtonText" /> property's name.
        /// </summary>
        public const string ConnectButtonTextPropertyName = "ConnectButtonText";

        private string connectButtonText = "Connect";

        /// <summary>
        /// Sets and gets the ConnectButtonText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ConnectButtonText
        {
            get
            {
                return connectButtonText;
            }

            set
            {
                if (connectButtonText == value)
                {
                    return;
                }

                connectButtonText = value;
                RaisePropertyChanged(ConnectButtonTextPropertyName);
            }
        }

        private RelayCommand<string> startCommand;

        /// <summary>
        /// Gets the DetectCOMsCommand.
        /// </summary>
        public RelayCommand<string> StartCommand
        {
            get
            {
                return startCommand
                    ?? (startCommand = new RelayCommand<string>(
                    p =>
                    {
                        if (CaptureDevice.IsRunning)
                        {
                            CaptureDevice.SignalToStop();
                            ConnectButtonText = "Connect";
                        }
                        else
                        {
                            CaptureDevice.Start();
                            ConnectButtonText = "Disconnect";
                        }
                    }));
            }
        }

        /// <summary>
        /// The <see cref="GrasperForceL" /> property's name.
        /// </summary>
        public const string GrasperForceLPropertyName = "GrasperForceL";

        private double grasperForceL = 0;

        /// <summary>
        /// Sets and gets the GrasperForceL property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double GrasperForceL
        {
            get
            {
                return grasperForceL;
            }

            set
            {
                if (grasperForceL == value)
                {
                    return;
                }

                grasperForceL = value;
                RaisePropertyChanged(GrasperForceLPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="GrasperForceR" /> property's name.
        /// </summary>
        public const string GrasperForceRPropertyName = "GrasperForceR";

        private double grasperForceR = 200;

        /// <summary>
        /// Sets and gets the GrasperForceR property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double GrasperForceR
        {
            get
            {
                return grasperForceR;
            }

            set
            {
                if (grasperForceR == value)
                {
                    return;
                }

                grasperForceR = value;
                RaisePropertyChanged(GrasperForceRPropertyName);
            }
        }

        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}

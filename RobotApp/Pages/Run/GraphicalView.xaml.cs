namespace RobotApp.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Threading;

    using Accord.Video.DirectShow;

    using Emgu.CV;
    using Emgu.CV.Util;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Structure;

    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using Mocoding.EasyDocDb.Yaml;
    using HelixToolkit.Wpf;
    using RobotApp.ViewModel;

    /// <summary>
    /// Interaction logic for GraphicalViewPage.xaml
    /// </summary>
    public partial class GraphicalView : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        private string path = System.IO.Directory.GetCurrentDirectory();
        private bool showModel = false;

        #region Camera parameters
        private const int camNum = 1;
        private VideoCapture[] _Capture;
        Image<Bgr, Byte>[] img; // image captured
        Image<Gray, Byte>[] Gray_Frame; // image for processing

        static Image<Gray, Byte>[,] Frame_array_buffer; // number of images to calibrate camera over
        int frame_buffer_savepoint = 0;
        bool start_Flag = false;
        bool readFlag = false;

        private Accord.Video.DirectShow.VideoCaptureDevice CaptureDevice;
        private FilterInfoCollection _deviceList;
        private VideoCapabilities[] _deviceCapabilites;

        private bool _wasRunning = false;
        private bool _isRunning = false;
        #endregion

        #region Calibration Parameters
        MCvPoint3D32f[][][] corners_object_list;
        PointF[][][] corners_points_list;

        const int width = 9; // width of chessboard no. squares in width - 1
        const int height = 6; // heght of chess board no. squares in heigth - 1
        float squareSize = 23; // in mm
        System.Drawing.Size patternSize = new System.Drawing.Size(width, height); // size of chess board to be detected
        Bgr[] line_colour_array = new Bgr[width * height]; // just for displaying coloured lines of detected chessboard

        Mat cameraMatrix = new Mat(3, 3, DepthType.Cv64F, 1);
        Mat distCoeffs = new Mat(8, 1, DepthType.Cv64F, 1);
        Mat[] rvecs, tvecs;

        IntrinsicCameraParameters IC = new IntrinsicCameraParameters();
        ExtrinsicCameraParameters[] EX_Param;

        #endregion

        #region Current mode variables
        public enum Mode
        {
            Caluculating_Intrinsics,
            Calibrated,
            SavingFrames,
            ManualSaveFrame,
            None
        }
        Mode currentMode = Mode.None;
        #endregion

        #region Robot model variables
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
        #endregion

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
        }

        public GraphicalView()
        {
            this.DataContext = this;
            InitializeComponent();

            _Capture = new VideoCapture[camNum];
            img = new Image<Bgr, byte>[camNum];
            Gray_Frame = new Image<Gray, byte>[camNum];
            Frame_array_buffer = new Image<Gray, byte>[camNum, frameBuffer];
            corners_object_list = new MCvPoint3D32f[camNum][][];
            corners_points_list = new PointF[camNum][][];

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

            Random R = new Random();
            for (int i = 0; i < line_colour_array.Length; i++)
            {
                line_colour_array[i] = new Bgr(R.Next(0, 255), R.Next(0, 255), R.Next(0, 255));
            }
            //Run();
            Write_BTN.IsEnabled = false;
            Main_Picturebox.Size = new System.Drawing.Size(820, 820);

            DeviceNames = new ObservableCollection<string>();
            SettingNames = new ObservableCollection<string>();
            _deviceList = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            // Get a list of all video capture source names
            for (int i = 0; i < _deviceList.Count; i++)
            {
                DeviceNames.Add(_deviceList[i].Name);
            }

            // paths to *.stl files
            if (showModel)
            {
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
        }

        /// <summary>
        /// main function processing of the image data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _Capture_GrabFrame(int n)
        {
            bool foundCorners;
            VectorOfPointF corners = new VectorOfPointF(); // corners found from chessboard
            //lets get a frame from our capture device
            img[n] = _Capture[n].QueryFrame().ToImage<Bgr, byte>();
            Gray_Frame[n] = img[n].Convert<Gray, Byte>();

            if (img[n].Bytes != null)
            {
                switch (currentMode)
                {
                    case Mode.ManualSaveFrame:
                        foundCorners = CvInvoke.FindChessboardCorners(Gray_Frame[n], patternSize, corners, CalibCbType.AdaptiveThresh);
                        // we use this loop so we can show a colour image rather than a gray: //CameraCalibration.DrawChessboardCorners(Gray_Frame, patternSize, corners);

                        if (foundCorners) // chess board found
                        {
                            // make mesurments more accurate by using FindCornerSubPixel
                            CvInvoke.CornerSubPix(Gray_Frame[n], corners, new Size(11, 11), new Size(-1, -1), new MCvTermCriteria(30, 0.1));

                            // dram the results
                            CvInvoke.DrawChessboardCorners(img[n], patternSize, corners, foundCorners);

                            if (readFlag)
                            {
                                Frame_array_buffer[n, frame_buffer_savepoint] = Gray_Frame[n].Copy(); //store the image
                                frame_buffer_savepoint++;//increase buffer positon
                                readFlag = false;
                                Thread.Sleep(600);

                                //check the state of buffer
                                if (frame_buffer_savepoint == frameBuffer) currentMode = Mode.Caluculating_Intrinsics; //buffer full
                            }
                        }
                        break;
                    case Mode.SavingFrames:
                        foundCorners = CvInvoke.FindChessboardCorners(Gray_Frame[n], patternSize, corners, CalibCbType.AdaptiveThresh);

                        if (foundCorners) //chess board found
                        {
                            //make mesurments more accurate by using FindCornerSubPixel
                            CvInvoke.CornerSubPix(Gray_Frame[n], corners, new Size(11, 11), new Size(-1, -1), new MCvTermCriteria(30, 0.1));

                            //if go button has been pressed start aquiring frames else we will just display the points
                            if (start_Flag)
                            {
                                Frame_array_buffer[n, frame_buffer_savepoint] = Gray_Frame[n].Copy(); //store the image
                                frame_buffer_savepoint++;//increase buffer positon

                                //check the state of buffer
                                if (frame_buffer_savepoint == frameBuffer) currentMode = Mode.Caluculating_Intrinsics; //buffer full
                            }

                            // dram the results
                            CvInvoke.DrawChessboardCorners(img[n], patternSize, corners, foundCorners);

                            // calibrate the delay bassed on size of buffer
                            // if buffer small you want a big delay if big small delay
                            Thread.Sleep(100); // allow the user to move the board to a different position
                        }
                        break;

                    case Mode.Caluculating_Intrinsics:
                        // we can do this in the loop above to increase speed
                        VectorOfPointF[] cornerVec = new VectorOfPointF[frameBuffer];
                        for (int k = 0; k < frameBuffer; k++)
                        {
                            cornerVec[k] = new VectorOfPointF();
                            foundCorners = CvInvoke.FindChessboardCorners(Frame_array_buffer[n, k], patternSize, cornerVec[k], CalibCbType.AdaptiveThresh|CalibCbType.FastCheck|CalibCbType.NormalizeImage);
                            // for accuracy
                            CvInvoke.CornerSubPix(Frame_array_buffer[n, k], cornerVec[k], new Size(11, 11), new Size(-1, -1), new MCvTermCriteria(30, 0.1));

                            // Fill our objects list with the real world mesurments for the intrinsic calculations
                            List<MCvPoint3D32f> object_list = new List<MCvPoint3D32f>();
                            for (int i = 0; i < height; i++)
                            {
                                for (int j = 0; j < width; j++)
                                {
                                    object_list.Add(new MCvPoint3D32f(j * squareSize, i * squareSize, 0.0F));
                                }
                            }
                            corners_object_list[n][k] = object_list.ToArray();
                            corners_points_list[n][k] = cornerVec[k].ToArray();
                        }
                        //our error should be as close to 0 as possible
                        double error = CvInvoke.CalibrateCamera(corners_object_list[n], corners_points_list[n], Gray_Frame[n].Size, cameraMatrix, distCoeffs, CalibType.RationalModel, new MCvTermCriteria(30, 0.1), out rvecs, out tvecs);
                        //If Emgu.CV.CvEnum.CALIB_TYPE == CV_CALIB_USE_INTRINSIC_GUESS and/or CV_CALIB_FIX_ASPECT_RATIO are specified, some or all of fx, fy, cx, cy must be initialized before calling the function
                        //if you use FIX_ASPECT_RATIO and FIX_FOCAL_LEGNTH options, these values needs to be set in the intrinsic parameters before the CalibrateCamera function is called. Otherwise 0 values are used as default.
                        System.Windows.Forms.MessageBox.Show("Intrinsic Calculation Error: " + error.ToString(), "Results", MessageBoxButtons.OK, MessageBoxIcon.Information); //display the results to the user
                        currentMode = Mode.Calibrated;
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            Write_BTN.IsEnabled = true;
                        }));
                        break;

                    case Mode.Calibrated:
                        //calculate the camera intrinsics
                        Matrix<float> Map1 = new Matrix<float>(img[n].Size);
                        Matrix<float> Map2 = new Matrix<float>(img[n].Size);
                        CvInvoke.InitUndistortRectifyMap(cameraMatrix, distCoeffs, null, cameraMatrix, img[n].Size, DepthType.Cv8U, Map1, Map2);
                        //remap the image to the particular intrinsics
                        //In the current version of EMGU any pixel that is not corrected is set to transparent allowing the original image to be displayed if the same
                        //image is mapped backed, in the future this should be controllable through the flag '0'
                        Image<Bgr, Byte> temp = img[n].CopyBlank();
                        CvInvoke.Remap(img[n], temp, Map1, Map2, Inter.Nearest);
                        img[n] = temp.Copy();

                        //set up to allow another calculation
                        SetButtonState(true);
                        start_Flag = false;
                        break;
                    default:
                        break;
                }

                if(n == 0)
                {
                    Image<Bgr, byte> mainImage = img[n].Resize(((double)Main_Picturebox.Width / (double)img[n].Width), Inter.Linear);
                    Main_Picturebox.Image = mainImage;
                }
            }
        }

        private RelayCommand<string> start_BTN_Click;

        /// <summary>
        /// Gets the DetectCOMsCommand.
        /// </summary>
        public RelayCommand<string> Start_BTN_Click
        {
            get
            {
                return start_BTN_Click
                    ?? (start_BTN_Click = new RelayCommand<string>(
                    p =>
                    {
                        //if (currentMode != Mode.SavingFrames) currentMode = Mode.SavingFrames;
                        //Start_BTN.IsEnabled = false;
                        ////set up the arrays needed
                        //Frame_array_buffer = new Image<Gray, byte>[frameBuffer];
                        //corners_object_list = new MCvPoint3D32f[Frame_array_buffer.Length][];
                        //corners_points_list = new PointF[Frame_array_buffer.Length][];
                        //frame_buffer_savepoint = 0;
                        ////allow the start
                        //start_Flag = true;

                        if (currentMode != Mode.ManualSaveFrame) currentMode = Mode.ManualSaveFrame;
                        Start_BTN.IsEnabled = false;
                        //set up the arrays needed

                        Frame_array_buffer = new Image<Gray, byte>[camNum, frameBuffer];
                        corners_object_list = new MCvPoint3D32f[camNum][][];
                        corners_points_list = new PointF[camNum][][];
                        frame_buffer_savepoint = 0;
                        Read_BTN.IsEnabled = true;
                    }));
            }
        }

        private RelayCommand writeCalibrationData;

        /// <summary>
        /// Gets the WriteCalibrationData.
        /// </summary>
        public RelayCommand WriteCalibrationData
        {
            get
            {
                return writeCalibrationData
                    ?? (writeCalibrationData = new RelayCommand(
                    () =>
                    {
                        string imageDir = path + "\\imageSet\\";
                        string fileExt = ".png";
                        KeyValuePair<ImwriteFlags, int> parms = new KeyValuePair<ImwriteFlags, int>();

                        if (currentMode == Mode.Calibrated)
                        {
                            //WriteYAMLFile();
                            string fullPath = path + "\\cal_ObjList.txt";
                            if (!File.Exists(fullPath))
                            {
                                using (StreamWriter sw = File.CreateText(fullPath))
                                {

                                }
                            }
                            else
                                File.Delete(fullPath);
                            for (int i = 0; i < corners_object_list[0].Length; i++)
                            {
                                using (StreamWriter sw = File.AppendText(fullPath))
                                {
                                    sw.Write(corners_object_list[0][0][i].X);
                                    sw.Write("\t");
                                    sw.Write(corners_object_list[0][0][i].Y);
                                    sw.Write("\t");
                                    sw.WriteLine(corners_object_list[0][0][i].Z);
                                }
                            }
                            fullPath = path + "\\cal_PntList.txt";
                            if (!File.Exists(fullPath))
                            {
                                using (StreamWriter sw = File.CreateText(fullPath))
                                {

                                }
                            }
                            else
                                File.Delete(fullPath);
                            for (int i = 0; i < corners_points_list[0].Length; i++)
                            {
                                using (StreamWriter sw = File.AppendText(fullPath))
                                {
                                    sw.Write(corners_points_list[0][0][i].X);
                                    sw.Write("\t");
                                    sw.WriteLine(corners_points_list[0][0][i].Y);
                                }
                            }

                            for (int i = 0; i < frameBuffer; i++)
                            {
                                for (int j = 0; j < camNum; j++)
                                {
                                    CvInvoke.Imwrite(imageDir + "left" + i.ToString() + fileExt, Frame_array_buffer[j, i]);
                                }
                            }
                        }
                    }));
            }
        }

        double computeReprojectionErrors()
        {
            return 0;
        //    vector<Point2f> imagePoints2;
        //    int i, totalPoints = 0;
        //    double totalErr = 0, err;
        //    perViewErrors.resize(objectPoints.size());

        //    for(i = 0; i<(int)objectPoints.size(); i++ )
        //    {
                
        //        projectPoints(Mat(objectPoints[i]), rvecs[i], tvecs[i],
        //                      cameraMatrix, distCoeffs, imagePoints2);
        //        err = norm(Mat(imagePoints[i]), Mat(imagePoints2), CV_L2);
        //        int n = (int)objectPoints[i].size();
        //        perViewErrors[i] = (float) std::sqrt(err* err/n);
        //        totalErr += err* err;
        //        totalPoints += n;
        //}
        //return Math.Sqrt(totalErr/totalPoints);
    }

    private void WriteYAMLFile()
        {
            string fullPath = path + "\\cal_IntrinsicParms.yml";
            YamlSerializer ser = new YamlSerializer();

            if (!File.Exists(fullPath))
            {
                using (StreamWriter sw = File.CreateText(fullPath))
                {
                    sw.Write(ser.Serialize<IntrinsicCameraParameters>(IC));
                }
            }
            else
            {
                File.Delete(fullPath);
                using (StreamWriter sw = File.CreateText(fullPath))
                {
                    sw.Write(ser.Serialize<IntrinsicCameraParameters>(IC));
                }
            }
        }

        private RelayCommand readData;

        /// <summary>
        /// Gets the ReadData.
        /// </summary>
        public RelayCommand ReadData
        {
            get
            {
                return readData
                    ?? (readData = new RelayCommand(
                    () =>
                    {
                        if(currentMode == Mode.ManualSaveFrame)
                        {
                            readFlag = true;
                        }
                        else
                        {
                            corners_object_list = new MCvPoint3D32f[1][][];
                            string fullPath = path + "\\cal_ObjList.txt";
                            string[][] path1Data = File.ReadLines(fullPath).Select(line => line.Split('\t')).ToArray();
                            corners_object_list[0][0] = new MCvPoint3D32f[path1Data.GetLength(0)];
                            for (int i = 0; i < path1Data.GetLength(0); i++)
                            {
                                double[] dubData = Array.ConvertAll<string, double>(path1Data[i], Convert.ToDouble);
                                corners_object_list[0][0][i].X = (float)dubData[0];
                                corners_object_list[0][0][i].Y = (float)dubData[1];
                                corners_object_list[0][0][i].Z = (float)dubData[2];
                            }

                            corners_points_list = new PointF[1][][];
                            fullPath = path + "\\cal_PntList.txt";
                            string[][] path2Data = File.ReadLines(fullPath).Select(line => line.Split('\t')).ToArray();
                            corners_points_list[0][0] = new PointF[path2Data.GetLength(0)];
                            for (int i = 0; i < path2Data.GetLength(0); i++)
                            {
                                double[] dubData = Array.ConvertAll<string, double>(path2Data[i], Convert.ToDouble);
                                corners_points_list[0][0][i].X = (float)dubData[0];
                                corners_points_list[0][0][i].Y = (float)dubData[1];
                            }

                            double error = CvInvoke.CalibrateCamera(corners_object_list[0], corners_points_list[0], Gray_Frame[0].Size, cameraMatrix, distCoeffs, CalibType.RationalModel, new MCvTermCriteria(30, 0.1), out rvecs, out tvecs);
                            //If Emgu.CV.CvEnum.CALIB_TYPE == CV_CALIB_USE_INTRINSIC_GUESS and/or CV_CALIB_FIX_ASPECT_RATIO are specified, some or all of fx, fy, cx, cy must be initialized before calling the function
                            //if you use FIX_ASPECT_RATIO and FIX_FOCAL_LEGNTH options, these values needs to be set in the intrinsic parameters before the CalibrateCamera function is called. Otherwise 0 values are used as default.
                            System.Windows.Forms.MessageBox.Show("Intrinsic Calculation Error: " + error.ToString(), "Results", MessageBoxButtons.OK, MessageBoxIcon.Information); //display the results to the user
                            currentMode = Mode.Calibrated;
                        }
                    }));
            }
        }

        /// <summary>
        /// The <see cref="FrameBuffer" /> property's name.
        /// </summary>
        public const string FrameBufferPropertyName = "FrameBuffer";

        private int frameBuffer = 33;

        /// <summary>
        /// Sets and gets the FrameBuffer property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int FrameBuffer
        {
            get
            {
                return frameBuffer;
            }

            set
            {
                if (frameBuffer == value)
                {
                    return;
                }

                frameBuffer = value;
                RaisePropertyChanged(FrameBufferPropertyName);
            }
        }

        /// <summary>
        /// Ussed to safly set the button state from capture thread
        /// </summary>
        /// <param name="state"></param>
        delegate void SetButtonStateDelegate(bool state);
        private void SetButtonState(bool state)
        {
            //if (Start_BTN.InvokeRequired)
            //{
            //    try
            //    {
            //        // update textbox asynchronously
            //        SetButtonStateDelegate ut = new SetButtonStateDelegate(SetButtonState);
            //        //if (this.IsHandleCreated && !this.IsDisposed)
            //        this.BeginInvoke(ut, new object[] { state });
            //    }
            //    catch (Exception ex)
            //    {
            //    }
            //}
            //else
            //{
            this.Dispatcher.Invoke((Action)(() =>
            {
                Start_BTN.IsEnabled = state;
            }));
            //}
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

        /// <summary>
        /// The <see cref="SelectedDeviceName" /> property's name.
        /// </summary>
        public const string SelectedDeviceNamePropertyName = "SelectedDeviceName";

        private int selectedDevice = -1;

        /// <summary>
        /// Sets and gets the SelectedDeviceName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int SelectedDeviceName
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

                if (_Capture[0] != null && _isRunning)
                {
                    _wasRunning = true;
                    _isRunning = false;
                }

                for (int i = 0; i < camNum; i++)
                {
                    _Capture[i] = new VideoCapture(selectedDevice + i);

                }
                CaptureDevice = new VideoCaptureDevice(_deviceList[selectedDevice].MonikerString);
                _deviceCapabilites = CaptureDevice.VideoCapabilities;
                SelectedSetting = 0;
                CreateCapabilityList();
                //_Capture.ImageGrabbed += _Capture_ImageGrabbed;

                if (_wasRunning)
                {
                    _wasRunning = false;
                    _isRunning = true;

                    //Parallel.For(0, camNum, n => { while (_isRunning) _Capture_GrabFrame(n); });

                    Task t = Task.Run(() =>
                    {
                        while (_isRunning)
                        {
                            _Capture_GrabFrame(0);
                        }
                    });
                }
            }
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
                {
                    if(_isRunning)
                    {
                        _isRunning = false;
                        _wasRunning = true;
                    }

                    for(int i = 0; i < camNum; i++)
                    {
                        _Capture[i].SetCaptureProperty(CapProp.FrameWidth, _deviceCapabilites[selectedSetting].FrameSize.Width);
                        _Capture[i].SetCaptureProperty(CapProp.FrameHeight, _deviceCapabilites[selectedSetting].FrameSize.Height);
                        _Capture[i].SetCaptureProperty(CapProp.Fps, _deviceCapabilites[selectedSetting].AverageFrameRate);
                    }

                    if (_isRunning)
                    {
                        _wasRunning = false;
                        _isRunning = true;

                        //Parallel.For(0, camNum, n => { while (_isRunning) _Capture_GrabFrame(n); });

                        Task t = Task.Run(() =>
                        {
                            while (_isRunning)
                            {
                                _Capture_GrabFrame(0);
                            }
                        });
                    }
                }
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
                        if (_isRunning)
                        {
                            ConnectButtonText = "Connect";
                            _isRunning = false;
                        }
                        else
                        {
                            ConnectButtonText = "Disconnect";
                            _isRunning = true;

                            //Parallel.For(0, camNum, n => { while (_isRunning) _Capture_GrabFrame(n); });
                            //for (int i = 0; i < camNum; i++)
                            //{
                                Task t1 = Task.Run(() =>
                                {
                                    while (_isRunning)
                                    {
                                        _Capture_GrabFrame(0);
                                    }
                                });

                            Task t2 = Task.Run(() =>
                            {
                                while (_isRunning)
                                {
                                    _Capture_GrabFrame(1);
                                }
                            });

                            //}
                        }
                    }));
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

    public class CameraConfig
    {
        public DateTime CalibrationTime { get; set; }
        public Size BoardSize { get; set; }
        public int Nframes { get; set; }

        public Mat IntrinsicMatrix { get; set; }
        public Mat DistortionCoeffs { get; set; }

        public double ReprojectionError { get; set; }


    }
}

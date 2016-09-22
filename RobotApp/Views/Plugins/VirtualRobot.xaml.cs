using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using HelixToolkit.Wpf;
using RobotApp.Pages;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for VirtualRobot.xaml
    /// </summary>
    public partial class VirtualRobot : PluginBase
    {
        private readonly Dispatcher dispatcher;

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
        public ModelVisual3D needleVisual = new ModelVisual3D();

        public ModelVisual3D wholeModel2 = new ModelVisual3D();
        public ModelVisual3D rightVisual2 = new ModelVisual3D();
        public ModelVisual3D rightUpperVisual2 = new ModelVisual3D();
        public ModelVisual3D rightForeVisual2 = new ModelVisual3D();
        public ModelVisual3D rightForeBodyVisual2 = new ModelVisual3D();
        public ModelVisual3D rightTipVisual2 = new ModelVisual3D();
        public ModelVisual3D leftVisual2 = new ModelVisual3D();
        public ModelVisual3D leftUpperVisual2 = new ModelVisual3D();
        public ModelVisual3D leftForeVisual2 = new ModelVisual3D();
        public ModelVisual3D leftForeBodyVisual2 = new ModelVisual3D();
        public ModelVisual3D grasperVisual2 = new ModelVisual3D();
        public ModelVisual3D jawOneVisual2 = new ModelVisual3D();
        public ModelVisual3D jawTwoVisual2 = new ModelVisual3D();
        public ModelVisual3D yolkVisual2 = new ModelVisual3D();
        public ModelVisual3D rightSpaceVisual2 = new ModelVisual3D();
        public ModelVisual3D leftSpaceVisual2 = new ModelVisual3D();
        public ModelVisual3D needleVisual2 = new ModelVisual3D();

        public ModelVisual3D wholeModel3 = new ModelVisual3D();
        public ModelVisual3D rightVisual3 = new ModelVisual3D();
        public ModelVisual3D rightUpperVisual3 = new ModelVisual3D();
        public ModelVisual3D rightForeVisual3 = new ModelVisual3D();
        public ModelVisual3D rightForeBodyVisual3 = new ModelVisual3D();
        public ModelVisual3D rightTipVisual3 = new ModelVisual3D();
        public ModelVisual3D leftVisual3 = new ModelVisual3D();
        public ModelVisual3D leftUpperVisual3 = new ModelVisual3D();
        public ModelVisual3D leftForeVisual3 = new ModelVisual3D();
        public ModelVisual3D leftForeBodyVisual3 = new ModelVisual3D();
        public ModelVisual3D grasperVisual3 = new ModelVisual3D();
        public ModelVisual3D jawOneVisual3 = new ModelVisual3D();
        public ModelVisual3D jawTwoVisual3 = new ModelVisual3D();
        public ModelVisual3D yolkVisual3 = new ModelVisual3D();
        public ModelVisual3D rightSpaceVisual3 = new ModelVisual3D();
        public ModelVisual3D leftSpaceVisual3 = new ModelVisual3D();
        public ModelVisual3D needleVisual3 = new ModelVisual3D();

        public VirtualRobotWindow newWindow = new VirtualRobotWindow();
        public GraphicalView graphicView;
        
        
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

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["RightUpperBevel"].UniqueID, (message) =>
            {
                rub = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["RightLowerBevel"].UniqueID, (message) =>
            {
                rlb = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["RightElbow"].UniqueID, (message) =>
            {
                rel = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LeftUpperBevel"].UniqueID, (message) =>
            {
                lub = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LeftLowerBevel"].UniqueID, (message) =>
            {
                llb = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LeftElbow"].UniqueID, (message) =>
            {
                lel = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["GrasperOpen"].UniqueID, (message) =>
            {
                jOpen = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["GrasperTwist"].UniqueID, (message) =>
            {
                jTwist = message.Value;
                DisplayModel();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["GrasperTwist"].UniqueID, (message) =>
            {
                cTwist = message.Value;
                DisplayModel();
            });

            base.PostLoadSetup();
        }

        public VirtualRobot()
        {
            InitializeComponent();
            this.TypeName = "Virtual Robot";
            
            Inputs.Add("RightUpperBevel", new ViewModel.InputSignalViewModel("RightUpperBevel", this.InstanceName));
            Inputs.Add("RightLowerBevel", new ViewModel.InputSignalViewModel("RightLowerBevel", this.InstanceName));
            Inputs.Add("RightElbow", new ViewModel.InputSignalViewModel("RightElbow", this.InstanceName));
            Inputs.Add("LeftUpperBevel", new ViewModel.InputSignalViewModel("LeftUpperBevel", this.InstanceName));
            Inputs.Add("LeftLowerBevel", new ViewModel.InputSignalViewModel("LeftLowerBevel", this.InstanceName));
            Inputs.Add("LeftElbow", new ViewModel.InputSignalViewModel("LeftElbow", this.InstanceName));
            Inputs.Add("GrasperOpen", new ViewModel.InputSignalViewModel("GrasperOpen", this.InstanceName));
            Inputs.Add("GrasperTwist", new ViewModel.InputSignalViewModel("GrasperTwist", this.InstanceName));
            Inputs.Add("CauteryTwist", new ViewModel.InputSignalViewModel("CauteryTwist", this.InstanceName));

            PostLoadSetup();

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
            string needlePath = startupPath + "needle2.stl";

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
            var needle = up.Load(needlePath, this.dispatcher);

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
            GeometryModel3D Nmodel = needle.Children[0] as GeometryModel3D;

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
            Nmodel.Material = material;
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

            rightTipVisual2.Content = TRmodel;

            rightTipVisual3.Content = TRmodel;
            // Define right forearm group
            rightForeBodyVisual.Content = FRmodel;
            rightForeVisual.Children.Clear();
            rightForeVisual.Children.Add(rightForeBodyVisual);
            rightForeVisual.Children.Add(rightTipVisual);

            rightForeBodyVisual2.Content = FRmodel;
            rightForeVisual2.Children.Clear();
            rightForeVisual2.Children.Add(rightForeBodyVisual2);
            rightForeVisual2.Children.Add(rightTipVisual2);

            rightForeBodyVisual3.Content = FRmodel;
            rightForeVisual3.Children.Clear();
            rightForeVisual3.Children.Add(rightForeBodyVisual3);
            rightForeVisual3.Children.Add(rightTipVisual3);
            // Define right arm group
            rightUpperVisual.Content = URmodel;
            rightVisual.Children.Clear();
            rightVisual.Children.Add(rightUpperVisual);
            rightVisual.Children.Add(rightForeVisual);

            rightUpperVisual2.Content = URmodel;
            rightVisual2.Children.Clear();
            rightVisual2.Children.Add(rightUpperVisual2);
            rightVisual2.Children.Add(rightForeVisual2);

            rightUpperVisual3.Content = URmodel;
            rightVisual3.Children.Clear();
            rightVisual3.Children.Add(rightUpperVisual3);
            rightVisual3.Children.Add(rightForeVisual3);
            // Left grasper open/close
            jawOneVisual.Content = GJ1model;
            jawTwoVisual.Content = GJ2model;
            needleVisual.Content = Nmodel;

            jawOneVisual2.Content = GJ1model;
            jawTwoVisual2.Content = GJ2model;
            needleVisual2.Content = Nmodel;

            jawOneVisual3.Content = GJ1model;
            jawTwoVisual3.Content = GJ2model;
            needleVisual3.Content = Nmodel;

            // Define grasper group
            yolkVisual.Content = GYmodel;
            grasperVisual.Children.Clear();
            grasperVisual.Children.Add(yolkVisual);
            grasperVisual.Children.Add(jawOneVisual);
            grasperVisual.Children.Add(jawTwoVisual);
            grasperVisual.Children.Add(needleVisual);

            yolkVisual2.Content = GYmodel;
            grasperVisual2.Children.Clear();
            grasperVisual2.Children.Add(yolkVisual2);
            grasperVisual2.Children.Add(jawOneVisual2);
            grasperVisual2.Children.Add(jawTwoVisual2);
            grasperVisual2.Children.Add(needleVisual2);

            yolkVisual3.Content = GYmodel;
            grasperVisual3.Children.Clear();
            grasperVisual3.Children.Add(yolkVisual3);
            grasperVisual3.Children.Add(jawOneVisual3);
            grasperVisual3.Children.Add(jawTwoVisual3);
            grasperVisual3.Children.Add(needleVisual3);

            // Define left forearm group
            leftForeBodyVisual.Content = FLmodel;
            leftForeVisual.Children.Clear();
            leftForeVisual.Children.Add(leftForeBodyVisual);
            leftForeVisual.Children.Add(grasperVisual);

            leftForeBodyVisual2.Content = FLmodel;
            leftForeVisual2.Children.Clear();
            leftForeVisual2.Children.Add(leftForeBodyVisual2);
            leftForeVisual2.Children.Add(grasperVisual2);

            leftForeBodyVisual3.Content = FLmodel;
            leftForeVisual3.Children.Clear();
            leftForeVisual3.Children.Add(leftForeBodyVisual3);
            leftForeVisual3.Children.Add(grasperVisual3);
            // Define left arm group
            leftUpperVisual.Content = ULmodel;
            leftVisual.Children.Clear();
            leftVisual.Children.Add(leftUpperVisual);
            leftVisual.Children.Add(leftForeVisual);

            leftUpperVisual2.Content = ULmodel;
            leftVisual2.Children.Clear();
            leftVisual2.Children.Add(leftUpperVisual2);
            leftVisual2.Children.Add(leftForeVisual2);

            leftUpperVisual3.Content = ULmodel;
            leftVisual3.Children.Clear();
            leftVisual3.Children.Add(leftUpperVisual3);
            leftVisual3.Children.Add(leftForeVisual3);
            // workspace
            rightSpaceVisual.Content = RWSmodel;
            leftSpaceVisual.Content = LWSmodel;
            needleVisual.Content = Nmodel;
            staticVisual.Children.Clear();
            staticVisual.Children.Add(rightSpaceVisual);
            staticVisual.Children.Add(leftSpaceVisual);

            // Add left and right arms to full model
            wholeModel.Content = SHmodel;
            wholeModel.Children.Clear();
            wholeModel.Children.Add(rightVisual);
            wholeModel.Children.Add(leftVisual);
//            wholeModel.Children.Add(staticVisual);

            wholeModel2.Content = SHmodel;
            wholeModel2.Children.Clear();
            wholeModel2.Children.Add(rightVisual2);
            wholeModel2.Children.Add(leftVisual2);

            wholeModel3.Content = SHmodel;
            wholeModel3.Children.Clear();
            wholeModel3.Children.Add(rightVisual3);
            wholeModel3.Children.Add(leftVisual3);

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
                rightTipVisual2.Transform = rTipTransform;
                rightTipVisual3.Transform = rTipTransform;
                // Right elbow rotation
                rightForeVisual.Transform = relTransform;
                rightForeVisual2.Transform = relTransform;
                rightForeVisual3.Transform = relTransform;
                // Right shoulder rotations
                Transform3DGroup rightArmTransform = new Transform3DGroup();
                rightArmTransform.Children.Add(rsxTransform);
                rightArmTransform.Children.Add(rsyTransform);
                rightVisual.Transform = rightArmTransform;
                rightVisual2.Transform = rightArmTransform;
                rightVisual3.Transform = rightArmTransform;
                // Left grasper open/close
                jawOneVisual.Transform = jaw1Transform;
                jawTwoVisual.Transform = jaw2Transform;
                jawOneVisual2.Transform = jaw1Transform;
                jawTwoVisual2.Transform = jaw2Transform;
                jawOneVisual3.Transform = jaw1Transform;
                jawTwoVisual3.Transform = jaw2Transform;
                // Left grasper rotations
                grasperVisual.Transform = graspTransform;
                grasperVisual2.Transform = graspTransform;
                grasperVisual3.Transform = graspTransform;
                // Left elbow rotaions
                leftForeVisual.Transform = lelTransform;
                leftForeVisual2.Transform = lelTransform;
                leftForeVisual3.Transform = lelTransform;
                // Left shoulder rotations
                Transform3DGroup leftArmTransform = new Transform3DGroup();
                leftArmTransform.Children.Add(lsxTransform);
                leftArmTransform.Children.Add(lsyTransform);
                leftVisual.Transform = leftArmTransform;
                leftVisual2.Transform = leftArmTransform;
                leftVisual3.Transform = leftArmTransform;
                // Whole model transform
                Transform3DGroup modelTransform = new Transform3DGroup();
                modelTransform.Children.Add(modelXTransform);
                modelTransform.Children.Add(modelYTransform);
                wholeModel.Transform = modelTransform;
                wholeModel2.Transform = modelTransform;
                wholeModel3.Transform = modelTransform;

                // Add content to HelixViewport3D in VirtualRobotWindow.xaml
                newWindow.FullModel = wholeModel;
                newWindow.DModel = wholeModel2;
                newWindow.SModel = wholeModel3;
                
            }));
            
        }

        /// <summary>
        /// The <see cref="XScale" /> property's name.
        /// </summary>
        public const string XScalePropertyName = "XScale";
        private double xScale = 1;
        /// <summary>
        /// Sets and gets the XScale property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double XScale
        {
            get
            {
                return xScale;
            }
            set
            {
                if (xScale == value)
                {
                    return;
                }
                xScale = value;
                RaisePropertyChanged(XScalePropertyName);
                DisplayModel();
            }
        }

        /// <summary>
        /// The <see cref="YScale" /> property's name.
        /// </summary>
        public const string YScalePropertyName = "YScale";
        private double yScale = 1;
        /// <summary>
        /// Sets and gets the YScale property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double YScale
        {
            get
            {
                return yScale;
            }
            set
            {
                if (yScale == value)
                {
                    return;
                }
                yScale = value;
                RaisePropertyChanged(YScalePropertyName);
                DisplayModel();
            }
        }

        /// <summary>
        /// The <see cref="ZScale" /> property's name.
        /// </summary>
        public const string ZScalePropertyName = "ZScale";
        private double zScale = 1;
        /// <summary>
        /// Sets and gets the BoxLength property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double ZScale
        {
            get
            {
                return zScale;
            }
            set
            {
                if (zScale == value)
                {
                    return;
                }
                zScale = value;
                RaisePropertyChanged(ZScalePropertyName);
                DisplayModel();
            }
        }

        /// <summary>
        /// The <see cref="BoxX" /> property's name.
        /// </summary>
        public const string BoxXPropertyName = "BoxX";
        private double boxX = 0;
        /// <summary>
        /// Sets and gets the BoxX property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double BoxX
        {
            get
            {
                return boxX;
            }
            set
            {
                if (boxX == value)
                {
                    return;
                }
                boxX = value;
                RaisePropertyChanged(BoxXPropertyName);
                DisplayModel();
            }
        }

        /// <summary>
        /// The <see cref="BoxY" /> property's name.
        /// </summary>
        public const string BoxYPropertyName = "BoxY";
        private double boxY = 0;
        /// <summary>
        /// Sets and gets the BoxY property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double BoxY
        {
            get
            {
                return boxY;
            }
            set
            {
                if (boxY == value)
                {
                    return;
                }
                boxY = value;
                RaisePropertyChanged(BoxYPropertyName);
                DisplayModel();
            }
        }

        /// <summary>
        /// The <see cref="BoxZ" /> property's name.
        /// </summary>
        public const string BoxZPropertyName = "BoxZ";
        private double boxZ = 0;
        /// <summary>
        /// Sets and gets the BoxZ property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double BoxZ
        {
            get
            {
                return boxZ;
            }
            set
            {
                if (boxZ == value)
                {
                    return;
                }
                boxZ = value;
                RaisePropertyChanged(BoxZPropertyName);
                DisplayModel();
            }
        }

        private RelayCommand<string> startCommand;

        /// <summary>
        /// Gets the StartCommand.
        /// </summary>
        public RelayCommand<string> StartCommand
        {
            get
            {
                return startCommand
                    ?? (startCommand = new RelayCommand<string>(
                    p =>
                    {
                        newWindow.Show();
                    }));
            }
        }
    }
}

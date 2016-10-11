using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Threading;
using path_generation;
using System.Windows.Media.Media3D;
using System.Windows.Forms;
namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for Clutch.xaml
    /// </summary>
    public partial class AutoSuture : PluginBase
    {
        //private const double PICK_ENTRY = 1, PICK_EXIT = 2, INITIALIZE_SUTURING = 3, PRE_SUTURING = 4, DO_SUTURING = 5, END_SUTURING = 6;
        double x, y, z, twist;
        double x_clutchOffset = 0, y_clutchOffset = 0, z_clutchOffset = 0, twist_clutchOffset = 0;
        Suturing suturing = new Suturing();
        System.Windows.Forms.Timer stepTimer = new System.Windows.Forms.Timer();





        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["X"].UniqueID, (message) =>
            {
                x = message.Value;
                if (suturing.state < 3)
                    Outputs["X"].Value = x + x_clutchOffset;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Y"].UniqueID, (message) =>
            {
                y = message.Value;
                if (suturing.state < 3)
                    Outputs["Y"].Value = y + y_clutchOffset;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Z"].UniqueID, (message) =>
            {
                z = message.Value;
                if (suturing.state < 3)
                    Outputs["Z"].Value = z + z_clutchOffset;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Roll/Twist"].UniqueID, (message) =>
            {
                twist = message.Value;
                if (suturing.state < 3)
                    Outputs["Twist"].Value = twist + twist_clutchOffset;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["leftUpperBevel"].UniqueID, (message) =>
            {
                suturing.needle.kinematics.leftUpperBevel = message.Value;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["leftLowerBevel"].UniqueID, (message) =>
            {
                suturing.needle.kinematics.leftLowerBevel = message.Value;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["leftElbow"].UniqueID, (message) =>
            {
                suturing.needle.kinematics.leftElbow = message.Value;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Entry"].UniqueID, (message) =>
            {
                if (suturing.state == 1)// select entry point
                {
                    suturing.SELECT_ENTRY(x, y, z);
                }
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Exit"].UniqueID, (message) =>
            {
                if (suturing.state == 2)// select entry point
                {
                    suturing.SELECT_EXIT(x, y, z);
                }
            });

            base.PostLoadSetup();
        }
        public AutoSuture()
        {
            this.TypeName = "AutoSuture";
            this.PluginInfo = "";
            InitializeComponent();

            // OUTPUTS
            Outputs.Add("X", new ViewModel.OutputSignalViewModel("X"));
            Outputs.Add("Y", new ViewModel.OutputSignalViewModel("Y"));
            Outputs.Add("Z", new ViewModel.OutputSignalViewModel("Z"));
            Outputs.Add("Twist", new ViewModel.OutputSignalViewModel("Twist"));
            Outputs.Add("Trajectory_X", new ViewModel.OutputSignalViewModel("Trajectory_X"));
            Outputs.Add("Trajectory_Y", new ViewModel.OutputSignalViewModel("Trajectory_Y"));
            Outputs.Add("Trajectory_Z", new ViewModel.OutputSignalViewModel("Trajectory_Z"));
            Outputs.Add("Trajectory_Twist", new ViewModel.OutputSignalViewModel("Trajectory_Twist"));
            Outputs.Add("Clutch", new ViewModel.OutputSignalViewModel("Clutch"));

            // INPUTS
            Inputs.Add("X", new ViewModel.InputSignalViewModel("X", this.InstanceName));
            Inputs.Add("Y", new ViewModel.InputSignalViewModel("Y", this.InstanceName));
            Inputs.Add("Z", new ViewModel.InputSignalViewModel("Z", this.InstanceName));
            Inputs.Add("Roll/Twist", new ViewModel.InputSignalViewModel("Roll/Twist", this.InstanceName));
            Inputs.Add("Entry", new ViewModel.InputSignalViewModel("Entry", this.InstanceName));
            Inputs.Add("Exit", new ViewModel.InputSignalViewModel("Exit", this.InstanceName));
            Inputs.Add("leftUpperBevel", new ViewModel.InputSignalViewModel("leftUpperBevel", this.InstanceName));
            Inputs.Add("leftLowerBevel", new ViewModel.InputSignalViewModel("leftLowerBevel", this.InstanceName));
            Inputs.Add("leftElbow", new ViewModel.InputSignalViewModel("leftElbow", this.InstanceName));

            //End Sutring button
            EndSuturingButton.IsEnabled = false;

            // set up output timer for performing suturing
            stepTimer.Interval = 100;
            stepTimer.Tick += StepTimer_Tick;

            PostLoadSetup();
        }
        private void StepTimer_Tick(object sender, EventArgs e)
        {
            if (suturing.state == 3)// checked if entry&exit are valid. create the trajectory and needle
            {
                if (suturing.INITIALIZE_SUTURING())
                    Outputs["Clutch"].Value = 1; // enalble clutch if suturing was initialized
                else
                    end_suturing();
            }
            if (suturing.state == 4)
            {
                //suturing.PRE_SUTURING();
                suturing.state++;
            }
            if (suturing.state == 5) //calculation of needle holder position
            {
                if(suturing.DO_SUTURING())
                    end_suturing();
                //update_output(suturing.grasper.pos, suturing.grasper.twist);
                update_grasper_output(suturing.needle.needle_holder_position, -suturing.needle.needle_holder_twist * 180 / Math.PI);
                update_trajectori_output(suturing.trajectory.needle_tip_position);
            }
        }

        const double MAX_POSITION_INCREMENT = 300;
        const double MAX_TWIST_INCREMENT = 450;
        public void update_grasper_output(Vector3D pos_new, double twist_new)
        {
            update_grasper_output(pos_new);
            update_grasper_output(twist_new);

        }
        public void update_grasper_output(Vector3D pos_new)
        {
            Vector3D pos_incr = new Vector3D(Outputs["X"].Value - pos_new.X, Outputs["Y"].Value - pos_new.Y, Outputs["Z"].Value - pos_new.Z);
            if (pos_incr.Length < 300)
            {
                Outputs["X"].Value = pos_new.X;
                Outputs["Y"].Value = pos_new.Y;
                Outputs["Z"].Value = pos_new.Z;
            }
            else
            {
                end_suturing();
                Console.Write("\nPosition increment is: {0}\n", pos_incr.Length);
                Console.Write("\nCurrent position increment is: [{0}, {1}, {2}]\nNext position is: [{3}, {4}, {5}]", Outputs["X"].Value, Outputs["Y"].Value, Outputs["Z"].Value, pos_new.X, pos_new.Y, pos_new.Z);
                MessageBox.Show("The increment for the position is bigger than 30 (mm). See the output for more info.");
            }
        }
        public void update_grasper_output(double twist_new)
        {
            double twist_incr = Outputs["Twist"].Value - twist_new;
            if (Math.Abs(twist_incr) < 450)
            {
                Outputs["Twist"].Value = twist_new;
                //Outputs["Twist"].Value = -needle.get_needle_holder_twist() * 180 / Math.PI;
            }
            else
            {
                end_suturing();
                Console.Write("\nTwist increment is {0}", Math.Abs(twist_incr));
                Console.Write("\nCurrent twist increment is: {0}\nNext twist is: {1}", Outputs["Twist"].Value, twist_new);
                MessageBox.Show("The increment for the twist is bigger than 45 degrees. See the output for more info.");
            }
        }
        public void update_trajectori_output(Vector3D pos_new)
        {
            Outputs["Trajectory_X"].Value = pos_new.X;
            Outputs["Trajectory_Y"].Value = pos_new.Y;
            Outputs["Trajectory_Z"].Value = pos_new.Z;
        }
        /*


        private Vector3D get_forearm_orientation()
        {
            double LengthUpperArm = 68.58;
            double LengthForearm = 96.393;
            // calculate forward kinematics and haptic forces, assuming kineAngle[0] is leftUpperBevel and kineAngle[1] is leftLowerBevel
            double theta1 = ((leftUpperBevel + leftLowerBevel) / 2) * Math.PI / 180;
            double theta2 = ((leftUpperBevel - leftLowerBevel) / 2) * Math.PI / 180;
            double theta3 = leftElbow * Math.PI / 180;

            // calculate forward kinematics and haptic forces
            double shoulder_Z = LengthUpperArm * Math.Cos(theta1) * Math.Cos(theta2) - LengthForearm * (Math.Sin(theta1) * Math.Sin(theta3) - Math.Cos(theta1) * Math.Cos(theta2) * Math.Cos(theta3));
            double shoulder_Y = LengthUpperArm * Math.Sin(theta2) + LengthForearm * Math.Sin(theta2) * Math.Cos(theta3);
            double shoulder_X = LengthUpperArm * Math.Sin(theta1) * Math.Cos(theta2) + LengthForearm * (Math.Cos(theta1) * Math.Sin(theta3) + Math.Sin(theta1) * Math.Cos(theta2) * Math.Cos(theta3));

            double elbow_Z = LengthUpperArm * Math.Cos(theta1) * Math.Cos(theta2);
            double elbow_Y = LengthUpperArm * Math.Sin(theta2) ;
            double elbow_X = LengthUpperArm * Math.Sin(theta1) * Math.Cos(theta2);

            Vector3D forearm_orientation = new Vector3D(shoulder_X - elbow_X, shoulder_Y - elbow_Y, shoulder_Z - elbow_Z);
            return forearm_orientation;
        }*/
        //private Vector3D find_grasper_location(Vector3D tip)
        //{
            /*Matrix3D T5 = new Matrix3D(1, 0,  0,  0,
                                     0, 1, 0, 2 * trajectory.needle_radius,
                                     0, 0,  1,  0,
                                     0, 0,  0,  1);
            T5.Invert();
            Vector3D temp = new Vector3D();
            temp = T5.Transform(tip);
            return temp;*/
            //Vector3D grasper = new Vector3D();
            //Matrix3D T = transformation_matrix();
            //Vector3D e_x = new Vector3D(T.M11, T.M12, T.M13);
            //grasper = Vector3D.Add(tip, 2 * trajectory.needle_radius * e_x);
            //return grasper;

        //}
        //private Vector3D find_tip_location(Vector3D grasper)
        //{
            /*Vector3D tip = new Vector3D(0,-1,0);
            Vector3D rotated = 2 * trajectory.needle_radius * transformation_matrix().Transform(tip);
            Vector3D corrected = new Vector3D(rotated.Y, -rotated.Z, rotated.X);
            tip = grasper + corrected;
            //tip = grasper + 2 * trajectory.needle_radius * transformation_matrix().Transform(tip);

            NeedleKinematics kinematics = new NeedleKinematics();
            kinematics.update_kinematics(leftUpperBevel, leftLowerBevel, leftElbow, twist);
            Matrix3D M5 = kinematics.transformation_matrix(5);
            Vector3D v5 = new Vector3D(M5.M14, M5.M24, M5.M34);
            v5 = NeedleKinematics.correction(v5);
            Matrix3D M4 = kinematics.transformation_matrix(4);
            Vector3D v4 = new Vector3D(M4.M14, M4.M24, M4.M34);
            v4 = NeedleKinematics.correction(v4);
            Matrix3D M45 = kinematics.transformation_matrix(45);
            Vector3D v45 = new Vector3D(M45.M14, M45.M24, M45.M34);
            return tip;*/
            //kinematics.update_kinematics(leftUpperBevel, leftLowerBevel, leftElbow, twist);

            //Matrix3D M4 = kinematics.transformation_matrix(4);
            //Vector3D v4 = new Vector3D(M4.M14, M4.M24, M4.M34);
            //v4 = NeedleKinematics.correction(v4);
            //Matrix3D M6 = kinematics.transformation_matrix(6);
            //Matrix3D M5 = kinematics.transformation_matrix(5);
            //Vector3D v5 = new Vector3D(M5.M14, M5.M24, M5.M34);
            //Vector3D v6 = new Vector3D(M6.M14, M6.M24, M6.M34);

            //v5 = NeedleKinematics.correction(v5);
            //v6 = NeedleKinematics.correction(v6);

            //return v6;
        //}
        /*private Matrix3D transformation_matrix()
        {
            double LengthUpperArm = 68.58;
            double LengthForearm = 96.393;
            // calculate forward kinematics and haptic forces, assuming kineAngle[0] is leftUpperBevel and kineAngle[1] is leftLowerBevel
            double theta1 = ((leftUpperBevel + leftLowerBevel) / 2) * Math.PI / 180; //theta1 = 0;
            double theta2 = ((leftUpperBevel - leftLowerBevel) / 2) * Math.PI / 180; //theta2 = 0;
            double theta3 = leftElbow * Math.PI / 180;// theta3 = 0;
            //Matrix3D temp = new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
            //Vector3D v1 = new Vector3D(0, 0, 10);
            //Vector3D v2 = new Vector3D();
            double c1 = Math.Cos(theta1);
            double s1 = Math.Sin(theta1);
            double c2 = Math.Cos(theta2);
            double s2 = Math.Sin(theta2);
            double c3 = Math.Cos(theta3);
            double s3 = Math.Sin(theta3); //twist = 90;
            double theta4 = twist * Math.PI / 180;
            double c4 = Math.Cos(theta4);
            double s4 = Math.Sin(theta4);
            Matrix3D T1 = new Matrix3D(c1,   -s1,   0,   0,
                                       s1,   c1,    0,   0,
                                        0,   0,     1,   0,
                                        0,   0,     0,   1);
            Matrix3D T2 = new Matrix3D(c2,  -s2,    0,      0,
                                       0,   0,  -1,     0,
                                       s2,  c2,  0,      0,
                                        0,  0,   0,      1);
            Matrix3D T3 = new Matrix3D(c3, -s3, 0, LengthUpperArm,
                                       0, 0, 1, 0,
                                       -s3, -c3, 0, 0,
                                       0, 0, 0, 1);
            Matrix3D T4 = new Matrix3D(1, 0, 0, LengthForearm,
                                       0, c4, -s4, 0,
                                        0, s4, c4, 0,
                                        0, 0, 0, 1);
            Matrix3D T = new Matrix3D();
            T = Matrix3D.Multiply(T1, Matrix3D.Multiply(T2, Matrix3D.Multiply(T3, T4)));
            ///Matrix3D T_rearranged = new Matrix3D(T.M21, T.M22, T.M23, T.M24,
                                                 T.M31, T.M32, T.M33, T.M34,
                                                 T.M11, T.M12, T.M13, T.M14,
                                                 T.OffsetX, T.OffsetX, T.OffsetZ, T.M44);///
            //T = T1 * T2 * T3 * T4;
            //Vector3D e_x = new Vector3D(0,0,0);
            //e_x = T.Transform(e_x);
            return T;
        }*/

        /// <summary>
        /// The <see cref="StartSuturingButtonText" /> property's name.
        /// </summary>
        public const string StartSuturingButtonTextPropertyName = "StartSuturingButtonText";

        private string startSuturingButtonText = "Start Sturing";

        /// <summary>
        /// Sets and gets the ConnectButtonText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string StartSuturingButtonText
        {
            get
            {
                return startSuturingButtonText;
            }

            set
            {
                if (startSuturingButtonText == value)
                {
                    return;
                }

                startSuturingButtonText = value;
                RaisePropertyChanged(StartSuturingButtonTextPropertyName);
            }
        }



        private RelayCommand startSuturingCommand;

        /// <summary>
        /// Gets the ResetOffsetsCommand.
        /// </summary>
        public RelayCommand StartSuturingCommand
        {
            get
            {
                return startSuturingCommand
                    ?? (startSuturingCommand = new RelayCommand(
                    () =>
                    {
                        start_suturing();
                    }));
                
            }
        }
        private void start_suturing()
        {
            stepTimer.Start();
            suturing.START_SUTURING();
            Outputs["Clutch"].Value = 0;
            //Outputs["Twist"].Value = 0;
            StartSuturingButtonText = "Suturing...";
            StartSuturingButton.IsEnabled = false;
            EndSuturingButton.IsEnabled = true;
        }
        private RelayCommand endSuturingCommand;

        /// <summary>
        /// Gets the ResetOffsetsCommand.
        /// </summary>
        public RelayCommand EndSuturingCommand
        {
            get
            {
                return endSuturingCommand
                    ?? (endSuturingCommand = new RelayCommand(
                    () =>
                    {
                        if (!EndSuturingCommand.CanExecute(null))
                        {
                            return;
                        }
                        end_suturing();
                    }));
            }
        }
        private void end_suturing()
        {
            suturing.END_SUTURING();
            x_clutchOffset = Outputs["X"].Value - x;
            y_clutchOffset = Outputs["Y"].Value - y;
            z_clutchOffset = Outputs["Z"].Value - z;
            twist_clutchOffset = Outputs["Twist"].Value - twist;
            Outputs["Clutch"].Value = 0;
            StartSuturingButtonText = "Start Suturing";
            StartSuturingButton.IsEnabled = true;
            EndSuturingButton.IsEnabled = false;
            stepTimer.Stop();
        }

    }
}

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
        Trajectory trajectory; // ideal trajectory; gives the needle tip at every moment
        Needle needle; // the trajectory will initialize the needle, then will get updated based on new needle tip.
        NeedleKinematics kinematics;
        double x, y, z, twist;
        double x_clutchOffset = 0, y_clutchOffset = 0, z_clutchOffset = 0, twist_clutchOffset = 0;
        double leftUpperBevel, leftLowerBevel, leftElbow; // for calculating orientation of forearm
        double t = 0;
        static int state;

        System.Windows.Forms.Timer stepTimer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer interpolationTimer = new System.Windows.Forms.Timer();


        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["X"].UniqueID, (message) =>
            {
                x = message.Value;
                if (state < 3)
                    Outputs["X"].Value = x + x_clutchOffset;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Y"].UniqueID, (message) =>
            {
                y = message.Value;
                if (state < 3)
                    Outputs["Y"].Value = y + y_clutchOffset;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Z"].UniqueID, (message) =>
            {
                z = message.Value;
                if (state < 3)
                    Outputs["Z"].Value = z + z_clutchOffset;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Roll/Twist"].UniqueID, (message) =>
            {
                twist = message.Value;
                if (state < 3)
                    Outputs["Twist"].Value = twist + twist_clutchOffset;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["leftUpperBevel"].UniqueID, (message) =>
            {
                leftUpperBevel = message.Value;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["leftLowerBevel"].UniqueID, (message) =>
            {
                leftLowerBevel = message.Value;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["leftElbow"].UniqueID, (message) =>
            {
                leftElbow = message.Value;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Entry"].UniqueID, (message) =>
            {
                if (state == 1)// select entry point
                {
                    SELECT_ENTRY();
                }
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Exit"].UniqueID, (message) =>
            {
                if (state == 2)// select entry point
                {
                    SELECT_EXIT();
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

            /*// Initializing
            state = 1; // state initialization: state 1 indicates entry, state 2 exit and state 3 the suturing
            trajectory = new Trajectory();
            needle = new Needle();
            Outputs["Clutch"].Value = 0;
             * */
            EndSuturingButton.IsEnabled = false;

            // set up output timer
            stepTimer.Interval = 100;
            stepTimer.Tick += StepTimer_Tick;

            PostLoadSetup();
        }
        private void StepTimer_Tick(object sender, EventArgs e)
        {
            if (state == 3)// checked if entry&exit are valid. create the trajectory and needle
            {
                INITIALIZE_SUTURING();
            }
            if (state == 4)
            {
                //PRE_SUTURING();
                state++;
            }
            if (state == 5) //calculation of needle holder position
            {
                DO_SUTURING();
            }
        }
        private void SELECT_ENTRY()
        {
            Console.Write("\nState 1: ENTRY POINT selected\n");
            Vector3D entry_point = new Vector3D(x, y, z);
            //Vector3D entry_point = new Vector3D(-30, 0, 130);
            trajectory.entry_point = entry_point;
            //trajectory.entry_point = find_tip_location(entry_point);
            Print.print_vector(entry_point);
            state++;
        }
        private void SELECT_EXIT()
        {
            Console.Write("\nState 2 EXIT POINT selected\n");
            Vector3D exit_point = new Vector3D(x, y, z);
            //Vector3D exit_point = new Vector3D(-25, 0, 130);
            trajectory.exit_point = exit_point;
            //trajectory.exit_point = find_tip_location(exit_point);
            Print.print_vector(exit_point);
            state++;
        }
        private void INITIALIZE_SUTURING()
        {
            if ((trajectory.exit_point - trajectory.entry_point).Length > 2 * trajectory.needle_radius)
            {
                state = 1;
                END_SUTURING();
                MessageBox.Show("Entery and exit points are not valid!\nPick the entry and exit points again.");
            }
            else
            {
                trajectory.create();
                needle.local_coordinate = trajectory.local_coordinate;
                Outputs["Clutch"].Value = 1; // enalble clutch
                needle.set_needle_tip_position(trajectory.get_needle_tip_position());
                needle.set_needle_tip_twist(trajectory.get_needle_tip_twist());
                state++;
            }
        }
        private void PRE_SUTURING()
        {
            kinematics.update_kinematics(leftUpperBevel, leftLowerBevel, leftElbow, twist);
            Vector3D start_position = new Vector3D(Outputs["X"].Value, Outputs["Y"].Value, Outputs["Z"].Value);
            Vector3D target_position = needle.get_needle_holder_position(kinematics.transformation_matrix(46));
            double start_twist = Outputs["Twist"].Value;
            double target_twist = twist_correction(needle.get_needle_holder_twist());

            if (vector_interpolation(start_position, target_position) & digit_interpolation(start_twist, target_twist))
                state++;
            System.Threading.Thread.Sleep(100);
        }
        private void DO_SUTURING()
        {
            t = t + trajectory.incr;
            Console.Write("\n****************t: {0}\n", t);
            //Console.WriteLine("{0}\t{1}\t{2}", p.pos.x, p.pos.y, p.pos.z);
            //dof4 end_effector;
            //end_effector.pos = trajectory.get_needle_tip_position();
            //end_effector.twist = trajectory.get_needle_tip_twist();
            needle.set_needle_tip_position(trajectory.get_needle_tip_position());
            needle.set_needle_tip_twist(trajectory.get_needle_tip_twist());
            kinematics.update_kinematics(leftUpperBevel, leftLowerBevel, leftElbow, trajectory.get_needle_tip_twist());
            //update_output(needle.get_needle_holder_position(kinematics.transformation_matrix(46)), twist_correction(needle.get_needle_holder_twist()));
            update_output(needle.get_needle_holder_position(), twist_correction(needle.get_needle_holder_twist()));
            if (t >= 1 * Math.PI)
            {
                END_SUTURING();
                Console.Write("\nAutomatically ended\n");
            }
        }
        private void update_output(Vector3D pos_new, double twist_new)
        {
            Vector3D pos_incr = new Vector3D(Outputs["X"].Value - pos_new.X, Outputs["Y"].Value - pos_new.Y, Outputs["Z"].Value - pos_new.Z);
            double twist_incr = Outputs["Twist"].Value - twist_new;
            if (pos_incr.Length < 300 & Math.Abs(twist_incr) < 450)
            {
                Outputs["X"].Value = pos_new.X;
                Outputs["Y"].Value = pos_new.Y;
                Outputs["Z"].Value = pos_new.Z;
                Outputs["Twist"].Value = twist_new;
                //Outputs["Twist"].Value = -needle.get_needle_holder_twist() * 180 / Math.PI;
            }
            else
            {
                state = 1;
                END_SUTURING();
                Console.Write("\nPosition increment is: {0}\nTwist increment is {1}", pos_incr.Length, Math.Abs(twist_incr));
                Console.Write("\nCurrent position increment is: [{0}, {1}, {2}]\nNext position is: [{3}, {4}, {5}]", Outputs["X"].Value, Outputs["Y"].Value, Outputs["Z"].Value, pos_new.X, pos_new.Y, pos_new.Z);
                Console.Write("\nCurrent twist increment is: {0}\nNext twist is: {1}", Outputs["Twist"].Value, twist_new);
                MessageBox.Show("The increment for the position is bigger than 30 (mm) and/or for the twist is bigger than 45 degrees. See the output for more info.");
            }
        }
        private void update_output(Vector3D pos_new)
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
                state = 1;
                END_SUTURING();
                Console.Write("\nPosition increment is: {0}\n", pos_incr.Length);
                Console.Write("\nCurrent position increment is: [{0}, {1}, {2}]\nNext position is: [{3}, {4}, {5}]", Outputs["X"].Value, Outputs["Y"].Value, Outputs["Z"].Value, pos_new.X, pos_new.Y, pos_new.Z);
                MessageBox.Show("The increment for the position is bigger than 30 (mm). See the output for more info.");
            }
        }
        private void update_output(double twist_new)
        {
            double twist_incr = Outputs["Twist"].Value - twist_new;
            if (Math.Abs(twist_incr) < 450)
            {
                Outputs["Twist"].Value = twist_new;
                //Outputs["Twist"].Value = -needle.get_needle_holder_twist() * 180 / Math.PI;
            }
            else
            {
                state = 1;
                END_SUTURING();
                Console.Write("\nTwist increment is {0}", Math.Abs(twist_incr));
                Console.Write("\nCurrent twist increment is: {0}\nNext twist is: {1}", Outputs["Twist"].Value, twist_new);
                MessageBox.Show("The increment for the twist is bigger than 45 degrees. See the output for more info.");
            }
        }
        private double twist_correction(double t)
        {
            return (-t * 180 / Math.PI);
        }
        private bool vector_interpolation(Vector3D start, Vector3D target)
        {
            Vector3D mid = new Vector3D();
            Vector3D line = target - start;
            double length = line.Length;
            
            if (length > 5)
            {
                mid = start + 5 * line / length;
                update_output(mid);
                //Outputs["X"].Value = mid.X;
                //Outputs["Y"].Value = mid.Y;
                //Outputs["Z"].Value = mid.Z;
                return false;
            }
            else
            {
                mid = target;
                update_output(mid);
                //Outputs["X"].Value = mid.X;
                //Outputs["Y"].Value = mid.Y;
                //Outputs["Z"].Value = mid.Z;
                return true;
            }
        }
        private bool digit_interpolation(double start, double target)
        {
            double mid;
            double difference = target - start;
            if (Math.Abs(difference) > 5)
            {
                mid = start + 5 * Math.Sign(difference);
                update_output(mid);
                //Outputs["Twist"].Value = mid;
                return false;
            }
            else
            {
                mid = target;
                update_output(mid);
                //Outputs["Twist"].Value = mid;
                return true;
            }
        }
        private void START_SUTURING()
        {
            Console.Write("\nSuturing satrted.\n");
            trajectory = new Trajectory();
            needle = new Needle();
            kinematics = new NeedleKinematics();

            t = 0;
            state = 1; // state initialization: state 1 indicates entry, state 2 exit and state 3 the suturing
            Outputs["Clutch"].Value = 0;
            //Outputs["Twist"].Value = 0;
            stepTimer.Start();
            StartSuturingButtonText = "Suturing...";
            StartSuturingButton.IsEnabled = false;
            EndSuturingButton.IsEnabled = true;
        }
        private void END_SUTURING()
        {
            Console.Write("\nSuturing ended.\n");
            stepTimer.Stop();
            t = 0;
            state = 0;
            x_clutchOffset = Outputs["X"].Value - x;
            y_clutchOffset = Outputs["Y"].Value - y;
            z_clutchOffset = Outputs["Z"].Value - z;
            twist_clutchOffset = Outputs["Twist"].Value - twist;
            Outputs["Clutch"].Value = 0;
            StartSuturingButtonText = "Start Suturing";
            StartSuturingButton.IsEnabled = true;
            EndSuturingButton.IsEnabled = false;
            // set up interpolation timer
            interpolationTimer.Interval = 50;
            interpolationTimer.Tick += interpolationTimer_Tick;
            interpolationTimer.Start();
        }
        private void interpolationTimer_Tick(object sender, EventArgs e)
        {
            while (!digit_interpolation(Outputs["Twist"].Value, 0)) ;
            interpolationTimer.Stop();
        }

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
        }
        private Vector3D find_grasper_location(Vector3D tip)
        {
            /*Matrix3D T5 = new Matrix3D(1, 0,  0,  0,
                                     0, 1, 0, 2 * trajectory.needle_radius,
                                     0, 0,  1,  0,
                                     0, 0,  0,  1);
            T5.Invert();
            Vector3D temp = new Vector3D();
            temp = T5.Transform(tip);
            return temp;*/
            Vector3D grasper = new Vector3D();
            Matrix3D T = transformation_matrix();
            Vector3D e_x = new Vector3D(T.M11, T.M12, T.M13);
            grasper = Vector3D.Add(tip, 2 * trajectory.needle_radius * e_x);
            return grasper;

        }
        private Vector3D find_tip_location(Vector3D grasper)
        {
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
            kinematics.update_kinematics(leftUpperBevel, leftLowerBevel, leftElbow, twist);

            Matrix3D M4 = kinematics.transformation_matrix(4);
            Vector3D v4 = new Vector3D(M4.M14, M4.M24, M4.M34);
            v4 = NeedleKinematics.correction(v4);
            Matrix3D M6 = kinematics.transformation_matrix(6);
            Matrix3D M5 = kinematics.transformation_matrix(5);
            Vector3D v5 = new Vector3D(M5.M14, M5.M24, M5.M34);
            Vector3D v6 = new Vector3D(M6.M14, M6.M24, M6.M34);

            v5 = NeedleKinematics.correction(v5);
            v6 = NeedleKinematics.correction(v6);

            return v6;
        }
        private Matrix3D transformation_matrix()
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
            /*Matrix3D T_rearranged = new Matrix3D(T.M21, T.M22, T.M23, T.M24,
                                                 T.M31, T.M32, T.M33, T.M34,
                                                 T.M11, T.M12, T.M13, T.M14,
                                                 T.OffsetX, T.OffsetX, T.OffsetZ, T.M44);*/
            //T = T1 * T2 * T3 * T4;
            //Vector3D e_x = new Vector3D(0,0,0);
            //e_x = T.Transform(e_x);
            return T;
        }

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
                        START_SUTURING();
                    }));
                
            }
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
                        END_SUTURING();
                    }));
            }
        }
    }
}

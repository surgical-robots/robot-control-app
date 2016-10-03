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
        double x, y, z, twist;
        double x_clutchOffset = 0, y_clutchOffset = 0, z_clutchOffset = 0, twist_clutchOffset = 0;
        double leftUpperBevel, leftLowerBevel, leftElbow; // for calculating orientation of forearm
        double t = 0;
        static int state;

        System.Windows.Forms.Timer stepTimer = new System.Windows.Forms.Timer();

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
                    Console.Write("\nState 1: ENTRY POINT selected\n");
                    Vector3D entry_point = new Vector3D(x, y, z);
                    //Vector3D entry_point = new Vector3D(-30, 0, 130);
                    trajectory.entry_point = entry_point;
                    Print.print_vector(trajectory.entry_point);
                    state++;
                }
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Exit"].UniqueID, (message) =>
            {
                if (state == 2)// select entry point
                {
                    Console.Write("\nState 2 EXIT POINT selected\n");
                    Vector3D exit_point = new Vector3D(x, y, z);
                    //Vector3D exit_point = new Vector3D(-25, 0, 130);
                    trajectory.exit_point = exit_point;
                    Print.print_vector(trajectory.exit_point);
                    state++;
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
            stepTimer.Tick += StepTimer_Tick; ;

            PostLoadSetup();
        }

        private void StepTimer_Tick(object sender, EventArgs e)
        {
            if (state == 3)// checked if entry&exit are valid. create the trajectory and needle
            {
                if ((trajectory.exit_point - trajectory.entry_point).Length > 2 * trajectory.needle_radius)
                {
                    state = 1;
                    end_suturing();
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
            if (state == 4)
            {
                

                Vector3D start_position = new Vector3D(Outputs["X"].Value, Outputs["Y"].Value, Outputs["Z"].Value);
                Vector3D target_position = needle.get_needle_holder_position();
                double start_twist = Outputs["Twist"].Value;
                double target_twist = twist_correction(needle.get_needle_holder_twist());

                if (vector_interpolation(start_position, target_position) & digit_interpolation(start_twist, target_twist))
                    state++;
                
                //state++;
            }
            if (state == 5) //calculation of needle holder position
            {
                t = t + trajectory.incr;
                Console.Write("\n****************t: {0}\n", t);
                //Console.WriteLine("{0}\t{1}\t{2}", p.pos.x, p.pos.y, p.pos.z);
                //dof4 end_effector;
                //end_effector.pos = trajectory.get_needle_tip_position();
                //end_effector.twist = trajectory.get_needle_tip_twist();
                needle.set_needle_tip_position(trajectory.get_needle_tip_position());
                needle.set_needle_tip_twist(trajectory.get_needle_tip_twist());
                update_output(needle.get_needle_holder_position(), twist_correction(needle.get_needle_holder_twist()));
                if (t >=  0.5 * Math.PI)
                {
                    end_suturing();
                    Console.Write("\nAutomatically ended\n");
                }
            }
        }
        private void update_output(Vector3D pos_new, double twist_new)
        {
            Vector3D pos_incr = new Vector3D(Outputs["X"].Value - pos_new.X, Outputs["Y"].Value - pos_new.Y, Outputs["Z"].Value - pos_new.Z);
            double twist_incr = Outputs["Twist"].Value - twist_new;
            if (pos_incr.Length < 30 & Math.Abs(twist_incr) < 45)
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
                end_suturing();
                Console.Write("\nPosition increment is: {0}\nTwist increment is {1}", pos_incr.Length, Math.Abs(twist_incr));
                Console.Write("\nCurrent position increment is: [{0}, {1}, {2}]\nNext position is: [{3}, {4}, {5}]", Outputs["X"].Value, Outputs["Y"].Value, Outputs["Z"].Value, pos_new.X, pos_new.Y, pos_new.Z);
                Console.Write("\nCurrent twist increment is: {0}\nNext twist is: {1}", Outputs["Twist"].Value, twist_new);
                MessageBox.Show("The increment for the position is bigger than 30 (mm) and/or for the twist is bigger than 45 degrees. See the output for more info.");
            }
        }
        private void update_output(Vector3D pos_new)
        {
            Vector3D pos_incr = new Vector3D(Outputs["X"].Value - pos_new.X, Outputs["Y"].Value - pos_new.Y, Outputs["Z"].Value - pos_new.Z);
            if (pos_incr.Length < 30)
            {
                Outputs["X"].Value = pos_new.X;
                Outputs["Y"].Value = pos_new.Y;
                Outputs["Z"].Value = pos_new.Z;
            }
            else
            {
                state = 1;
                end_suturing();
                Console.Write("\nPosition increment is: {0}\n", pos_incr.Length);
                Console.Write("\nCurrent position increment is: [{0}, {1}, {2}]\nNext position is: [{3}, {4}, {5}]", Outputs["X"].Value, Outputs["Y"].Value, Outputs["Z"].Value, pos_new.X, pos_new.Y, pos_new.Z);
                MessageBox.Show("The increment for the position is bigger than 30 (mm). See the output for more info.");
            }
        }
        private void update_output(double twist_new)
        {
            double twist_incr = Outputs["Twist"].Value - twist_new;
            if (Math.Abs(twist_incr) < 45)
            {
                Outputs["Twist"].Value = twist_new;
                //Outputs["Twist"].Value = -needle.get_needle_holder_twist() * 180 / Math.PI;
            }
            else
            {
                state = 1;
                end_suturing();
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
        private void start_suturing()
        {
            Console.Write("\nSuturing satrted.\n");
            trajectory = new Trajectory();
            needle = new Needle();
            t = 0;
            state = 1; // state initialization: state 1 indicates entry, state 2 exit and state 3 the suturing
            Outputs["Clutch"].Value = 0;
            //Outputs["Twist"].Value = 0;
            stepTimer.Start();
            StartSuturingButtonText = "Suturing...";
            StartSuturingButton.IsEnabled = false;
            EndSuturingButton.IsEnabled = true;
        }
        private void end_suturing()
        {
            Console.Write("\nSuturing ended.\n");
            stepTimer.Stop();
            t = 0;
            state = 1;
            x_clutchOffset = Outputs["X"].Value - x;
            y_clutchOffset = Outputs["Y"].Value - y;
            z_clutchOffset = Outputs["Z"].Value - z;
            twist_clutchOffset = Outputs["Twist"].Value - twist;
            Outputs["Clutch"].Value = 0;
            StartSuturingButtonText = "Start Suturing";
            StartSuturingButton.IsEnabled = true;
            EndSuturingButton.IsEnabled = false;
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
                        /*if (!StartSuturingCommand.CanExecute(null))
                        {
                            return;
                        }*/

                        start_suturing();
                        
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
                        
                        /*state = 1;
                        t = 0;
                        Outputs["Clutch"].Value = 0;
                        stepTimer.Stop();
                         * */

                        end_suturing();
                    }));
            }
        }
    }
}

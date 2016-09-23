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
        //int version=2;
        //trajectory_version2 obj2;
        //trajectory_version3 obj3;
        Trajectory trajectory;
        Needle needle;
        //double r = 14;
        double x, y, z;
        double leftUpperBevel, leftLowerBevel, leftElbow; // for calculating orientation of forearm
        double t = 0;
        //double t_incr = Math.PI / 100;
        static int state;
        //double x_entry, y_entry, z_entry, x_exit, y_exit, z_exit, x_needle, y_needle, z_needle;
        //Vector needle_old, needle_new; // orientaton
        System.Windows.Forms.Timer stepTimer = new System.Windows.Forms.Timer();

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["X"].UniqueID, (message) =>
            {
                x = message.Value;
                if (state < 3)
                    Outputs["X"].Value = x;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Y"].UniqueID, (message) =>
            {
                y = message.Value;
                if (state < 3)
                    Outputs["Y"].Value = y;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Z"].UniqueID, (message) =>
            {
                z = message.Value;
                if (state < 3)
                    Outputs["Z"].Value = z;
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
                    //Vector3D entry_point = new Vector3D(x, y, z);
                    Vector3D entry_point = new Vector3D(-30, 0, 130);
                    trajectory.entry_point = entry_point;
                    state++;
                }
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Exit"].UniqueID, (message) =>
            {
                if (state == 2)// select entry point
                {
                    Console.Write("\nState 2 EXIT POINT selected\n");
                    //Vector3D exit_point = new Vector3D(x, y, z);
                    Vector3D exit_point = new Vector3D(-25, 0, 130);
                    trajectory.exit_point = exit_point;
                    state++;
                }


                        /*switch (version)
                        {
                            case 2:
                                obj2 = new trajectory_version2(entry_point, exit_point); // initializing trajectory
                                break;
                            case 3:
                                obj3 = new trajectory_version3(entry_point, exit_point); // initializing trajectory
                                break;
                        }*/
                        
                    

 

            });

            base.PostLoadSetup();
        }
        private void locate_needle()
        {

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
            Inputs.Add("Entry", new ViewModel.InputSignalViewModel("Entry", this.InstanceName));
            Inputs.Add("Exit", new ViewModel.InputSignalViewModel("Exit", this.InstanceName));
            Inputs.Add("leftUpperBevel", new ViewModel.InputSignalViewModel("leftUpperBevel", this.InstanceName));
            Inputs.Add("leftLowerBevel", new ViewModel.InputSignalViewModel("leftLowerBevel", this.InstanceName));
            Inputs.Add("leftElbow", new ViewModel.InputSignalViewModel("leftElbow", this.InstanceName));


            state = 1; // state initialization: state 1 indicates entry, state 2 exit and state 3 the suturing
            trajectory = new Trajectory();
            needle = new Needle();

            // set up output timer
            stepTimer.Interval = 100;
            stepTimer.Tick += StepTimer_Tick; ;

            PostLoadSetup();
        }

        private void StepTimer_Tick(object sender, EventArgs e)
        {
            if (state ==3)// checked if entry&exit are valid. create the trajectory and needle
            {
                if ((trajectory.exit_point - trajectory.entry_point).Length > trajectory.needle_radius)
                {
                    MessageBox.Show("Entery and exit points are not valid!");
                    state = 1;
                }
                else
                {
                    Console.Write("\nSuturing satrted.\n");
                    trajectory.create();
                    needle.local_coordinate = trajectory.local_coordinate;
                    Outputs["Clutch"].Value = 1; // enalble clutch
                    state++;
                }
            }
            if (state == 4) //calculation of needle holder position
            {
                //dof4 end_effector;
                //end_effector.pos = trajectory.get_needle_tip_position();
                //end_effector.twist = trajectory.get_needle_tip_twist();
                needle.set_needle_tip_position(trajectory.get_needle_tip_position());
                needle.set_needle_tip_twist(trajectory.get_needle_tip_twist());

                //Console.Write("\n****************S3 SUTURING STATRTS\n");
                /*dof4 p;
                switch (version)
                {
                    default:
                        p = obj2.end_effector(get_forearm_orientation(), t_incr);
                        break;
                    case 3:
                        p = obj3.end_effector(get_forearm_orientation(), t_incr);
                        break;
                }*/
                t = t + trajectory.incr;
                Console.Write("\n****************t: {0}\n", t);
                //Console.WriteLine("{0}\t{1}\t{2}", p.pos.x, p.pos.y, p.pos.z);

                Outputs["X"].Value = needle.get_needle_holder_position().X;
                Outputs["Y"].Value = needle.get_needle_holder_position().Y;
                Outputs["Z"].Value = needle.get_needle_holder_position().Z;
                Outputs["Twist"].Value = -needle.get_needle_holder_twist() * 180 / Math.PI;
                
                // calculating twist between two sequence
                /*
                double twist;
                if (needle_old != null)
                    twist = Math.Acos((needle_new.x * needle_old.x + needle_new.y * needle_old.y + needle_new.z * needle_old.z) / (Math.Sqrt(needle_new.x * needle_new.x + needle_new.y * needle_new.y + needle_new.z * needle_new.z) * Math.Sqrt(needle_old.x * needle_old.x + needle_old.y * needle_old.y + needle_old.z * needle_old.z)));
                else
                    twist = 0;
                Outputs["Twist"].Value = twist * 180/Math.PI;
                needle_old = p.ori;
                */
            

            //Outputs["pickPoint"].Value = entry_enabled == true ? 1 : 0;
            if (t > 1.5 * Math.PI)
            {
                stepTimer.Stop();
                state = 1;
                t = 0;
                //Outputs["Clutch"].Value = 0;
                Console.Write("\nAutomatically ended\n");
            }
                
            }
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
                        t = 0;
                        stepTimer.Start();
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
                        state = 1;
                        t = 0;
                        Outputs["Clutch"].Value = 0;
                        stepTimer.Stop();
                    }));
            }
        }
    }
}

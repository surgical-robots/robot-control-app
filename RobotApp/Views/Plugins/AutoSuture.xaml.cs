using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Threading;
using path_generation;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for Clutch.xaml
    /// </summary>
    public partial class AutoSuture : PluginBase
    {
        double x, y, z;
        double leftUpperBevel, leftLowerBevel, leftElbow; // for calculating orientation of forearm
        double t = 0;
        double t_incr = Math.PI / 10;
        static int state;
        double x_entry, y_entry, z_entry, x_exit, y_exit, z_exit, x_needle, y_needle, z_needle;
        Vector needle_old, needle_new; // orientaton
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
                if (state == 1)
                {
                    x_entry = x;
                    y_entry = y;
                    z_entry = z;
                    Console.Write("\n****************S1 ENTRY POINT\n");
                    state++;
                }
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Exit"].UniqueID, (message) =>
            {
                if (state == 2)
                {
                    x_exit = x;
                    y_exit = y;
                    z_exit = z;
                    Console.Write("\n****************S2 EXIT POINT\n");
                    state++;
                }
            });

            base.PostLoadSetup();
        }
        private void locate_needle()
        {

        }
        public AutoSuture()
        {
            Console.Write("\n****************S0 START\n");
            this.TypeName = "AutoSuture";
            this.PluginInfo = "";
            InitializeComponent();
            state = 1; // state initialization: state 1 indicates entry, state 2 exit and state 3 the suturing
            needle_old = null;
            needle_new = null;
            // OUTPUTS
            Outputs.Add("X", new ViewModel.OutputSignalViewModel("X"));
            Outputs.Add("Y", new ViewModel.OutputSignalViewModel("Y"));
            Outputs.Add("Z", new ViewModel.OutputSignalViewModel("Z"));
            Outputs.Add("Twist", new ViewModel.OutputSignalViewModel("Twist"));


            // INPUTS
            Inputs.Add("X", new ViewModel.InputSignalViewModel("X", this.InstanceName));
            Inputs.Add("Y", new ViewModel.InputSignalViewModel("Y", this.InstanceName));
            Inputs.Add("Z", new ViewModel.InputSignalViewModel("Z", this.InstanceName));
            Inputs.Add("Entry", new ViewModel.InputSignalViewModel("Entry", this.InstanceName));
            Inputs.Add("Exit", new ViewModel.InputSignalViewModel("Exit", this.InstanceName));
            Inputs.Add("leftUpperBevel", new ViewModel.InputSignalViewModel("leftUpperBevel", this.InstanceName));
            Inputs.Add("leftLowerBevel", new ViewModel.InputSignalViewModel("leftLowerBevel", this.InstanceName));
            Inputs.Add("leftElbow", new ViewModel.InputSignalViewModel("leftElbow", this.InstanceName));
            // set up output timer
            stepTimer.Interval = 100;
            stepTimer.Tick += StepTimer_Tick; ;

            PostLoadSetup();
        }

        private void StepTimer_Tick(object sender, EventArgs e)
        {
            //if (state < 3)
            //{
            //    Outputs["X"].Value = x;
            //    Outputs["Y"].Value = y;
            //    Outputs["Z"].Value = z;
            //}
            if (state == 3) //calculation of needle center
            {
                //Console.Write("\n****************S3 SUTURING STATRTS\n");
                x_needle = (x_entry + x_exit) / 2;
                y_needle = (y_entry + y_exit) / 2;
                z_needle = (z_entry + z_exit) / 2;
                point p;
                trajectory obj = new trajectory(x_needle, y_needle, z_needle);
                p = obj.end_effector(get_ori2(), t);
                t = t + t_incr;
                Console.Write("\n****************t: {0}\n", t);
                //Console.WriteLine("{0}\t{1}\t{2}", p.pos.x, p.pos.y, p.pos.z);
                Outputs["X"].Value = p.pos.x;
                Outputs["Y"].Value = p.pos.y;
                Outputs["Z"].Value = p.pos.z;
                needle_new = p.ori;
                //needle_new.y = p.ori.y;
                //needle_new.z = p.ori.z;
                // calculating twist between two sequence
                double twist;
                if (needle_old != null)
                    twist = Math.Acos((needle_new.x * needle_old.x + needle_new.y * needle_old.y + needle_new.z * needle_old.z) / (Math.Sqrt(needle_new.x * needle_new.x + needle_new.y * needle_new.y + needle_new.z * needle_new.z) * Math.Sqrt(needle_old.x * needle_old.x + needle_old.y * needle_old.y + needle_old.z * needle_old.z)));
                else
                    twist = 0;
                Outputs["Twist"].Value = twist * 180/Math.PI;
                needle_old = p.ori;
                //needle_old.y = p.ori.y;
                //needle_old.z = p.ori.z;
            

            //Outputs["pickPoint"].Value = entry_enabled == true ? 1 : 0;
            if (t > 2 * Math.PI)
            {
                //stepTimer.Stop();
                state = 1;
                t = 0;
                Console.Write("\n**************** End\n");
            }
                
            }
        }
        private Vector get_ori2()
        {
            Vector ori2 = new Vector();
            /*
            // calculate forward kinematics and haptic forces
            double kineZ = LengthUpperArm * Math.Cos(kineAngle[0]) * Math.Cos(kineAngle[1]) - LengthForearm * (Math.Sin(kineAngle[0]) * Math.Sin(kineAngle[2]) - Math.Cos(kineAngle[0]) * Math.Cos(kineAngle[1]) * Math.Cos(kineAngle[2]));
            double kineY = LengthUpperArm * Math.Sin(kineAngle[1]) + LengthForearm * Math.Sin(kineAngle[1]) * Math.Cos(kineAngle[2]);
            double kineX = LengthUpperArm * Math.Sin(kineAngle[0]) * Math.Cos(kineAngle[1]) + LengthForearm * (Math.Cos(kineAngle[0]) * Math.Sin(kineAngle[2]) + Math.Sin(kineAngle[0]) * Math.Cos(kineAngle[1]) * Math.Cos(kineAngle[2]));
*/
            ori2.x = 1;// leftElbow;
            ori2.y = 1;// leftUpperBevel;
            ori2.z = 1;// leftUpperBevel;
            return ori2;
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
                        stepTimer.Stop();
                    }));
            }
        }
    }
}

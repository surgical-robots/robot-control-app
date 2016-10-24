using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Threading;
using path_generation.OnePointSuturing;
using System.Windows.Media.Media3D;
using System.Windows.Forms;
namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for Clutch.xaml
    /// </summary>
    public partial class AutoSuture2 : PluginBase
    {
        //private const double PICK_ENTRY = 1, PICK_EXIT = 2, INITIALIZE_SUTURING = 3, PRE_SUTURING = 4, DO_SUTURING = 5, END_SUTURING = 6;
        double x, y, z, twist;
        Joints joints;
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
                {
                    Outputs["Twist"].Value =
                    joints.twist = twist + twist_clutchOffset;
                }
                    
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["leftUpperBevel"].UniqueID, (message) =>
            {
                joints.UpperBevel = message.Value;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["leftLowerBevel"].UniqueID, (message) =>
            {
                joints.LowerBevel = message.Value;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["leftElbow"].UniqueID, (message) =>
            {
                joints.Elbow = message.Value;
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Entry"].UniqueID, (message) =>
            {
                if (suturing.state == 1)// select entry point
                {
                    //suturing.SELECT_ENTRY(x, y, z); // two-point
                    suturing.SELECT_ENTRY(joints); // one-point
                }
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Exit"].UniqueID, (message) =>
            {
                if (suturing.state == 2)// select entry point
                {
                    //suturing.SELECT_EXIT(x, y, z);
                    suturing.SELECT_EXIT(joints); // two-point suturing
                }
            });

            base.PostLoadSetup();
        }
        public AutoSuture2()
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
                update_grasper_output(NeedleKinematics.correction(NeedleKinematics.get_translation(suturing.needle.tail)), suturing.needle.kinematics.joint.twist);
                update_trajectori_output(NeedleKinematics.correction(NeedleKinematics.get_translation(suturing.needle.head)));
                Print.print_vector(NeedleKinematics.get_translation(suturing.needle.head));
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

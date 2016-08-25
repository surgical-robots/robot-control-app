using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Threading;
//using path_generation;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for Clutch.xaml
    /// </summary>
    public partial class AutoSuture : PluginBase
    {
        double x, y, z;
        bool clutchIsEnabled;
        double t = 0;
        double t_incr = Math.PI / 10;

        System.Windows.Forms.Timer stepTimer = new System.Windows.Forms.Timer();

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["X"].UniqueID, (message) =>
            {
                x = message.Value;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Y"].UniqueID, (message) =>
            {
                y = message.Value;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Z"].UniqueID, (message) =>
            {
                z = message.Value;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Clutch"].UniqueID, (message) =>
            {
                clutchIsEnabled = message.Value > 0.5 ? true : false;
            });

            base.PostLoadSetup();
        }

        public AutoSuture()
        {
            this.TypeName = "AutoSuture";
            this.PluginInfo = "";
            InitializeComponent();
            clutchIsEnabled = false;

            // OUTPUTS
            Outputs.Add("X", new ViewModel.OutputSignalViewModel("X"));
            Outputs.Add("Y", new ViewModel.OutputSignalViewModel("Y"));
            Outputs.Add("Z", new ViewModel.OutputSignalViewModel("Z"));

            // INPUTS
            Inputs.Add("X", new ViewModel.InputSignalViewModel("X", this.InstanceName));
            Inputs.Add("Y", new ViewModel.InputSignalViewModel("Y", this.InstanceName));
            Inputs.Add("Z", new ViewModel.InputSignalViewModel("Z", this.InstanceName));
            Inputs.Add("Clutch", new ViewModel.InputSignalViewModel("Clutch", this.InstanceName));

            // set up output timer
            stepTimer.Interval = 50;
            stepTimer.Tick += StepTimer_Tick; ;

            PostLoadSetup();
        }

        private void StepTimer_Tick(object sender, EventArgs e)
        {
            //point p;
            //trajectory obj = new trajectory(0, 0, 130);
            //p = obj.end_effector(t);
            //t = t + t_incr;
            //Console.WriteLine("{0}\t{1}\t{2}", p.pos.x, p.pos.y, p.pos.z);
            //Outputs["X"].Value = p.pos.x;
            //Outputs["Y"].Value = p.pos.y;
            //Outputs["Z"].Value = p.pos.z;
            if (t > 2 * Math.PI)
                stepTimer.Stop();
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

    }
}

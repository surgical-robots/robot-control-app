using System;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for GrasperLimits.xaml
    /// </summary>
    public partial class GrasperLimits : PluginBase
    {
        System.Windows.Forms.Timer openTimer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer closeTimer = new System.Windows.Forms.Timer();

        double GrasperSetpoint = 0;
        private double grasperTwist = 0;
        private double grasperMax = 1500;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["GrasperClose"].UniqueID, (message) =>
            {
                if (message.Value > 0.5)
                    closeTimer.Start();
                else
                    closeTimer.Stop();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["GrasperOpen"].UniqueID, (message) =>
            {
                if (message.Value > 0.5)
                    openTimer.Start();
                else
                    openTimer.Stop();
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["GrasperTwist"].UniqueID, (message) =>
            {
                grasperTwist = message.Value;
                Outputs["GrasperSetpoint"].Value = GrasperSetpoint - grasperTwist;
            });

            base.PostLoadSetup();
        }

        public GrasperLimits()
        {
            this.TypeName = "Grasper Limits";
            InitializeComponent();

            Outputs.Add("GrasperSetpoint", new ViewModel.OutputSignalViewModel("Grasper Setpoint"));

            Inputs.Add("GrasperClose", new ViewModel.InputSignalViewModel("Grasper Close", this.InstanceName));
            Inputs.Add("GrasperOpen", new ViewModel.InputSignalViewModel("Grasper Open", this.InstanceName));
            Inputs.Add("GrasperTwist", new ViewModel.InputSignalViewModel("Grasper Twist", this.InstanceName));

            openTimer.Interval = GraspPeriod;
            openTimer.Tick += openTimer_Tick;

            closeTimer.Interval = GraspPeriod;
            closeTimer.Tick += closeTimer_Tick;

            PostLoadSetup();
        }

        void openTimer_Tick(object sender, EventArgs e)
        {
            if(GrasperSetpoint < grasperMax)
            {
                GrasperSetpoint += (grasperMax / 50);
                Outputs["GrasperSetpoint"].Value = GrasperSetpoint - grasperTwist;
            }
        }

        void closeTimer_Tick(object sender, EventArgs e)
        {
            if (GrasperSetpoint > 0)
            {
                GrasperSetpoint -= (grasperMax / 50);
                Outputs["GrasperSetpoint"].Value = GrasperSetpoint - grasperTwist;
            }
        }

        /// <summary>
        /// The <see cref="GraspPeriod" /> property's name.
        /// </summary>
        public const string GraspPeriodPropertyName = "GraspPeriod";

        private int graspPeriod = 5;

        /// <summary>
        /// Sets and gets the GraspPeriod property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int GraspPeriod
        {
            get
            {
                return graspPeriod;
            }

            set
            {
                if (graspPeriod == value)
                {
                    return;
                }

                graspPeriod = value;
                openTimer.Interval = graspPeriod;
                closeTimer.Interval = graspPeriod;
                RaisePropertyChanged(GraspPeriodPropertyName);
            }
        }
    }
}

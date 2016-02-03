using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Command;
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

            base.PostLoadSetup();
        }

        public GrasperLimits()
        {
            this.TypeName = "Grasper Limits";
            InitializeComponent();

            Outputs.Add("GrasperSetpoint", new ViewModel.OutputSignalViewModel("Grasper Setpoint"));

            Inputs.Add("GrasperClose", new ViewModel.InputSignalViewModel("GrasperClose", this.InstanceName));
            Inputs.Add("GrasperOpen", new ViewModel.InputSignalViewModel("GrasperOpen", this.InstanceName));

            openTimer.Interval = GraspPeriod;
            openTimer.Tick += openTimer_Tick;

            closeTimer.Interval = GraspPeriod;
            closeTimer.Tick += closeTimer_Tick;

            PostLoadSetup();
        }

        void openTimer_Tick(object sender, EventArgs e)
        {
            if(GrasperSetpoint < 100)
            {
                GrasperSetpoint += 2;
                Outputs["GrasperSetpoint"].Value = GrasperSetpoint;
            }
        }

        void closeTimer_Tick(object sender, EventArgs e)
        {
            if (GrasperSetpoint > 0)
            {
                GrasperSetpoint -= 2;
                Outputs["GrasperSetpoint"].Value = GrasperSetpoint;
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

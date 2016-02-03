using GalaSoft.MvvmLight.Command;
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
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for Clutch.xaml
    /// </summary>
    public partial class Clutch : PluginBase
    {
        double x, y, z;
        double offsetX, offsetY, offsetZ;
        double clutchStartPositionX, clutchStartPositionY, clutchStartPositionZ;
        bool clutchIsEnabled;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["X"].UniqueID, (message) =>
            {
                x = message.Value;
                if (!ClutchIsEnabled)
                    Outputs["X"].Value = x + offsetX;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Y"].UniqueID, (message) =>
            {
                y = message.Value;
                if (!ClutchIsEnabled)
                    Outputs["Y"].Value = y + offsetY;

            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Z"].UniqueID, (message) =>
            {
                z = message.Value;
                if (!ClutchIsEnabled)
                    Outputs["Z"].Value = z + offsetZ;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Clutch"].UniqueID, (message) =>
            {
                ClutchIsEnabled = message.Value > 0.5 ? true : false;
            });

            base.PostLoadSetup();
        }

        private RelayCommand resetOffsetsCommand;

        /// <summary>
        /// Gets the ResetOffsetsCommand.
        /// </summary>
        public RelayCommand ResetOffsetsCommand
        {
            get
            {
                return resetOffsetsCommand
                    ?? (resetOffsetsCommand = new RelayCommand(
                    () =>
                    {
                        offsetX = 0;
                        offsetY = 0;
                        offsetZ = 0;
                    }));
            }
        }

        public bool ClutchIsEnabled 
        {
            get { return clutchIsEnabled; }
            set {
                if (value == clutchIsEnabled)
                    return;

                clutchIsEnabled = value;

                if(clutchIsEnabled == true)
                {
                    clutchStartPositionX = x;
                    clutchStartPositionY = y;
                    clutchStartPositionZ = z;
                }
                if(clutchIsEnabled == false)
                {
                    // We transitioned from off to on. Look at the last input and set
                    // that as our offset.
                    offsetX -= x - clutchStartPositionX;
                    offsetY -= y - clutchStartPositionY;
                    offsetZ -= z - clutchStartPositionZ;

                }
                
                this.RaisePropertyChanged("ClutchIsEnabled");
                    

            } 
        }
        public Clutch()
        {
            this.TypeName = "Clutch";
            InitializeComponent();
            clutchIsEnabled = false;

            /// OUTPUTS
            Outputs.Add("X", new ViewModel.OutputSignalViewModel("X"));
            Outputs.Add("Y", new ViewModel.OutputSignalViewModel("Y"));
            Outputs.Add("Z", new ViewModel.OutputSignalViewModel("Z"));

            /// X input
            Inputs.Add("X", new ViewModel.InputSignalViewModel("X", this.InstanceName));
            /// Y input
            Inputs.Add("Y", new ViewModel.InputSignalViewModel("Y", this.InstanceName));
            /// Z input
            Inputs.Add("Z", new ViewModel.InputSignalViewModel("Z", this.InstanceName));
            Inputs.Add("Clutch", new ViewModel.InputSignalViewModel("Clutch", this.InstanceName));

            PostLoadSetup();
        }
    }
}

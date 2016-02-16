using GalaSoft.MvvmLight.Command;
using System;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for PositionOffset.xaml
    /// </summary>
    [Serializable]
    public partial class HomePosition : PluginBase
    {
        double offsetX, offsetY, offsetZ;

        bool canResetHomePosition = true;

        public bool InvertX { get; set; }
        public bool InvertY { get; set; }
        public bool InvertZ { get; set; }

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Offset"].UniqueID, (message) =>
            {
                if (message.Value > 0.5)
                {
                    if (canResetHomePosition)
                    {
                        // Calculate the offsets
                        ResetHomeCommand.Execute(null);

                        canResetHomePosition = false;
                    }

                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["X"].UniqueID, (message) =>
            {
                Inputs["X"].Value = message.Value;
                message.Value = InvertX ? -message.Value : message.Value;
                if (InvertX == true)
                    Outputs["X"].Value = message.Value - offsetX;
                else
                    Outputs["X"].Value = message.Value + offsetX;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Y"].UniqueID, (message) =>
            {
                Inputs["Y"].Value = message.Value;
                message.Value = InvertY ? -message.Value : message.Value;
                if (InvertY == true)
                    Outputs["Y"].Value = message.Value - offsetY;
                else
                    Outputs["Y"].Value = message.Value + offsetY;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Z"].UniqueID, (message) =>
            {
                Inputs["Z"].Value = message.Value;
                message.Value = InvertZ ? -message.Value : message.Value;
                if (InvertZ == true)
                    Outputs["Z"].Value = message.Value - offsetZ;
                else
                    Outputs["Z"].Value = message.Value + offsetZ;
            });

            base.PostLoadSetup();
        }

        public HomePosition()
        {
            this.TypeName = "Home Position";
            InitializeComponent();

            Outputs.Add("X", new ViewModel.OutputSignalViewModel("X"));
            Outputs.Add("Y", new ViewModel.OutputSignalViewModel("Y"));
            Outputs.Add("Z", new ViewModel.OutputSignalViewModel("Z"));

            Inputs.Add("Offset", new ViewModel.InputSignalViewModel("Offset", this.InstanceName));
            Inputs.Add("X", new ViewModel.InputSignalViewModel("X", this.InstanceName));
            Inputs.Add("Y", new ViewModel.InputSignalViewModel("Y", this.InstanceName));
            Inputs.Add("Z", new ViewModel.InputSignalViewModel("Z", this.InstanceName));

            PostLoadSetup();
        }

        private RelayCommand resetHomeAbilityCommand;

        /// <summary>
        /// Gets the ResetHomePositionCommand.
        /// </summary>
        public RelayCommand ResetHomeAbilityCommand
        {
            get
            {
                return resetHomeAbilityCommand
                    ?? (resetHomeAbilityCommand = new RelayCommand(
                    () =>
                    {
                        if (!ResetHomeAbilityCommand.CanExecute(null))
                        {
                            return;
                        }
                        canResetHomePosition = true;

                    },
                    () => true));
            }
        }

        private RelayCommand resetHomeCommand;

        /// <summary>
        /// Gets the SetOffsetCommand.
        /// </summary>
        public RelayCommand ResetHomeCommand
        {
            get
            {
                return resetHomeCommand
                    ?? (resetHomeCommand = new RelayCommand(
                    () =>
                    {
                        if (!ResetHomeCommand.CanExecute(null))
                        {
                            return;
                        }

                        offsetX = HomeX - Inputs["X"].Value;
                        offsetY = HomeY - Inputs["Y"].Value;
                        offsetZ = HomeZ - Inputs["Z"].Value;

                    },
                    () => true));
            }
        }


        /// <summary>
        /// The <see cref="HomeZ" /> property's name.
        /// </summary>
        public const string HomeZPropertyName = "HomeZ";

        private double homeZ = 0;

        /// <summary>
        /// Sets and gets the OffsetZ property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double HomeZ
        {
            get
            {
                return homeZ;
            }

            set
            {
                if (homeZ == value)
                {
                    return;
                }

                homeZ = value;
                RaisePropertyChanged(HomeZPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="HomeY" /> property's name.
        /// </summary>
        public const string HomeYPropertyName = "HomeY";

        private double homeY = 0;

        /// <summary>
        /// Sets and gets the OffsetY property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double HomeY
        {
            get
            {
                return homeY;
            }

            set
            {
                if (homeY == value)
                {
                    return;
                }

                homeY = value;
                RaisePropertyChanged(HomeYPropertyName);
            }
        }
        /// <summary>
        /// The <see cref="HomeX" /> property's name.
        /// </summary>
        public const string HomeXPropertyName = "HomeX";

        private double homeX = 0;

        /// <summary>
        /// Sets and gets the OffsetX property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double HomeX
        {
            get
            {
                return homeX;
            }

            set
            {
                if (homeX == value)
                {
                    return;
                }

                homeX = value;
                RaisePropertyChanged(HomeXPropertyName);
            }
        }

    }
}

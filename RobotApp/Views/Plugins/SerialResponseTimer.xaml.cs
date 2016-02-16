using System;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Diagnostics;
using RobotApp.ViewModel;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for SerialResponseTimer.xaml
    /// </summary>
    public partial class SerialResponseTimer : PluginBase
    {
        public ObservableCollection<ControllerViewModel> PluginControllers { get; set; }
        Stopwatch stopwatch = new Stopwatch();
        public System.Windows.Forms.Timer StatusTimer = new System.Windows.Forms.Timer();
        int msgCount = 0;
        long toStart = 0;
        long toStop = 0;

        public SerialResponseTimer()
        {
            this.TypeName = "Serial Response Timer";
            InitializeComponent();
            PluginControllers = MainViewModel.Instance.Controllers;
            StatusTimer.Tick += StatusTimer_Tick;
            StatusTimer.Interval = 1;

        }

        void StatusTimer_Tick(object sender, EventArgs e)
        {
            //if (MainViewModel.Instance.Robot.statusSet)
            //{
            //    msgCount++;
            //    MainViewModel.Instance.Robot.statusSet = false;
            //}
            if (msgCount > 1000)
            {
                stopwatch.Stop();
                CounterTime = stopwatch.ElapsedMilliseconds;
                StatusTimer.Stop();
//                ToCount = MainViewModel.Instance.com.TimeoutCount - toStart;
                StartText = "Done";
            }
        }

        /// <summary>
        /// The <see cref="SelectedController" /> property's name.
        /// </summary>
        public const string SelectedControllerPropertyName = "SelectedController";

        private ControllerViewModel selectedController = null;

        /// <summary>
        /// Sets and gets the SelectedController property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ControllerViewModel SelectedController
        {
            get
            {
                return selectedController;
            }

            set
            {
                if (selectedController == value)
                {
                    return;
                }

                selectedController = value;
                RaisePropertyChanged(SelectedControllerPropertyName);
            }
        }

        private RelayCommand<string> startCommand;

        /// <summary>
        /// Gets the ConnectCommand.
        /// </summary>
        public RelayCommand<string> StartCommand
        {
            get
            {
                return startCommand
                    ?? (startCommand = new RelayCommand<string>(
                    p =>
                    {
                        msgCount = 0;
//                        toStart = MainViewModel.Instance.com.TimeoutCount;
                        stopwatch.Start();
                        StatusTimer.Start();
                    }));
            }
        }

        /// <summary>
        /// The <see cref="startText" /> property's name.
        /// </summary>
        public const string StartTextPropertyName = "StartText";

        private string startText = "Start";

        /// <summary>
        /// Sets and gets the startText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string StartText
        {
            get
            {
                return startText;
            }

            set
            {
                if (startText == value)
                {
                    return;
                }

                startText = value;
                RaisePropertyChanged(StartTextPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CounterTime" /> property's name.
        /// </summary>
        public const string CounterTimePropertyName = "CounterTime";

        private long counterTime = 0;

        /// <summary>
        /// Sets and gets the CounterTime property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public long CounterTime
        {
            get
            {
                return counterTime;
            }

            set
            {
                if (counterTime == value)
                {
                    return;
                }

                counterTime = value;
                RaisePropertyChanged(CounterTimePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ToCount" /> property's name.
        /// </summary>
        public const string ToCountPropertyName = "ToCount";

        private long toCount = 0;

        /// <summary>
        /// Sets and gets the ToCount property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public long ToCount
        {
            get
            {
                return toCount;
            }

            set
            {
                if (toCount == value)
                {
                    return;
                }

                toCount = value;
                RaisePropertyChanged(ToCountPropertyName);
            }
        }
    }
}

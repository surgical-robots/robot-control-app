using System;
using System.Linq;
using System.IO;
using GalaSoft.MvvmLight.Command;
using RobotApp.ViewModel;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for PathPusher.xaml
    /// </summary>
    public partial class PathPusher : PluginBase
    {
        System.Windows.Forms.Timer stepTimer = new System.Windows.Forms.Timer();

        private static int arraySize;
        private double[] posXR;
        private double[] posYR;
        private double[] posZR;
        private double[] posTTR;
        private double[] posTAR;
        private double[] posXL;
        private double[] posYL;
        private double[] posZL;
        private double[] posTTL;
        private double[] posTAL;

        public bool pause = true;

        public int frameCount = 0;

        public bool loopable;
        public bool loopDone;
        public string[][] pathData;

        public PathPusher()
        {
            this.TypeName = "Path Pusher";
            // set up outputs for two armed 5DOF robot
            Outputs.Add("XR", new OutputSignalViewModel("X Right"));
            Outputs.Add("YR", new OutputSignalViewModel("Y Right"));
            Outputs.Add("ZR", new OutputSignalViewModel("Z Right"));
            Outputs.Add("ToolTwistR", new OutputSignalViewModel("Tool Twist Right"));
            Outputs.Add("ToolActionR", new OutputSignalViewModel("Tool Actuation Right"));
            Outputs.Add("XL", new OutputSignalViewModel("X Left"));
            Outputs.Add("YL", new OutputSignalViewModel("Y Left"));
            Outputs.Add("ZL", new OutputSignalViewModel("Z Left"));
            Outputs.Add("ToolTwistL", new OutputSignalViewModel("Tool Twist Left"));
            Outputs.Add("ToolActionL", new OutputSignalViewModel("Tool Actuation Left"));
            // set up output timer
            stepTimer.Interval = timerInterval;
            stepTimer.Tick += stepTimer_Tick;
            // find path files in RobotApp\bin\RobotPathFiles
            string searchDirectory = Directory.GetCurrentDirectory() + "\\RobotPathFiles";
            ReportList = new DirectoryInfo(searchDirectory).GetFiles("*.lou");

            InitializeComponent();
        }

        void stepTimer_Tick(object sender, EventArgs e)
        {
            updateFrame();
        }

        void updateFrame()
        {
            // update right arm outputs
            Outputs["XR"].Value = posXR[frameCount];
            Outputs["YR"].Value = posYR[frameCount];
            Outputs["ZR"].Value = posZR[frameCount];
            Outputs["ToolTwistR"].Value = posTTR[frameCount];
            Outputs["ToolActionR"].Value = posTAR[frameCount];
            // update left arm outputs
            Outputs["XL"].Value = posXL[frameCount];
            Outputs["YL"].Value = posYL[frameCount];
            Outputs["ZL"].Value = posZL[frameCount];
            Outputs["ToolTwistL"].Value = posTTL[frameCount];
            Outputs["ToolActionL"].Value = posTAL[frameCount];
            // update frame and check if done
            if (frameCount < (arraySize - 1))
                frameCount++;
            else
            {
                if (LoopIt)
                    frameCount = 0;
                else
                {
                    loopDone = true;
                    stepTimer.Stop();
                    ButtonText = "Path Finished... Restart Path Output";
                }
            }
        }

        /// <summary>
        /// The <see cref="ReportList" /> property's name.
        /// </summary>
        public const string ReportListPropertyName = "ReportList";

        private FileInfo[] reportList = null;

        /// <summary>
        /// Sets and gets the ReportList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public FileInfo[] ReportList
        {
            get
            {
                return reportList;
            }

            set
            {
                if (reportList == value)
                {
                    return;
                }

                reportList = value;
                RaisePropertyChanged(ReportListPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="PathFile" /> property's name.
        /// </summary>
        public const string PathFilePropertyName = "PathFile";

        private string pathFile = null;

        /// <summary>
        /// Sets and gets the PathFile property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string PathFile
        {
            get
            {
                return pathFile;
            }

            set
            {
                if (pathFile == value)
                {
                    return;
                }

                pathFile = value;
                RaisePropertyChanged(PathFilePropertyName);
            }
        }

        private RelayCommand<string> readCommand;

        /// <summary>
        /// Gets the ReadCommand.
        /// </summary>
        public RelayCommand<string> ReadCommand
        {
            get
            {
                return readCommand
                    ?? (readCommand = new RelayCommand<string>(
                    p =>
                    {
                        string startupPath = System.IO.Directory.GetCurrentDirectory();
                        startupPath = startupPath + "\\";
                        string FullPath = startupPath + PathFile;

                        pathData = File.ReadLines(FullPath).Select(line => line.Split('\t')).ToArray();

                        //double[] dData = Array.ConvertAll<string, double>(pathData[0], Convert.ToDouble);
                        loopable = pathData[0][0] == "1";
                        posXR = Array.ConvertAll<string, double>(pathData[1], Convert.ToDouble);
                        posYR = Array.ConvertAll<string, double>(pathData[2], Convert.ToDouble);
                        posZR = Array.ConvertAll<string, double>(pathData[3], Convert.ToDouble);
                        posTTR = Array.ConvertAll<string, double>(pathData[4], Convert.ToDouble);
                        posTAR = Array.ConvertAll<string, double>(pathData[5], Convert.ToDouble);
                        posXL = Array.ConvertAll<string, double>(pathData[6], Convert.ToDouble);
                        posYL = Array.ConvertAll<string, double>(pathData[7], Convert.ToDouble);
                        posZL = Array.ConvertAll<string, double>(pathData[8], Convert.ToDouble);
                        posTTL = Array.ConvertAll<string, double>(pathData[9], Convert.ToDouble);
                        posTAL = Array.ConvertAll<string, double>(pathData[10], Convert.ToDouble);

                        if (loopable)
                        {
                            LoopText.Visibility = System.Windows.Visibility.Visible;
                            LoopCheckBox.Visibility = System.Windows.Visibility.Visible;
                        }

                        arraySize = posXL.Length;
                    }));
            }
        }

        private RelayCommand<string> pushCommand;

        /// <summary>
        /// Gets the StartCommand.
        /// </summary>
        public RelayCommand<string> PushCommand
        {
            get
            {
                return pushCommand
                    ?? (pushCommand = new RelayCommand<string>(
                    p =>
                    {
                        if(loopDone)
                        {
                            loopDone = false;
                            frameCount = 0;
                            stepTimer.Start();
                            pause = false;
                            ButtonText = "Pause Path Output";
                        }
                        else if (pause == true)
                        {
                            stepTimer.Start();
                            pause = false;
                            ButtonText = "Pause Path Output";
                        }
                        else
                        {
                            stepTimer.Stop();
                            pause = true;
                            ButtonText = "Resume Path Output";
                        }
                    }));
            }
        }

        /// <summary>
        /// The <see cref="ButtonText" /> property's name.
        /// </summary>
        public const string ButtonTextPropertyName = "ButtonText";

        private string buttonText = "Start Path Output";

        /// <summary>
        /// Sets and gets the ButtonText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ButtonText
        {
            get
            {
                return buttonText;
            }

            set
            {
                if (buttonText == value)
                {
                    return;
                }

                buttonText = value;
                RaisePropertyChanged(ButtonTextPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="TimerInterval" /> property's name.
        /// </summary>
        public const string TimerIntervalPropertyName = "TimerInterval";

        private int timerInterval = 50;

        /// <summary>
        /// Sets and gets the TimerInterval property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int TimerInterval
        {
            get
            {
                return timerInterval;
            }

            set
            {
                if (timerInterval == value)
                {
                    return;
                }

                timerInterval = value;
                if (timerInterval < 15)
                    stepTimer.Interval = 15;
                else
                    stepTimer.Interval = timerInterval;
                RaisePropertyChanged(TimerIntervalPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="LoopIt" /> property's name.
        /// </summary>
        public const string LoopItPropertyName = "LoopIt";

        private bool loopIt = false;

        /// <summary>
        /// Sets and gets the LoopIt property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool LoopIt
        {
            get
            {
                return loopIt;
            }

            set
            {
                if (loopIt == value)
                {
                    return;
                }

                loopIt = value;
                RaisePropertyChanged(LoopItPropertyName);
            }
        }
    }
}

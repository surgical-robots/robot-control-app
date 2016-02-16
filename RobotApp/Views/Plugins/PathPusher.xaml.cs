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

        public static int arraySize;
        public double[] posXR;
        public double[] posYR;
        public double[] posZR;
        public double[] posXL;
        public double[] posYL;
        public double[] posZL;
        public double[] posGJ;
        public double[] posGT;
        public double[] posCT;

        public bool pause = true;

        public int frameCount = 0;

        public bool loopable;
        public bool loopDone;
        public string[][] pathData;

        public PathPusher()
        {
            this.TypeName = "Path Pusher";

            Outputs.Add("XRight", new OutputSignalViewModel("X Right"));
            Outputs.Add("YRight", new OutputSignalViewModel("Y Right"));
            Outputs.Add("ZRight", new OutputSignalViewModel("Z Right"));
            Outputs.Add("XLeft", new OutputSignalViewModel("X Left"));
            Outputs.Add("YLeft", new OutputSignalViewModel("Y Left"));
            Outputs.Add("ZLeft", new OutputSignalViewModel("Z Left"));
            Outputs.Add("GraspJaw", new OutputSignalViewModel("Grasper Jaws"));
            Outputs.Add("GraspTwist", new OutputSignalViewModel("Grasper Twist"));
            Outputs.Add("CautTwist", new OutputSignalViewModel("Cautery Twist"));

            stepTimer.Interval = timerInterval;
            stepTimer.Tick += stepTimer_Tick;

            ReportList = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("*.lou");

            InitializeComponent();
        }

        void stepTimer_Tick(object sender, EventArgs e)
        {
            updateFrame();
        }

        void updateFrame()
        {
            Outputs["XRight"].Value = posXR[frameCount];
            Outputs["YRight"].Value = posYR[frameCount];
            Outputs["ZRight"].Value = posZR[frameCount];

            Outputs["XLeft"].Value = posXL[frameCount];
            Outputs["YLeft"].Value = posYL[frameCount];
            Outputs["ZLeft"].Value = posZL[frameCount];

            Outputs["GraspJaw"].Value = posGJ[frameCount];
            Outputs["GraspTwist"].Value = posGT[frameCount];
            Outputs["CautTwist"].Value = posCT[frameCount];

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
                        posXL = Array.ConvertAll<string, double>(pathData[4], Convert.ToDouble);
                        posYL = Array.ConvertAll<string, double>(pathData[5], Convert.ToDouble);
                        posZL = Array.ConvertAll<string, double>(pathData[6], Convert.ToDouble);
                        posGJ = Array.ConvertAll<string, double>(pathData[7], Convert.ToDouble);
                        posGT = Array.ConvertAll<string, double>(pathData[8], Convert.ToDouble);
                        posCT = Array.ConvertAll<string, double>(pathData[9], Convert.ToDouble);

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

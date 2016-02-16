using System;
using System.IO;
using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for PrintData.xaml
    /// </summary>
    public partial class PrintData : PluginBase
    {
        public double motorOneCurrent = 0;
        public double motorTwoCurrent = 0;
        public double motorThreeCurrent = 0;
        public double motorFourCurrent = 0;
        public double motorOnePosition = 0;
        public double motorTwoPosition = 0;
        public double motorThreePosition = 0;
        public double motorFourPosition = 0;
        public bool printing = false;
        public string path = System.IO.Directory.GetCurrentDirectory();
        public Stopwatch dataTimer = new Stopwatch();
        System.Windows.Forms.Timer writeTimer = new System.Windows.Forms.Timer();
        public long millisTime;

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Motor1Current"].UniqueID, (message) =>
            {
                if ((motorOneCurrent != message.Value))
                {
                    motorOneCurrent = message.Value;
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Motor1Position"].UniqueID, (message) =>
            {
                if(motorOnePosition != message.Value)
                {
                    motorOnePosition = message.Value;
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Motor2Current"].UniqueID, (message) =>
            {
                if(motorTwoCurrent != message.Value)
                {
                    motorTwoCurrent = message.Value;
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Motor2Position"].UniqueID, (message) =>
            {
                if(motorTwoPosition != message.Value)
                {
                    motorTwoPosition = message.Value;
                }
            });
            Messenger.Default.Register<Messages.Signal>(this, Inputs["Motor3Current"].UniqueID, (message) =>
            {
                if(motorThreeCurrent != message.Value)
                {
                    motorThreeCurrent = message.Value;
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Motor3Position"].UniqueID, (message) =>
            {
                if(motorThreePosition != message.Value)
                {
                    motorThreePosition = message.Value;
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Motor4Current"].UniqueID, (message) =>
            {
                if(motorFourCurrent != message.Value)
                {
                    motorFourCurrent = message.Value;
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Motor4Position"].UniqueID, (message) =>
            {
                if(motorFourPosition != message.Value)
                {
                    motorFourPosition = message.Value;
                }
            });

            base.PostLoadSetup();
        }

        public PrintData()
        {
            Inputs.Add("Motor1Current", new ViewModel.InputSignalViewModel("Motor1Current", this.InstanceName));
            Inputs.Add("Motor1Position", new ViewModel.InputSignalViewModel("Motor1Position", this.InstanceName));
            Inputs.Add("Motor2Current", new ViewModel.InputSignalViewModel("Motor2Current", this.InstanceName));
            Inputs.Add("Motor2Position", new ViewModel.InputSignalViewModel("Motor2Position", this.InstanceName));
            Inputs.Add("Motor3Current", new ViewModel.InputSignalViewModel("Motor3Current", this.InstanceName));
            Inputs.Add("Motor3Position", new ViewModel.InputSignalViewModel("Motor3Position", this.InstanceName));
            Inputs.Add("Motor4Current", new ViewModel.InputSignalViewModel("Motor4Current", this.InstanceName));
            Inputs.Add("Motor4Position", new ViewModel.InputSignalViewModel("Motor4Position", this.InstanceName));

            this.TypeName = "Print Data";
            InitializeComponent();

            writeTimer.Interval = 10;
            writeTimer.Tick += writeTimer_Tick;

            PostLoadSetup();
        }

        public void writeTimer_Tick(object sender, EventArgs e)
        {
            if (printing)
                PrintFile();
        }

        public void PrintFile()
        {
            string fullPath = path + "\\Data\\" + OutputFileName;
            if(printing == true)
            {
                if (!File.Exists(fullPath))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = File.CreateText(fullPath))
                    {
                        sw.Write(OutputFileName);
                        sw.Write("\t");
                        DateTime millisTime = DateTime.Now;
                        sw.WriteLine(millisTime);
                        if((motor1Print == true) && (motor2Print == true))
                        {
                            sw.Write("\t");
                            sw.Write("\t");
                            sw.Write("\t");
                            sw.Write(motor1Name);
                        }
                        else if ((motor1Print == true) && (motor2Print == false))
                        {
                            sw.Write("\t");
                            sw.Write("\t");
                            sw.Write("\t");
                            sw.WriteLine(motor1Name);
                        }
                        if ((motor2Print == true) && (motor3Print == true))
                        {
                            sw.Write("\t");
                            sw.Write("\t");
                            sw.Write(motor2Name);
                        }
                        else if ((motor2Print == true) && (motor3Print == false))
                        {
                            sw.Write("\t");
                            sw.Write("\t");
                            sw.WriteLine(motor2Name);
                        }
                        if ((motor3Print == true) && (motor4Print == true))
                        {
                            sw.Write("\t");
                            sw.Write("\t");
                            sw.Write(motor3Name);
                        }
                        else if ((motor3Print == true) && (motor4Print == false))
                        {
                            sw.Write("\t");
                            sw.Write("\t");
                            sw.WriteLine(motor3Name);
                        }
                        if (motor4Print == true)
                        {
                            sw.Write("\t");
                            sw.Write("\t");
                            sw.WriteLine(motor4Name);
                        }
                        sw.Write("Elapsed Time (ms)");
                        if ((motor1Print == true) && (motor2Print == true))
                        {
                            sw.Write("\t");
                            sw.Write("Position");
                            sw.Write("\t");
                            sw.Write("Current");
                        }
                        else if ((motor1Print == true) && (motor2Print == false))
                        {
                            sw.Write("\t");
                            sw.Write("Position");
                            sw.Write("\t");
                            sw.WriteLine("Current");
                        }
                        if ((motor2Print == true) && (motor3Print == true))
                        {
                            sw.Write("\t");
                            sw.Write("Position");
                            sw.Write("\t");
                            sw.Write("Current");
                        }
                        else if ((motor2Print == true) && (motor3Print == false))
                        {
                            sw.Write("\t");
                            sw.Write("Position");
                            sw.Write("\t");
                            sw.WriteLine("Current");
                        }
                        if ((motor3Print == true) && (motor4Print == true))
                        {
                            sw.Write("\t");
                            sw.Write("Position");
                            sw.Write("\t");
                            sw.Write("Current");
                        }
                        else if ((motor3Print == true) && (motor4Print == false))
                        {
                            sw.Write("\t");
                            sw.Write("Position");
                            sw.Write("\t");
                            sw.WriteLine("Current");
                        }
                        if (motor4Print == true)
                        {
                            sw.Write("\t");
                            sw.Write("Position");
                            sw.Write("\t");
                            sw.WriteLine("Current");
                        }
                    }
                }
                using (StreamWriter sw = File.AppendText(fullPath))
                {
                    millisTime = dataTimer.ElapsedMilliseconds;
                    sw.Write(millisTime);
                    sw.Write("\t");
                    sw.Write("\t");
                    if ((motor1Print == true) && (motor2Print == true))
                    {
                        sw.Write("\t");
                        sw.Write(motorOnePosition);
                        sw.Write("\t");
                        sw.Write("\t");
                        sw.Write(motorOneCurrent);
                    }
                    else if ((motor1Print == true) && (motor2Print == false))
                    {
                        sw.Write("\t");
                        sw.Write(motorOnePosition);
                        sw.Write("\t");
                        sw.Write("\t");
                        sw.WriteLine(motorOneCurrent);
                    }
                    if ((motor2Print == true) && (motor3Print == true))
                    {
                        sw.Write("\t");
                        sw.Write(motorTwoPosition);
                        sw.Write("\t");
                        sw.Write("\t");
                        sw.Write(motorTwoCurrent);
                    }
                    else if ((motor2Print == true) && (motor3Print == false))
                    {
                        sw.Write("\t");
                        sw.Write(motorTwoPosition);
                        sw.Write("\t");
                        sw.Write("\t");
                        sw.WriteLine(motorTwoCurrent);
                    }
                    if ((motor3Print == true) && (motor4Print == true))
                    {
                        sw.Write("\t");
                        sw.Write(motorThreePosition);
                        sw.Write("\t");
                        sw.Write("\t");
                        sw.Write(motorThreeCurrent);
                    }
                    else if ((motor3Print == true) && (motor4Print == false))
                    {
                        sw.Write("\t");
                        sw.Write(motorThreePosition);
                        sw.Write("\t");
                        sw.Write("\t");
                        sw.WriteLine(motorThreeCurrent);
                    }
                    if (motor4Print == true)
                    {
                        sw.Write("\t");
                        sw.Write(motorFourPosition);
                        sw.Write("\t");
                        sw.Write("\t");
                        sw.WriteLine(motorFourCurrent);
                    }
                }
             }
       }

        /// <summary>
        /// The <see cref="OutputFileName" /> property's name.
        /// </summary>
        public const string OutputFileNamePropertyName = "OutputFileName";

        private string outputFileName = "data.txt";

        /// <summary>
        /// Sets and gets the OutputFileName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string OutputFileName
        {
            get
            {
                return outputFileName;
            }

            set
            {
                if (outputFileName == value)
                {
                    return;
                }
                outputFileName = value;
                RaisePropertyChanged(OutputFileNamePropertyName);

                string fullPath = path + outputFileName;
                if (File.Exists(fullPath))
                    ButtonText = OutputFileName + " Already Exists. Click to Overwrite";
            }
        }

        /// <summary>
        /// The <see cref="Motor1Name" /> property's name.
        /// </summary>
        public const string Motor1NamePropertyName = "Motor1Name";

        private string motor1Name = "Motor One";

        /// <summary>
        /// Sets and gets the Motor1Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Motor1Name
        {
            get
            {
                return motor1Name;
            }

            set
            {
                if (motor1Name == value)
                {
                    return;
                }

                motor1Name = value;
                RaisePropertyChanged(Motor1NamePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Motor1Print" /> property's name.
        /// </summary>
        public const string Motor1PrintPropertyName = "Motor1Print";

        private bool motor1Print = false;

        /// <summary>
        /// Sets and gets the Motor1Print property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool Motor1Print
        {
            get
            {
                return motor1Print;
            }

            set
            {
                if (motor1Print == value)
                {
                    return;
                }

                motor1Print = value;
                RaisePropertyChanged(Motor1PrintPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Motor2Name" /> property's name.
        /// </summary>
        public const string Motor2NamePropertyName = "Motor2Name";

        private string motor2Name = "Motor Two";

        /// <summary>
        /// Sets and gets the Motor2Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Motor2Name
        {
            get
            {
                return motor2Name;
            }

            set
            {
                if (motor2Name == value)
                {
                    return;
                }

                motor2Name = value;
                RaisePropertyChanged(Motor2NamePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Motor2Print" /> property's name.
        /// </summary>
        public const string Motor2PrintPropertyName = "Motor2Print";

        private bool motor2Print = false;

        /// <summary>
        /// Sets and gets the Motor2Print property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool Motor2Print
        {
            get
            {
                return motor2Print;
            }

            set
            {
                if (motor2Print == value)
                {
                    return;
                }

                motor2Print = value;
                RaisePropertyChanged(Motor2PrintPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Motor3Name" /> property's name.
        /// </summary>
        public const string Motor3NamePropertyName = "Motor3Name";

        private string motor3Name = "Motor Three";

        /// <summary>
        /// Sets and gets the Motor3Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Motor3Name
        {
            get
            {
                return motor3Name;
            }

            set
            {
                if (motor3Name == value)
                {
                    return;
                }

                motor3Name = value;
                RaisePropertyChanged(Motor3NamePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Motor3Print" /> property's name.
        /// </summary>
        public const string Motor3PrintPropertyName = "Motor3Print";

        private bool motor3Print = false;

        /// <summary>
        /// Sets and gets the Motor3Print property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool Motor3Print
        {
            get
            {
                return motor3Print;
            }

            set
            {
                if (motor3Print == value)
                {
                    return;
                }

                motor3Print = value;
                RaisePropertyChanged(Motor3PrintPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Motor4Name" /> property's name.
        /// </summary>
        public const string Motor4NamePropertyName = "Motor4Name";

        private string motor4Name = "Motor Four";

        /// <summary>
        /// Sets and gets the Motor4Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Motor4Name
        {
            get
            {
                return motor4Name;
            }

            set
            {
                if (motor4Name == value)
                {
                    return;
                }

                motor4Name = value;
                RaisePropertyChanged(Motor4NamePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Motor4Print" /> property's name.
        /// </summary>
        public const string Motor4PrintPropertyName = "Motor4Print";

        private bool motor4Print = false;

        /// <summary>
        /// Sets and gets the Motor4Print property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool Motor4Print
        {
            get
            {
                return motor4Print;
            }

            set
            {
                if (motor4Print == value)
                {
                    return;
                }

                motor4Print = value;
                RaisePropertyChanged(Motor4PrintPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ButtonText" /> property's name.
        /// </summary>
        public const string ButtonTextPropertyName = "ButtonText";

        private string buttonText = "Start Printing Selected Data";

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

        private RelayCommand printCommand;

        /// <summary>
        /// Gets the PrintCommand.
        /// </summary>
        public RelayCommand PrintCommand
        {
            get
            {
                return printCommand
                    ?? (printCommand = new RelayCommand(
                    () =>
                    {
                     if(printing == false)
                     {
                         printing = true;
                         ButtonText = "Stop Printing Motor Data";
                         dataTimer.Restart();
                         writeTimer.Start();
                     }
                     else
                     {
                         printing = false;
                         ButtonText = "Continue Printing Motor Data";
                         dataTimer.Stop();
                         writeTimer.Stop();
                     }
   
                    }));
            }
        }

    }
}

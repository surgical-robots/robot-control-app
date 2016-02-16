using RobotApp.ViewModel;
using GalaSoft.MvvmLight.Command;
using System.IO;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for WriteControllerFile.xaml
    /// </summary>
    public partial class WriteControllerFile : PluginBase
    {
        public string path = System.IO.Directory.GetCurrentDirectory();

        public WriteControllerFile()
        {
            this.TypeName = "Write Controller File";
            this.InstanceName = "Write Controller File";

            InitializeComponent();
        }

        public void WriteFile()
        {
            string fullPath = path + "\\ConfigFiles\\" + OutputFileName + ".controllers";
                if (!File.Exists(fullPath))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = File.CreateText(fullPath))
                    {

                    }
                }
                using (StreamWriter sw = File.AppendText(fullPath))
                {
                    if(MainViewModel.Instance.Controllers != null)
                    {
                        foreach (ControllerViewModel controller in MainViewModel.Instance.Controllers)
                        {
                            sw.Write(controller.Id);
                            sw.Write("\t");
                            foreach (MotorViewModel motor in controller.Motors)
                            {
                                sw.Write(motor.EncoderCountsPerRevolution);
                                sw.Write("\t");
                                sw.Write(motor.CurrentMax);
                                sw.Write("\t");
                                sw.Write(motor.SpeedMax);
                                sw.Write("\t");
                                sw.Write(motor.Kp);
                                sw.Write("\t");
                                sw.Write(motor.PotZero);
                                sw.Write("\t");
                            }
                            sw.WriteLine("");
                        }
                    }
                }
            }

        /// <summary>
        /// The <see cref="OutputFileName" /> property's name.
        /// </summary>
        public const string OutputFileNamePropertyName = "OutputFileName";

        private string outputFileName = "";

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
            }
        }

        /// <summary>
        /// The <see cref="ButtonText" /> property's name.
        /// </summary>
        public const string ButtonTextPropertyName = "ButtonText";

        private string buttonText = "Print Controller File";

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
                        WriteFile();
                    }));
            }
        }

    }
}

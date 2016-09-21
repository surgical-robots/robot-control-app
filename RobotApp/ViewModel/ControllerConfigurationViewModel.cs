using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotControl;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace RobotApp.ViewModel
{
    public class ControllerConfigurationViewModel : ViewModelBase
    {
        public Robot robot { get { return MainViewModel.Robot; } }

        public bool updating = false;

        public RelayCommand DetectControllerCommand { get; set; }

        public MainViewModel MainViewModel { get { return MainViewModel.Instance; } }

        private RelayCommand loadCommand;

        /// <summary>
        /// Gets the LoadCommand.
        /// </summary>
        public RelayCommand LoadCommand
        {
            get
            {
                return loadCommand
                    ?? (loadCommand = new RelayCommand(ExecuteLoadCommand));
            }
        }

        private void ExecuteLoadCommand()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = Directory.GetCurrentDirectory();
            dlg.Filter = "Robot Configuration Files|*.ControllerConfig";
            dlg.RestoreDirectory = true;

            Nullable<bool> result = dlg.ShowDialog();

            if(result == true)
                MainViewModel.LoadData(dlg.FileName);
        }

        private RelayCommand saveCommand;

        /// <summary>
        /// Gets the SaveCommand.
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand
                    ?? (saveCommand = new RelayCommand(ExecuteSaveCommand));
            }
        }

        private void ExecuteSaveCommand()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "config";
            dlg.DefaultExt = ".ControllerConfig";
            dlg.Filter = "Robot Configuration Files|*.ControllerConfig";

            Nullable<bool> result = dlg.ShowDialog();

            if(result == true)
                MainViewModel.SaveData(dlg.FileName);
        }

        public ControllerConfigurationViewModel()
        {

            ConfigList = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("*.ControllerConfig");

            DetectControllerCommand = new RelayCommand(
                () =>
                    {
                        MainViewModel.ScanForControllers();
                    },
                () =>
                    {
                        if (MainViewModel.Robot == null)
                            return false;
                        if (MainViewModel.Robot.Com == null)
                            return false;
                        return true;
                    }
                );

            MainViewModel.Robot.PropertyChanged += Robot_PropertyChanged;
        }

        private FileInfo[] configList = null;

        /// <summary>
        /// Sets and gets the ConfigList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public FileInfo[] ConfigList
        {
            get
            {
                return configList;
            }

            set
            {
                if (configList == value)
                {
                    return;
                }

                configList = value;
            }
        }

        private string configPath = null;

        /// <summary>
        /// Sets and gets the ConfigPath property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ConfigPath
        {
            get
            {
                return configPath;
            }

            set
            {
                if (configPath == value)
                {
                    return;
                }

                configPath = value;
            }
        }

        private RelayCommand<string> updateCommand;

        /// <summary>
        /// Gets the UpdateCommand.
        /// </summary>
        public RelayCommand<string> UpdateCommand
        {
            get
            {
                return updateCommand
                    ?? (updateCommand = new RelayCommand<string>(
                    p =>
                    {
                        if (!updating && MainViewModel.Controllers.Count > 0)
                        {
                            UpdateText = "Stop Updates";
                            updating = true;
                            robot.StartUpdates();
                        }
                        else
                        {
                            robot.StopUpdates();
                            UpdateText = "Start Updates";
                            updating = false;
                            
                        }
                    }));
            }
        }

        /// <summary>
        /// The <see cref="UpdateText" /> property's name.
        /// </summary>
        public const string UpdateTextPropertyName = "UpdateText";

        private string updateText = "Start Updates";

        /// <summary>
        /// Sets and gets the UpdateText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string UpdateText
        {
            get
            {
                return updateText;
            }

            set
            {
                if (updateText == value)
                {
                    return;
                }

                updateText = value;
                RaisePropertyChanged(UpdateTextPropertyName);
            }
        }

        private RelayCommand<string> homeAllCommand;

        /// <summary>
        /// Gets the UpdateCommand.
        /// </summary>
        public RelayCommand<string> HomeAllCommand
        {
            get
            {
                return homeAllCommand
                    ?? (homeAllCommand = new RelayCommand<string>(
                    p =>
                    {
                        if(MainViewModel.Controllers != null)
                        {
                            foreach (ControllerViewModel controller in MainViewModel.Controllers)
                            {
                                if(controller.HomeJoint)
                                {
                                    foreach(MotorViewModel motor in controller.Motors)
                                    {
                                        motor.Motor.SpeedMax = 32;
                                        controller.Controller.Robot.SendCommand(JointCommands.ResetCounters, controller.Controller, new byte[] { (byte)motor.Id, (byte)0x01 });
                                    }
                                }
                            }
                        }

                        Thread.Sleep(1000);

                        foreach(ControllerViewModel controller in MainViewModel.Controllers)
                        {
                            if(controller.HomeJoint)
                            {
                                foreach(MotorViewModel motor in controller.Motors)
                                {
                                    motor.Motor.SpeedMax = motor.SpeedMax;
                                }
                            }
                        }
                    }));
            }
        }

        void Robot_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DetectControllerCommand.RaiseCanExecuteChanged();
        }

    }
}

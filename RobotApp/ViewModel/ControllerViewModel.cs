using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotControl;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Media;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.ComponentModel;
using RobotApp.Views;

namespace RobotApp.ViewModel
{
    [DataContract]
    [Serializable]
    public class ControllerViewModel : INotifyPropertyChanged
    {
        [DataMember]
        private Controller controller;
        public Controller Controller {
            get
            {
                return controller;
            }
            set
            {
                controller = value;
            }
        }

        [DataMember]
        public ObservableCollection<MotorViewModel> Motors { get; set; }

        public int counter = 0;

        public ControllerViewModel()
        {
            
        }

        public ControllerViewModel(Controller controller)
        {
            Controller = controller;
            Motors = new ObservableCollection<MotorViewModel>();
        }

        [IgnoreDataMember]
        [NonSerialized]
        private RelayCommand deleteMotorCommand;

        /// <summary>
        /// Gets the DeleteMotorCommand.
        /// </summary>
        [IgnoreDataMember]
        public RelayCommand DeleteMotorCommand
        {
            get
            {
                return deleteMotorCommand
                    ?? (deleteMotorCommand = new RelayCommand(
                    () =>
                    {
                        if (!DeleteMotorCommand.CanExecute(null))
                        {
                            return;
                        }
//                        MainViewModel.Instance.Robot.setpointTimer.Stop();
                        if (MainViewModel.Instance.com != null)
                            MainViewModel.Instance.com.SendData = true;
                        Motor motor = Motors.Last().Motor;
                        Motors.Last().Dispose();
                        Motors.Remove(Motors.Last());
                        MainViewModel.Instance.Robot.Motors.Remove(motor);
                        counter--;
//                        MainViewModel.Instance.Robot.setpointTimer.Start();
                    },
                    () => true));
            }
        }
        [IgnoreDataMember]
        [NonSerialized]
        private RelayCommand addMotorCommand;

        /// <summary>
        /// Gets the AddMotorCommand.
        /// </summary>
        [IgnoreDataMember]
        public RelayCommand AddMotorCommand
        {
            get
            {
                return addMotorCommand
                    ?? (addMotorCommand = new RelayCommand(
                    () =>
                    {
                        if (!AddMotorCommand.CanExecute(null))
                        {
                            return;
                        }
                        //MainViewModel.Instance.Robot.setpointTimer.Stop();
                        if(MainViewModel.Instance.com != null)
                            MainViewModel.Instance.com.SendData = true;
                        Debug.Print("Add new motor");
                        var mvm = new MotorViewModel() { Controller = this.Controller };
                        mvm.Id = counter++;
                        Motors.Add(mvm);
                        MainViewModel.Instance.Robot.Com_UpdateSetpoints();
                        //MainViewModel.Instance.Robot.setpointTimer.Start();
                        //MainViewModel.Instance.Robot.Motors.Add(mvm.Motor);
                    },
                    () => true));
            }
        }

        [IgnoreDataMember]
        [NonSerialized]
        private RelayCommand deleteCommand;

        /// <summary>
        /// Gets the DeleteCommand.
        /// </summary>
        [IgnoreDataMember]
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand
                    ?? (deleteCommand = new RelayCommand(
                    () =>
                    {
                        if (!DeleteCommand.CanExecute(null))
                        {
                            return;
                        }
                        foreach(var motor in this.Motors)
                        {
                            MainViewModel.Instance.Robot.Motors.Remove(motor.Motor);
                        }
                        if (MainViewModel.Instance.Robot.Com != null)
                            MainViewModel.Instance.Robot.Com.RemoveAddress(this.Id);
                        Messenger.Default.Send<Messages.RemoveController>(new Messages.RemoveController() { ControllerToRemove = this });

                    },
                    () => true));
            }
        }

        public uint Id { get { return Controller.Id; } }

        public string IdString { get { return "Address: " + Controller.Id.ToString(); } }

        [DataMember]
        public bool LedIsEnabled
        {
            get { return Controller.IdentificationLedIsEnabled; }
            set { Controller.IdentificationLedIsEnabled = value; }
        }

        [DataMember]
        public string FriendlyName
        {
            get 
            {
                if (Controller.FriendlyName != null && Controller.FriendlyName.Length > 0)
                    return Controller.FriendlyName;
                else
                    return "<No name>";
            }
            set 
            {
                Controller.FriendlyName = value;
                if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("FriendlyName"));
            }
        }

        /// <summary>
        /// The <see cref="GetData" /> property's name.
        /// </summary>
        public const string GetDataPropertyName = "GetData";

        private bool getData = false;
        [DataMember]
        /// <summary>
        /// Sets and gets the GetData property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool GetData
        {
            get
            {
                return getData;
            }

            set
            {
                if (getData == value)
                {
                    return;
                }

                getData = value;
                if (getData)
                {
                    //MainViewModel.Instance.Robot.Com.AddAddress(this.Id);
                    this.Controller.GetHalls = true;
                    this.Controller.GetCurrent = true;
                    this.Controller.GetPots = true;
                }
                else
                {
                    //MainViewModel.Instance.Robot.Com.RemoveAddress(this.Id);
                    this.Controller.GetHalls = false;
                    this.Controller.GetCurrent = false;
                    this.Controller.GetPots = false;
                }
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(GetDataPropertyName));
            }
        }

        /// <summary>
        /// The <see cref="GetHalls" /> property's name.
        /// </summary>
        public const string GetHallsPropertyName = "GetHalls";

        private bool getHalls = false;
        /// <summary>
        /// Sets and gets the GetHalls property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool GetHalls
        {
            get
            {
                return getHalls;
            }

            set
            {
                if (getHalls == value)
                {
                    return;
                }

                getHalls = value;
                if (getHalls)
                    //MainViewModel.Instance.Robot.Com.AddAddress(this.Id);
                    this.Controller.GetHalls = true;
                else
                    //MainViewModel.Instance.Robot.Com.RemoveAddress(this.Id);
                    this.Controller.GetHalls = false;

                foreach (MotorViewModel motor in Motors)
                {
                    motor.Motor.UpdateConfiguration();
                }
                
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(GetHallsPropertyName));
            }
        }

        /// <summary>
        /// The <see cref="GetPots" /> property's name.
        /// </summary>
        public const string GetPotsPropertyName = "GetPots";

        private bool getPots = false;
        /// <summary>
        /// Sets and gets the GetPots property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool GetPots
        {
            get
            {
                return getPots;
            }

            set
            {
                if (getPots == value)
                {
                    return;
                }

                getPots = value;
                if (getPots)
                    //MainViewModel.Instance.Robot.Com.AddAddress(this.Id);
                    this.Controller.GetPots = true;
                else
                    //MainViewModel.Instance.Robot.Com.RemoveAddress(this.Id);
                    this.Controller.GetPots = false;

                foreach (MotorViewModel motor in Motors)
                {
                    motor.Motor.UpdateConfiguration();
                }
                
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(GetPotsPropertyName));
            }
        }

        /// <summary>
        /// The <see cref="GetCurrent" /> property's name.
        /// </summary>
        public const string GetCurrentPropertyName = "GetCurrent";

        private bool getCurrent = false;
        /// <summary>
        /// Sets and gets the GetCurrent property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool GetCurrent
        {
            get
            {
                return getCurrent;
            }

            set
            {
                if (getCurrent == value)
                {
                    return;
                }

                getCurrent = value;
                if (getCurrent)
                    //MainViewModel.Instance.Robot.Com.AddAddress(this.Id);
                    this.Controller.GetCurrent = true;
                else
                    //MainViewModel.Instance.Robot.Com.RemoveAddress(this.Id);
                    this.Controller.GetCurrent = false;

                foreach(MotorViewModel motor in Motors)
                {
                    motor.Motor.UpdateConfiguration();
                }

                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(GetCurrentPropertyName));
            }
        }

        /// <summary>
        /// The <see cref="HomeJoint" /> property's name.
        /// </summary>
        public const string HomeJointPropertyName = "HomeJoint";

        private bool homeJoint = false;
        [DataMember]
        /// <summary>
        /// Sets and gets the GetCurrent property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool HomeJoint
        {
            get
            {
                return homeJoint;
            }

            set
            {
                if (homeJoint == value)
                    return;
                homeJoint = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(HomeJointPropertyName));
            }
        }

        [field: NonSerializedAttribute()] 
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

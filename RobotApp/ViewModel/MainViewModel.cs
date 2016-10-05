using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using RobotControl;
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Collections.Specialized;
using System.Threading.Tasks;
using RobotApp.Views.Plugins;
using GalaSoft.MvvmLight.Command;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;
using RobotApp.Models;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Concurrent;
using GeomagicTouch;

namespace RobotApp.ViewModel
{
/// <summary>
/// This is the main logic for the app
/// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// This static property returns a reference to the MainViewModel so that any other ViewModel can access it.
        /// </summary>
        public static MainViewModel Instance { get { return instance;  } }
        static MainViewModel instance;

        public IPacketTransport com;

        /// <summary>
        /// Robot data model
        /// </summary>
        /// <summary>
        /// The <see cref="Robot" /> property's name.
        /// </summary>
        public const string RobotPropertyName = "Robot";

        private Robot robot = null;

        /// <summary>
        /// Sets and gets the Robot property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Robot Robot
        {
            get
            {
                return robot;
            }

            set
            {
                if (robot == value)
                {
                    return;
                }

                robot = value;
                if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(RobotPropertyName));
            }
        }

        /// <summary>
        /// Collection of view models for the Joints
        /// </summary>
        public ObservableCollection<ControllerViewModel> Controllers { get; set; }
        
        public ObservableCollection<PluginBase> Plugins { get; set; }

        public ObservableCollection<Device> Devices { get; set; }

        /// <summary>
        /// Collection of view models for the Motors
        /// </summary>
        public ObservableCollection<MotorViewModel> Motors { get; set; }

        //public SignalSinkRegistryViewModel InputSignalRegistry { get; set; }
        public ObservableDictionary<string, string> InputSignalRegistry { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            instance = this; // this sets the static Instance property to this instance.

            Plugins = new ObservableCollection<PluginBase>();

            Devices = new ObservableCollection<Device>();

            Motors = new ObservableCollection<MotorViewModel>();

            Plugins.CollectionChanged += Plugins_CollectionChanged;

            Controllers = new ObservableCollection<ControllerViewModel>();

            Controllers.CollectionChanged += Controllers_CollectionChanged;
            
            Robot = new Robot();

            // This lets us know what's going on with the Robot's internal Joint collection.
            Robot.Controllers.CollectionChanged += RobotControllers_CollectionChanged;

            //InputSignalRegistry = new ViewModel.SignalSinkRegistryViewModel();
            InputSignalRegistry = new ObservableDictionary<string, string>();

            Messenger.Default.Register<Messages.RemoveController>(this,
               (message) =>
               {
                   Controllers.Remove(message.ControllerToRemove);
                   com = this.Robot.Com;
               }
            );
        }

        void Plugins_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (PluginBase item in e.OldItems)
                {
                    item.Dispose();
                }
            }
        }

        void Controllers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach(ControllerViewModel item in e.OldItems)
                {
                    Robot.Controllers.Remove(item.Controller.Id);
                }
            }
        }

        /// <summary>
        /// This will create a new JointItemViewModel whenever we get a new joint (say, when we run ScanForJoints())
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RobotControllers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Controller controller;
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    controller = e.NewItems[0] as Controller;
                    if (controller != null)
                        foreach(var controllerVm in this.Controllers)
                        {
                            if (controller.Id == controllerVm.Id)
                                return;
                        }
                        this.Controllers.Add(new ControllerViewModel(controller));
                    break;
            }
        }

        /// <summary>
        /// This sends out a discovery message that instructs the motor driver boards to report their ID.
        /// </summary>
        public void ScanForControllers()
        {
            Robot.DiscoverControllers();
        }

        /// <summary>
        /// Gets the LoadCommand.
        /// </summary>
        public void LoadData(string ConfigPath)
        {
            //robot.setpointTimer.Stop();
            if(robot.Com != null)
                robot.Com.SendData = true;
            BinaryFormatter ser = new BinaryFormatter();
            FileStream reader;
            com = this.Robot.Com;

            if(ConfigPath != null)
                reader = new FileStream(ConfigPath, FileMode.Open);
            else
                reader = new FileStream("data.ControllerConfig", FileMode.Open);

            ApplicationData appData = (ApplicationData)ser.Deserialize(reader);
            reader.Close();
            this.Controllers = appData.Controllers;
            this.InputSignalRegistry = appData.InputSignalRegistry;
            // We have to manually create new plugin instances
           
            foreach(var plugin in appData.Plugins)
            {
                PluginBase newPlugin = plugin.GetPlugin();
                newPlugin.PostLoadSetup(); // This function calls any user code that's specified
                Plugins.Add(newPlugin);

                //(PluginBase)Activator.CreateInstance(AppDomain.CurrentDomain, "RobotApp", plugin.TypeName);
            }
            foreach (var controllerVm in this.Controllers)
            {
                // In the future, ControllerViewModel should have a Robot property that propogates to its controllers and motors
                if (controllerVm.Controller != null)
                {
                    //this.Controllers.Add(new ControllerViewModel(new Controller() { Robot = this.Robot, Id = controllerVm.Id, FriendlyName = controllerVm.FriendlyName }));
                    //controllerVm.Controller = new Controller() { Robot = this.Robot, Id = controllerVm.Id, FriendlyName = controllerVm.FriendlyName };
                    controllerVm.Controller.Robot = Robot;
                    this.Robot.Controllers.Add(controllerVm.Id, controllerVm.Controller);
                    foreach (var motor in controllerVm.Motors)
                    {
                        motor.Controller = controllerVm.Controller;
                        motor.Controller.Robot = Robot;
                        if (motor.Motor.ControlMode != ControlMode.Reserved) motor.Controller.SendNum++;
                        this.Motors.Add(motor);
                        MainViewModel.Instance.Robot.Motors.Add(motor.Motor);
                        motor.Motor.UpdateConfiguration();
                        motor.SetupMessenger();
                    }
                }
            }

            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Controllers"));
            //robot.setpointTimer.Start();
            robot.Com_UpdateSetpoints();
        }


        public void SaveData(string ConfigPath)
        {
            //XmlSerializer ser = new XmlSerializer(typeof(MainViewModel));
            BinaryFormatter ser = new BinaryFormatter();
            FileStream writer;

            //if( ConfigPath != null)
            //    writer = new FileStream(ConfigPath, FileMode.Truncate);
            //else if(File.Exists("data.ControllerConfig"))
            if(File.Exists(ConfigPath))
            {
                writer = new FileStream(ConfigPath, FileMode.Truncate);
                //writer = new FileStream("data.ControllerConfig", FileMode.Truncate);
            }
            else
            {
                writer = new FileStream(ConfigPath, FileMode.Create);
                //writer = new FileStream("data.ControllerConfig", FileMode.Create);
            }

            // Create a new ApplicationData and copy everything into it
            ApplicationData appData = new ApplicationData();
            appData.Controllers = this.Controllers;
            appData.InputSignalRegistry = InputSignalRegistry;
            foreach(PluginBase plugin in Plugins)
            {
                appData.Plugins.Add(new PluginDataItem(plugin));
            }

            ser.Serialize(writer, appData);
            writer.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
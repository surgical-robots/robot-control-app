using GalaSoft.MvvmLight.Command;
using RobotApp.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.IO;
using GeomagicTouch;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for GeomagicTouchConfigurationView.xaml
    /// </summary>
    public partial class GeomagicTouch : PluginBase
    {
        public System.Windows.Forms.Timer UpdateTimer = new System.Windows.Forms.Timer();

        public ObservableCollection<string> DeviceNames { get; set; }

        /// <summary>
        /// This function is manually called at the end of the constructor (below) as well as automatically getting called after deserialization.
        /// Place any code in here that you want executed after deserialization or construction.
        /// </summary>
         public override void PostLoadSetup()
         {
             Messenger.Default.Register<Messages.Signal>(this, Inputs["ForceX"].UniqueID, (message) =>
             {
                 if(Device != null)
                    Device.SetpointX = message.Value;
             });

             Messenger.Default.Register<Messages.Signal>(this, Inputs["ForceY"].UniqueID, (message) =>
             {
                 if (Device != null)
                     Device.SetpointY = message.Value;
             });

             Messenger.Default.Register<Messages.Signal>(this, Inputs["ForceZ"].UniqueID, (message) =>
             {
                 if (Device != null)
                     Device.SetpointZ = message.Value;
             });
            
             Messenger.Default.Register<Messages.Signal>(this, Inputs["HapticsEnabled"].UniqueID, (message) =>
             {
                 if (Device != null)
                 {
                     if (message.Value > 0.5)
                         Device.SetpointEnabled = true;
                     else
                         Device.SetpointEnabled = false;
                 }
             });

             base.PostLoadSetup();
         }

        /// <summary>
        /// Create a new GeomagicTouch. 
        /// 
        /// This function only runs when creating new instances of this class -- it does *not* run when instances of this class are deserialized.
        /// </summary>
        public GeomagicTouch()
        {
            this.DataContext = this;
            InitializeComponent();

            DeviceNames = new ObservableCollection<string>();

            Outputs.Add("X", new OutputSignalViewModel("X Position"));
            Outputs.Add("Y", new OutputSignalViewModel("Y Position"));
            Outputs.Add("Z", new OutputSignalViewModel("Z Position"));
            Outputs.Add("Theta1", new OutputSignalViewModel("Pitch / Theta 1"));
            Outputs.Add("Theta2", new OutputSignalViewModel("Yaw / Theta 2"));
            Outputs.Add("Theta3", new OutputSignalViewModel("Roll / Theta 3"));
            Outputs.Add("Inkwell", new OutputSignalViewModel("Inkwell Switch"));
            Outputs.Add("Button1", new OutputSignalViewModel("Button 1"));
            Outputs.Add("Button2", new OutputSignalViewModel("Button 2"));
            TypeName = "Geomagic Touch";

            Inputs.Add("ForceX", new ViewModel.InputSignalViewModel("SetpointX", this.InstanceName));
            Inputs.Add("ForceY", new ViewModel.InputSignalViewModel("SetpointY", this.InstanceName));
            Inputs.Add("ForceZ", new ViewModel.InputSignalViewModel("SetpointZ", this.InstanceName));
            Inputs.Add("HapticsEnabled", new ViewModel.InputSignalViewModel("HapticsEnabled", this.InstanceName));

            // Get a list of all GeomagicTouch device names
            foreach(string device in GetGeomagicDevices())
            {
                DeviceNames.Add(device);
            }

//            UpdateTimer = new System.Timers.Timer();
            UpdateTimer.Tick += UpdateTimer_Tick;
            UpdateTimer.Interval = 15;

            // Call any additional setup work that needs to happen in either constructor's case, or loading the plugin from deserialization.
            PostLoadSetup();
        }


        /// <summary>
        /// The <see cref="Device" /> property's name.
        /// </summary>
        public const string DevicePropertyName = "Device";

        private Device device = null;

        /// <summary>
        /// Sets and gets the Device property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Device Device
        {
            get
            {
                return device;
            }

            set
            {
                if (device == value)
                {
                    return;
                }

                device = value;
                RaisePropertyChanged(DevicePropertyName);

            }
        }

        /// <summary>
        /// Gets a list of Geomagic devices
        /// </summary>
        /// <returns></returns>
        string[] GetGeomagicDevices()
        {
            string[] fileNames = new string[1];
            try {
                fileNames = Directory.GetFiles(@"C:\Users\Public\Documents\3DSystems\", "*.config");
            for(int i=0;i<fileNames.Length;i++)
            {
                fileNames[i] = Path.GetFileNameWithoutExtension(fileNames[i]);
            }
            }
            catch {  }
            return fileNames;
        }


        void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (Device == null)
                return;
            if (AngleType == 0)
                Device.UpdateTransform();
            else
                Device.Update();
            Outputs["X"].Value = Device.X;
            Outputs["Y"].Value = Device.Y;
            Outputs["Z"].Value = Device.Z;
            Outputs["Theta1"].Value = Device.Theta1 * (180 / Math.PI);
            Outputs["Theta2"].Value = Device.Theta2 * (180 / Math.PI);
            Outputs["Theta3"].Value = Device.Theta3 * (180 / Math.PI);
            Outputs["Inkwell"].Value = Device.IsInInkwell ? 1.0 : 0.0;
            Outputs["Button1"].Value = Device.Button1 ? 1.0 : 0.0;
            Outputs["Button2"].Value = Device.Button2 ? 1.0 : 0.0;
        }

        /// <summary>
        /// The <see cref="SelectedDeviceName" /> property's name.
        /// </summary>
        public const string SelectedDeviceNamePropertyName = "SelectedDeviceName";

        private string selectedDevice = "";

        /// <summary>
        /// Sets and gets the SelectedDeviceName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SelectedDeviceName
        {
            get
            {
                return selectedDevice;
            }

            set
            {
                if (selectedDevice == value)
                {
                    return;
                }

                selectedDevice = value;
                RaisePropertyChanged(SelectedDeviceNamePropertyName);
                ConnectCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// The <see cref="ConnectButtonText" /> property's name.
        /// </summary>
        public const string ConnectButtonTextPropertyName = "ConnectButtonText";

        private string connectButtonText = "Connect";

        /// <summary>
        /// Sets and gets the ConnectButtonText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ConnectButtonText
        {
            get
            {
                return connectButtonText;
            }

            set
            {
                if (connectButtonText == value)
                {
                    return;
                }

                connectButtonText = value;
                RaisePropertyChanged(ConnectButtonTextPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="UpdatePeriod" /> property's name.
        /// </summary>
        public const string UpdatePeriodPropertyName = "UpdatePeriod";

        private int updatePeriod = 15;

        /// <summary>
        /// Sets and gets the UpdatePeriod property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int UpdatePeriod
        {
            get
            {
                return updatePeriod;
            }

            set
            {
                if (updatePeriod == value)
                {
                    return;
                }

                updatePeriod = value;
                UpdateTimer.Interval = updatePeriod;
                RaisePropertyChanged(UpdatePeriodPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="AngleType" /> property's name.
        /// </summary>
        public const string AngleTypePropertyName = "AngleType";

        private int angleType = 0;

        /// <summary>
        /// Sets and gets the AngleType property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int AngleType
        {
            get
            {
                return angleType;
            }

            set
            {
                if (angleType == value)
                {
                    return;
                }

                angleType = value;
                RaisePropertyChanged(AngleTypePropertyName);
            }
        }

        private RelayCommand connectCommand;

        /// <summary>
        /// Gets the ConnectCommand.
        /// </summary>
        public RelayCommand ConnectCommand
        {
            get
            {
                return connectCommand
                    ?? (connectCommand = new RelayCommand(
                    () =>
                    {
                        if (!ConnectCommand.CanExecute(null))
                        {
                            return;
                        }

                        Device = new Device(SelectedDeviceName);
                        MainViewModel.Instance.Devices.Add(Device);
                        this.RaisePropertyChanged("Device");
                        ConnectButtonText = "Connected";
                        ConnectCommand.RaiseCanExecuteChanged();
                        StartCommand.RaiseCanExecuteChanged();
                        
                    },
                    // We can only connect if a device is selected, we don't currently have a device connected, 
                    // and our device name is real
                    () => SelectedDeviceName != null && Device == null && SelectedDeviceName.Length > 0)); 
            }
        }

        private RelayCommand startCommand;

        /// <summary>
        /// Gets the StartCommand.
        /// </summary>
        public RelayCommand StartCommand
        {
            get
            {
                return startCommand
                    ?? (startCommand = new RelayCommand(
                    () =>
                    {
                        if (!StartCommand.CanExecute(null))
                        {
                            return;
                        }

                        Device.Start();
                        UpdateTimer.Start();
                    },
                    () => Device != null));
            }
        }

        //public override void Dispose()
        //{
        //    if(Device != null)
        //        Device.Stop();
        //}

        public void Dispose()
        {
            if (Device != null)
                Device.Stop();
        }

    }
}

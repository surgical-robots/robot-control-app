using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kinematics;
using GeomagicTouch;
using System.Diagnostics;
using GalaSoft.MvvmLight.Command;
using System.IO;
namespace RobotApp.ViewModel.Plugins
{
    public class GeomagicTouchViewModel : SignalPluginViewModelBase
    {
        public System.Timers.Timer UpdateTimer { get; set; }

        public ObservableCollection<string> DeviceNames { get; set; }

        public ObservableDictionary<string, SignalSourceViewModel> SignalSources { get; set; }

        /// <summary>
        /// Constructs a new GeomagicTouch View Model
        /// </summary>
        public GeomagicTouchViewModel()
        {
            DeviceNames = new ObservableCollection<string>();
            SignalSources = new ObservableDictionary<string, SignalSourceViewModel>();

            SignalSources.Add("X", new SignalSourceViewModel("X Position"));
            SignalSources.Add("Y", new SignalSourceViewModel("Y Position"));
            SignalSources.Add("Z", new SignalSourceViewModel("Z Position"));
            SignalSources.Add("Theta1", new SignalSourceViewModel("Gimbal Theta 1"));
            SignalSources.Add("Theta2", new SignalSourceViewModel("Gimbal Theta 2"));
            SignalSources.Add("Theta3", new SignalSourceViewModel("Gimbal Theta 3"));
            SignalSources.Add("Inkwell", new SignalSourceViewModel("Inkwell Switch"));
            SignalSources.Add("Button1", new SignalSourceViewModel("Button 1"));
            SignalSources.Add("Button2", new SignalSourceViewModel("Button 2"));
            SignalSources.Add("Button3", new SignalSourceViewModel("Button 1"));
            SignalSources.Add("Button4", new SignalSourceViewModel("Button 2"));

            TypeName = "Geomagic Touch";

            // Get a list of all GeomagicTouch device names
            foreach(string device in GetGeomagicDevices())
            {
                DeviceNames.Add(device);
            }

            UpdateTimer = new System.Timers.Timer();
            UpdateTimer.Elapsed += UpdateTimer_Elapsed;
            UpdateTimer.Interval = 50;
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
            string[] fileNames = Directory.GetFiles(@"C:\Users\Public\Documents\SensAble\", "*.config");
            for(int i=0;i<fileNames.Length;i++)
            {
                fileNames[i] = Path.GetFileNameWithoutExtension(fileNames[i]);
            }
            return fileNames;
        }


        void UpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Device == null)
                return;
            Device.Update();
            SignalSources["X"].Value = Device.X;
            SignalSources["Y"].Value = Device.Y;
            SignalSources["Z"].Value = Device.Z;
            SignalSources["Theta1"].Value = Device.Theta1;
            SignalSources["Theta2"].Value = Device.Theta2;
            SignalSources["Theta3"].Value = Device.Theta3;
            SignalSources["Inkwell"].Value = Device.IsInInkwell ? 1.0 : 0.0;
            SignalSources["Button1"].Value = Device.Button1 ? 1.0 : 0.0;
            SignalSources["Button2"].Value = Device.Button2 ? 1.0 : 0.0;
            SignalSources["Button3"].Value = Device.Button3 ? 1.0 : 0.0;
            SignalSources["Button4"].Value = Device.Button4 ? 1.0 : 0.0;
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

        public override void Dispose()
        {
            if(Device != null)
                Device.Stop();
        }
    }
}

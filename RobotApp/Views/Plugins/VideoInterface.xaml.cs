using GalaSoft.MvvmLight.Command;
using System;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using AForge.Video.DirectShow;


namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for GeomagicTouchConfigurationView.xaml
    /// </summary>
    public partial class VideoInterface : PluginBase
    {
        public ObservableCollection<string> DeviceNames { get; set; }
        public ObservableCollection<string> SettingNames { get; set; }
        public VideoCaptureDevice CaptureDevice;
        private FilterInfoCollection _deviceList;
        private VideoCapabilities[] _deviceCapabilites;
        private bool _wasRunning = false;


        /// <summary>
        /// Create a new VideoInterface. 
        /// 
        /// This function only runs when creating new instances of this class -- it does *not* run when instances of this class are deserialized.
        /// </summary>
        public VideoInterface()
        {
            this.DataContext = this;
            InitializeComponent();
            TypeName = "Video Interface";

            DeviceNames = new ObservableCollection<string>();
            SettingNames = new ObservableCollection<string>();
            _deviceList = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            
            // Get a list of all video capture source names
            for (int i = 0; i < _deviceList.Count; i++ )
            {
                DeviceNames.Add(_deviceList[i].Name);
            }

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
                if (CaptureDevice != null && CaptureDevice.IsRunning)
                    CaptureDevice.SignalToStop();
                if (_wasRunning)
                    CaptureDevice.NewFrame -= CaptureDevice_NewFrame;
                
                for(int i = 0; i < _deviceList.Count; i++)
                {
                    if(_deviceList[i].Name == selectedDevice)
                    {
                        CaptureDevice = new VideoCaptureDevice(_deviceList[i].MonikerString);
                        _deviceCapabilites = CaptureDevice.VideoCapabilities;
                        SelectedSetting = 0;
                        CreateCapabilityList();
                        CaptureDevice.NewFrame += CaptureDevice_NewFrame;
                        _wasRunning = true;
                    }
                }
            }
        }

        void CaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            Bitmap frame = eventArgs.Frame;
            this.Dispatcher.Invoke((Action)(() =>
            {
                VideoImage.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(frame.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }));
//            VideoImage.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(frame.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        void CreateCapabilityList()
        {
            string dummyString = "";
            SettingNames.Clear();
            foreach(VideoCapabilities settings in _deviceCapabilites)
            {
                dummyString = "";
                dummyString = settings.FrameSize.Width + "x" + settings.FrameSize.Height + " " + settings.AverageFrameRate + "FPS";
                SettingNames.Add(dummyString);
            }
        }

        /// <summary>
        /// The <see cref="SelectedSetting" /> property's name.
        /// </summary>
        public const string SelectedSettingPropertyName = "SelectedSetting";

        private int selectedSetting = 0;

        /// <summary>
        /// Sets and gets the SelectedSetting property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int SelectedSetting
        {
            get
            {
                return selectedSetting;
            }

            set
            {
                if (selectedSetting == value)
                {
                    return;
                }

                selectedSetting = value;
                RaisePropertyChanged(SelectedSettingPropertyName);
                if(selectedSetting != -1)
                    CaptureDevice.VideoResolution = _deviceCapabilites[selectedSetting];
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

        private RelayCommand<string> startCommand;

        /// <summary>
        /// Gets the DetectCOMsCommand.
        /// </summary>
        public RelayCommand<string> StartCommand
        {
            get
            {
                return startCommand
                    ?? (startCommand = new RelayCommand<string>(
                    p =>
                    {
                        if(CaptureDevice.IsRunning)
                        {
                            CaptureDevice.SignalToStop();
                            //CaptureDevice.Stop();
                            //CaptureDevice.WaitForStop();
                            ConnectButtonText = "Connect";
                        }
                        else
                        {
                            CaptureDevice.Start();
                            ConnectButtonText = "Disconnect";
                        }
                    }));
            }
        }
    }
}

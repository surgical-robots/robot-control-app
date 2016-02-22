using System.Collections.ObjectModel;
using System.IO;
using GalaSoft.MvvmLight.Command;
using RobotApp.ViewModel;
using GeomagicTouch;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for LoadHapticObject.xaml
    /// </summary>
    public partial class LoadHapticObject : PluginBase
    {
        Device HapticDevice;

        public ObservableCollection<string> DeviceNames { get; set; }

        public LoadHapticObject()
        {
            TypeName = "Load Haptic Object";
            InitializeComponent();
            DeviceNames = new ObservableCollection<string>();

            // Add available haptic devices to dropdown list
            foreach(var device in MainViewModel.Instance.Devices)
            {
                DeviceNames.Add(device.Name);
            }

            ObjectList = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("*.obj");
        }

        /// <summary>
        /// The <see cref="SelectedDeviceName" /> property's name.
        /// </summary>
        public const string SelectedDeviceNamePropertyName = "SelectedDeviceName";

        private string selectedDeviceName = "";

        /// <summary>
        /// Sets and gets the SelectedDeviceName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SelectedDeviceName
        {
            get
            {
                return selectedDeviceName;
            }

            set
            {
                if (selectedDeviceName == value)
                {
                    return;
                }

                selectedDeviceName = value;

                foreach(var device in MainViewModel.Instance.Devices)
                {
                    if (device.Name == selectedDeviceName)
                    {
                        HapticDevice = device;
                        break;
                    }
                }

                RaisePropertyChanged(SelectedDeviceNamePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ObjectList" /> property's name.
        /// </summary>
        public const string ObjectListPropertyName = "ObjectList";

        private FileInfo[] objectList = null;

        /// <summary>
        /// Sets and gets the ReportList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public FileInfo[] ObjectList
        {
            get
            {
                return objectList;
            }

            set
            {
                if (objectList == value)
                {
                    return;
                }

                objectList = value;
                RaisePropertyChanged(ObjectListPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SelectedObject" /> property's name.
        /// </summary>
        public const string SelectedObjectPropertyName = "SelectedObject";

        private string selectedObject = null;

        /// <summary>
        /// Sets and gets the SelectedObject property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SelectedObject
        {
            get
            {
                return selectedObject;
            }

            set
            {
                if (selectedObject == value)
                {
                    return;
                }

                selectedObject = value;
                RaisePropertyChanged(SelectedObjectPropertyName);
            }
        }

        private RelayCommand<string> loadObjCommand;

        /// <summary>
        /// Gets the StartCommand.
        /// </summary>
        public RelayCommand<string> LoadObjCommand
        {
            get
            {
                return loadObjCommand
                    ?? (loadObjCommand = new RelayCommand<string>(
                    p =>
                    {
                        string startupPath = System.IO.Directory.GetCurrentDirectory();
                        startupPath = startupPath + "\\";
                        string FullPath = startupPath + SelectedObject;
                        //int vertexNum, faceNum;
                        //float[][] vertexArray;
                        //int[][] faceArray;

                        //var lines = File.ReadAllLines(FullPath);
                        ////List of double[]. Each entry of the list contains 3D vertex x,y,z in double array form
                        //var verts = lines.Where(l => Regex.IsMatch(l, @"^v(\s+-?\d+\.?\d+([eE][-+]?\d+)?){3,3}$"))
                        //    .Select(l => Regex.Split(l, @"\s+", RegexOptions.None).Skip(1).ToArray()) //Skip v
                        //    .Select(nums => new float[] { float.Parse(nums[0]), float.Parse(nums[1]), float.Parse(nums[2]) })
                        //    .ToList();

                        ////List of int[]. Each entry of the list contains zero based index of vertex reference
                        ////Obj format is 1 based index. This is converting into C# zero based, so on write out you need to convert back.
                        //var faces = lines.Where(l => Regex.IsMatch(l, @"^f(\s\d+(\/+\d+)?){3,3}$"))
                        //    .Select(l => Regex.Split(l, @"\s+", RegexOptions.None).Skip(1).ToArray())//Skip f
                        //    .Select(i => i.Select(a => Regex.Match(a, @"\d+", RegexOptions.None).Value).ToArray())
                        //    .Select(nums => new int[] { int.Parse(nums[0]) - 1, int.Parse(nums[1]) - 1, int.Parse(nums[2]) - 1 })
                        //    .ToList();
                        //vertexNum = verts.Count;
                        //faceNum = faces.Count;
                        //vertexArray = verts.ToArray();
                        //faceArray = faces.ToArray();

                        HapticDevice.LoadObj(FullPath);
                    }));
            }
        }

    }
}

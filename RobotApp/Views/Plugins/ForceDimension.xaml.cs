using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight.Command;
using RobotApp.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using ForceDimension;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for ForceDimension.xaml
    /// </summary>
    public partial class ForceDimension : PluginBase
    {
        private Thread UpdateThread;
        ObservableCollection<Device> Devices = new ObservableCollection<Device>();
        public ObservableCollection<string> DeviceNames { get; set; }
        Device device;
        double forceX = 0;
        double forceY = 0;
        double forceZ = 0;
        int deviceCount = 0;
        sbyte selsectedDeviceID = 0;
        bool forceEnabled = false;
        bool locked = false;
        Vector3D holdPos = new Vector3D();

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["ForceX"].UniqueID, (message) =>
            {
                if (device != null && !locked)
                {
                    if(forceEnabled)
                    {
                        forceX = message.Value;
                        UpdateForces();
                    }
                    else
                    {
                        forceX = 0;
                        UpdateForces();
                    }
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["ForceY"].UniqueID, (message) =>
            {
                if (device != null && !locked)
                {
                    if (forceEnabled)
                    {
                        forceY = message.Value;
                        UpdateForces();
                    }
                    else
                    {
                        forceY = 0;
                        UpdateForces();
                    }
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["ForceZ"].UniqueID, (message) =>
            {
                if (device != null && !locked)
                {
                    if (forceEnabled)
                    {
                        forceY = message.Value;
                        UpdateForces();
                    }
                    else
                    {
                        forceY = 0;
                        UpdateForces();
                    }
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["HapticsEnabled"].UniqueID, (message) =>
            {
                if (device != null)
                {
                    if (message.Value > 0.5)
                        forceEnabled = true;
                    else
                        forceEnabled = false;
                }
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["Lock"].UniqueID, (message) =>
            {
                if(message.Value > 0.5 && !locked)
                {
                    holdPos = new Vector3D(device.X, device.Y, device.Z);
                    locked = true;
                    //device.Brake();
                }
                else if (message.Value > 0.5 && locked)
                {
                    locked = false;
                    forceX = 0;
                    forceY = 0;
                    forceZ = 0;
                    UpdateForces();
                    //device.Brake();
                }
            });

            device.GetDeviceCount();

            base.PostLoadSetup();
        }

        public ForceDimension()
        {
            this.TypeName = "Force Dimension";
            InitializeComponent();

            DeviceNames = new ObservableCollection<string>();

            Outputs.Add("X", new OutputSignalViewModel("X"));
            Outputs.Add("Y", new OutputSignalViewModel("Y"));
            Outputs.Add("Z", new OutputSignalViewModel("Z"));
            Outputs.Add("ThetaX", new OutputSignalViewModel("Theta X"));
            Outputs.Add("ThetaY", new OutputSignalViewModel("Theta Y"));
            Outputs.Add("ThetaZ", new OutputSignalViewModel("Theta Z"));
            Outputs.Add("R00", new OutputSignalViewModel("R00"));
            Outputs.Add("R01", new OutputSignalViewModel("R01"));
            Outputs.Add("R02", new OutputSignalViewModel("R02"));
            Outputs.Add("R10", new OutputSignalViewModel("R10"));
            Outputs.Add("R11", new OutputSignalViewModel("R11"));
            Outputs.Add("R12", new OutputSignalViewModel("R12"));
            Outputs.Add("R20", new OutputSignalViewModel("R20"));
            Outputs.Add("R21", new OutputSignalViewModel("R21"));
            Outputs.Add("R22", new OutputSignalViewModel("R22"));

            Outputs.Add("GripperPos", new OutputSignalViewModel("Gripper"));

            Inputs.Add("ForceX", new ViewModel.InputSignalViewModel("Force X", this.InstanceName));
            Inputs.Add("ForceY", new ViewModel.InputSignalViewModel("Force Y", this.InstanceName));
            Inputs.Add("ForceZ", new ViewModel.InputSignalViewModel("Force Z", this.InstanceName));
            Inputs.Add("HapticsEnabled", new ViewModel.InputSignalViewModel("Haptics Enabled", this.InstanceName));
            Inputs.Add("Lock", new ViewModel.InputSignalViewModel("Lock", this.InstanceName));

            device = new Device();
            deviceCount = device.GetDeviceCount();

            for (int i = 0; i < deviceCount; i++ )
            {
                Devices.Add(new Device((sbyte)i));
                string dummyName = Convert.ToString(Devices[i].SerialNum);
                if (Devices[i].IsLeft)
                    dummyName = dummyName + " (Left)";
                DeviceNames.Add(dummyName);
            }

            PostLoadSetup();
        }

        private RelayCommand<string> detectControllerCommand;

        /// <summary>
        /// Gets the DetectControllerCommand.
        /// </summary>
        public RelayCommand<string> DetectControllerCommand
        {
            get
            {
                return detectControllerCommand
                    ?? (detectControllerCommand = new RelayCommand<string>(
                    p =>
                    {
                        deviceCount = device.GetDeviceCount();
                        if (deviceCount > 0)
                            ConnectController();
                    }));
            }
        }

        public void ConnectController()
        {
            if (!device.IsInitialized)
                device = Devices[SelectedDeviceIndex];
            if (device.IsInitialized)
            {
                device.Start();
                UpdateThread = new Thread(new ThreadStart(UpdateState));
                UpdateThread.Start();
            }
        }

        public void UpdateState()
        {
            while (device.IsInitialized)
            {
                device.UpdateDevice();

                Outputs["X"].Value = device.X * 1000;
                Outputs["Y"].Value = device.Y * 1000;
                Outputs["Z"].Value = device.Z * 1000;
                Outputs["ThetaX"].Value = -device.Theta1;
                Outputs["ThetaY"].Value = device.Theta2;
                Outputs["ThetaZ"].Value = device.Theta3;
                Outputs["R00"].Value = device.R00;
                if(device.IsLeft)
                    Outputs["R01"].Value = device.R01;
                else
                    Outputs["R01"].Value = -device.R01;
                Outputs["R02"].Value = device.R02;
                Outputs["R10"].Value = device.R10;
                Outputs["R11"].Value = device.R11;
                Outputs["R12"].Value = device.R12;
                Outputs["R20"].Value = device.R20;
                Outputs["R21"].Value = device.R21;
                Outputs["R22"].Value = device.R22;
                Outputs["GripperPos"].Value = device.GripperPos;

                double spring = 5000;
                if(locked)
                {
                    forceX = (holdPos.X - device.X) * spring;
                    forceY = (holdPos.Y - device.Y) * spring;
                    forceZ = (holdPos.Z - device.Z) * spring;
                    if (forceX > 20) forceX = 20;
                    else if (forceX < -20) forceX = -20;
                    if (forceY > 20) forceY = 20;
                    else if (forceY < -20) forceY = -20;
                    if (forceZ > 20) forceZ = 20;
                    else if (forceZ < -20) forceZ = -20;
                    UpdateForces();
                }
            }
        }

        void UpdateForces()
        {
            device.UpdateForces(forceX, forceY, forceZ);
        }

        /// <summary>
        /// The <see cref="SelectedDeviceIndex" /> property's name.
        /// </summary>
        public const string SelectedDeviceIndexPropertyName = "SelectedDeviceIndex";

        private int selectedDeviceIndex = 0;

        /// <summary>
        /// Sets and gets the Index property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public int SelectedDeviceIndex
        {
            get
            {
                return selectedDeviceIndex;
            }

            set
            {
                if (selectedDeviceIndex == value)
                {
                    return;
                }

                selectedDeviceIndex = value;
                selsectedDeviceID = (sbyte)selectedDeviceIndex;
                RaisePropertyChanged(SelectedDeviceIndexPropertyName);
            }
        }

    }
}

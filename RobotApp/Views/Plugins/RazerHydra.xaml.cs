using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight.Command;
using RobotApp.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using RazerHydra;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for ForceDimension.xaml
    /// </summary>
    public partial class RazerHydra : PluginBase
    {
        System.Windows.Forms.Timer updateTimer = new System.Windows.Forms.Timer();
        Device device;

        int deviceCount = 0;
        sbyte selsectedDeviceID = 0;

        public override void PostLoadSetup()
        {
            base.PostLoadSetup();
        }

        public RazerHydra()
        {
            this.TypeName = "Razer Hydra";
            InitializeComponent();

            Outputs.Add("L_X", new OutputSignalViewModel("Left X"));
            Outputs.Add("L_Y", new OutputSignalViewModel("Left Y"));
            Outputs.Add("L_Z", new OutputSignalViewModel("Left Z"));
            Outputs.Add("L_ThetaX", new OutputSignalViewModel("Left Theta X"));
            Outputs.Add("L_ThetaY", new OutputSignalViewModel("Left Theta Y"));
            Outputs.Add("L_ThetaZ", new OutputSignalViewModel("Left Theta Z"));
            Outputs.Add("L_R00", new OutputSignalViewModel("Left R00"));
            Outputs.Add("L_R01", new OutputSignalViewModel("Left R01"));
            Outputs.Add("L_R02", new OutputSignalViewModel("Left R02"));
            Outputs.Add("L_R10", new OutputSignalViewModel("Left R10"));
            Outputs.Add("L_R11", new OutputSignalViewModel("Left R11"));
            Outputs.Add("L_R12", new OutputSignalViewModel("Left R12"));
            Outputs.Add("L_R20", new OutputSignalViewModel("Left R20"));
            Outputs.Add("L_R21", new OutputSignalViewModel("Left R21"));
            Outputs.Add("L_R22", new OutputSignalViewModel("Left R22"));
            Outputs.Add("L_Grip", new OutputSignalViewModel("Left Gripper"));
            Outputs.Add("L_JoystickX", new OutputSignalViewModel("Left Joystick X"));
            Outputs.Add("L_JoystickY", new OutputSignalViewModel("Left Joystick Y"));
            Outputs.Add("L_Bumper", new OutputSignalViewModel("Left Bumper"));
            Outputs.Add("L_Button1", new OutputSignalViewModel("Left Button 1"));
            Outputs.Add("L_Button2", new OutputSignalViewModel("Left Button 2"));
            Outputs.Add("L_Button3", new OutputSignalViewModel("Left Button 3"));
            Outputs.Add("L_Button4", new OutputSignalViewModel("Left Button 4"));

            Outputs.Add("R_X", new OutputSignalViewModel("Right X"));
            Outputs.Add("R_Y", new OutputSignalViewModel("Right Y"));
            Outputs.Add("R_Z", new OutputSignalViewModel("Right Z"));
            Outputs.Add("R_ThetaX", new OutputSignalViewModel("Right Theta X"));
            Outputs.Add("R_ThetaY", new OutputSignalViewModel("Right Theta Y"));
            Outputs.Add("R_ThetaZ", new OutputSignalViewModel("Right Theta Z"));
            Outputs.Add("R_R00", new OutputSignalViewModel("Right R00"));
            Outputs.Add("R_R01", new OutputSignalViewModel("Right R01"));
            Outputs.Add("R_R02", new OutputSignalViewModel("Right R02"));
            Outputs.Add("R_R10", new OutputSignalViewModel("Right R10"));
            Outputs.Add("R_R11", new OutputSignalViewModel("Right R11"));
            Outputs.Add("R_R12", new OutputSignalViewModel("Right R12"));
            Outputs.Add("R_R20", new OutputSignalViewModel("Right R20"));
            Outputs.Add("R_R21", new OutputSignalViewModel("Right R21"));
            Outputs.Add("R_R22", new OutputSignalViewModel("Right R22"));
            Outputs.Add("R_Grip", new OutputSignalViewModel("Right Gripper"));
            Outputs.Add("R_JoystickX", new OutputSignalViewModel("Right Joystick X"));
            Outputs.Add("R_JoystickY", new OutputSignalViewModel("Right Joystick Y"));
            Outputs.Add("R_Bumper", new OutputSignalViewModel("Right Bumper"));
            Outputs.Add("R_Button1", new OutputSignalViewModel("Right Button 1"));
            Outputs.Add("R_Button2", new OutputSignalViewModel("Right Button 2"));
            Outputs.Add("R_Button3", new OutputSignalViewModel("Right Button 3"));
            Outputs.Add("R_Button4", new OutputSignalViewModel("Right Button 4"));

            updateTimer.Interval = 15;
            updateTimer.Tick += updateTimer_Tick;

            device = new Device();

            if(device.IsInitialized)
            {
                device.Start();
                updateTimer.Start();
            }

            //PostLoadSetup();
        }

        void updateTimer_Tick(object sender, EventArgs e)
        {
            UpdateState();
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
            if (device.IsInitialized)
            {
                device.Start();
                updateTimer.Start();
            }
        }

        private void UpdateState()
        {
            if(device.IsInitialized)
            {
                device.UpdateDevice();

                Outputs["L_X"].Value = device.X_L;
                Outputs["L_Y"].Value = device.Y_L;
                Outputs["L_Z"].Value = device.Z_L;
                Outputs["L_ThetaX"].Value = -device.Theta1_L;
                Outputs["L_ThetaY"].Value = device.Theta2_L;
                Outputs["L_ThetaZ"].Value = device.Theta3_L;
                Outputs["L_R00"].Value = device.R00_L;
                Outputs["L_R01"].Value = device.R01_L;
                Outputs["L_R02"].Value = device.R02_L;
                Outputs["L_R10"].Value = device.R10_L;
                Outputs["L_R11"].Value = device.R11_L;
                Outputs["L_R12"].Value = device.R12_L;
                Outputs["L_R20"].Value = device.R20_L;
                Outputs["L_R21"].Value = device.R21_L;
                Outputs["L_R22"].Value = device.R22_L;
                Outputs["L_Grip"].Value = device.GripperPos_L;
                Outputs["L_JoystickX"].Value = device.JoystickX_L;
                Outputs["L_JoystickY"].Value = device.JoystickY_L;
                Outputs["L_Bumper"].Value = device.Bumper_L;
                Outputs["L_Button1"].Value = device.Button1_L;
                Outputs["L_Button2"].Value = device.Button2_L;
                Outputs["L_Button3"].Value = device.Button3_L;
                Outputs["L_Button4"].Value = device.Button4_L;

                Outputs["R_X"].Value = device.X_R;
                Outputs["R_Y"].Value = device.Y_R;
                Outputs["R_Z"].Value = device.Z_R;
                Outputs["R_ThetaX"].Value = device.Theta1_R;
                Outputs["R_ThetaY"].Value = device.Theta2_R;
                Outputs["R_ThetaZ"].Value = device.Theta3_R;
                Outputs["R_R00"].Value = device.R00_R;
                Outputs["R_R01"].Value = device.R01_R;
                Outputs["R_R02"].Value = device.R02_R;
                Outputs["R_R10"].Value = device.R10_R;
                Outputs["R_R11"].Value = device.R11_R;
                Outputs["R_R12"].Value = device.R12_R;
                Outputs["R_R20"].Value = device.R20_R;
                Outputs["R_R21"].Value = device.R21_R;
                Outputs["R_R22"].Value = device.R22_R;
                Outputs["R_Grip"].Value = device.GripperPos_R;
                Outputs["R_JoystickX"].Value = device.JoystickX_R;
                Outputs["R_JoystickY"].Value = device.JoystickY_R;
                Outputs["R_Bumper"].Value = device.Bumper_R;
                Outputs["R_Button1"].Value = device.Button1_R;
                Outputs["R_Button2"].Value = device.Button2_R;
                Outputs["R_Button3"].Value = device.Button3_R;
                Outputs["R_Button4"].Value = device.Button4_R;
            }
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

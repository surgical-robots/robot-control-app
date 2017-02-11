using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Threading;
using GalaSoft.MvvmLight.Command;
using RobotApp.ViewModel;
using SharpDX.XInput;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for ButtonInterface.xaml
    /// </summary>
    public partial class XBoxController : PluginBase
    {
        private Thread UpdateThread;
        private SharpDX.XInput.Controller Controller;
        private Vibration vibrateOn;
        private Vibration vibrateOff;

        public XBoxController()
        {
            this.TypeName = "XBox Controller";
            InitializeComponent();

            vibrateOn.LeftMotorSpeed = 60000;
            vibrateOn.RightMotorSpeed = 60000;
            vibrateOff.LeftMotorSpeed = 0;
            vibrateOff.RightMotorSpeed = 0;

            Outputs.Add("JoystickLeftX", new OutputSignalViewModel("Left Joystick X"));
            Outputs.Add("JoystickLeftY", new OutputSignalViewModel("Left Joystick Y"));
            Outputs.Add("JoystickRightX", new OutputSignalViewModel("Right Joystick X"));
            Outputs.Add("JoystickRightY", new OutputSignalViewModel("Right Joystick Y"));
            Outputs.Add("TriggerLeft", new OutputSignalViewModel("Left Trigger"));
            Outputs.Add("TriggerRight", new OutputSignalViewModel("Right Trigger"));
            Outputs.Add("ButtonA", new OutputSignalViewModel("Button A"));
            Outputs.Add("ButtonB", new OutputSignalViewModel("Button B"));
            Outputs.Add("ButtonX", new OutputSignalViewModel("Button X"));
            Outputs.Add("ButtonY", new OutputSignalViewModel("Button Y"));

            Controller = new Controller(UserIndex.One);
            if(Controller.IsConnected)
            {
                Controller.SetVibration(vibrateOn);
                Thread.Sleep(100);
                Controller.SetVibration(vibrateOff);
                UpdateThread = new Thread(new ThreadStart(UpdateState));
                UpdateThread.Start();
            }
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
                        ConnectController();
                    }));
            }
        }

        public void ConnectController()
        {
            if (!Controller.IsConnected)
                Controller = new Controller(UserIndex.One);
            if (Controller.IsConnected)
            {
                UpdateThread = new Thread(new ThreadStart(UpdateState));
                UpdateThread.Start();
            }
        }

        public void UpdateState()
        {
            while(Controller.IsConnected)
            {
                State currentState = Controller.GetState();
                var buttons = currentState.Gamepad.Buttons;

                Outputs["JoystickLeftX"].Value = (double)currentState.Gamepad.LeftThumbX / 32768;
                Outputs["JoystickLeftY"].Value = (double)currentState.Gamepad.LeftThumbY / 32768;
                Outputs["JoystickRightX"].Value = (double)currentState.Gamepad.RightThumbX / 32768;
                Outputs["JoystickRightY"].Value = (double)currentState.Gamepad.RightThumbY / 32768;
                Outputs["TriggerLeft"].Value = (double)currentState.Gamepad.LeftTrigger / 255;
                Outputs["TriggerRight"].Value = (double)currentState.Gamepad.RightTrigger / 255;
                if(buttons == GamepadButtonFlags.A)
                    Outputs["ButtonA"].Value = 1;
                else
                    Outputs["ButtonA"].Value = 0;
                if (buttons == GamepadButtonFlags.B)
                    Outputs["ButtonB"].Value = 1;
                else
                    Outputs["ButtonB"].Value = 0;
                if (buttons == GamepadButtonFlags.X)
                    Outputs["ButtonX"].Value = 1;
                else
                    Outputs["ButtonX"].Value = 0;
                if (buttons == GamepadButtonFlags.Y)
                    Outputs["ButtonY"].Value = 1;
                else
                    Outputs["ButtonY"].Value = 0;

            }
        }
    }
}

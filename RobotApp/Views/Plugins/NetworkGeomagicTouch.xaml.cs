using GalaSoft.MvvmLight.Command;
using RobotApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using RobotApp.Views.Plugins.NetworkGeomagicTouchFiles;
using RobotApp.ViewModel.Plugins;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for NetworkGeomagicTouchConfigurationView.xaml
    /// </summary>
    public partial class NetworkGeomagicTouch : PluginBase
    {
        
        NetworkGeomagicTouchViewModel networkgeomagicTouchViewModel;
        public SocketMessage forceMessage = new SocketMessage();

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["FxLeft"].UniqueID, (message) =>
            {
                forceMessage.XForceLeft = message.Value;
                networkgeomagicTouchViewModel.talkerSocket.sendData(forceMessage);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["FyLeft"].UniqueID, (message) =>
            {
                forceMessage.YForceLeft = message.Value;
                networkgeomagicTouchViewModel.talkerSocket.sendData(forceMessage);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["FzLeft"].UniqueID, (message) =>
            {
                forceMessage.ZForceLeft = message.Value;
                networkgeomagicTouchViewModel.talkerSocket.sendData(forceMessage);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["FxRight"].UniqueID, (message) =>
            {
                forceMessage.XForceRight = message.Value;
                networkgeomagicTouchViewModel.talkerSocket.sendData(forceMessage);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["FyRight"].UniqueID, (message) =>
            {
                forceMessage.YForceRight = message.Value;
                networkgeomagicTouchViewModel.talkerSocket.sendData(forceMessage);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["FzRight"].UniqueID, (message) =>
            {
                forceMessage.ZForceRight = message.Value;
                networkgeomagicTouchViewModel.talkerSocket.sendData(forceMessage);
            });

            base.PostLoadSetup();
        }


        /// <summary>
        /// Constructs a new NetworkGeomagicTouch View Model
        /// </summary>
        public NetworkGeomagicTouch()
        {
            networkgeomagicTouchViewModel = new NetworkGeomagicTouchViewModel();
            this.DataContext = this;

            InitializeComponent();

            forceMessage.MessageType = "ForceData";

            //left omni outputs
            Outputs.Add("XLeft", new OutputSignalViewModel("Left X Position"));
            Outputs.Add("YLeft", new OutputSignalViewModel("Left Y Position"));
            Outputs.Add("ZLeft", new OutputSignalViewModel("Left Z Position"));
            Outputs.Add("Theta1Left", new OutputSignalViewModel("Left Gimbal Theta 1"));
            Outputs.Add("Theta2Left", new OutputSignalViewModel("Left Gimbal Theta 2"));
            Outputs.Add("Theta3Left", new OutputSignalViewModel("Left Gimbal Theta 3"));
            Outputs.Add("InkwellLeft", new OutputSignalViewModel("Left Inkwell Switch"));
            Outputs.Add("Button1Left", new OutputSignalViewModel("Left Button 1"));
            Outputs.Add("Button2Left", new OutputSignalViewModel("Left Button 2"));
            // left omni inputs
            Inputs.Add("FxLeft", new ViewModel.InputSignalViewModel("Left Force X", this.InstanceName));
            Inputs.Add("FyLeft", new ViewModel.InputSignalViewModel("Left Force Y", this.InstanceName));
            Inputs.Add("FzLeft", new ViewModel.InputSignalViewModel("Left Force Z", this.InstanceName));

            //right omni outputs
            Outputs.Add("XRight", new OutputSignalViewModel("Right X Position"));
            Outputs.Add("YRight", new OutputSignalViewModel("Right Y Position"));
            Outputs.Add("ZRight", new OutputSignalViewModel("Right Z Position"));
            Outputs.Add("Theta1Right", new OutputSignalViewModel("Right Gimbal Theta 1"));
            Outputs.Add("Theta2Right", new OutputSignalViewModel("Right Gimbal Theta 2"));
            Outputs.Add("Theta3Right", new OutputSignalViewModel("Right Gimbal Theta 3"));
            Outputs.Add("InkwellRight", new OutputSignalViewModel("Right Inkwell Switch"));
            Outputs.Add("Button1Right", new OutputSignalViewModel("Right Button 1"));
            Outputs.Add("Button2Right", new OutputSignalViewModel("Right Button 2"));
            // right omni inputs
            Inputs.Add("FxRight", new ViewModel.InputSignalViewModel("Right Force X", this.InstanceName));
            Inputs.Add("FyRight", new ViewModel.InputSignalViewModel("Right Force Y", this.InstanceName));
            Inputs.Add("FzRight", new ViewModel.InputSignalViewModel("Right Force Z", this.InstanceName));

            //plugin name
            TypeName = "Network Geomagic Touch";

            PostLoadSetup();
        }

        public System.Timers.Timer Timer
        {
            get
            {
                return networkgeomagicTouchViewModel.Timer;
            }
            set
            {
                networkgeomagicTouchViewModel.Timer = value;
                this.RaisePropertyChanged("Timer");
            }
        }
        public int IdOfUserWithControl
        {
            get
            {
                return networkgeomagicTouchViewModel.IdOfUserWithControl;
            }
            set
            {
                networkgeomagicTouchViewModel.IdOfUserWithControl = value;
                this.RaisePropertyChanged("IdOfUserWithControl");
            }
        }

        public MTObservableCollection<ConnectedUser> ConnectedUsers
        {
            get
            {
                return networkgeomagicTouchViewModel.ConnectedUsers;
            }
            set
            {
                networkgeomagicTouchViewModel.ConnectedUsers = value;
                this.RaisePropertyChanged("ConnectedUsers");
            }
        }

        public bool IsStartConsoleButtonEnabled
        {
            get
            {
                return networkgeomagicTouchViewModel.IsStartConsoleButtonEnabled;
            }
            set
            {
                networkgeomagicTouchViewModel.IsStartConsoleButtonEnabled = value;
                this.RaisePropertyChanged("IsStartConsoleButtonEnabled");
            }
        }
        public void testButtonClick(object sender, RoutedEventArgs e)
        {
            networkgeomagicTouchViewModel.SomeoneIsConnecting("129.93.6.91", "extraUser");
        }

        public void UserListButtonClick(object sender, RoutedEventArgs e)
        {
            ConnectedUser cu = (ConnectedUser)((Button)sender).DataContext;
            this.IdOfUserWithControl = cu.MyNumber;
            //networkgeomagicTouchViewModel.ConnectedUsers.Single(s => s.Name.Equals(cu.Name)).HasControl = true;
            
            this.ConnectedUsers.Single(s => s.Name.Equals(cu.Name)).HasControl = true;
            //foreach (var user in networkgeomagicTouchViewModel.ConnectedUsers)
            foreach(var user in this.ConnectedUsers)
            {
                if (user.Name != cu.Name)
                {
                    user.HasControl = false;
                }
            }
            networkgeomagicTouchViewModel.viewShouldUpdate = true;
            this.UpdateLayout();
        }

        private RelayCommand startButtonCommand;
        public RelayCommand StartButtonCommand
        {
            get
            {
                return startButtonCommand
                    ?? (startButtonCommand = new RelayCommand(
                    () =>
                    {
                        //do whatever in here for start button click
                        //for now just start the timer
                        //do whatever in here for start button click
                        //for now just start the timer
                        networkgeomagicTouchViewModel.StartButtonCommand();

                        Timer = new System.Timers.Timer();
                        Timer.Elapsed += Timer_Elapsed;
                        Timer.Interval = 5;
                        Timer.Start();
                    }));
            }
        }
        void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (networkgeomagicTouchViewModel.viewShouldUpdate)
            {
                this.RaisePropertyChanged("IsStartConsoleButtonEnabled");
                this.RaisePropertyChanged("ConnectedUsers");
                networkgeomagicTouchViewModel.viewShouldUpdate = false;
                try
                {
                    this.UpdateLayout();
                }
                catch
                {

                }
            }

            
            /////////////////////////////////////////////////////////////////show Left Omni////////////////////////////////////////////////
            Outputs["XLeft"].Value = networkgeomagicTouchViewModel.listenerSockets.ElementAt(IdOfUserWithControl).SocketMessage.XOmniLeft;
            Outputs["YLeft"].Value = networkgeomagicTouchViewModel.listenerSockets.ElementAt(IdOfUserWithControl).SocketMessage.YOmniLeft;
            Outputs["ZLeft"].Value = networkgeomagicTouchViewModel.listenerSockets.ElementAt(IdOfUserWithControl).SocketMessage.ZOmniLeft;

            Outputs["Theta1Left"].Value = networkgeomagicTouchViewModel.listenerSockets.ElementAt(IdOfUserWithControl).SocketMessage.Gimbal1OmniLeft;
            Outputs["Theta2Left"].Value = networkgeomagicTouchViewModel.listenerSockets.ElementAt(IdOfUserWithControl).SocketMessage.Gimbal2OmniLeft;
            Outputs["Theta3Left"].Value = networkgeomagicTouchViewModel.listenerSockets.ElementAt(IdOfUserWithControl).SocketMessage.Gimbal3OmniLeft;

            //add logic here for button state due to binary representation of button1 and button2
            double LeftButtonState = networkgeomagicTouchViewModel.listenerSockets.ElementAt(IdOfUserWithControl).SocketMessage.ButtonsLeft;
            if (LeftButtonState == 0)
            {
                Outputs["Button1Left"].Value = 0;
                Outputs["Button2Left"].Value = 0;

            }
            else if (LeftButtonState == 1)
            {
                Outputs["Button1Left"].Value = 1;
                Outputs["Button2Left"].Value = 0;
            }
            else if (LeftButtonState == 2)
            {
                Outputs["Button1Left"].Value = 0;
                Outputs["Button2Left"].Value = 1;
            }
            else if (LeftButtonState == 3)
            {
                Outputs["Button1Left"].Value = 1;
                Outputs["Button2Left"].Value = 1;
            }
            Outputs["InkwellLeft"].Value = networkgeomagicTouchViewModel.listenerSockets.ElementAt(IdOfUserWithControl).SocketMessage.InkwellLeft;

            //////////////////////////////////////////////////////////////////show Right Omni/////////////////////////////////////////////////
            Outputs["XRight"].Value = networkgeomagicTouchViewModel.listenerSockets.ElementAt(IdOfUserWithControl).SocketMessage.XOmniRight;
            Outputs["YRight"].Value = networkgeomagicTouchViewModel.listenerSockets.ElementAt(IdOfUserWithControl).SocketMessage.YOmniRight;
            Outputs["ZRight"].Value = networkgeomagicTouchViewModel.listenerSockets.ElementAt(IdOfUserWithControl).SocketMessage.ZOmniRight;

            Outputs["Theta1Right"].Value = networkgeomagicTouchViewModel.listenerSockets.ElementAt(IdOfUserWithControl).SocketMessage.Gimbal1OmniRight;
            Outputs["Theta2Right"].Value = networkgeomagicTouchViewModel.listenerSockets.ElementAt(IdOfUserWithControl).SocketMessage.Gimbal2OmniRight;
            Outputs["Theta3Right"].Value = networkgeomagicTouchViewModel.listenerSockets.ElementAt(IdOfUserWithControl).SocketMessage.Gimbal3OmniRight;

            //add logic here
            double RightButtonState = networkgeomagicTouchViewModel.listenerSockets.ElementAt(IdOfUserWithControl).SocketMessage.ButtonsRight;
            if (RightButtonState == 0)
            {
                Outputs["Button1Right"].Value = 0;
                Outputs["Button2Right"].Value = 0;

            }
            else if (RightButtonState == 1)
            {
                Outputs["Button1Right"].Value = 1;
                Outputs["Button2Right"].Value = 0;
            }
            else if (RightButtonState == 2)
            {
                Outputs["Button1Right"].Value = 0;
                Outputs["Button2Right"].Value = 1;
            }
            else if (RightButtonState == 3)
            {
                Outputs["Button1Right"].Value = 1;
                Outputs["Button2Right"].Value = 1;
            }
            Outputs["InkwellRight"].Value = networkgeomagicTouchViewModel.listenerSockets.ElementAt(IdOfUserWithControl).SocketMessage.InkwellRight;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using RobotApp.Views.Plugins.NetworkGeomagicTouchFiles;
using RobotApp.Views.Plugins;

namespace RobotApp.ViewModel.Plugins
{
    public class NetworkGeomagicTouchViewModel : PluginViewModelBase
    {
        public System.Timers.Timer timer;
        public TalkerSocket talkerSocket;
        public List<ListenerSocket> listenerSockets = new List<ListenerSocket>();
        public bool viewShouldUpdate;

        int MyUnderlyingListenerPort = 12000;
        int ClientUnderlyingListenerPort = 11000;
        public NetworkGeomagicTouchViewModel()
        {
            this.timer = new System.Timers.Timer();
            this.talkerSocket = new TalkerSocket();
            this.listenerSockets = new List<ListenerSocket>();
            this.isStartConsoleButtonEnabled = true;
            this.connectedUsers = new MTObservableCollection<ConnectedUser>();
            viewShouldUpdate = false;
            this.idOfUserWithControl = 0;
        }

        //private class variables
        private RelayCommand startButtonCommand;
        private bool isStartConsoleButtonEnabled;
        private MTObservableCollection<ConnectedUser> connectedUsers;
        private int idOfUserWithControl;

        //view accessors
        public System.Timers.Timer Timer
        {
            get
            {
                return timer;
            }
            set
            {
                timer = value;
                this.RaisePropertyChanged("Timer");
            }
        }
        public int IdOfUserWithControl
        {
            get
            {
                return this.idOfUserWithControl;
            }
            set
            {
                this.idOfUserWithControl = value;
                this.RaisePropertyChanged("IdOfUserWithControl");
            }
        }

        public MTObservableCollection<ConnectedUser> ConnectedUsers
        {
            get
            {
                return connectedUsers;
            }
            set
            {
                connectedUsers = value;
                this.RaisePropertyChanged("ConnectedUsers");
            }
        }

        
        public bool IsStartConsoleButtonEnabled
        {
            get
            {
                return isStartConsoleButtonEnabled;
            }
            set
            {
                isStartConsoleButtonEnabled = value;
                this.RaisePropertyChanged("IsStartConsoleButtonEnabled");
            }
        }
        
        public void StartButtonCommand()
        {
            ListenerSocket tmp = new ListenerSocket(MyUnderlyingListenerPort, this);
            listenerSockets.Add(tmp);
            isStartConsoleButtonEnabled = false;
            listenerSockets.ElementAt(0).StartListening();
            viewShouldUpdate = true;
        }

        public void SomeoneIsConnecting(string IPaddress, string Name)
        {
            ConnectedUser tmp = new ConnectedUser();
            tmp.Ip = IPaddress;
            tmp.Name = Name;
            tmp.MyNumber = connectedUsers.Count + 1;
            tmp.HasControl = false;
            MTObservableCollection<ConnectedUser> newList = new MTObservableCollection<ConnectedUser>();
            newList = connectedUsers;
            newList.Add(tmp);
            connectedUsers = new MTObservableCollection<ConnectedUser>();
            connectedUsers = newList;
            this.RaisePropertyChanged("ConnectedUsers");
            SocketMessage socketMessage = new SocketMessage();
            socketMessage.Port = (listenerSockets.Count + 12001).ToString();
            socketMessage.MessageType = "PortInformation";
            talkerSocket = new TalkerSocket(IPaddress, ClientUnderlyingListenerPort);
            ListenerSocket newListenSocket = new ListenerSocket(Int32.Parse(socketMessage.Port), this);
            listenerSockets.Add(newListenSocket);
            listenerSockets.ElementAt(listenerSockets.Count - 1).StartListening();
            talkerSocket.sendData(socketMessage);
            viewShouldUpdate = true;
        }
    }
}

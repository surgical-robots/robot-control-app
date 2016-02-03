using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Threading;
using RobotApp.ViewModel.Plugins;

namespace RobotApp.Views.Plugins.NetworkGeomagicTouchFiles
{
    public class ListenerSocket
    {
        IPEndPoint localEndPoint;
        UdpClient udpListener = new UdpClient();
        SocketMessage _socketMessage = new SocketMessage();
        string _nameOfAttachedClient = "";
        NetworkGeomagicTouchViewModel masterViewModel = new NetworkGeomagicTouchViewModel();
        int _port;

        public ListenerSocket(int port, NetworkGeomagicTouchViewModel viewModel)
        {
            this._socketMessage = new SocketMessage();
            this._port = port;
            this._nameOfAttachedClient = "";
            this.masterViewModel = viewModel;
        }

        public string NameOfAttachedClient
        {
            private set { this._nameOfAttachedClient = value; }
            get { return this._nameOfAttachedClient; }
        }
        public SocketMessage SocketMessage
        {
            private set { this._socketMessage = value; }
            get { return this._socketMessage; }
        }

        public System.Timers.Timer Timer {get;set;}

        public void StartListening()
        {
            localEndPoint = new IPEndPoint(IPAddress.Parse(GetIP()), this._port);
            udpListener = new UdpClient(localEndPoint);
            Thread Listener = new Thread(new ThreadStart(Listen));
            Listener.Start();
            //Timer = new System.Timers.Timer();
            
            //Timer.Elapsed += Listen;
            //Timer.Interval = 5;
            //Timer.Start();

        }

        //void Listen(object sender, System.Timers.ElapsedEventArgs e)
        void Listen()
        {
            while (true)
            {
                var remoteEP = new IPEndPoint(IPAddress.Any, this._port);
                var data = udpListener.Receive(ref remoteEP); // listen on specified
                //gets ip of sender
                //Console.WriteLine(remoteEP.Address.ToString());
                ProcessSocketMessage(System.Text.Encoding.ASCII.GetString(data));

            }
            //return null SocketMessage for now
            
        }


        //feed this message processor a clean message that has the below function run on it
        //this.jsonIncomingMessage.Text = message;
        //string tmp = jsonIncomingMessage.Text;
        void ProcessSocketMessage(string message)
        {
            SocketMessage recievedObject = JsonConvert.DeserializeObject<SocketMessage>(message);
            if (recievedObject.MessageType == "OmniMessage")
            {
                //update omni
                this.SocketMessage = recievedObject;
            }
            else if (recievedObject.MessageType == "PermissionToConnect")
            {
                masterViewModel.SomeoneIsConnecting(recievedObject.IpAddress, recievedObject.Name);
            }
            else
            {
                //unhandled
            }
        }


        String GetIP()
        {
            String strHostName = Dns.GetHostName();

            // Find host by name
            IPHostEntry iphostentry = Dns.GetHostByName(strHostName);

            // Grab the first IP addresses
            String IPStr = "";
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                IPStr = ipaddress.ToString();
                return IPStr;
            }
            return IPStr;
        }
    }
}

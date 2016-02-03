using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RobotApp.Views.Plugins.NetworkGeomagicTouchFiles
{
    public class TalkerSocket
    {
        IPEndPoint _remoteEndPoint;
        UdpClient udpTalker = new UdpClient();

        public TalkerSocket()
        {

        }
        public TalkerSocket(string IpAddress, int port)
        {
            this._remoteEndPoint = new IPEndPoint(IPAddress.Parse(IpAddress), port);
            udpTalker.Connect(_remoteEndPoint);
        }

        public void sendData(SocketMessage socketMessage)
        {
            string jsonString = JsonConvert.SerializeObject(socketMessage);
            // send data
            byte[] messageToSend = System.Text.Encoding.ASCII.GetBytes(jsonString);
            udpTalker.Send(messageToSend, messageToSend.Length);

        }
    }
}

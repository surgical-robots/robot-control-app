using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Net;

namespace TelSurge.DataModels
{
    public class SocketMessage
    {
        public OmniPosition OmniPosition { get; set; }
        public OmniPosition PositionOffset { get; set; }

        //other message type information
        public bool inControl { get; set; }
        public string IPAddress { get; set; }
        public double[] Forces { get; set; } // {LX, LY, LZ, RX, RY, RZ}
        public bool sendFreezeCmd { get; set; }
        public bool isFrozen { get; set; }
        public bool hasOmnis { get; set; }
        public bool EmergencySwitch { get; set; }
        public bool ClearMarkingsReq { get; set; }

        //name of connectee
        public string Name { get; set; }
        public string Port { get; set; }

        public List<string> ConnectedClients { get; set; }



        public SocketMessage()
        {
            isFrozen = false;
            EmergencySwitch = false;
            ClearMarkingsReq = false;
        }

        public SocketMessage(string name, string ipAddr)
        {
            this.Name = name;
            this.IPAddress = ipAddr;
            isFrozen = false;
            EmergencySwitch = false;
            ClearMarkingsReq = false;
        }
    }
}

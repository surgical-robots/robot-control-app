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
        public User User { get; set; }
        public OmniPosition Forces { get; set; } 
        public bool sendToggleFrozen { get; set; }
        public bool EmergencySwitch { get; set; }
        public bool ClearMarkingsReq { get; set; }

        //name of connectee
        public string Port { get; set; }

        public Surgery Surgery { get; set; }


        public SocketMessage(Surgery Surgery, User User)
        {
            this.Surgery = Surgery;
            this.User = User;
            EmergencySwitch = false;
            ClearMarkingsReq = false;
        }
    }
}

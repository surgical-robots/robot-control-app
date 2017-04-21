using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RobotControl
{
    class SocketTransport : IPacketTransport
    {

        TcpClient client;

        public string IpAddress { get; set; }

        public bool IsOpen
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool RequestData
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool SendData
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public event IPacketTransportDataReceived DataReceived;
        public event IPacketTransportUpdateSetpoints UpdateSetpoints;

        public void AddAddress(uint address)
        {
            throw new NotImplementedException();
        }

        public void RemoveAddress(uint address)
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] data)
        {
            
        }
    }
}

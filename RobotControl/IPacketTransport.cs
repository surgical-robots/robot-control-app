using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotControl
{
    public delegate void IPacketTransportDataReceived(byte[] data);

    public delegate void IPacketTransportUpdateSetpoints();

    /// <summary>
    /// This interface describes a generic packet transport to the 
    /// individual motor control boards. This allows protocol-agonstic communication
    /// over serial, 
    /// 
    /// /IP, USB HID, RF, or any other communication interface that
    /// can be modeled as a packet device
    /// </summary>
    public interface IPacketTransport
    {
        /// <summary>
        /// Gets or sets a value indicating whether the transport is open and able 
        /// to send data. This should be ideompotent! 
        /// </summary>
        bool IsOpen { get; set; }
        /// <summary>
        /// Send data through the transport. Before sending, this should open the 
        /// communications channel if not already open.
        /// </summary>
        /// <param name="data">Data to send</param>
        void Send(byte[] data);

        /// <summary>
        /// This event occurs whenever data is received from the communication transport.
        /// </summary>
        event IPacketTransportDataReceived DataReceived;

        /// <summary>
        /// This event occurs whenever data is received from the communication transport.
        /// </summary>
        event IPacketTransportUpdateSetpoints UpdateSetpoints;

        void AddAddress(uint address);

        void RemoveAddress(uint address);

        bool RequestData { get; set; }

        bool SendData { get; set; }
        
//        long TimeoutCount { get; set; }
    }
}

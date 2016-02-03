using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Diagnostics;
namespace RobotControl
{
    /// <summary>
    /// This class is used for direct communication with the robot motor drivers using RS485 instantiated on this computer as a COM port.
    /// </summary>
    public class SerialPortPacketTransport : IPacketTransport
    {
        byte[] TxBuffer = new byte[32];
        byte[] RxBuffer = new byte[32];

        SerialPort port;

        /// <summary>
        /// The serial port to use
        /// </summary>
        public SerialPort Port { 
            get
            {
                return port;
            }
            set
            {
                // We're not changing the port. Don't do anything.
                if (port == value)
                    return;
                bool wasOpen = false;
                // if we're changing the port, make sure to remove the event handler and close it
                if(port != null)
                {
                    port.DataReceived -= Port_DataReceived;
                    if (port.IsOpen)
                    {
                        wasOpen = true;
                        port.Close();
                    }
                        
                }
                port = value;
                port.BaudRate = 115200;
                port.DataReceived += Port_DataReceived;
                if (wasOpen)
                    port.Open();
            }
        }
        /// <summary>
        /// Initializes a new instance with the given com port.
        /// </summary>
        /// <param name="port"></param>
        public SerialPortPacketTransport(SerialPort port)
        {
            Port = port;
        }

        
        async void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesToRead;
            int startByte;
            // Thread.Sleep(1); // wait for messages to finish sending
            while(Port.BytesToRead > 0)
            {
                //await Port.BaseStream.ReadAsync(RxBuffer, 0, 1);
                Port.Read(RxBuffer, 0, 1);
                // Look for start byte
                if( RxBuffer[0] == 200)
                {
                    bytesToRead = Port.BytesToRead > (RxBuffer.Length - 1) ? (RxBuffer.Length - 1) : Port.BytesToRead;
                    //await Port.BaseStream.ReadAsync(RxBuffer, 1, bytesToRead);
                    Port.Read(RxBuffer, 1, bytesToRead);
                    //Port.DiscardInBuffer();
                    break;
                }
            }

            //bytesToRead = Port.BytesToRead > RxBuffer.Length ? RxBuffer.Length : Port.BytesToRead;
            //Port.Read(RxBuffer, 0, bytesToRead);
            //Port.DiscardInBuffer(); // throw away anything left over
           // Debug.WriteLine("Data received: " + new SoapHexBinary(RxBuffer).ToString());

            if (RxBuffer[0] == 200 && DataReceived != null)
                DataReceived(RxBuffer);
        }

        /// <summary>
        /// Gets or sets the state of the communication channel.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return Port.IsOpen;
            }
            set
            {
                // The "set" property should be idempotent, but we can't call Open() or Close() 
                // multiple times, so check to see what the current state of the port is first.
                if(value != Port.IsOpen)
                {
                    if (value == true)
                        Port.Open();
                    else
                        Port.Close();
                }
            }
        }

        /// <summary>
        /// Send the data to the robot.
        /// </summary>
        /// <param name="data">The data to send to the robot</param>
        /// <remarks>
        /// <para>
        /// This function will attempt to open the serial port if not already open. It will throw an exception if 
        /// data couldn't be sent. 
        /// </para>
        /// <para>
        /// The data packet will be preceeded by the DABAD000 sync word, and will be padded to fill the 32-byte 
        /// packet length defined by the protocol.
        /// </para>
        /// </remarks>
        public async void Send(byte[] data)
        {
            // open port if it's not already open
            IsOpen = true;
            // check again before we actually send
            if(IsOpen)
            {
                if (data.Length > 16)
                    throw new ArgumentException("Data length exceeds 16-byte allowed size");
                // Serial sync word is DABAD000
                TxBuffer[0] = 0xDA;
                TxBuffer[1] = 0xBA;
                TxBuffer[2] = 0xD0;
                TxBuffer[3] = 0x00;
                Array.Copy(data, 0, TxBuffer, 4, data.Length);
                //Port.Write(TxBuffer, 0, TxBuffer.Length);
                await Port.BaseStream.WriteAsync(TxBuffer, 0, TxBuffer.Length);
            }
        }

        public event IPacketTransportDataReceived DataReceived;
    }
}

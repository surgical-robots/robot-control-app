using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using RobotControl;

namespace RobotControl
{
    public class SerialPortPacketTransport : IPacketTransport
    {
        private byte[] TxBuffer = new byte[32];
        private byte[] RxBuffer = new byte[32];
        private SerialPortPacketTransport sppt;
        private int controllerCount = 0;
        private bool sending = false;
        private bool requestData = false;
        private bool sendData = false;
        private bool waitingForResponse = false;

        public SerialPort Port = new SerialPort();
        public bool IsOpen { get { return Port.IsOpen; } set { if (value == true) Port.Open(); else Port.Close(); } }
	    public bool RequestData { get{ return requestData; } set{ requestData = value; }}
	    public bool SendData { get{ return sendData; } set{ sendData = value; }}
	    public event IPacketTransportDataReceived DataReceived;
	    public event IPacketTransportUpdateSetpoints UpdateSetpoints;
	    public int[] Addresses = new int[10];
	    public Object lockObject = new Object();
	    public BackgroundWorker serialOpsThread;
	    public Robot robot;

        public SerialPortPacketTransport(String comPort, Robot robotIn)
        {
	        sppt = this;
	        bool wasOpen;
	        robot = robotIn;
	        SendData = true;

	        // Serial sync word is DABAD000
	        TxBuffer[0] = 0xDA;
	        TxBuffer[1] = 0xBA;
	        TxBuffer[2] = 0xD0;
	        TxBuffer[3] = 0x00;

	        // if setting to same port, do nothing
	        if (Port.PortName == comPort)
		        return;
	        // if port is already open, close port
	        if (IsOpen)
	        {
		        Port.Close();
                if(serialOpsThread != null)
                    serialOpsThread.CancelAsync();
		        wasOpen = true;
	        }

	        Port.PortName = comPort;
	        Port.BaudRate = 230400;
	        Port.Parity = Parity.None;
	        Port.DataBits = 8;
	        Port.StopBits = StopBits.One;
	        Port.ReadTimeout = 25;
	        Port.WriteTimeout = 25;
	        Port.Open();
            serialOpsThread = new BackgroundWorker();
            serialOpsThread.DoWork += serialOpsThread_DoWork;
            serialOpsThread.RunWorkerAsync();
        }

        void serialOpsThread_DoWork(object sender, DoWorkEventArgs e)
        {
            SerialOps();
        }

        public void Send(byte[] data)
        {
	        // check again before we actually send
	        if (IsOpen)
	        {
		        sending = true;

		        // Make sure that the message is the right length
		        if (data.Length > 28)
		        {
                    for (int i = 0; i < 28; i++)
                    {
                        TxBuffer[i + 4] = data[i];
                    }
                    throw new ArgumentException("Data length exceeds 28-byte allowed size");
		        }
		        else
		        {
			        for (int i = 0; i < data.Length; i++)
			        {
				        TxBuffer[i + 4] = data[i];
			        }
		        }
		        // aquire com lock
                lock (lockObject)
                {
                    try { Port.Write(TxBuffer, 0, (data[0] + 4)); }
                    catch (TimeoutException) { }
                    sending = false;
                }
	        }
        }

        public void AddAddress(uint address)
        {
	        Addresses[controllerCount] = (int)address;
	        controllerCount++;
        }

        public void RemoveAddress(uint address)
        {
	        int removeNum = 0;
	        bool found = false;

	        for (int i = 0; i < controllerCount; i++)
	        {
		        if (Addresses[i] == address)
		        {
			        removeNum = i;
			        found = true;
			        break;
		        }
	        }
	
	        if (found)
	        {
		        for (int j = removeNum; j < controllerCount; j++)
		        {
			        Addresses[j] = Addresses[j + 1];
		        }
		        controllerCount--;
	        }
        }

        void SerialOps()
        {
	        double setPoint;
	        int positionOne;
	        int positionTwo;
	        int position;
	        int byteLimit;
	        byte[] address = new byte[4];
	        byte[] sendMsg = new byte[32];
	        // Serial sync word is DABAD000
	        sendMsg[0] = 0xDA;
	        sendMsg[1] = 0xBA;
	        sendMsg[2] = 0xD0;
	        sendMsg[3] = 0x00;

	        int bytesToRead;
	        int timeout;
	
	        while (true)
	        {
		        if (!SendData && (robot.SerialControllers.Values.Count != null))
		        {
			        foreach (Controller controller in robot.SerialControllers.Values)
			        {
				        byteLimit = 4;
				        if (controller.GetHalls) byteLimit += 8;
				        if (controller.GetPots) byteLimit += 4;
				        if (controller.GetCurrent) byteLimit += 4;

				        if (controller.SendNum != 0)
				        {
					        address = BitConverter.GetBytes(controller.Id);
					        //setPoint = motor->EncoderClicksPerRevolution / (360) * (motor->Angle - motor->OffsetAngle);
					        positionOne = (int)Math.Round(controller.Motor1Setpoint);
					        positionTwo = (int)Math.Round(controller.Motor2Setpoint);
					        sendMsg[4] = (Byte)14;
					        for (int j = 0; j < 4; j++)
					        {
						        sendMsg[j + 5] = address[j];
					        }
					        if (requestData && (controller.GetHalls || controller.GetPots || controller.GetCurrent))
						        sendMsg[9] = (Byte)JointCommands.SetPosGetData;
					        else
						        sendMsg[9] = (Byte)JointCommands.DoubleMoveTo;
					        Array.Copy(BitConverter.GetBytes(positionOne), 0, sendMsg, 10, 4);
					        Array.Copy(BitConverter.GetBytes(positionTwo), 0, sendMsg, 14, 4);

                            lock(lockObject)
                            {
					            try { Port.Write(sendMsg, 0, (sendMsg[4] + 4)); }
					            catch (TimeoutException) {}
					            if (requestData && (controller.GetHalls || controller.GetPots || controller.GetCurrent))
					            {
						            waitingForResponse = true;
						            timeout = 100000;
						            // wait for data
						            while ((Port.BytesToRead < byteLimit) && (--timeout > 0));
						            while (Port.BytesToRead >= byteLimit)
						            {
							            Port.Read(RxBuffer, 0, 1);
							            if (RxBuffer[0] == 200)
							            {
								            bytesToRead = Port.BytesToRead > (RxBuffer.Length - 1) ? (RxBuffer.Length - 1) : Port.BytesToRead;
								            Port.Read(RxBuffer, 1, bytesToRead);
								            DataReceived(RxBuffer);
								            waitingForResponse = false;
								            break;
							            }
						            }
					            }                            
                            }

				        }
				        else if (requestData && (controller.GetHalls || controller.GetPots || controller.GetCurrent))
				        {
					        address = BitConverter.GetBytes(controller.Id);
					        // message length after DABADOOO
					        sendMsg[4] = (Byte)14;
					        for (int j = 0; j < 4; j++)
					        {
						        sendMsg[j + 5] = address[j];
					        }
					        sendMsg[9] = (Byte)JointCommands.GetStatus;
					        // Aquire com lock
                            lock(lockObject)
                            {
                                try { Port.Write(sendMsg, 0, (sendMsg[4] + 4)); }
					            catch (TimeoutException) {}
					            waitingForResponse = true;
					            timeout = 100000;
					            // wait for data
					            while ((Port.BytesToRead < byteLimit) && (--timeout > 0));
					            while (Port.BytesToRead >= byteLimit)
					            {
						            Port.Read(RxBuffer, 0, 1);
						            if (RxBuffer[0] == 200)
						            {
							            bytesToRead = Port.BytesToRead > (RxBuffer.Length - 1) ? (RxBuffer.Length - 1) : Port.BytesToRead;
							            Port.Read(RxBuffer, 1, bytesToRead);
							            DataReceived(RxBuffer);
							            waitingForResponse = false;
							            break;
						            }
					            }
                            }
				        }
			        }
			        SendData = true;
			        UpdateSetpoints();
		        }
		        if (Port.BytesToRead > 10)
		        {
			        while (Port.BytesToRead > 0)
			        {
				        Port.Read(RxBuffer, 0, 1);
				        if (RxBuffer[0] == 200)
				        {
					        bytesToRead = Port.BytesToRead > (RxBuffer.Length - 1) ? (RxBuffer.Length - 1) : Port.BytesToRead;
					        Port.Read(RxBuffer, 1, bytesToRead);
					        DataReceived(RxBuffer);
					        waitingForResponse = false;
				        }
			        }
		        }
	        }
        }
    }
}

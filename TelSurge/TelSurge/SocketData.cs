using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelSurge.DataModels;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace TelSurge
{
    class SocketData
    {
        private TelSurgeMain mainForm;
        private MarkUp markUp;
        public Queue<SocketMessage> dataBuffer = new Queue<SocketMessage>();
        private System.Diagnostics.Stopwatch dataWatch = new System.Diagnostics.Stopwatch();
        private bool dataAvailable = false;
        private int dataPort = 11001;
        private UdpClient dataListener = null;
        private volatile bool isListeningForData = true;

        public SocketData()
        {

        }
        private void readDataBuffer()
        {
            while (true)
            {
                if (dataBuffer.Count > 0)
                {
                    if (dataWatch.IsRunning)
                    {
                        if (dataWatch.ElapsedMilliseconds >= mainForm.networkDelay)
                        {
                            dataWatch.Reset();
                            //use data
                            if (dataAvailable)
                            {
                                SocketMessage dataMsg = dataBuffer.Dequeue();
                                if (!mainForm.isInControl)
                                {
                                    mainForm.inControlPosition = dataMsg.OmniPosition;
                                    mainForm.inControlIsFrozen = dataMsg.isFrozen;
                                }
                                if (!mainForm.cb_isMaster.Checked && mainForm.isInControl)
                                {
                                    mainForm.setForces(dataMsg.Forces[0], dataMsg.Forces[1], dataMsg.Forces[2], dataMsg.Forces[3], dataMsg.Forces[4], dataMsg.Forces[5]);
                                    if (dataMsg.sendFreezeCmd)
                                        mainForm.freezeCommandReceived();
                                }
                                dataAvailable = false;
                            }
                            mainForm.errorTimer.Start();
                        }
                    }
                    else
                    {
                        //use data
                        if (dataAvailable)
                        {
                            SocketMessage dataMsg = dataBuffer.Dequeue();
                            if (!mainForm.isInControl)
                            {
                                mainForm.inControlPosition = dataMsg.OmniPosition;
                                mainForm.inControlIsFrozen = dataMsg.isFrozen;
                            }
                            if (!mainForm.cb_isMaster.Checked && mainForm.isInControl)
                            {
                                mainForm.setForces(dataMsg.Forces[0], dataMsg.Forces[1], dataMsg.Forces[2], dataMsg.Forces[3], dataMsg.Forces[4], dataMsg.Forces[5]);
                                if (dataMsg.sendFreezeCmd)
                                    mainForm.freezeCommandReceived();
                            }
                            dataAvailable = false;
                        }
                    }
                }
            }
        }
        public void SendData(SocketMessage Message, List<string> SendTo)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                string json = JsonConvert.SerializeObject(Message);
                byte[] arry = Encoding.ASCII.GetBytes(json);
                foreach (string a in SendTo)
                {
                     s.SendTo(arry, new IPEndPoint(IPAddress.Parse(a), dataPort));
                }
            }
            catch (Exception ex)
            {
                mainForm.ShowError(ex.Message, ex.ToString());
            }
        }
        /// <summary>
        /// Serialize and send a SocketMessage to a specific IPAddress
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="addr"></param>
        public void SendDataTo(SocketMessage Message, IPAddress addr)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                string json = JsonConvert.SerializeObject(Message);
                byte[] arry = Encoding.ASCII.GetBytes(json);
                s.SendTo(arry, new IPEndPoint(addr, dataPort));
            }
            catch (Exception ex)
            {
                mainForm.ShowError(ex.Message, ex.ToString());
            }
        }
        
        public void StopListening()
        {
            isListeningForData = false;
        }
        /// <summary>
        /// Begins listening for SocketMessages over dataPort from all IP Addresses
        /// 1. Grab a new thread
        /// 2. Start listening Async
        /// </summary>
        public void StartListening()
        {
            Thread t2 = new Thread(new ThreadStart(listenForData));
            t2.IsBackground = true;
            t2.Start();
        }
        private void listenForData()
        {
            if (dataListener == null)
                dataListener = new UdpClient(dataPort);
            try
            {
                isListeningForData = true;
                dataListener.BeginReceive(new AsyncCallback(dataReceived), null);
            }
            catch (Exception ex)
            {
                mainForm.ShowError(ex.Message, ex.ToString());
            }
        }
        private void dataReceived(IAsyncResult ar)
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, dataPort);
                byte[] arry = dataListener.EndReceive(ar, ref ep);
                string json = Encoding.ASCII.GetString(arry);
                SocketMessage dataMsg = JsonConvert.DeserializeObject<SocketMessage>(json);
                dataBuffer.Enqueue(dataMsg);
                dataAvailable = true;
                if (mainForm.networkDataDelayChanged && mainForm.networkDelay > 0 && !dataWatch.IsRunning)
                {
                    mainForm.networkDataDelayChanged = false;
                    dataWatch.Start();
                }
                if (dataMsg.ClearMarkingsReq)
                {
                    //clear local markings
                    markUp.ClearMarkUp();
                }
            }
            catch (Exception ex)
            {
                //Just log error instead of showing, currently too many non-critical errors
                mainForm.logMessage(ex.Message, ex.ToString(), Logging.StatusTypes.Error);
            }

            mainForm.hasMasterData = true;
            if (isListeningForData)
                listenForData();
        }
    }
}

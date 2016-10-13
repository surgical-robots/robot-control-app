using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using TelSurge.DataModels;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;

namespace TelSurge
{
    public class SocketData
    {
        private TelSurgeMain Main;

        private UdpClient dataListener = null;
        private int dataPort;
        private bool logDataTurnAroundTime = false;
        private Stopwatch turnAroundTimer = new Stopwatch();
        private Queue<SocketMessage> dataBuffer = new Queue<SocketMessage>();
        private Stopwatch dataWatch = new Stopwatch();
        public bool sendToggleFrozen = false;
        private bool hasMasterData = false;
        public bool IsListeningForData { get; set; }

        /*
        private bool isListeningForData = false;
        */
        public SocketData(TelSurgeMain MainForm, int DataPort)
        {
            this.Main = MainForm;
            this.dataPort = DataPort;
            this.IsListeningForData = false;
        }
        public void MasterSendData()
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                foreach (User user in Main.Surgery.ConnectedClients)
                {
                    if (!user.MyIPAddress.Equals(Main.User.MyIPAddress))
                        s.SendTo(CreateMessageToSend(), new IPEndPoint(IPAddress.Parse(user.MyIPAddress), dataPort));
                }
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
        }
        public void SendUDPDataTo(IPAddress Address, byte[] Message)
        {
            try
            {
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                s.SendTo(Message, new IPEndPoint(Address, dataPort));
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
        }
        public void SendTCPDataTo(TcpClient Client, byte[] Message)
        {
            try
            {
                Stream s = Client.GetStream();
                s.Write(Message, 0, Message.Length);
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
        }
        public byte[] CreateMessageToSend() 
        {
            SocketMessage sm = new SocketMessage(Main.Surgery, Main.User);
            byte[] arry = { };
            try
            {
                sm.ClearMarkingsReq = Main.Markup.ClearMarkingsReq;
                Main.Markup.ClearMarkingsReq = false;
                sm.sendToggleFrozen = sendToggleFrozen;
                sendToggleFrozen = false;
                if (Main.User.IsMaster)
                {
                    sm.Forces = Main.HapticForces;
                }
                sm.Surgery = Main.Surgery;
                arry = SerializeObject(sm);
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
            return arry;
        }
        public static byte[] SerializeObject<T>(T Object) {
            string json = JsonConvert.SerializeObject(Object);
            return Encoding.ASCII.GetBytes(json);
        }
        public static T DeserializeObject<T>(byte[] Arry)
        {
            T obj = default(T);
            string json = Encoding.ASCII.GetString(Arry);
            try
            {
                obj = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, ex.ToString());
            }
            return obj;
        }
        public void ListenForData()
        {
            if (dataListener == null)
                dataListener = new UdpClient(dataPort);
            try
            {
                dataListener.BeginReceive(new AsyncCallback(dataReceived), null);
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
        }
        private void dataReceived(IAsyncResult AsyncResult)
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, dataPort);
                byte[] arry = dataListener.EndReceive(AsyncResult, ref ep);
                //Turnaround Time
                if (logDataTurnAroundTime)
                {
                    if (turnAroundTimer.IsRunning)
                    {
                        turnAroundTimer.Stop();
                        Logging.WriteToFile(turnAroundTimer.ElapsedMilliseconds.ToString(), "TurnAroundTimes.txt");
                    }
                    turnAroundTimer.Restart();
                }


                SocketMessage dataMsg = DeserializeObject<SocketMessage>(arry);
                if(dataMsg != null)
                {
                    Main.Surgery.Merge(dataMsg.Surgery, Main.User.IsInControl, Main.User.IsMaster);
                    if (Main.User.IsMaster)
                        Main.messageCount++;
                    if (dataMsg.sendToggleFrozen)
                    {
                        if (Main.User.IsFrozen)
                            Main.UnFreeze();
                        else
                            Main.Freeze();
                    }
                    if (Main.User.IsInControl && dataMsg.Forces != null)
                    {
                        Main.SetForceX(dataMsg.Forces.LeftX, true);
                        Main.SetForceY(dataMsg.Forces.LeftY, true);
                        Main.SetForceZ(dataMsg.Forces.LeftZ, true);
                        Main.SetForceX(dataMsg.Forces.RightX, false);
                        Main.SetForceY(dataMsg.Forces.RightY, false);
                        Main.SetForceZ(dataMsg.Forces.RightZ, false);
                    }
                    //dataBuffer.Enqueue(dataMsg);
                    //dataAvailable = true;
                    //if (networkDataDelayChanged && Main.User.NetworkDelay > 0 && !dataWatch.IsRunning)
                    //{
                    //    networkDataDelayChanged = false;
                    //    dataWatch.Start();
                    //}
                    if (dataMsg.ClearMarkingsReq)
                    {
                        Main.ClearMarkup();
                    }
                }
            }
            catch (Exception ex)
            {
                //Just log error instead of showing, currently too many non-critical errors
                Main.LogMessage(ex.Message, ex.ToString(), Logging.StatusTypes.Error);
            }

            hasMasterData = true;
            if (IsListeningForData)
                ListenForData();
        }
    }
}

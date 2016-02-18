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

namespace TelSurge
{
    class SocketData
    {
        private TelSurgeMain Main;
        private User User;
        private Surgery Surgery;
        private Markup Markup;

        private UdpClient dataListener = null;
        private int dataPort;
        private bool logDataTurnAroundTime = false;
        private Stopwatch turnAroundTimer = new Stopwatch();
        private Queue<SocketMessage> dataBuffer = new Queue<SocketMessage>();
        private bool dataAvailable = false;
        private Stopwatch dataWatch = new Stopwatch();
        private bool networkDataDelayChanged = false;
        private bool hasMasterData = false;
        public bool IsListeningForData { get; set; }

        /*
        private bool isListeningForData = false;
        */
        public SocketData(TelSurgeMain MainForm, User User, Surgery Surgery, Markup Markup, int DataPort)
        {
            this.Main = MainForm;
            this.User = User;
            this.Surgery = Surgery;
            this.Markup = Markup;
            this.dataPort = DataPort;
            this.IsListeningForData = false;
        }
        public void MasterSendData()
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                foreach (string a in Main.ConnectedClients)
                {
                    if (!a.Equals(User.MyIPAddress))
                        s.SendTo(createMessageToSend("Master"), new IPEndPoint(IPAddress.Parse(a), dataPort));
                }
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
        }
        public void SendDataTo(IPAddress Address, byte[] Message)
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
        private byte[] createMessageToSend(string FromName) 
        {
            SocketMessage sm = new SocketMessage(Surgery, FromName, User.MyIPAddress);
            byte[] arry = { };
            try
            {
                if (User.IsFrozen)
                    sm.OmniPosition = User.FrozenPosition;
                else
                    sm.OmniPosition = Surgery.InControlPosition;
                sm.isFrozen = User.IsFrozen;
                sm.ClearMarkingsReq = Markup.ClearMarkingsReq;
                Markup.ClearMarkingsReq = false;
                //sm.sendFreezeCmd = sendFreezeCmd;
                //sendFreezeComd = false;
                if (User.IsMaster)
                {
                    sm.Forces = Main.HapticForces;
                }
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
            string json = Encoding.ASCII.GetString(Arry);
            return JsonConvert.DeserializeObject<T>(json);
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
                dataBuffer.Enqueue(dataMsg);
                dataAvailable = true;
                if (networkDataDelayChanged && User.NetworkDelay > 0 && !dataWatch.IsRunning)
                {
                    networkDataDelayChanged = false;
                    dataWatch.Start();
                }
                if (dataMsg.ClearMarkingsReq)
                {
                    Main.ClearMarkup();
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

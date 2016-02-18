using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelSurge.DataModels;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;

namespace TelSurge
{
    class User
    {
        private TelSurgeMain Main;
        private Surgery Surgery;
        private SocketData SocketData;
        public string MyName { get; set; }
        public string MyIPAddress { get; set; }
        public bool IsFrozen { get; set; }
        public OmniPosition FrozenPosition { get; set; }
        public bool IsMaster { get; set; }
        public int NetworkDelay { get; set; }
        public bool ConnectedToMaster { get; set; }
        public bool IsInControl { get; set; }
        public bool[] externalButtons { get; set; }
        public bool HasOmnis { get; set; }
        private double followingForceConstant = 0.1;
        private double forceMax = 3;
        private int connectionPort;
        /*
        private string myName = "";
        private volatile bool isInControl = false;
        private volatile bool isFrozen = false;
        private volatile bool inControlIsFrozen = false;
        private string myIPAddress;
        public bool isMaster = false;
        public bool isConnectedToMaster = false;
        */

        //Geomagic Touch methods from newphantom.dll
        const string address = "newphantom.dll";

        [DllImport(address, CallingConvention = CallingConvention.Cdecl)]
        public static extern int initAndSchedule(string leftOmni, string rightOmni);
        [DllImport(address, CallingConvention = CallingConvention.Cdecl)]
        public static extern int stopAndDisable();
        [DllImport(address, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lock1();
        [DllImport(address, CallingConvention = CallingConvention.Cdecl)]
        public static extern void lock2();
        [DllImport(address, CallingConvention = CallingConvention.Cdecl)]
        public static extern void unlock1();
        [DllImport(address, CallingConvention = CallingConvention.Cdecl)]
        public static extern void unlock2();
        [DllImport(address, CallingConvention = CallingConvention.Cdecl)]
        public static extern double getX1();
        [DllImport(address, CallingConvention = CallingConvention.Cdecl)]
        public static extern double getY1();
        [DllImport(address, CallingConvention = CallingConvention.Cdecl)]
        public static extern double getZ1();
        [DllImport(address, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setForce1(double forceX, double forceY, double forceZ);
        [DllImport(address, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setForce2(double forceX, double forceY, double forceZ);
        [DllImport(address, CallingConvention = CallingConvention.Cdecl)]
        public static extern int setHaptic(int aHaptic);
        [DllImport(address, CallingConvention = CallingConvention.Cdecl)]
        public static extern void setViscous(int aViscous);
        [DllImport(address, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getpos1();
        [DllImport(address, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr getpos2();
        [DllImport(address, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReleaseMemory(IntPtr ptr);


        public OmniPosition GetOmniPositions()
        {
            OmniPosition currentPosition = new OmniPosition();
            double[] pos1 = { 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] pos2 = { 0, 0, 0, 0, 0, 0, 0, 0 };

            try
            {
                IntPtr ptr = getpos1();
                Marshal.Copy(ptr, pos1, 0, 8);
                ReleaseMemory(ptr);

                IntPtr ptr2 = getpos2();
                Marshal.Copy(ptr2, pos2, 0, 8);
                ReleaseMemory(ptr2);

                currentPosition = new OmniPosition(pos1, pos2);
                //add any external buttons
                if (externalButtons != null)
                    currentPosition.ExtraButtons = externalButtons;
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
            return currentPosition;

            /*
            //Update External Buttons Label
            if (externalButtons != null)
            {
                int j = 1;
                foreach (bool b in externalButtons)
                {
                    if (b)
                        lbl_ExBtns.Text = "Ex. Buttons : " + j;
                    j++;
                }
            }


            

            if (isInControl)
                inControlPosition = currentPosition.Add(positionOffset);

            if (currentPosition.ButtonsRight == 2 && !buttonPressed)
            {
                if (!isInControl)
                {
                    buttonPressed = true;
                    feedbackEnabled = !feedbackEnabled;
                }
            }
            //Check for emergency switch press
            if (isMaster && !isInControl)
            {
                if (EmergencySwitchBoundBtn.Contains("Left"))
                {
                    if (currentPosition.ButtonsLeft == EmergencySwitchBoundValue)
                        emergencySwitchControl();
                }
                else if (EmergencySwitchBoundBtn.Contains("Right"))
                {
                    if (currentPosition.ButtonsRight == EmergencySwitchBoundValue)
                        emergencySwitchControl();
                }
                else if (EmergencySwitchBoundBtn.Contains("Ex"))
                {
                    if (currentPosition.ExtraButtons[EmergencySwitchBoundValue])
                        emergencySwitchControl();
                }
            }

            if (cb_NoRobot.Checked)
            {
                //Check for R2 press (used for TelSurge Freeze)
                if (currentPosition.ButtonsRight == 2 && !buttonPressed)
                {
                    buttonPressed = true;
                    if (isInControl)
                    {
                        freezeCommandReceived();
                    }
                    if (!isInControl)
                    {
                        buttonPressed = true;
                        feedbackEnabled = !feedbackEnabled;
                    }
                }
                if (currentPosition.ButtonsRight == 1 && !buttonPressed)
                {
                    //Check for R1 press (used for TelSurge Emergency Switch)
                    emergencySwitchControl();
                    buttonPressed = true;
                }
            }
            //reset buttonPressed (to ensure code runs once per press) 
            if (currentPosition.ButtonsRight == 0 && buttonPressed)
                buttonPressed = false;

            if (isMaster || hasMasterData)
            {
                if (isFrozen)
                {
                    OutputPosition = frozenPosition;
                    //Don't want to freeze buttons
                    OutputPosition.ButtonsLeft = currentPosition.ButtonsLeft;
                    OutputPosition.ButtonsRight = currentPosition.ButtonsRight;
                    OutputPosition.ExtraButtons = currentPosition.ExtraButtons;
                }
                else
                {
                    OutputPosition = inControlPosition;
                }
            }

            tb_SendingLeft.Text = "X = " + OutputPosition.LeftX + Environment.NewLine + "Y = " + OutputPosition.LeftY + Environment.NewLine + "Z = " + OutputPosition.LeftZ;
            tb_SendingRight.Text = "X = " + OutputPosition.RightX + Environment.NewLine + "Y = " + OutputPosition.RightY + Environment.NewLine + "Z = " + OutputPosition.RightZ;
            */
        }
        public void SetOmniForce(OmniPosition Force) 
        {
            try
            {
                if (HasOmnis)
                {
                    setForce1(Force.LeftX, Force.LeftY, Force.LeftZ);
                    setForce2(Force.RightX, Force.RightY, Force.RightZ);
                }
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
        }
        public void OmniFollow(OmniPosition InControlPosition)
        {
            try
            {
                if (HasOmnis)
                {
                    OmniPosition currentPosition = GetOmniPositions();
                    double forceLX = followingForceConstant * (currentPosition.LeftX - InControlPosition.LeftX);
                    if (forceLX > forceMax) forceLX = forceMax;
                    double forceLY = followingForceConstant * (currentPosition.LeftY - InControlPosition.LeftY);
                    if (forceLY > forceMax) forceLY = forceMax;
                    double forceLZ = followingForceConstant * (currentPosition.LeftZ - InControlPosition.LeftZ);
                    if (forceLZ > forceMax) forceLZ = forceMax;
                    double forceRX = followingForceConstant * (currentPosition.RightX - InControlPosition.RightX);
                    if (forceRX > forceMax) forceRX = forceMax;
                    double forceRY = followingForceConstant * (currentPosition.RightY - InControlPosition.RightY);
                    if (forceRY > forceMax) forceRY = forceMax;
                    double forceRZ = followingForceConstant * (currentPosition.RightZ - InControlPosition.RightZ);
                    if (forceRZ > forceMax) forceRZ = forceMax;

                    SetOmniForce(new OmniPosition(forceLX, forceLY, forceLZ, forceRX, forceRY, forceRZ));
                }
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
        }
        private void SetMyDefaultIP()
        {
            //default IP address to machine's last address
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    MyIPAddress = ip.ToString();
                }
            }
        }
        public int InitializeOmnis(string LeftOmniName, string RightOmniName)
        {
            int success = 0;

            if (HasOmnis)
            {
                try
                {
                    success = initAndSchedule(LeftOmniName, RightOmniName);
                    if (success == 1)
                    {
                        lock1();
                        lock2();
                    }
                    else
                    {
                        Main.ShowError("Omni initialization error. Please check connections and try again.", "");
                    }
                }
                catch (Exception ex)
                {
                    Main.ShowError(ex.Message, ex.ToString());
                }
            }

            return success;



            if (success == 1 || !HasOmnis)
            {
                if (!IsMaster && MyIPAddress == null)
                    SetMyIP();
                //Only allow one successful connection to Omnis
                cb_noOmnisAttached.Enabled = false;
                UnderlyingTimer.Enabled = true;
                //do not allow switching between master and slave state once omnis are initialized
                cb_isMaster.Enabled = false;
                tb_InstanceName.Enabled = false;

                if (isMaster)
                {
                    //For now, display in control here
                    switchControl(true, myIPAddress);

                    //start listening for clients wanting to connect
                    Thread t = new Thread(new ThreadStart(listenForNewConnections));
                    t.IsBackground = true;
                    t.Start();

                    //start listening for data
                    isListeningForData = true;
                    Thread t2 = new Thread(new ThreadStart(listenForData));
                    t2.IsBackground = true;
                    t2.Start();
                }
                else
                {
                    ConnectToMasterButton.Visible = true;
                    myName = tb_InstanceName.Text;
                }

                if (cb_noOmnisAttached.Checked)
                {
                    //Don't show force controls if no omnis attched
                    btn_zeroForces.Visible = false;
                    groupBox3.Visible = false;
                    tb_forces.Visible = false;
                    lbl_forceStrength.Visible = false;
                    trb_forceStrength.Visible = false;
                }

                gb_SendingLeft.Visible = true;
                tb_SendingLeft.Visible = true;
                gb_SendingRight.Visible = true;
                tb_SendingRight.Visible = true;
                btn_Initialize.Enabled = false;
            }
        }
        public void ConnectToMaster()
        {
            TcpClient c = new TcpClient();
            try
            {
                c.Connect(IPAddress.Parse(Surgery.Master.MyIPAddress), connectionPort);
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.TimedOut)
                    Main.ShowError("Cannot connect to Master... Try Again.", ex.Message);
                else
                    Main.ShowError(ex.Message, ex.ToString());
            }
            ConnectedToMaster = c.Connected;
            if (ConnectedToMaster)
            {
                try
                {
                    SocketData.
                    string json = JsonConvert.SerializeObject(sm);
                    byte[] arry = Encoding.ASCII.GetBytes(json);
                    Stream s = c.GetStream();
                    s.Write(arry, 0, arry.Length);

                    //start listening for master's position
                    isListeningForData = true;
                    Thread t1 = new Thread(new ThreadStart(listenForData));
                    t1.IsBackground = true;
                    t1.Start();

                    Thread t2 = new Thread(new ThreadStart(listenForVideo));
                    t2.IsBackground = true;
                    t2.Start();

                    Thread t3 = new Thread(new ThreadStart(sendMarkings));
                    t3.IsBackground = true;
                    t3.Start();

                    Thread t4 = new Thread(new ThreadStart(listenForGrantReq));
                    t4.IsBackground = true;
                    t4.Start();

                    Thread t5 = new Thread(new ThreadStart(readVideoBuffer));
                    t5.IsBackground = true;
                    t5.Start();

                    Thread t6 = new Thread(new ThreadStart(readDataBuffer));
                    t6.IsBackground = true;
                    t6.Start();

                    if (!cb_noOmnisAttached.Checked) //if Omni's are attached
                    {
                        lbl_Connections.Text = "Connected to: ";
                        string dir = Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"..\..\Content\pc.png");
                        Image pcImg = Image.FromFile(dir);
                        ToolStripItem newItem = new ToolStripButton("Master", pcImg, sendClientGrantReq, "btn_Master");
                        newItem.ToolTipText = tb_ipAddress.Text;
                        ss_Connections.Items.Add(newItem);

                        btn_ReqControl.Enabled = true;
                    }
                    else
                    {
                        lbl_Connections.Text = "Connected to: Master";
                    }

                    lbl_Connections.Visible = true;
                    gb_SendingLeft.Visible = true;
                    tb_SendingLeft.Visible = true;
                    gb_SendingRight.Visible = true;
                    tb_SendingRight.Visible = true;
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message, ex.ToString());
                }
            }

            ConnectToMasterButton.Enabled = !connected;
            tb_ipAddress.Enabled = !connected;
            btn_zeroForces.Enabled = connected;
            groupBox3.Enabled = connected;
            tb_forces.Enabled = connected;
            lbl_forceStrength.Enabled = connected;
            trb_forceStrength.Enabled = connected;
        }

    }
}

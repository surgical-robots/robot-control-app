using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelSurge.DataModels;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace TelSurge
{
    public class User
    {
        private TelSurgeMain Main;
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
        public string EmergencySwitchBoundBtn { get; set; }
        public int EmergencySwitchBoundValue { get; set; }
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


        public User(TelSurgeMain MainForm, int ConnectionPort)
        {
            this.Main = MainForm;
            this.MyName = "";
            SetMyDefaultIP();
            this.IsFrozen = false;
            this.IsMaster = false;
            this.NetworkDelay = 0;
            this.ConnectedToMaster = false;
            this.IsInControl = false;
            this.HasOmnis = true;
            this.connectionPort = ConnectionPort;
        }
        public OmniPosition GetOmniPositions()
        {
            OmniPosition currentPosition = new OmniPosition();
            double[] pos1 = { 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] pos2 = { 0, 0, 0, 0, 0, 0, 0, 0 };

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
            if (HasOmnis)
            {
                setForce1(Force.LeftX, Force.LeftY, Force.LeftZ);
                setForce2(Force.RightX, Force.RightY, Force.RightZ);
            }
        }
        public void OmniFollow(OmniPosition InControlPosition)
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
                success = initAndSchedule(LeftOmniName, RightOmniName);
                if (success == 1)
                {
                    lock1();
                    lock2();
                }
            }

            return success;
            }
        public bool ConnectToMaster()
        {
            TcpClient c = new TcpClient();
            try
            {
                c.Connect(IPAddress.Parse(Main.Surgery.Master.MyIPAddress), connectionPort);
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
                SocketMessage sm = new SocketMessage(Main.Surgery, this);
                Main.SocketData.SendTCPDataTo(c, SocketData.SerializeObject<SocketMessage>(sm));
            }
            return ConnectedToMaster;
        }
    }
}

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
using System.IO.Ports;
using GeomagicTouch;
using System.Threading;

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
        public bool IsFollowing { get; set; }
        private bool[] externalButtons = new bool[0];
        public bool HasOmnis { get; set; }
        private double followingForceConstant = 0.1;
        private double forceMax = 3;
        private int connectionPort;
        public string EmergencySwitchBoundBtn { get; set; }
        public int EmergencySwitchBoundValue { get; set; }
        public string FollowingBoundBtn { get; set; }
        public int FollowingBoundValue { get; set; }
        public string FreezeBoundBtn { get; set; }
        public int FreezeBoundValue { get; set; }
        private Device LeftOmni = null;
        private Device RightOmni = null;
        public DateTime LastHeardFrom { get; set; }

        private SerialPort extButtonsPort = null;
        private bool extButtonsConnected = false;
        public int NumExternalButtons = 0;
        private bool EmergBtnPressed = false;
        private bool FollowBtnPressed = false;
        private bool FreezeBtnPressed = false;
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
        /*
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
        */

        public User()
        {
            this.MyName = "";
            this.IsFrozen = false;
            this.IsMaster = false;
            this.NetworkDelay = 0;
            this.ConnectedToMaster = false;
            this.IsInControl = false;
            this.HasOmnis = true;
            this.IsFollowing = false;
            this.EmergencySwitchBoundBtn = "";
            this.FollowingBoundBtn = "";
            this.FreezeBoundBtn = "";
        }
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
            this.IsFollowing = false;
            this.EmergencySwitchBoundBtn = "";
            this.FollowingBoundBtn = "";
            this.FreezeBoundBtn = "";
        }
        public OmniPosition GetOmniPositions()
        {
            LeftOmni.Update();
            RightOmni.Update();

            OmniPosition currentPosition = new OmniPosition(LeftOmni, RightOmni);
            //add any external buttons
            if (extButtonsConnected)
            {
                if (externalButtons.Length == 0)
                    externalButtons = new bool[NumExternalButtons];
                if (extButtonsPort.BytesToRead >= NumExternalButtons)
                {
                    //externalButtons = new bool[NumExternalButtons];
                    int intReturnASCII = 0;
                    string returnMessage = "";

                    for (int i = NumExternalButtons; i > 0 ; i--)
                    {
                        intReturnASCII = extButtonsPort.ReadByte();
                        returnMessage = returnMessage + Convert.ToChar(intReturnASCII);
                    }

                    if (returnMessage != "")
                    {
                        //if (externalButtons == new bool[0])
                        
                        for (int i = 0; i < NumExternalButtons; i++)
                        {
                            externalButtons[i] = Convert.ToBoolean(Char.GetNumericValue(returnMessage[i]));
                        }
                    }
                    extButtonsPort.DiscardInBuffer();
                }
                currentPosition.ExtraButtons = externalButtons;
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
            if (HasOmnis)
            {
                LeftOmni.SetpointX = Force.LeftX;
                LeftOmni.SetpointY = Force.LeftY;
                LeftOmni.SetpointZ = Force.LeftZ;
                RightOmni.SetpointX = Force.RightX;
                RightOmni.SetpointY = Force.RightY;
                RightOmni.SetpointZ = Force.RightZ;
            }
        }
        public void OmniFollow(OmniPosition InControlPosition)
        {
            if (HasOmnis)
            {
                OmniPosition currentPosition = GetOmniPositions();
                double forceLX = followingForceConstant * (InControlPosition.LeftX - currentPosition.LeftX);
                if (forceLX > forceMax) forceLX = forceMax;                        
                double forceLY = followingForceConstant * (InControlPosition.LeftY - currentPosition.LeftY);
                if (forceLY > forceMax) forceLY = forceMax;                        
                double forceLZ = followingForceConstant * (InControlPosition.LeftZ - currentPosition.LeftZ);
                if (forceLZ > forceMax) forceLZ = forceMax;
                double forceRX = followingForceConstant * (InControlPosition.RightX - currentPosition.RightX);
                if (forceRX > forceMax) forceRX = forceMax;                         
                double forceRY = followingForceConstant * (InControlPosition.RightY - currentPosition.RightY);
                if (forceRY > forceMax) forceRY = forceMax;                         
                double forceRZ = followingForceConstant * (InControlPosition.RightZ - currentPosition.RightZ);
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
        public bool InitializeOmnis(string LeftOmniName, string RightOmniName)
        {
            bool success = false;

            if (HasOmnis)
            {
                try
                {
                    LeftOmni = new Device(LeftOmniName);
                    RightOmni = new Device(RightOmniName);
                    LeftOmni.Start();
                    RightOmni.Start();
                    SetOmniForce(new OmniPosition());
                    LeftOmni.SetpointEnabled = true;
                    RightOmni.SetpointEnabled = true;
                    success = true;
                }
                catch (Exception ex)
                {
                    Main.ShowError(ex.Message, ex.ToString());
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
        public bool CheckForEmergencySwitch(OmniPosition Position)
        {
            bool btnPressed = checkButton(Position, EmergencySwitchBoundBtn, EmergencySwitchBoundValue);
            if (!btnPressed && EmergBtnPressed)
            {
                EmergBtnPressed = false;
                return true;
            }
            EmergBtnPressed = btnPressed;
            return false;
        }
        public void CheckIfFollowing(OmniPosition Position)
        {
            bool btnPressed = checkButton(Position, FollowingBoundBtn, FollowingBoundValue);
            if (!btnPressed && FollowBtnPressed)
            {
                FollowBtnPressed = false;
                IsFollowing = !IsFollowing;
            }
            FollowBtnPressed = btnPressed;
        }
        public bool CheckForFreeze(OmniPosition Position)
        {
            bool btnPressed = checkButton(Position, FreezeBoundBtn, FreezeBoundValue);
            if (!btnPressed && FreezeBtnPressed)
            {
                FreezeBtnPressed = false;
                return true;
            }
            FreezeBtnPressed = btnPressed;
            return false;
        }
        private bool checkButton(OmniPosition pos, string btn, int value)
        {
            if (!btn.Equals(""))
            {
                if (btn.Contains("Left"))
                {
                    if (pos.ButtonsLeft.Equals(value))
                        return true;
                }
                else if (btn.Contains("Right"))
                {
                    if (pos.ButtonsRight.Equals(value))
                        return true;
                }
                else
                {
                    for (int i = 0; i < pos.ExtraButtons.Count(); i++)
                    {
                        if (pos.ExtraButtons[i] && i.Equals(value))
                            return true;
                    }
                }
            }
            return false;
        }
        public void SetOmniForceX(double ForceX, bool IsLeft)
        {
            if (HasOmnis)
            {
                if (IsLeft)
                    LeftOmni.SetpointX = ForceX;
                else
                    RightOmni.SetpointX = ForceX;
            }
        }
        public void SetOmniForceY(double ForceY, bool IsLeft)
        {
            if (HasOmnis)
            {
                if (IsLeft)
                    LeftOmni.SetpointY = ForceY;
                else
                    RightOmni.SetpointY = ForceY;
            }
        }
        public void SetOmniForceZ(double ForceZ, bool IsLeft)
        {
            if (HasOmnis)
            {
                if (IsLeft)
                    LeftOmni.SetpointZ = ForceZ;
                else
                    RightOmni.SetpointZ = ForceZ;
            }
        }
        public void ConnectExternalButtons(SerialPort ConnectionPort, bool Disconnect, int numOfButtons)
        {
            extButtonsPort = ConnectionPort;

            if (Disconnect)
            {
                if (ConnectionPort.IsOpen)
                    ConnectionPort.Close();
                numOfButtons = 0;
                extButtonsConnected = false;
            }
            else
            {
                if (!ConnectionPort.IsOpen)
                    ConnectionPort.Open();

                NumExternalButtons = numOfButtons;
                extButtonsConnected = true;
            }
            
        }
        public void DisconnectOmnis()
        {
            LeftOmni.Stop();
            RightOmni.Stop();
        }
    }
}

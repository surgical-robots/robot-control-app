using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using TelSurge.DataModels;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LumiSoft.Net.UDP;
using LumiSoft.Net.Codec;
using LumiSoft.Media.Wave;
using System.Reflection;
using System.Diagnostics;

namespace TelSurge
{
    public partial class TelSurgeMain : Form
    {
        //External Omni Methods
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

        //OUTPUTS
        public OmniPosition OutputPosition { get; set; }

        //Variables used in construction of form
        private Capture _capture = null;
        private List<string> connectedClients = new List<string>();
        
        //Construct Form1
        public TelSurgeMain()
        {
            InitializeComponent();
            currentPosition = new OmniPosition(0, 0, 0, 0, 0, 0);

            try
            {
                OutputPosition = new OmniPosition();
                TelSurgeMain.CheckForIllegalCrossThreadCalls = false;
                fillOmniDDL();
                fillAudioDeviceDDL();

                //Set Force Trackbar
                //want force divider between 20 and 220
                //divider = 220 - trackbarValue 
                trb_forceStrength.Minimum = 0;
                trb_forceStrength.Maximum = 200;
                // The TickFrequency property establishes how many positions are between each tick-mark.
                trb_forceStrength.TickFrequency = 20;
                // The LargeChange property sets how many positions to move if the bar is clicked on either side of the slider.
                trb_forceStrength.LargeChange = 2;
                // The SmallChange property sets how many positions to move if the keyboard arrows are used to move the slider.
                trb_forceStrength.SmallChange = 1;
                //set initial value of trackbar
                trb_forceStrength.Value = 170;

                captureImageBox.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
                //try to set up capture from default camera
                _capture = new Capture();
                _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS, 30);
                _capture.ImageGrabbed += ProcessFrame;
            }
            catch (NullReferenceException nrex)
            {
                if (nrex.HResult == -2147467261)
                    ShowError("Cannot connect to default camera!", "No camera could be found on this machine, (" + Environment.MachineName + ").");
                else
                    ShowError(nrex.Message, nrex.ToString());
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }

        //Globals
        private string myName = "";
        private bool _captureInProgress = false;
        private Markings myMarkings = new Markings();
        private Markings clientMarkings = new Markings();
        private Markings combinedMarkings = new Markings();
        Color penColor = Color.Red;
        int penThickness = 5;
        string lastColorUsed = "Red";
        bool isDrawing = false;
        bool isFirstPoint = true;
        List<Point> tmpPoints = new List<Point>();
        int redFigureNum = 0;
        int blackFigureNum = 0;
        int blueFigureNum = 0;
        int whiteFigureNum = 0;
        int yellowFigureNum = 0;
        int greenFigureNum = 0;
        int controlPort = 11005;
        int connectionPort = 11004;
        int audioPort = 11003;
        int markingsPort = 11002;
        int dataPort = 11001;
        int videoPort = 11000;
        double forceOffset_LX = 0;
        double forceOffset_LY = 0;
        double forceOffset_LZ = 0;
        double forceOffset_RX = 0;
        double forceOffset_RY = 0;
        double forceOffset_RZ = 0;
        double sendForce_LX = 0;
        double sendForce_LY = 0;
        double sendForce_LZ = 0;
        double sendForce_RX = 0;
        double sendForce_RY = 0;
        double sendForce_RZ = 0;
        bool sendFreezeComd = false;
        private OmniPosition inControlPosition;
        private OmniPosition frozenPosition;
        private OmniPosition positionOffset = new OmniPosition();
        public OmniPosition currentPosition;
        bool newFigure = false;
        string logFile = "log.csv";
        bool audioIsRunning = false;
        private UdpServer audioServer = null;
        private WaveIn audioWaveIn = null;
        private WaveOut audioWaveOut = null;
        private int m_Codec = 0; //Encode Audio  0: ALAW, 1: ULAW
        private volatile bool isInControl = false;
        private bool buttonPressed = false;
        private bool feedbackEnabled = false;
        private volatile bool isFrozen = false;
        private volatile bool inControlIsFrozen = false;
        //private volatile bool isListeningForData = false;
        private volatile string inControlIP;
        private double repositioningError = 15;
        UdpClient markingsListener = null;
        UdpClient videoListener = null;
        UdpClient dataListener = null;
        TcpListener grantReqListener = null;
        private string sendGRAddr = "";
        private bool hasMasterData = false;
        private volatile bool applicationRunning = true;
        string exeFile = (new System.Uri(Assembly.GetEntryAssembly().CodeBase)).AbsolutePath;
        private bool imgHasBeenProcessed = false;
        public int networkDelay = 0; //in ms
        private bool EmergencySwitch = false;
        public Queue<Image<Bgr, Byte>> videoBuffer = new Queue<Image<Bgr, byte>>();
        private bool videoImageAvailable = false;
        private System.Diagnostics.Stopwatch videoWatch = new System.Diagnostics.Stopwatch();
        public bool networkVideoDelayChanged = false;
        public Queue<SocketMessage> dataBuffer = new Queue<SocketMessage>();
        private bool dataAvailable = false;
        private System.Diagnostics.Stopwatch dataWatch = new System.Diagnostics.Stopwatch();
        public bool networkDataDelayChanged = false;
        public bool[] externalButtons { get; set; }
        private bool isListeningForData = false;
        private bool clearMarkingsReq = false;
        private string myIPAddress;
        private bool receiveMasterVideo = true;
        public bool isMaster = false;
        public bool isConnectedToMaster = false;
        public bool videoIsPTZ = false;
        private volatile bool isZooming = false;
        private int zoomingRadius = 0;
        private Point startZoomPt;
        private Size videoImgSize;
        private int zoomScalingFactor = 5;
        private Point camClickPosFactor = new Point(10, 70);
        private Point camZoomPosFactor = new Point(-35, 15);
        private Image<Bgr, Byte> frame;
        private System.Diagnostics.Stopwatch turnAroundTimer = new System.Diagnostics.Stopwatch();
        private bool logDataTurnAroundTime = false;
        public string EmergencySwitchBoundBtn;
        public int EmergencySwitchBoundValue;

        //Core Methods
        private List<Point[]> getMarksList()
        {
            switch (penColor.Name)
            {
                case "Red":
                    return myMarkings.RedMarkings;
                case "Black":
                    return myMarkings.BlackMarkings;
                case "Blue":
                    return myMarkings.BlueMarkings;
                case "White":
                    return myMarkings.WhiteMarkings;
                case "Yellow":
                    return myMarkings.YellowMarkings;
                case "Green":
                    return myMarkings.GreenMarkings;
                default:
                    throw new Exception("Invalid color selected.");
            }
        }
        private void setFigureNum(int num)
        {
            switch (penColor.Name)
            {
                case "Red":
                    redFigureNum = num;
                    break;
                case "Black":
                    blackFigureNum = num;
                    break;
                case "Blue":
                    blueFigureNum = num;
                    break;
                case "White":
                    whiteFigureNum = num;
                    break;
                case "Yellow":
                    yellowFigureNum = num;
                    break;
                case "Green":
                    greenFigureNum = num;
                    break;
                default:
                    throw new Exception("Invalid color selected.");
            }
        }
        private int getFigureNum()
        {
            switch (penColor.Name)
            {
                case "Red":
                    return redFigureNum;
                case "Black":
                    return blackFigureNum;
                case "Blue":
                    return blueFigureNum;
                case "White":
                    return whiteFigureNum;
                case "Yellow":
                    return yellowFigureNum;
                case "Green":
                    return greenFigureNum;
                default:
                    throw new Exception("Invalid color selected.");
            }
        }
        private void ReleaseData()
        {
            if (_capture != null) _capture.Dispose();
        }
        private void showOmniPositions()
        {
            if (!cb_noOmnisAttached.Checked)
            {
                currentPosition = new OmniPosition(0, 0, 0, 0, 0, 0);
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
                    ShowError(ex.Message, ex.ToString());
                }
            }

            lbX1value.Text = "X : " + currentPosition.LeftX.ToString();
            lbY1value.Text = "Y : " + currentPosition.LeftY.ToString();
            lbZ1value.Text = "Z : " + currentPosition.LeftZ.ToString();

            lbGimbal11.Text = "Gimbal 1 : " + currentPosition.Gimbal1Left.ToString();
            lbGimbal21.Text = "Gimbal 2 : " + currentPosition.Gimbal2Left.ToString();
            lbGimbal31.Text = "Gimbal 3 : " + currentPosition.Gimbal3Left.ToString();

            lbButtons1.Text = "Buttons : " + currentPosition.ButtonsLeft;
            lbInk1.Text = "InkWell : " + currentPosition.InkwellLeft.ToString();
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
            

            lbX2Value.Text = "X : " + currentPosition.RightX.ToString();
            lbY2Value.Text = "Y : " + currentPosition.RightY.ToString();
            lbZ2Value.Text = "Z : " + currentPosition.RightZ.ToString();

            lbGimbal12.Text = "Gimbal 1 : " + currentPosition.Gimbal1Right.ToString();
            lbGimbal22.Text = "Gimbal 2 : " + currentPosition.Gimbal2Right.ToString();
            lbGimbal32.Text = "Gimbal 3 : " + currentPosition.Gimbal3Right.ToString();

            lbButtons2.Text = "Buttons : " + currentPosition.ButtonsRight.ToString();
            lbInk2.Text = "InkWell : " + currentPosition.InkwellRight.ToString();

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
        }
        private void freezeOmniPosition(OmniPosition pos)
        {
            //make sure this method is executed once
            buttonPressed = true;
            //save current position
            frozenPosition = pos;
            //Relieve any forces on omnis
            feedbackEnabled = false;
            setForces(new OmniPosition());
        }
        public void freezeCommandReceived()
        {
            if (isInControl)
            {
                if (!isFrozen)
                {
                    freezeOmniPosition(inControlPosition);
                    tb_InControl.Text = "You are frozen.";
                    isFrozen = true;
                }
                else
                {
                    positionOffset = frozenPosition.Subtract(currentPosition);
                    tb_InControl.Text = "You are in control!";
                    isFrozen = false;
                }
            }
            else
            {
                sendFreezeComd = true;
            }
            
        }
        private void setForces(OmniPosition position)
        {
            try
            {
                double forceLX = 0;
                double forceLY = 0;
                double forceLZ = 0;
                double forceRX = 0;
                double forceRY = 0;
                double forceRZ = 0;

                if (feedbackEnabled)
                {

                    IntPtr ptr = getpos1();
                    double[] pos1 = new double[8];
                    Marshal.Copy(ptr, pos1, 0, 8);
                    ReleaseMemory(ptr);

                    IntPtr ptr2 = getpos2();
                    double[] pos2 = new double[8];
                    Marshal.Copy(ptr2, pos2, 0, 8);
                    ReleaseMemory(ptr2);

                    forceLX = (position.LeftX - (pos1[0] - forceOffset_LX)) / getForceStrength();
                    forceLY = (position.LeftY - (pos1[1] - forceOffset_LY)) / getForceStrength();
                    forceLZ = (position.LeftZ - (pos1[2] - forceOffset_LZ)) / getForceStrength();
                    forceRX = (position.RightX - (pos2[0] - forceOffset_RX)) / getForceStrength();
                    forceRY = (position.RightY - (pos2[1] - forceOffset_RY)) / getForceStrength();
                    forceRZ = (position.RightZ - (pos2[2] - forceOffset_RZ)) / getForceStrength();
                }

                if (cb_noOmnisAttached.Checked)
                {
                    forceLX = position.LeftX;
                    forceLY = position.LeftY;
                    forceLZ = position.LeftZ;
                    forceRX = position.RightX;
                    forceRY = position.RightY;
                    forceRZ = position.RightZ;
                    tb_forces.Text = @"Left" + Environment.NewLine + "X = " + Math.Round(forceLX, 4) + " Y = " + Math.Round(forceLY, 4) + " Z = " + Math.Round(forceLZ, 4) + Environment.NewLine + "Right" + Environment.NewLine + "X = " + Math.Round(forceRX, 4) + " Y = " + Math.Round(forceRY, 4) + " Z = " + Math.Round(forceRZ, 4);
                }
                else
                {
                    setForce1(forceLX, forceLY, forceLZ);
                    setForce2(forceRX, forceRY, forceRZ);
                    tb_forces.Text = @"Left" + Environment.NewLine + "X = " + Math.Round(forceLX, 4) + " Y = " + Math.Round(forceLY, 4) + " Z = " + Math.Round(forceLZ, 4) + Environment.NewLine + "Right" + Environment.NewLine + "X = " + Math.Round(forceRX, 4) + " Y = " + Math.Round(forceRY, 4) + " Z = " + Math.Round(forceRZ, 4);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        public bool setForces(double leftX, double leftY, double leftZ, double rightX, double rightY, double rightZ)
        {
            double forceMax = 3;
            if (!isFrozen)
            {
                if (leftX > forceMax)
                    leftX = forceMax;
                if (leftY > forceMax)
                    leftY = forceMax;
                if (leftZ > forceMax)
                    leftZ = forceMax;
                if (rightX > forceMax)
                    rightX = forceMax;
                if (rightY > forceMax)
                    rightY = forceMax;
                if (rightZ > forceMax)
                    rightZ = forceMax;

                if (isInControl)
                {
                    try
                    {
                        setForce1(leftX, leftY, leftZ);
                        setForce2(rightX, rightY, rightZ);
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message, ex.ToString());
                    }
                }
                else
                {
                    sendForce_LX = leftX;
                    sendForce_LY = leftY;
                    sendForce_LZ = leftZ;
                    sendForce_RX = rightX;
                    sendForce_RY = rightY;
                    sendForce_RZ = rightZ;
                }
                return true;
            }
            return false;
        }
        public void setPosition(double LX, double LY, double LZ, double RX, double RY, double RZ)
        {
            if (cb_noOmnisAttached.Checked)
            {
                if (btn_ReqControl.Enabled == false && !isInControl)
                    btn_ReqControl.Enabled = true;

                currentPosition.LeftX = LX;
                currentPosition.LeftY = LY;
                currentPosition.LeftZ = LZ;

                currentPosition.RightX = RX;
                currentPosition.RightY = RY;
                currentPosition.RightZ = RZ;
            }
        }
        private void forceOmniPosition(object objPosition)
        {
            try
            {
                if (cb_noOmnisAttached.Checked)
                    throw new Exception("No Omnis are attached. Cannot force a position.");

                OmniPosition position = (OmniPosition)objPosition;
                double correctedLX = 0;
                double correctedLY = 0;
                double correctedLZ = 0;
                double correctedRX = 0;
                double correctedRY = 0;
                double correctedRZ = 0;
                bool positionMatch = false;
                int counter = 0;
                int countDown = 3;

                while (true)
                {
                    IntPtr ptr = getpos1();
                    double[] pos1 = new double[8];
                    Marshal.Copy(ptr, pos1, 0, 8);
                    ReleaseMemory(ptr);

                    IntPtr ptr2 = getpos2();
                    double[] pos2 = new double[8];
                    Marshal.Copy(ptr2, pos2, 0, 8);
                    ReleaseMemory(ptr2);

                    correctedLX = (pos1[0] - forceOffset_LX);
                    correctedLY = (pos1[1] - forceOffset_LY);
                    correctedLZ = (pos1[2] - forceOffset_LZ);
                    correctedRX = (pos2[0] - forceOffset_RX);
                    correctedRY = (pos2[1] - forceOffset_RY);
                    correctedRZ = (pos2[2] - forceOffset_RZ);

                    double forceLX = (position.LeftX - correctedLX) / 10;
                    if (forceLX > 3)
                        forceLX = 3;
                    double forceLY = (position.LeftY - correctedLY) / 10;
                    if (forceLY > 3)
                        forceLY = 3;
                    double forceLZ = (position.LeftZ - correctedLZ) / 10;
                    if (forceLZ > 3)
                        forceLZ = 3;
                    double forceRX = (position.RightX - correctedRX) / 10;
                    if (forceRX > 3)
                        forceRX = 3;
                    double forceRY = (position.RightY - correctedRY) / 10;
                    if (forceRY > 3)
                        forceRY = 3;
                    double forceRZ = (position.RightZ - correctedRZ) / 10;
                    if (forceRZ > 3)
                        forceRZ = 3;

                    setForce1(forceLX, forceLY, forceLZ);
                    setForce2(forceRX, forceRY, forceRZ);

                    if ((Math.Abs(correctedLX - position.LeftX) > repositioningError) || (Math.Abs(correctedLY - position.LeftY) > repositioningError) || (Math.Abs(correctedLZ - position.LeftZ) > repositioningError) || (Math.Abs(correctedRX - position.RightX) > repositioningError) || (Math.Abs(correctedRY - position.RightY) > repositioningError) || (Math.Abs(correctedRZ - position.RightZ) > repositioningError))
                    {
                        positionMatch = true;
                        tb_InControl.Text = "HOLD... " + countDown;
                    }
                    if (positionMatch)
                        counter++;
                    if (counter == 100)
                    {
                        countDown--;
                        if (countDown == 0)
                            break;

                        tb_InControl.Text = "HOLD... " + countDown;
                        counter = 0;
                    }
                    Thread.Sleep(10);
                }
                setForce1(0, 0, 0);
                setForce2(0, 0, 0);
                tb_InControl.Text = "You are in control!";
                isFrozen = false;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        private int getForceStrength()
        {
            return 220 - trb_forceStrength.Value;
        }
        private void fillOmniDDL()
        {
            if (!cb_noOmnisAttached.Checked)
            {
                spLeftOmni.DataSource = GetGeomagicDevices();
                spRightOmni.DataSource = GetGeomagicDevices();
            }
        }
        private string[] GetGeomagicDevices()
        {
            string[] fileNames = new string[1];
            try
            {
                //try both locations
                fileNames = Directory.GetFiles(@"C:\Users\Public\Documents\3DSystems\", "*.config");
                for (int i = 0; i < fileNames.Length; i++)
                {
                    fileNames[i] = Path.GetFileNameWithoutExtension(fileNames[i]);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
            return fileNames;
        }
        private void SetMyIP()
        {
            string ipAddress = "";
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = ip.ToString();
                }
            }
            myIPAddress = ipAddress;
        }
        public void SetMyIP(string SetToIP)
        {
            myIPAddress = SetToIP;
            if (isMaster)
                tb_ipAddress.Text = SetToIP;
        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            try
            {
                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

                foreach (ImageCodecInfo codec in codecs)
                {
                    if (codec.FormatID == format.Guid)
                    {
                        return codec;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
            return null;
        }
        private void logMessage(string msg, string detail, Logging.StatusTypes msgType)
        {
            Logging log = new Logging(msg, detail, logFile, msgType);
            log.Record();
        }
        public void ShowError(string msg, string detail)
        {
            //save error to log
            logMessage(msg, detail, Logging.StatusTypes.Error);
            //show error on form
            if (msg.Length > 150)
                lbl_Errors.Text = "Error: " + msg.Remove(150);
            else
                lbl_Errors.Text = "Error: " + msg;
            //set timer for displaying error
            errorTimer.Start();
        }
        private void fillAudioDeviceDDL()
        {
            try
            {
                ddl_AudioDevices.Items.Clear();
                foreach (WavInDevice device in WaveIn.Devices)
                {
                    ddl_AudioDevices.Items.Add(device.Name);
                }
                if (ddl_AudioDevices.Items.Count > 0)
                    ddl_AudioDevices.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        private void sendGrantReq(object emergency)
        {
            bool _emergency = (bool)emergency;
            TcpClient c = new TcpClient();
            try
            {
                c.Connect(IPAddress.Parse(sendGRAddr), controlPort);
                if (c.Connected)
                {
                    using (Stream s = c.GetStream())
                    {
                        byte[] arry = { Convert.ToByte(_emergency) };
                        s.Write(arry, 0, arry.Length);
                        arry = new byte[1];
                        s.Read(arry, 0, arry.Length);
                        if (arry[0] == 1)
                        {
                            //Accepted or Granted
                            if (isInControl)
                            {
                                //Give control
                                switchControl(false, sendGRAddr);
                            }
                            else
                            {
                                //take control
                                switchControl(true, myIPAddress);
                                //stop listening for data
                                //isListeningForData = false;
                            }    
                        }
                        else
                        {
                            MessageBox.Show("The user denied your request/grant.");
                            btn_ReqControl.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.HResult.Equals(-2146232800))
                    ShowError("Request was not accepted by other user.", "An IO exception was caught after trying to read an accept for passing control. It is assumed that the remote user denied the request.");
                else
                    ShowError(ex.Message, ex.ToString());
            }
        }
        private void switchControl(bool takeControl, string controlIP)
        {
            if (takeControl)
            {
                //if (btn_Initialize.Enabled || !inControlIsFrozen)
                if (btn_Initialize.Enabled)
                    tb_InControl.Text = "You are in control.";
                else
                {
                    frozenPosition = inControlPosition;
                    isFrozen = true;
                    tb_InControl.Text = "Press unfreeze button when ready...";
                }

                tb_InControl.BackColor = Color.Red;
                feedbackEnabled = false;
            }
            else
            {
                tb_InControl.Text = controlIP + " is in control.";
                tb_InControl.BackColor = Color.Green;
                isFrozen = false;
                if (frozenPosition != null)
                    inControlPosition = frozenPosition;
            }
            inControlIP = controlIP;
            isInControl = takeControl;
            groupBox3.Enabled = !takeControl;
            tb_forces.Enabled = !takeControl;
            btn_zeroForces.Enabled = !takeControl;
            lbl_forceStrength.Enabled = !takeControl;
            trb_forceStrength.Enabled = !takeControl;
            btn_ReqControl.Enabled = !takeControl;            
        }
        public void emergencySwitchControl()
        {
            if (!isInControl)
            {
                sendGRAddr = inControlIP;
                Thread t = new Thread(new ParameterizedThreadStart(sendGrantReq));
                t.IsBackground = true;
                t.Start(true);
            }
        }
        private void disconnect(string ipAddress)
        {
            try
            {
                if (isMaster)
                {
                    //if disconnected client was in control, take control
                    if (inControlIP.Equals(ipAddress))
                    {
                        inControlIsFrozen = true;
                        switchControl(true, myIPAddress);
                    }
                    connectedClients.Remove(ipAddress);
                    foreach (ToolStripItem itm in ss_Connections.Items)
                    {
                        if (itm.ToolTipText != null && itm.ToolTipText.Equals(ipAddress))
                        {
                            ss_Connections.Items.Remove(itm);
                            break;
                        }
                    }
                    if (connectedClients.Count < 1)
                    {
                        lbl_Connections.Text = "Connections: None";
                    }
                }
                else
                {
                    //send disconnect to Master
                    SocketMessage sm = new SocketMessage(myName, myIPAddress);
                    using (TcpClient c = new TcpClient())
                    {
                        c.Connect(IPAddress.Parse(tb_ipAddress.Text), connectionPort);
                        Stream s = c.GetStream();
                        string json = JsonConvert.SerializeObject(sm);
                        byte[] arry = Encoding.ASCII.GetBytes(json);
                        s.Write(arry, 0, arry.Length);
                    }
                }

                logMessage("Successfully disconnected.", "This machine was successfully disconnected with the surgery.", Logging.StatusTypes.Running);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        private void readVideoBuffer()
        {
            while (true)
            {
                if (videoBuffer.Count > 0)
                {
                    if (videoWatch.IsRunning)
                    {
                        if (videoWatch.ElapsedMilliseconds >= networkDelay)
                        {
                            videoWatch.Stop();
                            lbl_Errors.Text = videoWatch.ElapsedMilliseconds.ToString();
                            videoWatch.Reset();
                            //display image
                            if (videoImageAvailable)
                            {
                                captureImageBox.Image = videoBuffer.Dequeue();
                                videoImageAvailable = false;
                            }
                            errorTimer.Start();
                        }
                    }
                    else
                    {
                        //display image
                        if (videoImageAvailable)
                        {
                            captureImageBox.Image = videoBuffer.Dequeue();
                            videoImageAvailable = false;
                        }
                    }
                }
            }
        }
        private void readDataBuffer()
        {
            while (true)
            {
                if (dataBuffer.Count > 0)
                {
                    if (dataWatch.IsRunning)
                    {
                        if (dataWatch.ElapsedMilliseconds >= networkDelay)
                        {
                            videoWatch.Reset();
                            //use data
                            if (dataAvailable && dataBuffer.Count > 0)
                            {
                                SocketMessage dataMsg = dataBuffer.Dequeue();
                                if (!isInControl)
                                {
                                    inControlPosition = dataMsg.OmniPosition;
                                    positionOffset = dataMsg.PositionOffset;
                                    inControlIsFrozen = dataMsg.isFrozen;
                                }
                                if (!isMaster && isInControl)
                                {
                                    setForces(dataMsg.Forces[0], dataMsg.Forces[1], dataMsg.Forces[2], dataMsg.Forces[3], dataMsg.Forces[4], dataMsg.Forces[5]);
                                    if (dataMsg.sendFreezeCmd)
                                        freezeCommandReceived();
                                }
                                dataAvailable = false;
                            }
                            errorTimer.Start();
                        }
                    }
                    else
                    {
                        //use data
                        if (dataAvailable && dataBuffer.Count > 0)
                        {
                            SocketMessage dataMsg = dataBuffer.Dequeue();
                            if (!isInControl)
                            {
                                inControlPosition = dataMsg.OmniPosition;
                                positionOffset = dataMsg.PositionOffset;
                                inControlIsFrozen = dataMsg.isFrozen;
                            }
                            if (!isMaster && isInControl)
                            {
                                setForces(dataMsg.Forces[0], dataMsg.Forces[1], dataMsg.Forces[2], dataMsg.Forces[3], dataMsg.Forces[4], dataMsg.Forces[5]);
                                if (dataMsg.sendFreezeCmd)
                                    freezeCommandReceived();
                            }
                            dataAvailable = false;
                        }
                    }
                }
            }
        }
        public void changeVideoSource(int deviceIndex)
        {
            try
            {
                //if client, first stop receiving master video feed
                if (!isMaster)
                {
                    receiveMasterVideo = false;
                    _capture.Dispose();
                }
                if (_captureInProgress)
                {  //cannot change device during capture
                    _capture.Pause();
                }
                _capture = new Capture(deviceIndex);
                _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS, 30);
                _capture.ImageGrabbed += ProcessFrame;
                if (_captureInProgress || !isMaster)
                {
                    _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, captureImageBox.Height);
                    _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, captureImageBox.Width);
                    _capture.Start();
                    _captureInProgress = true;

                    lbl_Zoom.Visible = false;
                    lbl_ZoomIn.Visible = false;
                    lbl_ZoomOut.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        public void changeVideoSource(string deviceAddress)
        {
            try
            {
                //if client, first stop receiving master video feed
                if (!isMaster)
                {
                    receiveMasterVideo = false;
                    ReleaseData();
                }
                if (_captureInProgress)
                {  //cannot change device during capture
                    _capture.Stop();
                }
                _capture = new Capture(deviceAddress); //new Emgu.CV.CvInvoke.cvCreateFileCapture(deviceAddress);
                _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS, 30);
                _capture.ImageGrabbed += ProcessFrame;
                if (_captureInProgress || !isMaster)
                {
                    //_capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, captureImageBox.Height);
                    _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, captureImageBox.Width);
                    _capture.Start();
                    _captureInProgress = true;

                    //lbl_Zoom.Visible = true;
                    //lbl_ZoomIn.Visible = true;
                    //lbl_ZoomOut.Visible = true;

                }

            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        public void changeVideoSource()
        {
            //Start watching the video feed from master
            if (ConnectToMasterButton.Enabled)
                ShowError("Master is not connected! No video feed.", "Tried to receive Master video feed without a connection to Master. (ChangeVideoSource)");
            else
            {
                if (!isMaster)
                {
                    if (_captureInProgress)
                    {
                        _capture.Pause();
                    }
                    receiveMasterVideo = true;
                    videoBuffer.Clear();
                    Thread t = new Thread(new ThreadStart(listenForVideo));
                    t.IsBackground = true;
                    t.Start();
                    _captureInProgress = true;
                }
                lbl_Zoom.Visible = false;
                lbl_ZoomIn.Visible = false;
                lbl_ZoomOut.Visible = false;
            }
        }
        public static async void sendCmdToCamera(string cmd)
        {
            WebRequestHandler handler = new WebRequestHandler();
            handler.Credentials = new NetworkCredential("admin", "admin");
            HttpClient _client = new HttpClient(handler);
            HttpResponseMessage result;
            result = await _client.GetAsync("http://129.93.8.214/cgi-bin/ptzctrl.cgi?ptzcmd&" + cmd);
            result.EnsureSuccessStatusCode();
        }
        private void drawZoomingRect()
        {
            while (isZooming)
            {
                Point zoomBoxLoc = new Point();
                Size zoomBoxSize = new Size();
                //Size imageSize = captureImageBox.Image.Size;
                double theta = Math.Atan(videoImgSize.Width / videoImgSize.Height);
                zoomBoxLoc.X = Convert.ToInt32(startZoomPt.X - (zoomingRadius * Math.Sin(theta)));
                zoomBoxLoc.Y = Convert.ToInt32(startZoomPt.Y - (zoomingRadius * Math.Cos(theta)));
                zoomBoxSize.Width = 2 * (startZoomPt.X - zoomBoxLoc.X);
                zoomBoxSize.Height = 2 * (startZoomPt.Y - zoomBoxLoc.Y);
                Rectangle zoomBox = new Rectangle(zoomBoxLoc, zoomBoxSize);
                //draw zoomBox
                Image<Bgr, Byte> img = (Image<Bgr, Byte>)captureImageBox.Image;
                img.Draw(zoomBox, new Bgr(Color.Red), 2);
                captureImageBox.Image = img;
                //Thread.Sleep(10);
            }
        }

        //Events
        private void captureImageBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                if (!tmpPoints.Contains(e.Location))
                {
                    try
                    {
                        tmpPoints.Add(e.Location);
                        if (isFirstPoint)
                        {
                            getMarksList().Add(tmpPoints.ToArray());
                            isFirstPoint = false;
                        }
                        else
                        {
                            getMarksList()[getFigureNum()] = tmpPoints.ToArray();
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message, ex.ToString());
                    }
                }
            }
            else if (!startZoomPt.IsEmpty)
            {
                //calculate zoom radius
                int deltaX = Math.Abs(e.X - startZoomPt.X);
                int deltaY = Math.Abs(e.Y - startZoomPt.Y);
                zoomingRadius = Convert.ToInt32(Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
                if (zoomingRadius > 10)
                {
                    if (!isZooming)
                    {
                        //start thread to draw zooming box
                        Thread t = new Thread(new ThreadStart(drawZoomingRect));
                        t.IsBackground = true;
                        t.Start();
                    }
                    isZooming = true;
                }
                else
                {
                    isZooming = false;
                }
            }
        }
        private void captureImageBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;
                try
                {
                    for (int i = 0; i < tmpPoints.Count; i++)
                    {
                        tmpPoints[i].Offset((int)myMarkings.OffsetX, (int)myMarkings.OffsetY);
                    }
                    if (getFigureNum() < getMarksList().Count)
                    {
                        getMarksList()[getFigureNum()] = tmpPoints.ToArray();
                        tmpPoints.Clear();
                        setFigureNum(getFigureNum() + 1);
                    }
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message, ex.ToString());
                }
                lastColorUsed = penColor.Name;
                //btn_UndoMark.Visible = true;
                newFigure = true;
            }
            //If camera can be controlled over network and mouse down event occured in captureImageBox
            if (videoIsPTZ && !startZoomPt.IsEmpty)
            {
                //To center around a point, AXIS 215 PTZ needs: the point, image width, and image height
                Point frame = captureImageBox.Location;
                Point clickRelLoc = new Point(e.Location.X - frame.X + camClickPosFactor.X, e.Location.Y - frame.Y + camClickPosFactor.Y);
                //Size imageSize = captureImageBox.Image.Size;
                if (isZooming)
                {
                    //Calculate zoom factor
                    int zoomFactor = Convert.ToInt32(zoomScalingFactor * Math.Sqrt((videoImgSize.Width * videoImgSize.Width + videoImgSize.Height * videoImgSize.Height) / zoomingRadius));
                    //Send Zoom cammand to camera
                    startZoomPt.Offset(camZoomPosFactor.X, camZoomPosFactor.Y);
                    sendCmdToCamera("areazoom=" + startZoomPt.X + "," + startZoomPt.Y + "," + zoomFactor + "&imagewidth=" + videoImgSize.Width + "&imageheight=" + videoImgSize.Height);
                }
                else
                {
                    //center camera around click
                    //Send movement command to camera
                    sendCmdToCamera("center=" + clickRelLoc.X + "," + clickRelLoc.Y + "&imagewidth=" + videoImgSize.Width + "&imageheight=" + videoImgSize.Height);
                }
                startZoomPt = new Point();
                isZooming = false;
            }
        }
        private void captureImageBox_MouseDown(object sender, MouseEventArgs e)
        {
            if ((isMaster && _captureInProgress && !videoIsPTZ) || (!isMaster && !ConnectToMasterButton.Enabled && receiveMasterVideo))
            {
                isDrawing = true;
                isFirstPoint = true;
            }
            else if (videoIsPTZ)
            {
                startZoomPt = e.Location;
            }
        }
        private void ProcessFrame(object sender, EventArgs arg)
        {
            try
            {
                frame = _capture.RetrieveBgrFrame();
                frame = frame.Resize(((double)captureImageBox.Width / (double)frame.Width), Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);

                if (isMaster)
                {
                    //add Master's marks to image
                    if (myMarkings.RedMarkings.Count > 0)
                        frame.DrawPolyline(myMarkings.RedMarkings.ToArray(), false, new Bgr(Color.Red), penThickness);
                    if (myMarkings.BlackMarkings.Count > 0)
                        frame.DrawPolyline(myMarkings.BlackMarkings.ToArray(), false, new Bgr(Color.Black), penThickness);
                    if (myMarkings.BlueMarkings.Count > 0)
                        frame.DrawPolyline(myMarkings.BlueMarkings.ToArray(), false, new Bgr(Color.Blue), penThickness);
                    if (myMarkings.WhiteMarkings.Count > 0)
                        frame.DrawPolyline(myMarkings.WhiteMarkings.ToArray(), false, new Bgr(Color.White), penThickness);
                    if (myMarkings.YellowMarkings.Count > 0)
                        frame.DrawPolyline(myMarkings.YellowMarkings.ToArray(), false, new Bgr(Color.Yellow), penThickness);
                    if (myMarkings.GreenMarkings.Count > 0)
                        frame.DrawPolyline(myMarkings.GreenMarkings.ToArray(), false, new Bgr(Color.Green), penThickness);

                    //add Other marks to image
                    if (clientMarkings.RedMarkings.Count > 0)
                        frame.DrawPolyline(clientMarkings.RedMarkings.ToArray(), false, new Bgr(Color.Red), penThickness);
                    if (clientMarkings.BlackMarkings.Count > 0)
                        frame.DrawPolyline(clientMarkings.BlackMarkings.ToArray(), false, new Bgr(Color.Black), penThickness);
                    if (clientMarkings.BlueMarkings.Count > 0)
                        frame.DrawPolyline(clientMarkings.BlueMarkings.ToArray(), false, new Bgr(Color.Blue), penThickness);
                    if (clientMarkings.WhiteMarkings.Count > 0)
                        frame.DrawPolyline(clientMarkings.WhiteMarkings.ToArray(), false, new Bgr(Color.White), penThickness);
                    if (clientMarkings.YellowMarkings.Count > 0)
                        frame.DrawPolyline(clientMarkings.YellowMarkings.ToArray(), false, new Bgr(Color.Yellow), penThickness);
                    if (clientMarkings.GreenMarkings.Count > 0)
                        frame.DrawPolyline(clientMarkings.GreenMarkings.ToArray(), false, new Bgr(Color.Green), penThickness);
                }
                videoImgSize = frame.Size;
                captureImageBox.Image = frame;
                imgHasBeenProcessed = true;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        private void FlipHorizontalButtonClick(object sender, EventArgs e)
        {
            if (_capture != null) _capture.FlipHorizontal = !_capture.FlipHorizontal;
        }
        private void FlipVerticalButtonClick(object sender, EventArgs e)
        {
            if (_capture != null) _capture.FlipVertical = !_capture.FlipVertical;
        }
        private void btn_PenColor_Click(object sender, EventArgs e)
        {
            Button tmpBtn = (Button)sender;
            penColor = tmpBtn.BackColor;
        }
        private void btn_ClearMarks_Click(object sender, EventArgs e)
        {
            myMarkings.Clear();
            clientMarkings.Clear();
            tmpPoints = new List<Point>();
            redFigureNum = 0;
            blackFigureNum = 0;
            blueFigureNum = 0;
            whiteFigureNum = 0;
            yellowFigureNum = 0;
            greenFigureNum = 0;
            clearMarkingsReq = true;
            //send update for markings
            newFigure = true;
            btn_UndoMark.Visible = false;
            try
            {
                if (!isMaster)
                    sendDataTo(IPAddress.Parse(tb_ipAddress.Text));
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        private void btn_Capture_Click(object sender, EventArgs e)
        {
            try
            {
                if (_capture != null)
                {
                    if (_captureInProgress)
                    {  //stop the capture
                        btn_Capture.Text = "Start Capture";
                        _capture.Pause();
                    }
                    else
                    {
                        //start the capture
                        btn_Capture.Text = "Stop";
                        _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, captureImageBox.Height);
                        _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, captureImageBox.Width);
                        _capture.Start();
                    }

                    _captureInProgress = !_captureInProgress;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        private void btn_UndoMark_Click(object sender, EventArgs e)
        {
            tmpPoints = new List<Point>();
            switch (lastColorUsed)
            {
                case "Red":
                    myMarkings.RemoveFigure(myMarkings.RedMarkings[redFigureNum - 1]);
                    redFigureNum--;
                    break;
                case "Black":
                    myMarkings.RemoveFigure(myMarkings.BlackMarkings[blackFigureNum - 1]);
                    blackFigureNum--;
                    break;
                case "Blue":
                    myMarkings.RemoveFigure(myMarkings.BlueMarkings[blueFigureNum - 1]);
                    blueFigureNum--;
                    break;
                case "White":
                    myMarkings.RemoveFigure(myMarkings.WhiteMarkings[whiteFigureNum - 1]);
                    whiteFigureNum--;
                    break;
                case "Yellow":
                    myMarkings.RemoveFigure(myMarkings.YellowMarkings[yellowFigureNum - 1]);
                    yellowFigureNum--;
                    break;
                case "Green":
                    myMarkings.RemoveFigure(myMarkings.GreenMarkings[greenFigureNum - 1]);
                    greenFigureNum--;
                    break;
            }
            btn_UndoMark.Visible = false;
            newFigure = true;
            if (isMaster)
            {
                //if master, merge new local markings with client markings
                combinedMarkings = myMarkings.Merge(clientMarkings);
            }
        }
        private void InitializeOmnis_Click(object sender, EventArgs e)
        {
            if (!isMaster && tb_InstanceName.Text.Equals(""))
            {
                MessageBox.Show("Please enter a name.");
            }
            else
            {
                if (!cb_noOmnisAttached.Checked && spLeftOmni.SelectedIndex.Equals(spRightOmni.SelectedIndex))
                {
                    MessageBox.Show("Please select two different Omni Devices");
                }
                else
                {
                    int error = 0;

                    if (!cb_noOmnisAttached.Checked)
                    {
                        string Left = spLeftOmni.SelectedItem.ToString();
                        string Right = spRightOmni.SelectedItem.ToString();

                        try
                        {
                            error = initAndSchedule(Left, Right);
                            if (error == 1)
                            {
                                lock1();
                                lock2();

                                spLeftOmni.Enabled = false;
                                spRightOmni.Enabled = false;
                            }
                            else
                            {
                                ShowError("Omni initialization error. Please check connections and try again.", "");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    if (error == 1 || cb_noOmnisAttached.Checked)
                    {
                        if (!isMaster && myIPAddress == null)
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
            }
        }
        private void UnderlyingTimerTick(object sender, EventArgs e)
        {
            showOmniPositions();
            try
            {
                if (!isMaster)
                {
                    if (isInControl)
                        sendDataTo(IPAddress.Parse(tb_ipAddress.Text));
                }
                else
                {
                    //disperse inControl Position
                    if (connectedClients.Count > 0)
                        masterSendData();
                }

                if (!isInControl)
                {
                    if (inControlPosition == null)
                        setForces(new OmniPosition());
                    else
                        setForces(inControlPosition.Subtract(positionOffset));
                }
            }
            catch (Exception ex)
            {
                UnderlyingTimer.Enabled = false;
                ShowError("An error occured during routine program timer. Application must be reset.", ex.Message);
            }
        }
        private void ConnectToMasterButtonClick(object sender, EventArgs e)
        {
            TcpClient c = new TcpClient();
            try
            {
                c.Connect(IPAddress.Parse(tb_ipAddress.Text), connectionPort);
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.TimedOut)
                    ShowError("Cannot connect to Master... Try Again.", ex.Message);
                else
                    ShowError(ex.Message, ex.ToString());
            }
            bool connected = c.Connected;
            if (connected)
            {
                try
                {
                    inControlIP = tb_ipAddress.Text;
                    isConnectedToMaster = true;
                    SocketMessage sm = new SocketMessage(tb_InstanceName.Text, myIPAddress);
                    sm.hasOmnis = !cb_noOmnisAttached.Checked;
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
        private void btn_zeroForces_Click(object sender, EventArgs e)
        {
            try
            {
                IntPtr ptr = getpos1();
                double[] pos1 = new double[8];
                Marshal.Copy(ptr, pos1, 0, 8);

                ReleaseMemory(ptr);

                IntPtr ptr2 = getpos2();
                double[] pos2 = new double[8];
                Marshal.Copy(ptr2, pos2, 0, 8);

                ReleaseMemory(ptr2);

                forceOffset_LX = pos1[0] - inControlPosition.LeftX;
                forceOffset_LY = pos1[1] - inControlPosition.LeftY;
                forceOffset_LZ = pos1[2] - inControlPosition.LeftZ;
                forceOffset_RX = pos2[0] - inControlPosition.RightX;
                forceOffset_RY = pos2[1] - inControlPosition.RightY;
                forceOffset_RZ = pos2[2] - inControlPosition.RightZ;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        private void cb_isMaster_CheckedChanged(object sender, EventArgs e)
        {
            isMaster = cb_isMaster.Checked;
            if (isMaster)
            {
                //user wants to be master
                lbl_myIP.Text = "My IP Address";
                if (myIPAddress == null)
                    SetMyIP();
                tb_ipAddress.Text = myIPAddress;
            }
            else
            {
                lbl_myIP.Text = "Master's IP Address";
                tb_ipAddress.Text = "";
            }
            tb_ipAddress.Enabled = !isMaster;
            //cb_noOmnisAttached.Visible = !isChecked;
            btn_Capture.Visible = isMaster;
        }
        private void cb_noOmnisAttached_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_noOmnisAttached.Checked)
            {
                //isMaster = false;
                //cb_isMaster.Enabled = false;
                feedbackEnabled = false;
                //groupBox3.Text = "Master's Position";
                btn_zeroForces.Enabled = false;
                //lbl_myIP.Text = "Master's IP Address";
                spLeftOmni.Enabled = false;
                spRightOmni.Enabled = false;
            }
            else
            {
                cb_isMaster.Enabled = true;
                groupBox3.Text = "Forces";
                btn_zeroForces.Enabled = true;
                spLeftOmni.Enabled = true;
                spRightOmni.Enabled = true;
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                applicationRunning = false;

                if (!btn_Initialize.Enabled)
                {
                    unlock1();
                    unlock2();
                    stopAndDisable();
                }

                if (_capture != null && _captureInProgress)
                    _capture.Stop();

                if (!isMaster && !ConnectToMasterButton.Enabled)
                    disconnect(null);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        private void btn_StartAudio_Click(object sender, EventArgs e)
        {
            try
            {
                if (audioIsRunning)
                {
                    audioIsRunning = false;
                    audioServer.Dispose();
                    audioServer = null;
                    audioWaveOut.Dispose();
                    audioWaveOut = null;

                    audioWaveIn.Dispose();
                    audioWaveIn = null;
                    btn_StartAudio.BackColor = Color.Green;
                    ddl_AudioDevices.Enabled = true;
                }
                else
                {
                    audioIsRunning = true;
                    audioWaveOut = new WaveOut(WaveOut.Devices[0], 8000, 16, 1);
                    audioServer = new UdpServer();
                    audioServer.Bindings = new IPEndPoint[] { new IPEndPoint(IPAddress.Parse(myIPAddress), audioPort) };
                    audioServer.PacketReceived += new PacketReceivedHandler(AudioServer_PacketReceived);
                    audioServer.Start();

                    audioWaveIn = new WaveIn(WaveIn.Devices[ddl_AudioDevices.SelectedIndex], 8000, 16, 1, 400);
                    audioWaveIn.BufferFull += new BufferFullHandler(audioWaveIn_BufferFull);
                    audioWaveIn.Start();
                    btn_StartAudio.BackColor = Color.Red;
                    ddl_AudioDevices.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ShowError("Could not start/stop audio. Check input/output device.", ex.Message);
            }
        }
        private void audioWaveIn_BufferFull(byte[] buffer)
        {
            // Compress data. 
            byte[] encodedData = null;
            if (m_Codec == 0)
            {
                encodedData = G711.Encode_aLaw(buffer, 0, buffer.Length);
            }
            else if (m_Codec == 1)
            {
                encodedData = G711.Encode_uLaw(buffer, 0, buffer.Length);
            }

            // We just sent buffer to target end point.
            foreach (string addr in connectedClients)
            {
                //Send to all other clients
                if (!addr.Equals(myIPAddress))
                    audioServer.SendPacket(encodedData, 0, encodedData.Length, new IPEndPoint(IPAddress.Parse(addr), audioPort));
            }
            //If client, send to master
            if (!isMaster)
                audioServer.SendPacket(encodedData, 0, encodedData.Length, new IPEndPoint(IPAddress.Parse(tb_ipAddress.Text), audioPort));
        }
        private void AudioServer_PacketReceived(UdpPacket_eArgs e)
        {
            // Decompress data.
            byte[] decodedData = null;
            if (m_Codec == 0)
            {
                decodedData = G711.Decode_aLaw(e.Data, 0, e.Data.Length);
            }
            else if (m_Codec == 1)
            {
                decodedData = G711.Decode_uLaw(e.Data, 0, e.Data.Length);
            }

            // We just play received packet.
            try
            {
                audioWaveOut.Play(decodedData, 0, decodedData.Length);
            }
            catch (Exception ex)
            {
                ShowError("Could not play received audio.", ex.Message);
            }
        }
        void errorTimer_Tick(object sender, EventArgs e)
        {
            lbl_Errors.Text = "";
            errorTimer.Enabled = false;
            errorTimer.Stop();
        }
        private void btn_ReqControl_Click(object sender, EventArgs e)
        {
            sendGRAddr = inControlIP;
            Thread t = new Thread(new ParameterizedThreadStart(sendGrantReq));
            t.IsBackground = true;
            t.Start(false);
            btn_ReqControl.Enabled = false;
        }
        private void sendClientGrantReq(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;
            sendGRAddr = btn.ToolTipText;
            Thread t = new Thread(new ParameterizedThreadStart(sendGrantReq));
            t.IsBackground = true;
            t.Start(false);
        }
        private void networkDelayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NetDelay _netDelay;
            if (networkDelay.Equals(0))
                _netDelay = new NetDelay(this, false, networkDelay);
            else
                _netDelay = new NetDelay(this, true, networkDelay);

            _netDelay.Show();
        }
        private void connectButtonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectButtons cbForm = new ConnectButtons(this);
            cbForm.Show();
        }
        private void changeMyIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeMyIP changeMyIP = new ChangeMyIP(this);
            changeMyIP.Show();
        }
        private void changeVideoSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeVideoSource changeVidSource = new ChangeVideoSource(this);
            changeVidSource.Show();
        }
        private void addIPCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IPCameras ipCamerasForm = new IPCameras();
            ipCamerasForm.Show();
        }
        private void lbl_Zoom_Click(object sender, EventArgs e)
        {

                Label l = (Label)sender;
                int zoom = 0;
                if (l.Text.Equals("+"))
                    zoom = 2000;
                else
                    zoom = -2000;
                sendCmdToCamera("rzoom=" + zoom);

        }
        private void iPCameraControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CameraControl cameraControlForm = new CameraControl();
            cameraControlForm.Show();
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (logDataTurnAroundTime)
                turnAroundTimer.Stop();

            logDataTurnAroundTime = !logDataTurnAroundTime;
            toolStripMenuItem1.Checked = logDataTurnAroundTime;

        }

        //Thread Methods
        private void listenForNewConnections()
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Parse(myIPAddress), connectionPort);
                listener.Start();
                while (applicationRunning)
                {
                    //Accept Incoming Connection
                    TcpClient client = listener.AcceptTcpClient();
                    //get Name and IP of Incoming Connection
                    Stream s = client.GetStream();
                    byte[] arry = new byte[2000];
                    s.Read(arry, 0, arry.Length);
                    string json = Encoding.ASCII.GetString(arry);
                    SocketMessage sm = JsonConvert.DeserializeObject<SocketMessage>(json);
                    //if client already connected, treat as a disconnect
                    if (connectedClients.Contains(sm.IPAddress))
                    {
                        disconnect(sm.IPAddress);
                    }
                    else
                    {
                        if (lbl_Connections.Text.Equals("Connections: None"))
                            lbl_Connections.Text = "Connections: ";

                        if (!sm.hasOmnis)
                        {
                            //If client does not have Omnis, don't allow control to be given
                            lbl_Connections.Text += sm.Name;
                        }
                        else
                        {
                            string dir = Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"..\..\Content\pc.png");
                            Image clientImg = Image.FromFile(dir);
                            ToolStripItem newItem = new ToolStripButton(sm.Name, clientImg, sendClientGrantReq, "btn_" + sm.Name);
                            newItem.ToolTipText = sm.IPAddress;
                            ss_Connections.Items.Add(newItem);
                        }

                        if (connectedClients.Count == 0)
                        {
                            Thread sendVideoThread = new Thread(new ThreadStart(sendVideo));
                            sendVideoThread.IsBackground = true;
                            sendVideoThread.Start();

                            Thread listenForNewMarkings = new Thread(new ThreadStart(listenForMarkings));
                            listenForNewMarkings.IsBackground = true;
                            listenForNewMarkings.Start();

                            Thread listenForControlReq = new Thread(new ThreadStart(listenForGrantReq));
                            listenForControlReq.IsBackground = true;
                            listenForControlReq.Start();

                            Thread readFromDataBuffer = new Thread(new ThreadStart(readDataBuffer));
                            readFromDataBuffer.IsBackground = true;
                            readFromDataBuffer.Start();
                        }
                        connectedClients.Add(sm.IPAddress);

                        //Log Connection
                        logMessage("Connection successfully made to "+sm.Name+".", "A client successfully connected to this machine with the name "+sm.Name+" and address "+sm.IPAddress+" .", Logging.StatusTypes.Running);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        private void listenForMarkings()
        {
            try
            {
                if (markingsListener == null)
                    markingsListener = new UdpClient(markingsPort);

                markingsListener.BeginReceive(new AsyncCallback(markingsReceived), null);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        private async void markingsReceived(IAsyncResult ar)
        {
            await Task.Delay(networkDelay);
            IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, markingsPort);
            byte[] arry = markingsListener.EndReceive(ar, ref clientEP);
            string json = Encoding.ASCII.GetString(arry);
            clientMarkings = JsonConvert.DeserializeObject<Markings>(json);
            combinedMarkings = myMarkings.Merge(clientMarkings);

            if (applicationRunning)
                listenForMarkings();
        }
        private void sendMarkings()
        {
            try
            {
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                while (true)
                {
                    if (newFigure)
                    {
                        string json = JsonConvert.SerializeObject(myMarkings);
                        byte[] arry = Encoding.ASCII.GetBytes(json);
                        s.SendTo(arry, new IPEndPoint(IPAddress.Parse(tb_ipAddress.Text), markingsPort));
                        newFigure = false;
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        private void sendVideo()
        {
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 20L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            while (_captureInProgress)
            {
                try
                {
                    if (imgHasBeenProcessed)
                    {
                        IImage frame = captureImageBox.Image;
                        Bitmap imgToSend = frame.Bitmap;
                        MemoryStream ms = new MemoryStream();
                        imgToSend.Save(ms, jpgEncoder, myEncoderParameters);
                        byte[] arry = ms.ToArray();

                        foreach (string a in connectedClients)
                        {
                            s.SendTo(arry, new IPEndPoint(IPAddress.Parse(a), videoPort));
                        }
                        imgHasBeenProcessed = false;
                    }
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message, ex.ToString());
                }
            }
        }
        private void masterSendData()
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            SocketMessage sm = new SocketMessage("Master", myIPAddress);
            try
            {
                if (isFrozen)
                    sm.OmniPosition = frozenPosition;
                else
                    sm.OmniPosition = inControlPosition;
                sm.PositionOffset = positionOffset;
                sm.isFrozen = isFrozen;
                sm.ClearMarkingsReq = clearMarkingsReq;
                sm.Forces = new double[] {sendForce_LX, sendForce_LY, sendForce_LZ, sendForce_RX, sendForce_RY, sendForce_RZ};
                sm.sendFreezeCmd = sendFreezeComd;
                string json = JsonConvert.SerializeObject(sm);
                byte[] arry = Encoding.ASCII.GetBytes(json);
                foreach (string a in connectedClients)
                {
                    if (!a.Equals(myIPAddress))
                        s.SendTo(arry, new IPEndPoint(IPAddress.Parse(a), dataPort));
                }
                sendFreezeComd = false;
                clearMarkingsReq = false;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        private void sendDataTo(IPAddress addr)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            SocketMessage sm = new SocketMessage(myName, myIPAddress);
            try
            {
                if (isFrozen)
                    sm.OmniPosition = frozenPosition;
                else
                    sm.OmniPosition = inControlPosition;
                sm.PositionOffset = positionOffset;
                sm.isFrozen = isFrozen;
                sm.ClearMarkingsReq = clearMarkingsReq;
                string json = JsonConvert.SerializeObject(sm);
                byte[] arry = Encoding.ASCII.GetBytes(json);
                s.SendTo(arry, new IPEndPoint(addr, dataPort));
                clearMarkingsReq = false;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        private void listenForData()
        {
            if (dataListener == null)
                dataListener = new UdpClient(dataPort);
            try
            {
                dataListener.BeginReceive(new AsyncCallback(dataReceived), null);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }
        private void dataReceived(IAsyncResult ar)
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, dataPort);
                byte[] arry = dataListener.EndReceive(ar, ref ep);
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

                string json = Encoding.ASCII.GetString(arry);
                SocketMessage dataMsg = JsonConvert.DeserializeObject<SocketMessage>(json);
                dataBuffer.Enqueue(dataMsg);
                dataAvailable = true;
                if (networkDataDelayChanged && networkDelay > 0 && !dataWatch.IsRunning)
                {
                    networkDataDelayChanged = false;
                    dataWatch.Start();
                }
                if (dataMsg.ClearMarkingsReq)
                {
                    //clear local markings
                    myMarkings.Clear();
                    tmpPoints = new List<Point>();
                    redFigureNum = 0;
                    blackFigureNum = 0;
                    blueFigureNum = 0;
                    whiteFigureNum = 0;
                    yellowFigureNum = 0;
                    greenFigureNum = 0;
                    btn_UndoMark.Visible = false;
                    if (isMaster)
                    {
                        combinedMarkings.Clear();
                        clientMarkings.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                //Just log error instead of showing, currently too many non-critical errors
                logMessage(ex.Message, ex.ToString(), Logging.StatusTypes.Error);
            }
            
            hasMasterData = true;
            //if (isListeningForData)
            listenForData();
        }
        private void listenForVideo()
        {
            IPEndPoint masterEP = new IPEndPoint(IPAddress.Parse(tb_ipAddress.Text), videoPort);
            while (applicationRunning)
            {
                try
                {
                    if (videoListener == null)
                        videoListener = new UdpClient(videoPort);

                    //videoListener.BeginReceive(new AsyncCallback(videoImgReceived), null);
                    byte[] msg = videoListener.Receive(ref masterEP);
                    Image<Bgr, Byte> receivedImg = Image<Bgr, Byte>.FromRawImageData(msg);
                    if (isDrawing)
                        receivedImg.DrawPolyline(getMarksList().ToArray(), false, new Bgr(penColor), penThickness);
                    captureImageBox.Image = receivedImg;
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message, ex.ToString());
                }
            }
        }
        private void videoImgReceived(IAsyncResult ar)
        {
            try
            {
                IPEndPoint masterEP = new IPEndPoint(IPAddress.Parse(tb_ipAddress.Text), videoPort);
                byte[] arry = videoListener.EndReceive(ar, ref masterEP);
                Image<Bgr, Byte> receivedImg = Image<Bgr, Byte>.FromRawImageData(arry);
                //Image<Bgr, Byte> resizedImg = receivedImg.Resize(((double)captureImageBox.Width / (double)receivedImg.Width), Emgu.CV.CvEnum.INTER.CV_INTER_AREA);
                if (isDrawing)
                    receivedImg.DrawPolyline(getMarksList().ToArray(), false, new Bgr(penColor), penThickness);
                videoBuffer.Enqueue(receivedImg);
                videoImageAvailable = true;
                if (networkVideoDelayChanged && networkDelay > 0 && !videoWatch.IsRunning)
                {
                    networkVideoDelayChanged = false;
                    videoWatch.Start();
                }
                //myMarkings.OffsetX = receivedImg.Width - resizedImg.Width;
                //myMarkings.OffsetY = receivedImg.Height - resizedImg.Height;
                if (applicationRunning && receiveMasterVideo)
                    listenForVideo();
            }
            catch (Exception ex)
            {
                logMessage(ex.Message, ex.ToString(), Logging.StatusTypes.Error);
            }
        }
        private void listenForGrantReq()
        {
            try
            {
                if (grantReqListener == null)
                {
                    grantReqListener = new TcpListener(IPAddress.Parse(myIPAddress), controlPort);
                    grantReqListener.Start();
                }
                TcpClient client = grantReqListener.AcceptTcpClient();
                IPEndPoint remoteEP = (IPEndPoint)client.Client.RemoteEndPoint;

                if (applicationRunning)
                {
                    Thread t = new Thread(new ThreadStart(listenForGrantReq));
                    t.IsBackground = true;
                    t.Start();
                }

                //check if emergency switch
                int emergBit = 0;
                Stream s = client.GetStream();
                byte[] arry = new byte[1];
                s.Read(arry, 0, arry.Length);
                emergBit = arry[0];

                if (emergBit.Equals(0))
                {
                    //normal switch procedure
                    if (isInControl)
                    {
                        DialogResult res = MessageBox.Show("Grant control?", remoteEP.Address.ToString() + " has requested control.", MessageBoxButtons.YesNo);
                        if (res == System.Windows.Forms.DialogResult.Yes)
                        {
                            //send grant
                            arry = new byte[1] { 1 };
                            s.Write(arry, 0, arry.Length);

                            switchControl(false, remoteEP.Address.ToString());
                        }
                        else
                        {
                            //send Deny
                            arry = new byte[1] { 0 };
                            s.Write(arry, 0, arry.Length);
                        }
                    }
                    else
                    {
                        DialogResult res = MessageBox.Show("Accept control?", remoteEP.Address.ToString() + " wants to give you control.", MessageBoxButtons.YesNo);
                        if (res == System.Windows.Forms.DialogResult.Yes)
                        {
                            //send accept
                            arry = new byte[1] { 1 };
                            s.Write(arry, 0, arry.Length);

                            //take control
                            switchControl(true, myIPAddress);
                            //stop listening for position
                            //isListeningForData = false;
                        }
                        else
                        {
                            //send decline
                            arry = new byte[1] { 0 };
                            s.Write(arry, 0, arry.Length);
                        }

                    }
                }
                else
                {
                    //emergency switch procedure
                    arry = new byte[1] { 1 };
                    s.Write(arry, 0, arry.Length);

                    switchControl(false, remoteEP.Address.ToString());
                }
                s.Close();
                btn_ReqControl.Enabled = true;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message, ex.ToString());
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            AssignButtons form = new AssignButtons(this);
            form.ShowDialog();
        }
    }
}

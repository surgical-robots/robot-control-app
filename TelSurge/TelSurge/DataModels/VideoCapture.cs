using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace TelSurge
{
    public class VideoCapture
    {
        private TelSurgeMain Main;
        private Capture _capture = null;
        public bool IsCapturing = false;
        private int videoPort;
        private UdpClient videoListener = null;
        public bool IsListeningForVideo { get; set; }
        public enum CaptureType { Local, IP, MasterFeed }
        private CaptureType capturingType;
        public bool IsStreaming { get; set; }
        public string CaptureDevice { get; set; }
        public string PTZAddress { get; set; }
        /*
        private bool _captureInProgress = false;
        
        private double repositioningError = 15;
        
        private bool imgHasBeenProcessed = false;
        public Queue<Image<Bgr, Byte>> videoBuffer = new Queue<Image<Bgr, byte>>();
        private bool videoImageAvailable = false;
        private System.Diagnostics.Stopwatch videoWatch = new System.Diagnostics.Stopwatch();
        public bool networkVideoDelayChanged = false;
        private bool receiveMasterVideo = true;
        public bool videoIsPTZ = false;
        private volatile bool isZooming = false;
        private int zoomingRadius = 0;
        private Point startZoomPt;
        private Size videoImgSize;
        private int zoomScalingFactor = 5;
        private Point camClickPosFactor = new Point(10, 70);
        private Point camZoomPosFactor = new Point(-35, 15);
        private Image<Bgr, Byte> frame;
        */

        public VideoCapture(TelSurgeMain Main, int VideoPort)
        {
            this.Main = Main;
            this.videoPort = VideoPort;
            this.IsListeningForVideo = false;
            this.IsStreaming = false;
            this.CaptureDevice = "";
            this.PTZAddress = "";
        }
        private Image<Bgr, byte> addMarkup(Image<Bgr, byte> Frame) 
        {
            if (Main.Markup.MyMarkings.RedMarkings.Count > 0)
                Frame.DrawPolyline(Main.Markup.MyMarkings.GetAllPaths(Main.Markup.MyMarkings.RedMarkings), false, new Bgr(Color.Red), Main.Markup.PenThickness);
            if (Main.Markup.MyMarkings.BlackMarkings.Count > 0)
                Frame.DrawPolyline(Main.Markup.MyMarkings.GetAllPaths(Main.Markup.MyMarkings.BlackMarkings), false, new Bgr(Color.Black), Main.Markup.PenThickness);
            if (Main.Markup.MyMarkings.BlueMarkings.Count > 0)
                Frame.DrawPolyline(Main.Markup.MyMarkings.GetAllPaths(Main.Markup.MyMarkings.BlueMarkings), false, new Bgr(Color.Blue), Main.Markup.PenThickness);
            if (Main.Markup.MyMarkings.WhiteMarkings.Count > 0)
                Frame.DrawPolyline(Main.Markup.MyMarkings.GetAllPaths(Main.Markup.MyMarkings.WhiteMarkings), false, new Bgr(Color.White), Main.Markup.PenThickness);
            if (Main.Markup.MyMarkings.YellowMarkings.Count > 0)
                Frame.DrawPolyline(Main.Markup.MyMarkings.GetAllPaths(Main.Markup.MyMarkings.YellowMarkings), false, new Bgr(Color.Yellow), Main.Markup.PenThickness);
            if (Main.Markup.MyMarkings.GreenMarkings.Count > 0)
                Frame.DrawPolyline(Main.Markup.MyMarkings.GetAllPaths(Main.Markup.MyMarkings.GreenMarkings), false, new Bgr(Color.Green), Main.Markup.PenThickness);
            return Frame;
        }
        private void ProcessFrame(object sender, EventArgs arg)
        {
            try
            {
                Image<Bgr, byte> frame =  new Image<Bgr,byte>(100, 100);
                if (_capture != null)
                    frame = _capture.RetrieveBgrFrame();
                if (frame != null)
                {
                    frame = frame.Resize(((double)Main.CaptureImageBox.Width / (double)frame.Width), Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                    frame = addMarkup(frame);
                    Main.ShowVideoFrame(frame);
                    sendVideoStream(frame);
                }
            }
            catch (AccessViolationException ave)
            {
                Main.ShowError(ave.Message, ave.ToString());
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
        }
        public void ListenForVideo() 
        {
            if (videoListener == null)
                videoListener = new UdpClient(videoPort);

            videoListener.BeginReceive(new AsyncCallback(videoImgReceived), null);
        }
        private void videoImgReceived(IAsyncResult Ar)
        {
            IPEndPoint masterEP = new IPEndPoint(IPAddress.Parse(Main.Surgery.Master.MyIPAddress), videoPort);
            byte[] arry = videoListener.EndReceive(Ar, ref masterEP);
            Image<Bgr, Byte> receivedImg = Image<Bgr, Byte>.FromRawImageData(arry);
            //Image<Bgr, Byte> resizedImg = receivedImg.Resize(((double)captureImageBox.Width / (double)receivedImg.Width), Emgu.CV.CvEnum.INTER.CV_INTER_AREA);
            receivedImg = addMarkup(receivedImg);
            //myMarkings.OffsetX = receivedImg.Width - resizedImg.Width;
            //myMarkings.OffsetY = receivedImg.Height - resizedImg.Height;
            Main.CaptureImageBox.Image = receivedImg;

            if (IsListeningForVideo)
                ListenForVideo();
        }
        private void sendVideoStream(IImage Frame) 
        {
            if (IsStreaming)
            {
                ImageCodecInfo jpgEncoder = getEncoder(ImageFormat.Jpeg);
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 20L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                Bitmap imgToSend = Frame.Bitmap;
                MemoryStream ms = new MemoryStream();
                imgToSend.Save(ms, jpgEncoder, myEncoderParameters);
                byte[] arry = ms.ToArray();

                foreach (User usr in Main.Surgery.ConnectedClients)
                {
                    s.SendTo(arry, new IPEndPoint(IPAddress.Parse(usr.MyIPAddress), videoPort));
                }
            }
        }
        public void SwitchVideoFeed(CaptureType type) 
        {
            try
            {
                bool switchWhileCapturing = IsCapturing;
                capturingType = type;
                
                if (IsCapturing)
                    StopCapturing();
                if (type.Equals(CaptureType.MasterFeed))
                {
                    if (!Main.User.ConnectedToMaster)
                        Main.ShowError("Master is not connected! No video feed.", "Tried to receive Master video feed without a connection to Master. (ChangeVideoSource)");
                    else
                    {
                        IsListeningForVideo = true;
                        Thread t = new Thread(new ThreadStart(ListenForVideo));
                        t.IsBackground = true;
                        t.Start();
                    }
                }
                else
                {
                    if (!Main.User.IsMaster)
                        IsListeningForVideo = false;
                    
                    if (switchWhileCapturing || !Main.User.IsMaster)
                        StartCapturing();
                }
                capturingType = type;
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
        }
        private void releaseData()
        {
            if (_capture != null)
            {
                _capture.Dispose();
            }
        }
        private ImageCodecInfo getEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        public void StartCapturing()
        {
            try
            {
                if (CaptureDevice.Equals(""))
                    _capture = new Capture();
                else
                {
                    if (capturingType.Equals(CaptureType.Local))
                        _capture = new Capture(Convert.ToInt32(CaptureDevice));
                    else if (capturingType.Equals(CaptureType.IP))
                        _capture = new Capture(CaptureDevice);

                }
                _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS, 40);
                _capture.ImageGrabbed += ProcessFrame;
                _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, 720);
                _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, 1280);
                _capture.Start();
                IsCapturing = true;
            }
            catch (Exception) { }
        }
        public void StopCapturing()
        {
            if (IsCapturing)
            {
                _capture.Stop();
                releaseData();
                IsCapturing = false;
            }
        }

        
        /*
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
        */

    }
}

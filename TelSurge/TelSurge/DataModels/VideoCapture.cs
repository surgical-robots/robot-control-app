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
    class VideoCapture
    {
        private TelSurgeMain Main;
        private Markup Markup;
        private Surgery Surgery;
        private User User;
        private Capture _capture = null;
        public bool IsCapturing = false;
        private int videoPort;
        private UdpClient videoListener = null;
        public bool IsListeningForVideo { get; set; }
        public enum CaptureType { Local, IP, MasterFeed }
        private CaptureType CapturingType;
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

        public VideoCapture(TelSurgeMain MainForm, Markup Markup, Surgery Surgery, User User, int VideoPort)
        {
            this.Main = MainForm;
            this.Markup = Markup;
            this.Surgery = Surgery;
            this.User = User;
            this.videoPort = VideoPort;
            this.IsListeningForVideo = false;
            if (User.IsMaster)
                this.CapturingType = CaptureType.Local;
            else
                this.CapturingType = CaptureType.MasterFeed;
        }
        private void addMarkup(Image<Bgr, byte> Frame) 
        {
            if (Markup.MyMarkings.RedMarkings.Count > 0)
                Frame.DrawPolyline(Markup.MyMarkings.GetAllPaths(Markup.MyMarkings.RedMarkings), false, new Bgr(Color.Red), Markup.PenThickness);
            if (Markup.MyMarkings.BlackMarkings.Count > 0)
                Frame.DrawPolyline(Markup.MyMarkings.GetAllPaths(Markup.MyMarkings.BlackMarkings), false, new Bgr(Color.Black), Markup.PenThickness);
            if (Markup.MyMarkings.BlueMarkings.Count > 0)
                Frame.DrawPolyline(Markup.MyMarkings.GetAllPaths(Markup.MyMarkings.BlueMarkings), false, new Bgr(Color.Blue), Markup.PenThickness);
            if (Markup.MyMarkings.WhiteMarkings.Count > 0)
                Frame.DrawPolyline(Markup.MyMarkings.GetAllPaths(Markup.MyMarkings.WhiteMarkings), false, new Bgr(Color.White), Markup.PenThickness);
            if (Markup.MyMarkings.YellowMarkings.Count > 0)
                Frame.DrawPolyline(Markup.MyMarkings.GetAllPaths(Markup.MyMarkings.YellowMarkings), false, new Bgr(Color.Yellow), Markup.PenThickness);
            if (Markup.MyMarkings.GreenMarkings.Count > 0)
                Frame.DrawPolyline(Markup.MyMarkings.GetAllPaths(Markup.MyMarkings.GreenMarkings), false, new Bgr(Color.Green), Markup.PenThickness);
        }
        private void ProcessFrame(object sender, EventArgs arg)
        {
            try
            {
                Image<Bgr, byte> frame = _capture.RetrieveBgrFrame();
                frame = frame.Resize(((double)Main.CaptureImageBox.Width / (double)frame.Width), Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                addMarkup(frame);
                Main.CaptureImageBox.Image = frame;
                sendVideoStream(frame);
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
        }
        public void ChangeCaptureMode(double Height, double Width)
        {
            try
            {
                if (_capture != null)
                {
                    if (IsCapturing)
                    {  //stop the capture
                        _capture.Stop();
                    }
                    else
                    {
                        //start the capture
                        _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, Height);
                        _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, Width);
                        _capture.Start();
                    }

                    IsCapturing = !IsCapturing;
                }
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
            try
            {
                videoListener.BeginReceive(new AsyncCallback(videoImgReceived), null);
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
        }
        private void videoImgReceived(IAsyncResult Ar)
        {
            try
            {
                IPEndPoint masterEP = new IPEndPoint(IPAddress.Parse(Surgery.Master.MyIPAddress), videoPort);
                byte[] arry = videoListener.EndReceive(Ar, ref masterEP);
                Image<Bgr, Byte> receivedImg = Image<Bgr, Byte>.FromRawImageData(arry);
                //Image<Bgr, Byte> resizedImg = receivedImg.Resize(((double)captureImageBox.Width / (double)receivedImg.Width), Emgu.CV.CvEnum.INTER.CV_INTER_AREA);
                addMarkup(receivedImg);
                //myMarkings.OffsetX = receivedImg.Width - resizedImg.Width;
                //myMarkings.OffsetY = receivedImg.Height - resizedImg.Height;
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
            if (IsListeningForVideo)
                ListenForVideo();
        }
        private void sendVideoStream(IImage Frame) 
        {
            ImageCodecInfo jpgEncoder = getEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 20L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                Bitmap imgToSend = Frame.Bitmap;
                MemoryStream ms = new MemoryStream();
                imgToSend.Save(ms, jpgEncoder, myEncoderParameters);
                byte[] arry = ms.ToArray();

                foreach (User usr in Surgery.ConnectedClients)
                {
                    s.SendTo(arry, new IPEndPoint(IPAddress.Parse(usr.MyIPAddress), videoPort));
                }
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
        }
        private void switchVideoFeed(CaptureType type, string deviceInfo) 
        {
            if (IsCapturing)
            {
                _capture.Stop();
                releaseData();
            }
            if (type.Equals(CaptureType.MasterFeed))
            {
                if (!User.ConnectedToMaster)
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
                if (!User.IsMaster)
                    IsListeningForVideo = false;
                if (type.Equals(CaptureType.Local))
                    _capture = new Capture(Convert.ToInt32(deviceInfo));
                else if (type.Equals(CaptureType.IP))
                    _capture = new Capture(deviceInfo);
                _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS, 30);
                _capture.ImageGrabbed += ProcessFrame;
                if (IsCapturing)
                {
                    _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, Main.CaptureImageBox.Height);
                    _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, Main.CaptureImageBox.Width);
                    _capture.Start();
                }
            }
        }
        private void releaseData()
        {
            if (_capture != null) _capture.Dispose();
        }
        private ImageCodecInfo getEncoder(ImageFormat format)
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
                Main.ShowError(ex.Message, ex.ToString());
            }
            return null;
        }
        public void StartCapturing()
        {
            IsCapturing = true;
            switchVideoFeed(VideoCapture.CaptureType.Local, "0");
        }
        public void StopCapturing()
        {
            if (IsCapturing)
            {
                _capture.Stop();
                releaseData();
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

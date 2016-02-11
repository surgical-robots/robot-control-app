using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using TelSurge.DataModels;

namespace TelSurge
{
    class VideoCapture
    {
        private Capture _capture = null;
        private TelSurgeMain mainForm;
        private UdpClient videoListener = null;
        private int videoPort = 11000;
        private bool _captureInProgress;
        private bool imgHasBeenProcessed = false;
        public Queue<Image<Bgr, Byte>> videoBuffer = new Queue<Image<Bgr, byte>>();
        private bool videoImageAvailable = false;
        private System.Diagnostics.Stopwatch videoWatch = new System.Diagnostics.Stopwatch();
        private MarkUp markUp = new MarkUp();
        private IPEndPoint masterEP = null;
        public bool networkVideoDelayChanged = false;
        private string ListeningTo;
        ImageCodecInfo jpgEncoder;
        System.Drawing.Imaging.Encoder myEncoder;
        EncoderParameters myEncoderParameters;
        EncoderParameter myEncoderParameter;
        Socket s;
        

        public VideoCapture(TelSurgeMain telSurgeMain)
        {
            mainForm = telSurgeMain;
            //try to set up capture from default camera
            try
            {
                _capture = new Capture();
                _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS, 30);
                _capture.ImageGrabbed += ProcessFrame;

                jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                myEncoder = System.Drawing.Imaging.Encoder.Quality;
                myEncoderParameters = new EncoderParameters(1);
                myEncoderParameter = new EncoderParameter(myEncoder, 20L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            }
            catch (NullReferenceException nrex)
            {
                if (nrex.HResult == -2147467261)
                    mainForm.ShowError("Cannot connect to default camera!", "No camera could be found on this machine, (" + Environment.MachineName + ").");
                else
                    mainForm.ShowError(nrex.Message, nrex.ToString());
            }
            catch (Exception ex)
            {
                mainForm.ShowError(ex.Message, ex.ToString());
            }
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            try
            {
                Image<Bgr, Byte> frame = _capture.RetrieveBgrFrame();
                frame = frame.Resize(((double)mainForm.captureImageBox.Width / (double)frame.Width), Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                mainForm.captureImageBox.Image = markUp.OverlayMarkUp(frame);
                imgHasBeenProcessed = true;
            }
            catch (Exception ex)
            {
                mainForm.ShowError(ex.Message, ex.ToString());
            }
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
                mainForm.ShowError(ex.Message, ex.ToString());
            }
            return null;
        }
        private void sendVideo(List<string> SendToIPAddresses)
        {
            

            try
            {
                if (imgHasBeenProcessed)
                {
                    IImage frame = mainForm.captureImageBox.Image;
                    Bitmap imgToSend = frame.Bitmap;
                    MemoryStream ms = new MemoryStream();
                    imgToSend.Save(ms, jpgEncoder, myEncoderParameters);
                    byte[] arry = ms.ToArray();

                    foreach (string a in SendToIPAddresses)
                    {
                        s.SendTo(arry, new IPEndPoint(IPAddress.Parse(a), videoPort));
                    }
                    imgHasBeenProcessed = false;
                }
            }
            catch (Exception ex)
            {
                mainForm.ShowError(ex.Message, ex.ToString());
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
                        if (videoWatch.ElapsedMilliseconds >= mainForm.networkDelay)
                        {
                            videoWatch.Stop();
                            mainForm.lbl_Errors.Text = videoWatch.ElapsedMilliseconds.ToString();
                            videoWatch.Reset();
                            //display image
                            if (videoImageAvailable)
                            {
                                mainForm.captureImageBox.Image = videoBuffer.Dequeue();
                                videoImageAvailable = false;
                            }
                            //errorTimer.Start();
                        }
                    }
                    else
                    {
                        //display image
                        if (videoImageAvailable)
                        {
                            mainForm.captureImageBox.Image = videoBuffer.Dequeue();
                            videoImageAvailable = false;
                        }
                    }
                }
            }
        }
        public void StartListening(string ListenToIPAddress)
        {
            ListeningTo = ListenToIPAddress;
            Thread t = new Thread(new ThreadStart(listenForVideo));
            t.IsBackground = true;
            t.Start();
        }
        private void listenForVideo()
        {
            try
            {
                if (videoListener == null)
                    videoListener = new UdpClient(videoPort);
                if (masterEP == null)
                    masterEP = new IPEndPoint(IPAddress.Parse(ListeningTo), videoPort);

                videoListener.BeginReceive(new AsyncCallback(videoImgReceived), null);
            }
            catch (Exception ex)
            {
                mainForm.ShowError(ex.Message, ex.ToString());
            }
        }
        private void videoImgReceived(IAsyncResult ar)
        {
            try
            {
                byte[] arry = videoListener.EndReceive(ar, ref masterEP);
                Image<Bgr, Byte> receivedImg = Image<Bgr, Byte>.FromRawImageData(arry);
                //Image<Bgr, Byte> resizedImg = receivedImg.Resize(((double)captureImageBox.Width / (double)receivedImg.Width), Emgu.CV.CvEnum.INTER.CV_INTER_AREA);
                if (mainForm.networkDelay > 0)
                {
                    videoBuffer.Enqueue(receivedImg);
                    videoImageAvailable = true;
                    if (networkVideoDelayChanged && !videoWatch.IsRunning) 
                    {
                        networkVideoDelayChanged = false;
                        videoWatch.Start();
                    }
                }
                else
                {
                    mainForm.captureImageBox.Image = receivedImg;
                }
                //myMarkings.OffsetX = receivedImg.Width - resizedImg.Width;
                //myMarkings.OffsetY = receivedImg.Height - resizedImg.Height;
                if (mainForm.applicationRunning)
                    listenForVideo();
            }
            catch (Exception ex)
            {
                mainForm.logMessage(ex.Message, ex.ToString(), Logging.StatusTypes.Error);
            }
        }
    }
}

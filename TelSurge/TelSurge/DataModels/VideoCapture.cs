using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
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
using FFmpeg.AutoGen;
using Accord.Video.DirectShow;

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
        public string CaptureName { get; set; }
        public string PTZAddress { get; set; }

        VideoCaptureDevice captureDevice;
        VideoCapabilities[] _deviceCapabilites;

        public int vidWidth = 1280;
        public int vidHeight = 720;
        public int frameRate = 60;
        Image<Bgr, byte> img;
        byte[] data;

        public unsafe AVCodec* codec;
        public unsafe AVCodecContext* c;
        public unsafe AVFrame* frame;
        public unsafe AVFrame* yuy2Frame;
        public unsafe AVFrame* gbrFrame;
        public unsafe AVPacket* pkt;
        public unsafe AVPacket* enPkt;
        public unsafe AVOutputFormat* ofmt;
        public unsafe AVFormatContext* ifmt_ctx;
        public unsafe AVFormatContext* ofmt_ctx;
        public unsafe AVDeviceCapabilitiesQuery* dcq;
        public unsafe AVDeviceInfoList* dil;
        public unsafe AVDictionary* avdic;
        public unsafe SwsContext* gbr_swctx;
        public unsafe SwsContext* yuv_swctx;

        bool encoderInit = false;

        int bitrate = 2000000;

        public unsafe VideoCapture(TelSurgeMain Main, int VideoPort)
        {
            this.Main = Main;
            this.videoPort = VideoPort;
            this.IsListeningForVideo = false;
            this.IsStreaming = false;
            this.CaptureDevice = "";
            this.PTZAddress = "";

            ofmt = null;
            ifmt_ctx = null;
            ofmt_ctx = null;
            avdic = null;
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
        public void ListenForVideo() 
        {
            if (videoListener == null)
                videoListener = new UdpClient(videoPort);

            videoListener.BeginReceive(new AsyncCallback(videoImgReceived), null);
        }
        private unsafe void videoImgReceived(IAsyncResult Ar)
        {
            int ret;
            IPEndPoint masterEP = new IPEndPoint(IPAddress.Parse(Main.Surgery.Master.MyIPAddress), videoPort);
            byte[] array = videoListener.EndReceive(Ar, ref masterEP);
            Image<Bgr, byte> receivedImg;

            pkt = ffmpeg.av_packet_alloc();
            pkt->size = array.Length;
            fixed (byte* pArray = &array[0])
                pkt->data = pArray;

            ret = ffmpeg.avcodec_send_packet(c, pkt);
            ret = ffmpeg.avcodec_receive_frame(c, frame);
            if (ret == 0)
            {
                ret = ffmpeg.sws_scale(gbr_swctx, frame->data, frame->linesize, 0, frame->height, gbrFrame->data, gbrFrame->linesize);
                if (ret < 0)
                    Main.ShowError("", "Error during GBR scaling!");

                // copy GBR data into byte array from pointer
                Marshal.Copy((IntPtr)gbrFrame->data[0], data, 0, vidHeight * vidWidth * 3);
                // free the packet
                fixed (AVPacket** pPkt = &pkt)
                {
                    ffmpeg.av_packet_free(pPkt);
                }
                img.Bytes = data;

                receivedImg = addMarkup(img);
                Main.CaptureImageBox.Image = receivedImg;
            }

            if (IsListeningForVideo)
                ListenForVideo();
        }
        private unsafe void sendVideoStream(AVPacket* enPacket) 
        {
            if (IsStreaming)
            {
                byte[] byteArray = new byte[enPacket->size];
                Marshal.Copy((IntPtr)enPacket->data, byteArray, 0, enPacket->size);

                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                foreach (User usr in Main.Surgery.ConnectedClients)
                {
                    s.SendTo(byteArray, new IPEndPoint(IPAddress.Parse(usr.MyIPAddress), videoPort));
                }
            }
        }
        private unsafe void releaseData()
        {
            fixed (AVCodecContext** pC = &c)
            {
                ffmpeg.avcodec_free_context(pC);
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
                        InitDecoder();
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
                    {
                        InitEncoder();
                        StartCapturing();
                    }
                }
                capturingType = type;
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
        }
        public unsafe void StartCapturing()
        {
            try
            {
                int res = 0;
                ofmt = null;
                ifmt_ctx = null;
                ofmt_ctx = null;
                avdic = null;

                string fullDName = "video=" + CaptureName;

                AVInputFormat* fmt = ffmpeg.av_find_input_format("dshow");

                string resString = vidWidth.ToString() + "x" + vidHeight.ToString();

                ifmt_ctx = ffmpeg.avformat_alloc_context();
                fixed (AVDictionary** pAVdic = &avdic)
                {
                    res = ffmpeg.av_dict_set(pAVdic, "video_size", resString, 0);
                    res = ffmpeg.av_dict_set(pAVdic, "pixel_format", "yuyv422", 0);
                    fixed (AVFormatContext** pFmtCxt = &ifmt_ctx)
                    {
                        res = ffmpeg.avformat_open_input(pFmtCxt, fullDName, fmt, pAVdic);
                    }

                    ffmpeg.av_dict_free(pAVdic);
                }
                if (res < 0)
                {
                    Main.ShowError("", "Unable to open input device!");
                    return;
                }

                res = ffmpeg.avformat_find_stream_info(ifmt_ctx, null);
                ffmpeg.av_dump_format(ifmt_ctx, 0, fullDName, 0);

                yuy2Frame = ffmpeg.av_frame_alloc();
                if (yuy2Frame == null)
                {
                    Main.ShowError("", "Could not allocate video frame!");
                }

                yuy2Frame->format = (int)AVPixelFormat.AV_PIX_FMT_YUYV422;
                yuy2Frame->width = c->width;
                yuy2Frame->height = c->height;

                res = ffmpeg.av_frame_get_buffer(yuy2Frame, 32);
                if (res < 0)
                    Main.ShowError("", "Could not allocate video frame data!");

                gbrFrame = ffmpeg.av_frame_alloc();
                gbrFrame->format = (int)AVPixelFormat.AV_PIX_FMT_BGR24;
                gbrFrame->width = vidWidth;
                gbrFrame->height = vidHeight;
                res = ffmpeg.av_frame_get_buffer(gbrFrame, 32);

                gbr_swctx = ffmpeg.sws_getContext(yuy2Frame->width, yuy2Frame->height, (AVPixelFormat)yuy2Frame->format, gbrFrame->width, gbrFrame->height, (AVPixelFormat)gbrFrame->format, 4, null, null, null);
                if (gbr_swctx == null)
                    Main.ShowError("", "Error getting sws context!");

                yuv_swctx = ffmpeg.sws_getContext(yuy2Frame->width, yuy2Frame->height, (AVPixelFormat)yuy2Frame->format, frame->width, frame->height, (AVPixelFormat)frame->format, 4, null, null, null);
                if (gbr_swctx == null)
                    Main.ShowError("", "Error getting sws context!");

                IsCapturing = true;

                Task t = Task.Run(() =>
                {
                    while (IsCapturing)
                        {
                            GrabFrames();
                        }
                });
            }
            catch (Exception) { }
        }
        public void StopCapturing()
        {
            if (IsCapturing)
            {
                releaseData();
                IsCapturing = false;
            }
        }
        unsafe void GrabFrames()
        {
            int ret;

            // allocate video packet
            pkt = ffmpeg.av_packet_alloc();
            // read frame from capture device
            ret = ffmpeg.av_read_frame(ifmt_ctx, pkt);
            if (ret < 0)
            {
                Main.ShowError("", "Error reading new frame!");
            }

            yuy2Frame->data[0] = pkt->data;
            // convert YUY2 to GBR
            ret = ffmpeg.sws_scale(gbr_swctx, yuy2Frame->data, yuy2Frame->linesize, 0, yuy2Frame->height, gbrFrame->data, gbrFrame->linesize);
            if (ret < 0)
                Main.ShowError("", "Error during GBR scaling!");
            // convert YUY2 to YUV
            ret = ffmpeg.sws_scale(yuv_swctx, yuy2Frame->data, yuy2Frame->linesize, 0, yuy2Frame->height, frame->data, frame->linesize);
            if (ret < 0)
                Main.ShowError("", "Error during YUV scaling!");

            // copy GBR data into byte array from pointer
            Marshal.Copy((IntPtr)gbrFrame->data[0], data, 0, vidHeight * vidWidth * 3);
            // free the packet
            fixed (AVPacket** pPkt = &pkt)
            {
                ffmpeg.av_packet_free(pPkt);
            }
            img.Bytes = data;

            EncodeFrame();
        }
        unsafe void EncodeFrame()
        {
            int ret;
            if (!encoderInit)
            {
                ret = ffmpeg.avcodec_send_frame(c, frame);
                if (ret < 0)
                    Main.ShowError("", "Error sending a frame for encoding!");

                encoderInit = true;
            }

            ret = ffmpeg.avcodec_send_frame(c, frame);
            if (ret < 0)
                Main.ShowError("", "Error sending a frame for encoding!");

            enPkt = ffmpeg.av_packet_alloc();

            ret = ffmpeg.avcodec_receive_packet(c, enPkt);
            if (ret < 0)
                Main.ShowError("", "Error during encoding!");

            if (IsStreaming)
                sendVideoStream(enPkt);

            fixed (AVPacket** pPkt = &enPkt)
                ffmpeg.av_packet_free(pPkt);
        }
        unsafe void InitEncoder()
        {
            codec = ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_MPEG4);
            if (codec->name == null)
            {
                Main.ShowError("", "Error finding encoder!");
            }

            c = ffmpeg.avcodec_alloc_context3(codec);

            c->bit_rate = bitrate;
            c->width = vidWidth;
            c->height = vidHeight;
            AVRational dummy;
            dummy.num = 1;
            dummy.den = frameRate;
            c->time_base = dummy;
            dummy.num = frameRate;
            dummy.den = 1;
            c->framerate = dummy;

            c->gop_size = 200;
            c->max_b_frames = 1;
            c->pix_fmt = AVPixelFormat.AV_PIX_FMT_YUV420P;

            int ret = ffmpeg.avcodec_open2(c, codec, null);
            if (ret < 0)
            {
                Main.ShowError("", "Could not open codec!");
            }

            frame = ffmpeg.av_frame_alloc();
            if (frame == null)
            {
                Main.ShowError("", "Could not allocate video frame!");
            }

            frame->format = (int)AVPixelFormat.AV_PIX_FMT_YUV420P;
            frame->width = c->width;
            frame->height = c->height;

            ret = ffmpeg.av_frame_get_buffer(frame, 32);
            if (ret < 0)
                Main.ShowError("", "Could not allocate video frame data!");
        }
        unsafe void InitDecoder()
        {
            codec = ffmpeg.avcodec_find_decoder(AVCodecID.AV_CODEC_ID_MPEG4);
            if (codec->name == null)
            {
                Main.ShowError("", "Error finding encoder!");
            }

            c = ffmpeg.avcodec_alloc_context3(codec);

            c->bit_rate = bitrate;
            c->width = vidWidth;
            c->height = vidHeight;

            c->pix_fmt = AVPixelFormat.AV_PIX_FMT_YUV420P;

            int ret = ffmpeg.avcodec_open2(c, codec, null);
            if (ret < 0)
            {
                Main.ShowError("", "Could not open codec!");
            }

            frame = ffmpeg.av_frame_alloc();
            if (frame == null)
            {
                Main.ShowError("", "Could not allocate video frame!");
            }

            frame->format = (int)AVPixelFormat.AV_PIX_FMT_YUV420P;
            frame->width = c->width;
            frame->height = c->height;

            ret = ffmpeg.av_frame_get_buffer(frame, 32);
            if (ret < 0)
                Main.ShowError("", "Could not allocate video frame data!");

            gbrFrame = ffmpeg.av_frame_alloc();
            gbrFrame->format = (int)AVPixelFormat.AV_PIX_FMT_BGR24;
            gbrFrame->width = vidWidth;
            gbrFrame->height = vidHeight;
            ret = ffmpeg.av_frame_get_buffer(gbrFrame, 32);

            gbr_swctx = ffmpeg.sws_getContext(frame->width, frame->height, (AVPixelFormat)frame->format, gbrFrame->width, gbrFrame->height, (AVPixelFormat)gbrFrame->format, 4, null, null, null);
            if (gbr_swctx == null)
                Main.ShowError("", "Error getting sws context!");
        }
    }
}

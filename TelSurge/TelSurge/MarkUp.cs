using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelSurge.DataModels;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Threading;

namespace TelSurge
{
    class MarkUp
    {
        private TelSurgeMain mainForm;
        public Markings myMarkings = new Markings();
        public Markings clientMarkings = new Markings();
        public Markings combinedMarkings = new Markings();
        private int penThickness = 5;
        public bool isDrawing = false;
        UdpClient markingsListener = null;
        int markingsPort = 11002;
        bool newFigure = false;


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
                mainForm.ShowError(ex.Message, ex.ToString());
            }
        }
        private async void markingsReceived(IAsyncResult ar)
        {
            await Task.Delay(mainForm.networkDelay);
            IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, markingsPort);
            byte[] arry = markingsListener.EndReceive(ar, ref clientEP);
            string json = Encoding.ASCII.GetString(arry);
            clientMarkings = JsonConvert.DeserializeObject<Markings>(json);
            combinedMarkings = myMarkings.Merge(clientMarkings);

            if (mainForm.applicationRunning)
                listenForMarkings();
        }
        private void sendMarkings(string SendToIPAddress)
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
                        s.SendTo(arry, new IPEndPoint(IPAddress.Parse(SendToIPAddress), markingsPort));
                        newFigure = false;
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                mainForm.ShowError(ex.Message, ex.ToString());
            }
        }
        public void ClearMarkUp()
        {
            myMarkings.Clear();
            tmpPoints = new List<Point>();
            redFigureNum = 0;
            blackFigureNum = 0;
            blueFigureNum = 0;
            whiteFigureNum = 0;
            yellowFigureNum = 0;
            greenFigureNum = 0;
            btn_UndoMark.Visible = false;
            if (cb_isMaster.Checked)
            {
                combinedMarkings.Clear();
                clientMarkings.Clear();
            }
        }
        public Image<Bgr, Byte> OverlayMarkUp(Image<Bgr, Byte> Image)
        {
            //add marks to image
            if (combinedMarkings.RedMarkings.Count > 0)
                Image.DrawPolyline(combinedMarkings.RedMarkings.ToArray(), false, new Bgr(Color.Red), penThickness);
            if (combinedMarkings.BlackMarkings.Count > 0)
                Image.DrawPolyline(combinedMarkings.BlackMarkings.ToArray(), false, new Bgr(Color.Black), penThickness);
            if (combinedMarkings.BlueMarkings.Count > 0)
                Image.DrawPolyline(combinedMarkings.BlueMarkings.ToArray(), false, new Bgr(Color.Blue), penThickness);
            if (combinedMarkings.WhiteMarkings.Count > 0)
                Image.DrawPolyline(combinedMarkings.WhiteMarkings.ToArray(), false, new Bgr(Color.White), penThickness);
            if (combinedMarkings.YellowMarkings.Count > 0)
                Image.DrawPolyline(combinedMarkings.YellowMarkings.ToArray(), false, new Bgr(Color.Yellow), penThickness);
            if (combinedMarkings.GreenMarkings.Count > 0)
                Image.DrawPolyline(combinedMarkings.GreenMarkings.ToArray(), false, new Bgr(Color.Green), penThickness);

            return Image;
        }
    }
}

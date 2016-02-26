using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelSurge.DataModels;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace TelSurge
{
    public class Markup
    {
        private TelSurgeMain Main;
        public Markings MyMarkings = new Markings();
        UdpClient markingsListener = null;
        private int markingsPort;
        public int PenThickness { get; set; }
        public bool IsListeningForMarkup { get; set; }
        public bool ClearMarkingsReq { get; set; }

        /*
        
        private Markings clientMarkings = new Markings();
        private Markings combinedMarkings = new Markings();
        Color penColor = Color.Red;
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
        bool newFigure = false;
        private bool clearMarkingsReq = false;
        */

        public Markup(TelSurgeMain MainForm, int MarkingsPort)
        {
            this.Main = MainForm;
            this.markingsPort = MarkingsPort;
            this.IsListeningForMarkup = false;
            this.ClearMarkingsReq = false;
            this.PenThickness = 5;
        }
        public void SendMarkup(IPAddress Address) 
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s.SendTo(SocketData.SerializeObject<Markings>(MyMarkings), new IPEndPoint(Address, markingsPort));
        }
        public void ListenForMarkup()
        {
            try
            {
                if (markingsListener == null)
                    markingsListener = new UdpClient(markingsPort);

                markingsListener.BeginReceive(new AsyncCallback(markingsReceived), null);
            }
            catch (Exception ex)
            {
                Main.ShowError(ex.Message, ex.ToString());
            }
        }
        private void markingsReceived(IAsyncResult AsyncResult)
        {
            //await Task.Delay(Main.User.NetworkDelay);
            IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, markingsPort);
            byte[] arry = markingsListener.EndReceive(AsyncResult, ref clientEP);
            Markings clientMarkings = SocketData.DeserializeObject<Markings>(arry);
            MyMarkings.Merge(clientMarkings);
            

            if (IsListeningForMarkup)
                ListenForMarkup();
        }
        public List<Figure> GetCurrentFigureList(Color Color)
        {
            switch (Color.Name)
            {
                case "Red":
                    return MyMarkings.RedMarkings;
                case "Black":
                    return MyMarkings.BlackMarkings;
                case "Blue":
                    return MyMarkings.BlueMarkings;
                case "White":
                    return MyMarkings.WhiteMarkings;
                case "Yellow":
                    return MyMarkings.YellowMarkings;
                case "Green":
                    return MyMarkings.GreenMarkings;
                default:
                    throw new Exception("Invalid color selected.");
            }
        }
        public void SetCurrentFigureList(Color Color, List<Figure> FigList)
        {
            switch (Color.Name)
            {
                case "Red":
                    MyMarkings.RedMarkings = FigList;
                    break;
                case "Black":
                    MyMarkings.BlackMarkings = FigList;
                    break;
                case "Blue":
                    MyMarkings.BlueMarkings = FigList;
                    break;
                case "White":
                    MyMarkings.WhiteMarkings = FigList;
                    break;
                case "Yellow":
                    MyMarkings.YellowMarkings = FigList;
                    break;
                case "Green":
                    MyMarkings.GreenMarkings = FigList;
                    break;
                default:
                    throw new Exception("Invalid color selected.");
            }
        }

        /*----------------------------------------------------------------

                

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

                */

    }
}

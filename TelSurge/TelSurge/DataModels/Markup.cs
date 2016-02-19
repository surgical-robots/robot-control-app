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
        private void sendMarkup(IPAddress Address, int Port) 
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s.SendTo(SocketData.SerializeObject(MyMarkings), new IPEndPoint(Address, Port));
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
        private async void markingsReceived(IAsyncResult AsyncResult)
        {
            await Task.Delay(Main.User.NetworkDelay);
            IPEndPoint clientEP = new IPEndPoint(IPAddress.Parse(Main.Surgery.Master.MyIPAddress), markingsPort);
            byte[] arry = markingsListener.EndReceive(AsyncResult, ref clientEP);
            Markings clientMarkings = SocketData.DeserializeObject<Markings>(arry);
            MyMarkings.Merge(clientMarkings);
            

            if (IsListeningForMarkup)
                ListenForMarkup();
        }
        public List<Point[]> GetMarksList(Color Color)
        {
            switch (Color.Name)
            {
                case "Red":
                    return MyMarkings.GetAllPaths(MyMarkings.RedMarkings).ToList<Point[]>();
                case "Black":
                    return MyMarkings.GetAllPaths(MyMarkings.BlackMarkings).ToList<Point[]>();
                case "Blue":
                    return MyMarkings.GetAllPaths(MyMarkings.BlueMarkings).ToList<Point[]>();
                case "White":
                    return MyMarkings.GetAllPaths(MyMarkings.WhiteMarkings).ToList<Point[]>();
                case "Yellow":
                    return MyMarkings.GetAllPaths(MyMarkings.YellowMarkings).ToList<Point[]>();
                case "Green":
                    return MyMarkings.GetAllPaths(MyMarkings.GreenMarkings).ToList<Point[]>();
                default:
                    throw new Exception("Invalid color selected.");
            }
        }
        public void SetMarksList(Color Color, List<Point[]> List)
        {
            List<Figure> figureList = new List<Figure>();
            foreach (Point[] p in List) 
            {
                figureList.Add(new Figure(Color, p));
            }
            switch (Color.Name)
            {
                case "Red":
                    MyMarkings.RedMarkings = figureList;
                    break;
                case "Black":
                    MyMarkings.BlackMarkings = figureList;
                    break;
                case "Blue":
                    MyMarkings.BlueMarkings = figureList;
                    break;
                case "White":
                    MyMarkings.WhiteMarkings = figureList;
                    break;
                case "Yellow":
                    MyMarkings.YellowMarkings = figureList;
                    break;
                case "Green":
                    MyMarkings.GreenMarkings = figureList;
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

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Text;

namespace RobotApp.Views.Plugins
{

    class VirtualRobotPosition
    {
        public double RUpperBevel { get; set; }
        public double RLowerBevel { get; set; }
        public double RElbow { get; set; }
        public double RTwist { get; set; }
        public double ROpen { get; set; }
        public double RClose { get; set; }
        public double LUpperBevel { get; set; }
        public double LLowerBevel { get; set; }
        public double LElbow { get; set; }
        public double LTwist { get; set; }
        public double LOpen { get; set; }
        public double LClose { get; set; }
    }




    public partial class HololensConnector : PluginBase
    {
        public UdpClient udpServer;
        public IPEndPoint remoteEP;
        private VirtualRobotPosition virtualRobotPos;
        public string jsonstring;
        private bool connected;
        private Byte[] data;
        public System.Windows.Forms.Timer holoTimer = new System.Windows.Forms.Timer();

        public HololensConnector()//construction function
        {
            virtualRobotPos = new VirtualRobotPosition();
            holoTimer.Tick += holoTimer_Tick;
            holoTimer.Interval = 250;
            this.TypeName = "HololensConnector";
            this.PluginInfo = "Serves as a bridge between Hololens and RobotApp. Sends Hololens current virtual robot position.";
            InitializeComponent();
            jsonstring = string.Empty;
            connected = false;
            try
            {
                udpServer = new UdpClient(1236);
                remoteEP = new IPEndPoint(IPAddress.Any, 1236);// UDP server and port
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            // INPUTS
            Inputs.Add("RUpperBevel", new ViewModel.InputSignalViewModel("RUpperBevel", this.InstanceName));
            Inputs.Add("RLowerBevel", new ViewModel.InputSignalViewModel("RLowerBevel", this.InstanceName));
            Inputs.Add("RElbow", new ViewModel.InputSignalViewModel("RElbow", this.InstanceName));
            Inputs.Add("RTwist", new ViewModel.InputSignalViewModel("RTwist", this.InstanceName));
            Inputs.Add("ROpen", new ViewModel.InputSignalViewModel("ROpen", this.InstanceName));
            Inputs.Add("RClose", new ViewModel.InputSignalViewModel("RClose", this.InstanceName));
            Inputs.Add("LUpperBevel", new ViewModel.InputSignalViewModel("LUpperBevel", this.InstanceName));
            Inputs.Add("LLowerBevel", new ViewModel.InputSignalViewModel("LLowerBevel", this.InstanceName));
            Inputs.Add("LElbow", new ViewModel.InputSignalViewModel("LElbow", this.InstanceName));
            Inputs.Add("LTwist", new ViewModel.InputSignalViewModel("LTwist", this.InstanceName));
            Inputs.Add("LOpen", new ViewModel.InputSignalViewModel("LOpen", this.InstanceName));
            Inputs.Add("LClose", new ViewModel.InputSignalViewModel("LClose", this.InstanceName));
        }
        void holoTimer_Tick(object sender, EventArgs e) //send data
        {
            if (connected)
            {
                PostLoadSetup();
                jsonstring = JsonConvert.SerializeObject(virtualRobotPos);
                byte[] msg = Encoding.UTF8.GetBytes(jsonstring);
                udpServer.Send(msg, msg.Length, remoteEP); // send
            }
        }

        public override void PostLoadSetup()
        {

            Messenger.Default.Register<Messages.Signal>(this, Inputs["RUpperBevel"].UniqueID, (message) =>
            {
                virtualRobotPos.RUpperBevel = message.Value;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["RLowerBevel"].UniqueID, (message) =>
            {
                virtualRobotPos.RLowerBevel = message.Value;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["RElbow"].UniqueID, (message) =>
            {
                virtualRobotPos.RElbow = message.Value;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["RTwist"].UniqueID, (message) =>
            {
                virtualRobotPos.RTwist = message.Value;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["ROpen"].UniqueID, (message) =>
            {
                virtualRobotPos.ROpen = message.Value;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["RClose"].UniqueID, (message) =>
            {
                virtualRobotPos.RClose = message.Value;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LUpperBevel"].UniqueID, (message) =>
            {
                virtualRobotPos.LUpperBevel = message.Value;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LLowerBevel"].UniqueID, (message) =>
            {
                virtualRobotPos.LLowerBevel = message.Value;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LElbow"].UniqueID, (message) =>
            {
                virtualRobotPos.LElbow = message.Value;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LTwist"].UniqueID, (message) =>
            {
                virtualRobotPos.LTwist = message.Value;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LOpen"].UniqueID, (message) =>
            {
                virtualRobotPos.LOpen = message.Value;
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LClose"].UniqueID, (message) =>
            {
                virtualRobotPos.LClose = message.Value;
            });

            base.PostLoadSetup();

        }

        void StartSendingData(object sender, RoutedEventArgs e)// button's function, start server
        {
            Byte[] dataold = data;
            data = udpServer.Receive(ref remoteEP); //receive data from hololens, then start sending data to hololens
            if (data != dataold)
            {
                MessageBox.Show("hololens connected and start sending");
                connected = true;
                holoTimer.Start();
            }
        }


    }
}

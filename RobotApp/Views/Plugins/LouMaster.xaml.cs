using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Treehopper;
using Treehopper.Libraries;
namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for LouMaster.xaml
    /// </summary>
    public partial class LouMaster : PluginBase
    {
        TreehopperBoard Board { get; set; }

        /// <summary>
        /// The <see cref="LinkToRobot" /> property's name.
        /// </summary>
        public const string LinkToRobotPropertyName = "LinkToRobot";

        private bool linkToRobot = false;

        /// <summary>
        /// Sets and gets the LinkTorRobot property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool LinkToRobot
        {
            get
            {
                return linkToRobot;
            }

            set
            {
                if (linkToRobot == value)
                {
                    return;
                }

                linkToRobot = value;
                RaisePropertyChanged(LinkToRobotPropertyName);
            }
        }

        Ltc2305 LeftForearm;
        Ltc2305 LeftElbow;
        Ltc2305 LeftShoulder;
        Ltc2305 RightForearm;
        Ltc2305 RightElbow;
        Ltc2305 RightShoulder;

        System.Timers.Timer Timer = new System.Timers.Timer();

        public LouMaster()
        {
            InstanceName = "MasterLou";
            TypeName = "LouMaster";

            Outputs.Add("LeftForearmTwist", new ViewModel.OutputSignalViewModel("Left Forearm Twist"));
            Outputs.Add("LeftForearmTrigger", new ViewModel.OutputSignalViewModel("Left Forearm Trigger"));
            Outputs.Add("LeftElbow", new ViewModel.OutputSignalViewModel("Left Elbow"));
            Outputs.Add("LeftUpperArmTwist", new ViewModel.OutputSignalViewModel("Left Upper Arm Twist"));
            Outputs.Add("LeftShoulderVertical", new ViewModel.OutputSignalViewModel("Left Shoulder Vertical"));
            Outputs.Add("LeftShoulderHorizontal", new ViewModel.OutputSignalViewModel("Left Shoulder Horizontal"));
            Outputs.Add("LeftShoulder1", new ViewModel.OutputSignalViewModel("Left Shoulder #1"));
            Outputs.Add("LeftShoulder2", new ViewModel.OutputSignalViewModel("Left Shoulder #2"));
            Outputs.Add("RightForearmTwist", new ViewModel.OutputSignalViewModel("Right Forearm Twist"));
            Outputs.Add("RightForearmTrigger", new ViewModel.OutputSignalViewModel("Right Forearm Trigger"));
            Outputs.Add("RightElbow", new ViewModel.OutputSignalViewModel("Right Elbow"));
            Outputs.Add("RightUpperArmTwist", new ViewModel.OutputSignalViewModel("Right Upper Arm Twist"));
            Outputs.Add("RightShoulderVertical", new ViewModel.OutputSignalViewModel("Right Shoulder Vertical"));
            Outputs.Add("RightShoulderHorizontal", new ViewModel.OutputSignalViewModel("Right Shoulder Horizontal"));
            Outputs.Add("RightShoulder1", new ViewModel.OutputSignalViewModel("Right Shoulder #1"));
            Outputs.Add("RightShoulder2", new ViewModel.OutputSignalViewModel("Right Shoulder #2"));

            Messenger.Default.Register<Treehopper.WPF.Message.BoardConnectedMessage>(this,
                (message) =>
                {
                    this.Board = message.Board;
                    Start();
                });

            Messenger.Default.Register<Treehopper.WPF.Message.BoardDisconnectedMessage>(this,
                (message) =>
                {
                    this.Board = null;
                    Timer.Stop();
                });
            InitializeComponent();
            //Timer.Interval = 100;
            //Timer.Elapsed += Timer_Elapsed;
            Task t = Task.Run(() =>
            {
                while(true)
                {
                    Timer_Elapsed(this, null);
                    Thread.Sleep(10);
                }

            });
                
            
        }

        void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(Board != null)
            {
                if (linkToRobot)
                {
                    Outputs["LeftForearmTwist"].Value = Utilities.Map(LeftForearm.Read(Ltc2305Channels.Channel0), 4.137, 0.961, -90, 90);
                    Outputs["LeftForearmTrigger"].Value = LeftForearm.Read(Ltc2305Channels.Channel1) > 4.25 ? 0.0 : 1.0;

                    Outputs["LeftElbow"].Value = Utilities.Map(Board.Pin9.AnalogIn.Voltage, 3.867, 0.205, 125, -125);

                    Outputs["LeftShoulderVertical"].Value = Utilities.Map(LeftShoulder.Read(Ltc2305Channels.Channel1), 0.221, 3.929, -125, 125);
                    Outputs["LeftShoulderHorizontal"].Value = Utilities.Map(LeftShoulder.Read(Ltc2305Channels.Channel0), 2.147, 0.803, 0, -90);
                    Outputs["LeftShoulder1"].Value = Outputs["LeftShoulderHorizontal"].Value + Outputs["LeftShoulderVertical"].Value;
                    Outputs["LeftShoulder2"].Value = Outputs["LeftShoulderHorizontal"].Value - Outputs["LeftShoulderVertical"].Value;

                    Outputs["RightForearmTwist"].Value = Utilities.Map(RightForearm.Read(Ltc2305Channels.Channel0), 5, 1.86, -90, 90);
                    Outputs["RightForearmTrigger"].Value = RightForearm.Read(Ltc2305Channels.Channel1) > 4.25 ? 0.0 : 1.0;

                    Outputs["RightElbow"].Value = Utilities.Map(RightElbow.Read(Ltc2305Channels.Channel0), 3.591, 0.126, -125, 125);
                    // Outputs["RightElbow"].Value = RightElbow.Read(Ltc2305Channels.Channel0);

                    Outputs["RightShoulderVertical"].Value = Utilities.Map(RightShoulder.Read(Ltc2305Channels.Channel1), 4.7, 0.348, -125, 125);
                    Outputs["RightShoulderHorizontal"].Value = Utilities.Map(RightShoulder.Read(Ltc2305Channels.Channel0), 3.392, 1.85, 0, 90);
                    Outputs["RightShoulder1"].Value = Outputs["RightShoulderHorizontal"].Value + Outputs["RightShoulderVertical"].Value;
                    Outputs["RightShoulder2"].Value = Outputs["RightShoulderHorizontal"].Value - Outputs["RightShoulderVertical"].Value;
                }
                
            }
        }

        public void Start()
        {
            Board.Pin9.AnalogIn.IsEnabled = true;

            Board.I2C.Start();
            LeftForearm = new Ltc2305(0x08, Board.I2C);
            LeftElbow = new Ltc2305(0x19, Board.I2C);
            LeftShoulder = new Ltc2305(0x09, Board.I2C);

            RightForearm = new Ltc2305(0x1B, Board.I2C);
            RightElbow = new Ltc2305(0x0B, Board.I2C);
            RightShoulder = new Ltc2305(0x18, Board.I2C);

            Timer.Start();
            
        }
    }
}

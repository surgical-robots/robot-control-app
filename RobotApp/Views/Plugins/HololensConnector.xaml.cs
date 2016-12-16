using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for HololensConnector.xaml
    /// </summary>
    public partial class HololensConnector : PluginBase
    {
        string HololensAddress = "";

        VirtualRobotPosition virtualRobotPos;

        public HololensConnector()
        {
            this.TypeName = "HololensConnector";
            this.PluginInfo = "Serves as a bridge between Hololens and RobotApp. Sends Hololens current virtual robot position.";
            InitializeComponent();

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

            PostLoadSetup();
        }

        public override void PostLoadSetup()
        {
            Messenger.Default.Register<Messages.Signal>(this, Inputs["RUpperBevel"].UniqueID, (message) =>
            {
                virtualRobotPos.RUpperBevel = message.Value;
                SendHololensMessage(virtualRobotPos);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["RLowerBevel"].UniqueID, (message) =>
            {
                virtualRobotPos.RLowerBevel = message.Value;
                SendHololensMessage(virtualRobotPos);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["RElbow"].UniqueID, (message) =>
            {
                virtualRobotPos.RElbow = message.Value;
                SendHololensMessage(virtualRobotPos);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["RTwist"].UniqueID, (message) =>
            {
                virtualRobotPos.RTwist = message.Value;
                SendHololensMessage(virtualRobotPos);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["ROpen"].UniqueID, (message) =>
            {
                virtualRobotPos.ROpen = message.Value;
                SendHololensMessage(virtualRobotPos);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["RClose"].UniqueID, (message) =>
            {
                virtualRobotPos.RClose = message.Value;
                SendHololensMessage(virtualRobotPos);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LUpperBevel"].UniqueID, (message) =>
            {
                virtualRobotPos.LUpperBevel = message.Value;
                SendHololensMessage(virtualRobotPos);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LLowerBevel"].UniqueID, (message) =>
            {
                virtualRobotPos.LLowerBevel = message.Value;
                SendHololensMessage(virtualRobotPos);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LElbow"].UniqueID, (message) =>
            {
                virtualRobotPos.LElbow = message.Value;
                SendHololensMessage(virtualRobotPos);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LTwist"].UniqueID, (message) =>
            {
                virtualRobotPos.LTwist = message.Value;
                SendHololensMessage(virtualRobotPos);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LOpen"].UniqueID, (message) =>
            {
                virtualRobotPos.LOpen = message.Value;
                SendHololensMessage(virtualRobotPos);
            });

            Messenger.Default.Register<Messages.Signal>(this, Inputs["LClose"].UniqueID, (message) =>
            {
                virtualRobotPos.LClose = message.Value;
                SendHololensMessage(virtualRobotPos);
            });

            base.PostLoadSetup();
        }

        private void SendHololensMessage(VirtualRobotPosition pos)
        {
            /*TODO: Send update message to Hololens containing current virtual robot position.*/
        }

        private RelayCommand connectHololens;

        /// <summary>
        /// Gets the ResetOffsetsCommand.
        /// </summary>
        public RelayCommand ConnectHololens
        {
            get
            {
                return connectHololens
                    ?? (connectHololens = new RelayCommand(
                    () =>
                    {
                        if (HololensAddress != "")
                        {
                            //TODO: Connect to Hololens
                        }
                    }));
            }
        }
    }

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
}

using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using RobotApp.ViewModel;
using remoteApiNETWrapper;

namespace RobotApp.Views.Plugins
{
    /// <summary>
    /// Interaction logic for ButtonInterface.xaml
    /// </summary>
    public partial class VREPconnect : PluginBase
    {
        private Thread ListenThread;
        private SerialPort connectPort;
        private int baudRate = 57600;

        private bool connected = false;
        private bool listen = false;

        private int clientID = 0;
        private double[] angles;
        private int[] joints;
        private string[] names;
        private int jointCount;
        private int nameCount;

        public override void PostLoadSetup()
        {
            //for (int i = 0; i < jointCount; i++ )
            //{
            //    Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint"+(i+1)].UniqueID, (message) =>
            //    {
            //        angles[i] = message.Value;
            //        if (connected)
            //            UpdateSimulation();
            //    });
            //}
            if (jointCount > 0)
            {
                Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint1"].UniqueID, (message) =>
                {
                    angles[0] = message.Value;
                    if (connected)
                        UpdateSimulation();
                });
            }
            if (jointCount > 1)
            {
                Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint2"].UniqueID, (message) =>
                {
                    angles[1] = message.Value;
                    if (connected)
                        UpdateSimulation();
                });
            }
            if (jointCount > 2)
            {
                Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint3"].UniqueID, (message) =>
                {
                    angles[2] = message.Value;
                    if (connected)
                        UpdateSimulation();
                });
            }
            if (jointCount > 3)
            {
                Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint4"].UniqueID, (message) =>
                {
                    angles[3] = message.Value;
                    if (connected)
                        UpdateSimulation();
                });
            }
            if (jointCount > 4)
            {
                Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint5"].UniqueID, (message) =>
                {
                    angles[4] = message.Value;
                    if (connected)
                        UpdateSimulation();
                });
            }
            if (jointCount > 5)
            {
                Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint6"].UniqueID, (message) =>
                {
                    angles[5] = message.Value;
                    if (connected)
                        UpdateSimulation();
                });
            }
            if (jointCount > 6)
            {
                Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint7"].UniqueID, (message) =>
                {
                    angles[6] = message.Value;
                    if (connected)
                        UpdateSimulation();
                });
            }
            if (jointCount > 7)
            {
                Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint8"].UniqueID, (message) =>
                {
                    angles[7] = message.Value;
                    if (connected)
                        UpdateSimulation();
                });
            }
            if (jointCount > 8)
            {
                Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint9"].UniqueID, (message) =>
                {
                    angles[8] = message.Value;
                    if (connected)
                        UpdateSimulation();
                });
            }
            if (jointCount > 9)
            {
                Messenger.Default.Register<Messages.Signal>(this, Inputs["Joint10"].UniqueID, (message) =>
                {
                    angles[6] = message.Value;
                    if (connected)
                        UpdateSimulation();
                });
            }

            base.PostLoadSetup();
        }

        public VREPconnect()
        {
            this.TypeName = "V-REP Connect";
            InitializeComponent();

            AddressList = new List<string>();

            Outputs.Add("Button1", new OutputSignalViewModel("Button 1"));
            Outputs.Add("Button2", new OutputSignalViewModel("Button 2"));
            Outputs.Add("Button3", new OutputSignalViewModel("Button 3"));

            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            AddressList.Clear();
            foreach (IPAddress ip in host.AddressList)
            {
                AddressList.Add(ip.ToString());
            }

            //PostLoadSetup();
        }

        private RelayCommand<string> detectCOMsCommand;

        /// <summary>
        /// Gets the DetectCOMsCommand.
        /// </summary>
        public RelayCommand<string> DetectCOMsCommand
        {
            get
            {
                return detectCOMsCommand
                    ?? (detectCOMsCommand = new RelayCommand<string>(
                    p =>
                    {
                        IPHostEntry host;
                        host = Dns.GetHostEntry(Dns.GetHostName());
                        AddressList.Clear();
                        foreach (IPAddress ip in host.AddressList)
                        {
                            AddressList.Add(ip.ToString());
                        }
                    }));
            }
        }

        private RelayCommand<string> connectCommand;

        /// <summary>
        /// Gets the ConnectCommand.
        /// </summary>
        public RelayCommand<string> ConnectCommand
        {
            get
            {
                return connectCommand
                    ?? (connectCommand = new RelayCommand<string>(
                    p =>
                    {
                        if (selectedAddress == null || AddressList.Count == 0)
                            return;
                        simx_error errorCode;
                        if(!connected)
                        {
                            //int[] intData = new int[500];
                            //float[] floatData = new float[500];
                            //string[] stringData = new string[500];
                            unsafe
                            {
                                string str = Environment.SystemDirectory;
                                
                                int handlesCount;
                                int* handles;
                                int intDataCount, floatDataCount, stringDataCount;
                                int* intData;
                                float* floatData;
                                char* stringData;
                                VREPWrapper.simxFinish(-1);
                                clientID = VREPWrapper.simxStart(selectedAddress, 19997, true, true, 5000, 5);
                                VREPWrapper.simxStartSimulation(clientID, simx_opmode.oneshot);
                                errorCode = VREPWrapper.simxGetObjectGroupData(clientID, sim_object.joint_type, 0, &handlesCount, &handles, &intDataCount, &intData,
                                                                                &floatDataCount, &floatData, &stringDataCount, &stringData, simx_opmode.oneshot_wait);
                                jointCount = handlesCount;

                                if(jointCount != 0)
                                {
                                    joints = new int[jointCount];
                                    for (int i = 0; i < jointCount; i++)
                                        joints[i] = handles[i];

                                    for (int i = 0; i < jointCount; i++)
                                    {
                                        if (!Inputs.ContainsKey("Joint" + (i + 1)))
                                            Inputs.Add("Joint" + (i + 1), new ViewModel.InputSignalViewModel("Joint " + (i + 1), this.InstanceName));
                                    }

                                    angles = new double[jointCount];
                                    PostLoadSetup();

                                    nameCount = stringDataCount;
                                    names = new string[nameCount];
                                    names[0] = new string(stringData);
                                    //for (int i = 0; i < nameCount; i++)
                                    //    names[i] = stringData[i].ToString();
                                }
                            }
                            ConnectText = "Connected to V-REP... Click to disconnect";
                            connected = true;
                        }
                        else
                        {
                            errorCode = VREPWrapper.simxStopSimulation(clientID, simx_opmode.oneshot_wait);
                            for (int i = 0; i < jointCount; i++)
                            {
                                Messenger.Default.Unregister<Messages.Signal>(this, Inputs["Joint" + (i + 1)].UniqueID);
                                Inputs.Remove("Joint" + (i + 1));
                            }
//                            VREPWrapper.simxFinish(-1);
                            ConnectText = "Connect to V-REP";
                            connected = false;
                        }
                    }));
            }
        }

        public void UpdateSimulation()
        {
            simx_error errorCode;
            for(int i = 0; i < jointCount; i++)
            {
                errorCode = VREPWrapper.simxSetJointTargetPosition(clientID, joints[i], Convert.ToSingle(angles[i] * Math.PI / 180), simx_opmode.streaming);
//                errorCode = simx_error.noerror;
            }
        }

        /// <summary>
        /// The <see cref="AddressList" /> property's name.
        /// </summary>
        public const string AddressListPropertyName = "AddressList";

        private List<string> addressList = null;

        /// <summary>
        /// Sets and gets the PortList property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public List<string> AddressList
        {
            get
            {
                return addressList;
            }

            set
            {
                if (addressList == value)
                {
                    return;
                }

                addressList = value;
                RaisePropertyChanged(AddressListPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SelectedAddress" /> property's name.
        /// </summary>
        public const string SelectedPortPropertyName = "SelectedAddress";

        private string selectedAddress = null;

        /// <summary>
        /// Sets and gets the SelectedPort property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string SelectedAddress
        {
            get
            {
                return selectedAddress;
            }

            set
            {
                if (selectedAddress == value)
                {
                    return;
                }

                selectedAddress = value;
                RaisePropertyChanged(SelectedPortPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ConnectText" /> property's name.
        /// </summary>
        public const string ConnectTextPropertyName = "ConnectText";

        private string connectText = "Connect to Selected Device";

        /// <summary>
        /// Sets and gets the ConnectText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ConnectText
        {
            get
            {
                return connectText;
            }

            set
            {
                if (connectText == value)
                {
                    return;
                }

                connectText = value;
                RaisePropertyChanged(ConnectTextPropertyName);
            }
        }
    }
}

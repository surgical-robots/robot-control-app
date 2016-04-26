using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace TelSurge
{
    public partial class ConnectButtons : Form
    {
        public Thread ListenThread;
        private SerialPort connectedPort;
        private int baudRate = 57600;
        private bool connected = false;
        public bool listen = false;
        private Dictionary<string, int> numOfButtons = new Dictionary<string, int>();
        private TelSurgeMain _main;
        public int outputNum;

        public ConnectButtons(TelSurgeMain main)
        {
            _main = main;
            InitializeComponent();

            FindPorts();
        }

        private void FindPorts()
        {
            try
            {
                SerialPort tempPort;
                string[] ports = SerialPort.GetPortNames();
                List<string> availablePorts = new List<string>();
                foreach (string portName in ports)
                {
                    tempPort = new SerialPort(portName, baudRate);
                    if (Handshake(tempPort))
                        availablePorts.Add(tempPort.PortName);
                }

                comboBox1.DataSource = availablePorts;
                comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool Handshake(SerialPort sp)
        {
            try
            {
                //The below setting are for the Hello handshake
                byte[] buffer = new byte[2]
                    {
                        Convert.ToByte(16),
                        Convert.ToByte(128)
                    };
                int intReturnASCII = 0;
                char charReturnValue = (Char)intReturnASCII;

                if (!sp.IsOpen)
                    sp.Open();
                sp.Write(buffer, 0, 2);
                Thread.Sleep(500);
                int count = sp.BytesToRead;
                string returnMessage = "";
                while (count > 0)
                {
                    intReturnASCII = sp.ReadByte();
                    returnMessage = returnMessage + Convert.ToChar(intReturnASCII);
                    count--;
                }
                //   	        ComPort.name = returnMessage;
                sp.Close();
                if (returnMessage.Contains("FOOT"))
                {
                    numOfButtons.Add(sp.PortName, (int)Char.GetNumericValue(returnMessage[4]));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                connectedPort = new SerialPort((string)comboBox1.SelectedValue);
                _main.User.ConnectExternalButtons(connectedPort, connected, numOfButtons[connectedPort.PortName]);
                connected = !connected;
                if (connected)
                {
                    btn_Connect.Text = "Disconnect";
                }
                else
                {
                    btn_Connect.Text = "Connect";
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (connectedPort != null && connectedPort.IsOpen)
                connectedPort.Close();
            connectedPort = null;
            btn_Connect.Enabled = true;
            connected = false;
            btn_Connect.Text = "Connect";
        }

        private void btn_PortRefresh_Click(object sender, EventArgs e)
        {
            FindPorts();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using TelSurge.DataModels;
using System.Net;
using System.Net.Sockets;

namespace TelSurge
{
    public partial class ChangeMyIP : Form
    {
        TelSurgeMain _main;
        public ChangeMyIP(TelSurgeMain mainForm)
        {
            InitializeComponent();
            _main = mainForm;
            fillAddresses();
        }

        private void btn_Done_Click(object sender, EventArgs e)
        {
            _main.User.MyIPAddress = ddl_Addresses.Items[ddl_Addresses.SelectedIndex].ToString();
            if (_main.User.IsMaster)
                _main.tb_ipAddress.Text = _main.User.MyIPAddress;
            this.Close();
        }
        private void fillAddresses()
        {
            try
            {
                IPHostEntry host;
                host = Dns.GetHostEntry(Dns.GetHostName());
                ddl_Addresses.Items.Clear();
                foreach (IPAddress ip in host.AddressList)
                {
                    ddl_Addresses.Items.Add(ip.ToString());
                }
                ddl_Addresses.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

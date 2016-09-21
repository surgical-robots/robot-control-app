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
using DirectShowLib;

namespace TelSurge
{
    public partial class ChangeVideoSource : Form
    {
        TelSurgeMain _main;
        private int IPCamerasIndex = 1000;
        BindingList<IPCamera> ipSources;
        public ChangeVideoSource(TelSurgeMain mainForm)
        {
            InitializeComponent();
            _main = mainForm;
            fillDevices();
        }

        private void btn_Done_Click(object sender, EventArgs e)
        {
            int choice = ddl_Devices.SelectedIndex;
            if (ddl_Devices.Items[choice].Equals("Master Video Feed"))
            {
                //Master Video Feed
                _main.VideoCapture.CaptureDevice = "";
                _main.VideoCapture.SwitchVideoFeed(VideoCapture.CaptureType.MasterFeed);
            }
            else if (choice >= IPCamerasIndex)
            {
                //IP source
                _main.VideoCapture.CaptureDevice = ipSources[choice - IPCamerasIndex].Address;
                _main.VideoCapture.SwitchVideoFeed(VideoCapture.CaptureType.IP);
                _main.VideoCapture.PTZAddress = ipSources[choice - IPCamerasIndex].PTZAddress;
            }
            else
            {
                //Local device
                _main.VideoCapture.CaptureDevice = choice.ToString();
                _main.VideoCapture.SwitchVideoFeed(VideoCapture.CaptureType.Local);
            }
            this.Close();
        }
        private void fillDevices()
        {
            try
            {
                //Find system video devices
                DsDevice[] _SystemCamereas = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
                foreach (DsDevice d in _SystemCamereas)
                {
                    ddl_Devices.Items.Add(d.Name);
                }
                //Add IP video sources
                ipSources = IPCameras.ReadFromFile();
                for (int i = 0; i < ipSources.Count; i++)
                {
                    ddl_Devices.Items.Add(ipSources[i].Name);
                    if (i == 0)
                        IPCamerasIndex = ddl_Devices.Items.Count - 1;
                }
                if (!_main.User.IsMaster && _main.User.ConnectedToMaster) //If client, add option to return to receiving video from Master
                    ddl_Devices.Items.Add("Master Video Feed");

                ddl_Devices.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

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
                _main.changeVideoSource(); //Start receiving video from Master
                _main.videoIsPTZ = false;
            }
            else if (choice >= IPCamerasIndex)
            {
                _main.changeVideoSource(ipSources[choice - IPCamerasIndex].Address);
                //TODO: Determine if selected IP Camera has network controls
                //For now just use controls for specific camera
                if (ipSources[choice - IPCamerasIndex].Name.Equals("Axis 215"))
                    _main.videoIsPTZ = true;
            }
            else
            {
                _main.changeVideoSource(choice);
                _main.videoIsPTZ = false;
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
                if (!_main.isMaster && _main.isConnectedToMaster) //If client, add option to return to receiving video from Master
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

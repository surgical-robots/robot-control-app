using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TelSurge
{
    public partial class CameraControl : Form
    {

        private int PanTiltSpd = 10;
        private int ZoomSpd = 3;
        private TelSurgeMain _main;
        public CameraControl(TelSurgeMain Main)
        {
            InitializeComponent();
            _main = Main;
        }

        private void btn_Up_MouseDown(object sender, MouseEventArgs e)
        {
            _main.sendCmdToCamera("up&" + PanTiltSpd + "&" + PanTiltSpd);
        }
        private void btn_Down_MouseDown(object sender, MouseEventArgs e)
        {
            _main.sendCmdToCamera("down&" + PanTiltSpd + "&" + PanTiltSpd);
        }
        private void btn_Left_MouseDown(object sender, MouseEventArgs e)
        {
            _main.sendCmdToCamera("left&" + PanTiltSpd + "&" + PanTiltSpd);
        }
        private void btn_Right_MouseDown(object sender, MouseEventArgs e)
        {
            _main.sendCmdToCamera("right&" + PanTiltSpd + "&" + PanTiltSpd);
        }

        private void btn_PanTilt_MouseUp(object sender, MouseEventArgs e)
        {
            _main.sendCmdToCamera("ptzstop&" + PanTiltSpd + "&" + PanTiltSpd);
        }
        private void btn_StopPT_Click(object sender, EventArgs e)
        {
            _main.sendCmdToCamera("ptzstop&" + PanTiltSpd + "&" + PanTiltSpd);
        }

        private void btn_ZoomIn_MouseDown(object sender, MouseEventArgs e)
        {
            _main.sendCmdToCamera("zoomin&" + ZoomSpd + "&" + ZoomSpd);
        }
        private void btn_ZoomOut_MouseDown(object sender, MouseEventArgs e)
        {
            _main.sendCmdToCamera("zoomout&" + ZoomSpd + "&" + ZoomSpd);
        }
        private void btn_ZoomStop_MouseUp(object sender, MouseEventArgs e)
        {
            _main.sendCmdToCamera("zoomstop&" + ZoomSpd + "&" + ZoomSpd);
        }
    }
}

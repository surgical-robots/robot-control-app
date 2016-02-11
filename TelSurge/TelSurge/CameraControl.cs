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
        public CameraControl()
        {
            InitializeComponent();
        }

        private void btn_Up_MouseDown(object sender, MouseEventArgs e)
        {
            TelSurgeMain.sendCmdToCamera("up&" + PanTiltSpd + "&" + PanTiltSpd);
        }
        private void btn_Down_MouseDown(object sender, MouseEventArgs e)
        {
            TelSurgeMain.sendCmdToCamera("down&" + PanTiltSpd + "&" + PanTiltSpd);
        }
        private void btn_Left_MouseDown(object sender, MouseEventArgs e)
        {
            TelSurgeMain.sendCmdToCamera("left&" + PanTiltSpd + "&" + PanTiltSpd);
        }
        private void btn_Right_MouseDown(object sender, MouseEventArgs e)
        {
            TelSurgeMain.sendCmdToCamera("right&" + PanTiltSpd + "&" + PanTiltSpd);
        }

        private void btn_PanTilt_MouseUp(object sender, MouseEventArgs e)
        {
            TelSurgeMain.sendCmdToCamera("ptzstop&" + PanTiltSpd + "&" + PanTiltSpd);
        }
        private void btn_StopPT_Click(object sender, EventArgs e)
        {
            TelSurgeMain.sendCmdToCamera("ptzstop&" + PanTiltSpd + "&" + PanTiltSpd);
        }

        private void btn_ZoomIn_MouseDown(object sender, MouseEventArgs e)
        {
            TelSurgeMain.sendCmdToCamera("zoomin&" + ZoomSpd + "&" + ZoomSpd);
        }
        private void btn_ZoomOut_MouseDown(object sender, MouseEventArgs e)
        {
            TelSurgeMain.sendCmdToCamera("zoomout&" + ZoomSpd + "&" + ZoomSpd);
        }
        private void btn_ZoomStop_MouseUp(object sender, MouseEventArgs e)
        {
            TelSurgeMain.sendCmdToCamera("zoomstop&" + ZoomSpd + "&" + ZoomSpd);
        }
    }
}

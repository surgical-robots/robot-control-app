using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

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

        private void btn_laser_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a request using a URL that can receive a post. 
                WebRequest request = WebRequest.Create("https://api.particle.io/v1/devices/3c002d000c47343432313031/led?access_token=623b6d6ba0fcd4715c7c60d80f802e522f32903b");
                // Set the Method property of the request to POST.
                request.Method = "POST";
                // Create POST data and convert it to a byte array.
                string postData = "LED";
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                // Set the ContentLength property of the WebRequest.
                request.ContentLength = byteArray.Length;
                // Get the request stream.
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                //MessageBox.Show(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                //MessageBox.Show(responseFromServer);
                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                _main.ShowError(ex.Message, ex.ToString());
            }
        }
    }
}

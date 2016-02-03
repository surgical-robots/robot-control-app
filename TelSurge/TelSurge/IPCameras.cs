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
    public partial class IPCameras : Form
    {
        private BindingList<IPCamera> Sources = new BindingList<IPCamera>();
        
        public IPCameras()
        {
            InitializeComponent();
            fillSources();
        }
        private void fillSources()
        {
            try
            {
                Sources = ReadFromFile();

                gv_IPVidSources.DataSource = Sources;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void WriteToFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BindingList<IPCamera>));
            using (TextWriter writer = new StreamWriter(Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"..\..\Content\InternetCameras.xml")))
            {
                serializer.Serialize(writer, Sources);
            }
        }
        public static BindingList<IPCamera> ReadFromFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BindingList<IPCamera>));
            using (StreamReader sr = new StreamReader(Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"..\..\Content\InternetCameras.xml")))
            {
                try
                {
                    BindingList<IPCamera> sources = (BindingList<IPCamera>)serializer.Deserialize(sr);
                    return sources;
                }
                catch (Exception)
                {
                    return new BindingList<IPCamera>();
                }
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                WriteToFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                MessageBox.Show("Saved!");
            }
        }
    }
}

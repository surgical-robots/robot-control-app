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

namespace TelSurge
{
    public partial class NetDelay : Form
    {
        TelSurgeMain _main;
        public NetDelay(TelSurgeMain mainForm, bool enabled, int delay)
        {
            InitializeComponent();
            _main = mainForm;
            cb_Enable.Checked = enabled;
            trb_Delay.Value = delay;
            trb_Delay.Enabled = enabled;
            fillCities();
        }

        private void btn_Done_Click(object sender, EventArgs e)
        {
            _main.networkDelay = trb_Delay.Value;
            //_main.dataBuffer.Clear();  Switching delay could be dangerous. Instantaneous position change is possible.
            this.Close();
        }

        private void cb_Enable_CheckedChanged(object sender, EventArgs e)
        {
            trb_Delay.Enabled = !cb_ChooseCity.Checked && cb_Enable.Checked;
            if (!trb_Delay.Enabled)
                trb_Delay.Value = 0;
        }

        private void ddl_Cities_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_ChooseCity.Checked)
            {
                Location loc = (Location)ddl_Cities.SelectedValue;
                int value = loc.DistanceFromLincoln;
                if (value > trb_Delay.Maximum)
                {
                    value = trb_Delay.Maximum;
                    MessageBox.Show("The selected city has a delay greater than the max value!");
                }
                trb_Delay.Value = value;
            }
        }

        private void trb_Delay_ValueChanged(object sender, EventArgs e)
        {
            tb_value.Text = trb_Delay.Value.ToString();
        }

        private void cb_ChooseCity_CheckedChanged(object sender, EventArgs e)
        {
            ddl_Cities.Enabled = cb_ChooseCity.Checked;
            trb_Delay.Enabled = !cb_ChooseCity.Checked && cb_Enable.Checked;
            if (!trb_Delay.Enabled)
                trb_Delay.Value = 0;
        }
        private void fillCities()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Location>));
            List<Location> locations = new List<Location>();
            using (StreamReader sr = new StreamReader(Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"..\..\Content\CityDelays.xml")))
            {
                try
                {
                    locations = (List<Location>)serializer.Deserialize(sr);
                    ddl_Cities.DataSource = locations;
                    ddl_Cities.DisplayMember = "Name";
                    ddl_Cities.ValueMember = "DistanceFromLincoln";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}

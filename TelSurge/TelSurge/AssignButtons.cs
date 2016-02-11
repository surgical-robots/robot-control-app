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
    public partial class AssignButtons : Form
    {
        TelSurgeMain _main;
        public AssignButtons(TelSurgeMain main)
        {
            InitializeComponent();
            _main = main;
            fillAvailableBtns();
        }

        private void fillAvailableBtns()
        {
            lb_AvailableBtns.Items.Add("OmniLeft_Front");
            lb_AvailableBtns.Items.Add("OmniLeft_Back");
            lb_AvailableBtns.Items.Add("OmniRight_Front");
            lb_AvailableBtns.Items.Add("OmniRight_Back");
            if (_main.externalButtons != null)
            {
                for (int i = 0; i < _main.externalButtons.Count(); i++)
                    lb_AvailableBtns.Items.Add("Ext_" + i);
            }
        }

        private void AssignButtons_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Save on exit
            try {
                string selectedBtn = (string)lb_AvailableBtns.SelectedItem;
                _main.EmergencySwitchBoundBtn = selectedBtn;
                if (selectedBtn.Contains("Left"))
                {
                    _main.EmergencySwitchBoundValue = lb_AvailableBtns.SelectedIndex + 1;
                }
                else if (selectedBtn.Contains("Right"))
                {
                    _main.EmergencySwitchBoundValue = lb_AvailableBtns.SelectedIndex - 1;
                }
                else
                {
                    _main.EmergencySwitchBoundValue = lb_AvailableBtns.SelectedIndex - 4;
                }
            }
            catch (Exception ex)
            {
                _main.ShowError(ex.Message, ex.ToString());
            }
        }
    }
}

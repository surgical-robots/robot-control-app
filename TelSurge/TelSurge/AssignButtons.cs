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
            //Emergency Switch
            lb_AvailableEmergencyBtns.Items.Add("OmniLeft_Front");
            lb_AvailableEmergencyBtns.Items.Add("OmniLeft_Back");
            lb_AvailableEmergencyBtns.Items.Add("OmniRight_Front");
            lb_AvailableEmergencyBtns.Items.Add("OmniRight_Back");
            if (_main.User.externalButtons != null)
            {
                for (int i = 0; i < _main.User.externalButtons.Count(); i++)
                    lb_AvailableEmergencyBtns.Items.Add("Ext_" + i);
            }
            lb_AvailableEmergencyBtns.SelectedIndex = 0;

            //Following Button
            lb_AvailableFollowingBtns.Items.Add("OmniLeft_Front");
            lb_AvailableFollowingBtns.Items.Add("OmniLeft_Back");
            lb_AvailableFollowingBtns.Items.Add("OmniRight_Front");
            lb_AvailableFollowingBtns.Items.Add("OmniRight_Back");
            if (_main.User.externalButtons != null)
            {
                for (int i = 0; i < _main.User.externalButtons.Count(); i++)
                    lb_AvailableFollowingBtns.Items.Add("Ext_" + i);
            }
            lb_AvailableFollowingBtns.SelectedIndex = 1;
        }

        private void AssignButtons_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (lb_AvailableEmergencyBtns.SelectedIndex.Equals(lb_AvailableFollowingBtns.SelectedIndex))
            {
                MessageBox.Show("Please select different buttons for each input.", "Inputs must be bound to different buttons.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //Save on exit
                try
                {
                    //Emergency Switch
                    string selectedEmergencyBtn = (string)lb_AvailableEmergencyBtns.SelectedItem;
                    _main.User.EmergencySwitchBoundBtn = selectedEmergencyBtn;
                    if (selectedEmergencyBtn.Contains("Left"))
                    {
                        _main.User.EmergencySwitchBoundValue = lb_AvailableEmergencyBtns.SelectedIndex + 1;
                    }
                    else if (selectedEmergencyBtn.Contains("Right"))
                    {
                        _main.User.EmergencySwitchBoundValue = lb_AvailableEmergencyBtns.SelectedIndex - 2;
                    }
                    else
                    {
                        _main.User.EmergencySwitchBoundValue = lb_AvailableEmergencyBtns.SelectedIndex - 4;
                    }

                    //Following Button
                    string selectedFollowingBtn = (string)lb_AvailableFollowingBtns.SelectedItem;
                    _main.User.FollowingBoundBtn = selectedFollowingBtn;
                    if (selectedFollowingBtn.Contains("Left"))
                    {
                        _main.User.FollowingBoundValue = lb_AvailableFollowingBtns.SelectedIndex + 1;
                    }
                    else if (selectedFollowingBtn.Contains("Right"))
                    {
                        _main.User.FollowingBoundValue = lb_AvailableFollowingBtns.SelectedIndex - 2;
                    }
                    else
                    {
                        _main.User.FollowingBoundValue = lb_AvailableFollowingBtns.SelectedIndex - 4;
                    }
                }
                catch (Exception ex)
                {
                    _main.ShowError(ex.Message, ex.ToString());
                }
            }
        }
    }
}

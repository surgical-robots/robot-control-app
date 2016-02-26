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
            List<string> availableBtns = new List<string>() {
                "OmniLeft_Front",
                "OmniLeft_Back",
                "OmniRight_Front",
                "OmniRight_Back"
            };
            for (int i = 0; i < _main.User.NumExternalButtons; i++)
                availableBtns.Add("Ext_" + i);
            
            lb_AvailableEmergencyBtns.DataSource = availableBtns;
            lb_AvailableEmergencyBtns.SelectedIndex = 0;

            
            //Following Button
            lb_AvailableFollowingBtns.DataSource = availableBtns.ToList();
            lb_AvailableFollowingBtns.SelectedIndex = 0;

            //Freeze Button
            lb_AvailableFreezeBtns.DataSource = availableBtns.ToList();
            lb_AvailableFreezeBtns.SelectedIndex = 0;
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
                    string selectedButton = (string)lb_AvailableEmergencyBtns.SelectedItem;
                    _main.User.EmergencySwitchBoundBtn = selectedButton;
                    _main.User.EmergencySwitchBoundValue = getButtonValue(lb_AvailableEmergencyBtns);

                    //Following Button
                    selectedButton = (string)lb_AvailableFollowingBtns.SelectedItem;
                    _main.User.FollowingBoundBtn = selectedButton;
                    _main.User.FollowingBoundValue = getButtonValue(lb_AvailableFollowingBtns);

                    //Freeze Button
                    selectedButton = (string)lb_AvailableFreezeBtns.SelectedItem;
                    _main.User.FreezeBoundBtn = selectedButton;
                    _main.User.FreezeBoundValue = getButtonValue(lb_AvailableFreezeBtns);
                }
                catch (Exception ex)
                {
                    _main.ShowError(ex.Message, ex.ToString());
                }
            }
        }
        private int getButtonValue(ListBox lb)
        {
            string selectedButton = (string)lb.SelectedItem;
            if (selectedButton.Contains("Left"))
            {
                return lb.SelectedIndex + 1;
            }
            else if (selectedButton.Contains("Right"))
            {
                return lb.SelectedIndex - 1;
            }
            else
            {
                return lb.SelectedIndex - 4;
            }
        }
    }
}

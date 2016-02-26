namespace TelSurge
{
    partial class AssignButtons
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssignButtons));
            this.lb_AvailableEmergencyBtns = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lb_AvailableFollowingBtns = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lb_AvailableFreezeBtns = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb_AvailableEmergencyBtns
            // 
            this.lb_AvailableEmergencyBtns.FormattingEnabled = true;
            this.lb_AvailableEmergencyBtns.Location = new System.Drawing.Point(6, 19);
            this.lb_AvailableEmergencyBtns.Name = "lb_AvailableEmergencyBtns";
            this.lb_AvailableEmergencyBtns.Size = new System.Drawing.Size(181, 95);
            this.lb_AvailableEmergencyBtns.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lb_AvailableEmergencyBtns);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(193, 126);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Emergency Switch";
            // 
            // lb_AvailableFollowingBtns
            // 
            this.lb_AvailableFollowingBtns.FormattingEnabled = true;
            this.lb_AvailableFollowingBtns.Location = new System.Drawing.Point(6, 19);
            this.lb_AvailableFollowingBtns.Name = "lb_AvailableFollowingBtns";
            this.lb_AvailableFollowingBtns.Size = new System.Drawing.Size(181, 95);
            this.lb_AvailableFollowingBtns.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lb_AvailableFollowingBtns);
            this.groupBox2.Location = new System.Drawing.Point(12, 144);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(193, 126);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Following Button";
            // 
            // lb_AvailableFreezeBtns
            // 
            this.lb_AvailableFreezeBtns.FormattingEnabled = true;
            this.lb_AvailableFreezeBtns.Location = new System.Drawing.Point(6, 19);
            this.lb_AvailableFreezeBtns.Name = "lb_AvailableFreezeBtns";
            this.lb_AvailableFreezeBtns.Size = new System.Drawing.Size(181, 95);
            this.lb_AvailableFreezeBtns.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lb_AvailableFreezeBtns);
            this.groupBox3.Location = new System.Drawing.Point(211, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(193, 126);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Freeze Button";
            // 
            // AssignButtons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(417, 301);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AssignButtons";
            this.Text = "Assign Buttons";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AssignButtons_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lb_AvailableEmergencyBtns;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lb_AvailableFollowingBtns;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lb_AvailableFreezeBtns;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}
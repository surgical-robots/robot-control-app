namespace TelSurge
{
    partial class NetDelay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetDelay));
            this.trb_Delay = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.cb_Enable = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_Done = new System.Windows.Forms.Button();
            this.ddl_Cities = new System.Windows.Forms.ComboBox();
            this.cb_ChooseCity = new System.Windows.Forms.CheckBox();
            this.tb_value = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.trb_Delay)).BeginInit();
            this.SuspendLayout();
            // 
            // trb_Delay
            // 
            this.trb_Delay.LargeChange = 100;
            this.trb_Delay.Location = new System.Drawing.Point(37, 104);
            this.trb_Delay.Maximum = 2000;
            this.trb_Delay.Name = "trb_Delay";
            this.trb_Delay.Size = new System.Drawing.Size(379, 40);
            this.trb_Delay.SmallChange = 50;
            this.trb_Delay.TabIndex = 0;
            this.trb_Delay.TickFrequency = 50;
            this.trb_Delay.ValueChanged += new System.EventHandler(this.trb_Delay_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(409, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Use the track bar to set the amount of network delay between connections.";
            // 
            // cb_Enable
            // 
            this.cb_Enable.AutoSize = true;
            this.cb_Enable.Location = new System.Drawing.Point(13, 13);
            this.cb_Enable.Name = "cb_Enable";
            this.cb_Enable.Size = new System.Drawing.Size(72, 17);
            this.cb_Enable.TabIndex = 2;
            this.cb_Enable.Text = "Set Delay";
            this.cb_Enable.UseVisualStyleBackColor = true;
            this.cb_Enable.CheckedChanged += new System.EventHandler(this.cb_Enable_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "0 s";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(216, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "1 s";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(392, 131);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "2 s";
            // 
            // btn_Done
            // 
            this.btn_Done.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Done.Location = new System.Drawing.Point(181, 150);
            this.btn_Done.Name = "btn_Done";
            this.btn_Done.Size = new System.Drawing.Size(88, 29);
            this.btn_Done.TabIndex = 6;
            this.btn_Done.Text = "Done";
            this.btn_Done.UseVisualStyleBackColor = true;
            this.btn_Done.Click += new System.EventHandler(this.btn_Done_Click);
            // 
            // ddl_Cities
            // 
            this.ddl_Cities.Enabled = false;
            this.ddl_Cities.FormattingEnabled = true;
            this.ddl_Cities.Location = new System.Drawing.Point(142, 44);
            this.ddl_Cities.Name = "ddl_Cities";
            this.ddl_Cities.Size = new System.Drawing.Size(271, 21);
            this.ddl_Cities.TabIndex = 7;
            this.ddl_Cities.SelectedIndexChanged += new System.EventHandler(this.ddl_Cities_SelectedIndexChanged);
            // 
            // cb_ChooseCity
            // 
            this.cb_ChooseCity.AutoSize = true;
            this.cb_ChooseCity.Location = new System.Drawing.Point(46, 46);
            this.cb_ChooseCity.Name = "cb_ChooseCity";
            this.cb_ChooseCity.Size = new System.Drawing.Size(90, 17);
            this.cb_ChooseCity.TabIndex = 8;
            this.cb_ChooseCity.Text = "Choose a city";
            this.cb_ChooseCity.UseVisualStyleBackColor = true;
            this.cb_ChooseCity.CheckedChanged += new System.EventHandler(this.cb_ChooseCity_CheckedChanged);
            // 
            // tb_value
            // 
            this.tb_value.Location = new System.Drawing.Point(423, 105);
            this.tb_value.Name = "tb_value";
            this.tb_value.ReadOnly = true;
            this.tb_value.Size = new System.Drawing.Size(45, 20);
            this.tb_value.TabIndex = 9;
            // 
            // NetDelay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 188);
            this.Controls.Add(this.tb_value);
            this.Controls.Add(this.cb_ChooseCity);
            this.Controls.Add(this.ddl_Cities);
            this.Controls.Add(this.btn_Done);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cb_Enable);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trb_Delay);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NetDelay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Network Delay";
            ((System.ComponentModel.ISupportInitialize)(this.trb_Delay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trb_Delay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cb_Enable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_Done;
        private System.Windows.Forms.ComboBox ddl_Cities;
        private System.Windows.Forms.CheckBox cb_ChooseCity;
        private System.Windows.Forms.TextBox tb_value;
    }
}
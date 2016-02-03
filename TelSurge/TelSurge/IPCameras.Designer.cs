namespace TelSurge
{
    partial class IPCameras
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IPCameras));
            this.btn_Save = new System.Windows.Forms.Button();
            this.gb_IPVidSources = new System.Windows.Forms.GroupBox();
            this.gv_IPVidSources = new System.Windows.Forms.DataGridView();
            this.gb_IPVidSources.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gv_IPVidSources)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Save
            // 
            this.btn_Save.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Save.Location = new System.Drawing.Point(330, 550);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(88, 29);
            this.btn_Save.TabIndex = 6;
            this.btn_Save.Text = "Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // gb_IPVidSources
            // 
            this.gb_IPVidSources.Controls.Add(this.gv_IPVidSources);
            this.gb_IPVidSources.Location = new System.Drawing.Point(12, 12);
            this.gb_IPVidSources.Name = "gb_IPVidSources";
            this.gb_IPVidSources.Size = new System.Drawing.Size(726, 532);
            this.gb_IPVidSources.TabIndex = 7;
            this.gb_IPVidSources.TabStop = false;
            this.gb_IPVidSources.Text = "Current IP Video Sources";
            // 
            // gv_IPVidSources
            // 
            this.gv_IPVidSources.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gv_IPVidSources.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gv_IPVidSources.Location = new System.Drawing.Point(6, 19);
            this.gv_IPVidSources.Name = "gv_IPVidSources";
            this.gv_IPVidSources.Size = new System.Drawing.Size(714, 507);
            this.gv_IPVidSources.TabIndex = 0;
            // 
            // IPCameras
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 588);
            this.Controls.Add(this.gb_IPVidSources);
            this.Controls.Add(this.btn_Save);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "IPCameras";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "IP Cameras";
            this.gb_IPVidSources.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gv_IPVidSources)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.GroupBox gb_IPVidSources;
        private System.Windows.Forms.DataGridView gv_IPVidSources;
        private System.Windows.Forms.DataGridViewTextBoxColumn CameraName;
    }
}
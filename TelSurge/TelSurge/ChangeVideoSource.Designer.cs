namespace TelSurge
{
    partial class ChangeVideoSource
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeVideoSource));
            this.btn_Done = new System.Windows.Forms.Button();
            this.ddl_Devices = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btn_Done
            // 
            this.btn_Done.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Done.Location = new System.Drawing.Point(51, 39);
            this.btn_Done.Name = "btn_Done";
            this.btn_Done.Size = new System.Drawing.Size(88, 29);
            this.btn_Done.TabIndex = 6;
            this.btn_Done.Text = "Done";
            this.btn_Done.UseVisualStyleBackColor = true;
            this.btn_Done.Click += new System.EventHandler(this.btn_Done_Click);
            // 
            // ddl_Devices
            // 
            this.ddl_Devices.FormattingEnabled = true;
            this.ddl_Devices.Location = new System.Drawing.Point(12, 12);
            this.ddl_Devices.Name = "ddl_Devices";
            this.ddl_Devices.Size = new System.Drawing.Size(165, 21);
            this.ddl_Devices.TabIndex = 7;
            // 
            // ChangeVideoSource
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(192, 82);
            this.Controls.Add(this.ddl_Devices);
            this.Controls.Add(this.btn_Done);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChangeVideoSource";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Change Video Source";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Done;
        private System.Windows.Forms.ComboBox ddl_Devices;
    }
}
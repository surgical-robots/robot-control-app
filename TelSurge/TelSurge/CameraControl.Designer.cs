namespace TelSurge
{
    partial class CameraControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CameraControl));
            this.btn_Left = new System.Windows.Forms.Button();
            this.btn_Up = new System.Windows.Forms.Button();
            this.btn_Down = new System.Windows.Forms.Button();
            this.btn_Right = new System.Windows.Forms.Button();
            this.btn_ZoomIn = new System.Windows.Forms.Button();
            this.btn_ZoomOut = new System.Windows.Forms.Button();
            this.btn_StopPT = new System.Windows.Forms.Button();
            this.btn_laser = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Left
            // 
            this.btn_Left.FlatAppearance.BorderSize = 0;
            this.btn_Left.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Left.Image = ((System.Drawing.Image)(resources.GetObject("btn_Left.Image")));
            this.btn_Left.Location = new System.Drawing.Point(17, 104);
            this.btn_Left.Name = "btn_Left";
            this.btn_Left.Size = new System.Drawing.Size(67, 45);
            this.btn_Left.TabIndex = 0;
            this.btn_Left.UseVisualStyleBackColor = true;
            this.btn_Left.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_Left_MouseDown);
            this.btn_Left.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_PanTilt_MouseUp);
            // 
            // btn_Up
            // 
            this.btn_Up.FlatAppearance.BorderSize = 0;
            this.btn_Up.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Up.Image = ((System.Drawing.Image)(resources.GetObject("btn_Up.Image")));
            this.btn_Up.Location = new System.Drawing.Point(85, 36);
            this.btn_Up.Name = "btn_Up";
            this.btn_Up.Size = new System.Drawing.Size(45, 64);
            this.btn_Up.TabIndex = 1;
            this.btn_Up.UseVisualStyleBackColor = true;
            this.btn_Up.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_Up_MouseDown);
            this.btn_Up.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_PanTilt_MouseUp);
            // 
            // btn_Down
            // 
            this.btn_Down.FlatAppearance.BorderSize = 0;
            this.btn_Down.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Down.Image = ((System.Drawing.Image)(resources.GetObject("btn_Down.Image")));
            this.btn_Down.Location = new System.Drawing.Point(85, 151);
            this.btn_Down.Name = "btn_Down";
            this.btn_Down.Size = new System.Drawing.Size(45, 64);
            this.btn_Down.TabIndex = 2;
            this.btn_Down.UseVisualStyleBackColor = true;
            this.btn_Down.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_Down_MouseDown);
            this.btn_Down.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_PanTilt_MouseUp);
            // 
            // btn_Right
            // 
            this.btn_Right.FlatAppearance.BorderSize = 0;
            this.btn_Right.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Right.Image = ((System.Drawing.Image)(resources.GetObject("btn_Right.Image")));
            this.btn_Right.Location = new System.Drawing.Point(131, 104);
            this.btn_Right.Name = "btn_Right";
            this.btn_Right.Size = new System.Drawing.Size(67, 45);
            this.btn_Right.TabIndex = 3;
            this.btn_Right.UseVisualStyleBackColor = true;
            this.btn_Right.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_Right_MouseDown);
            this.btn_Right.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_PanTilt_MouseUp);
            // 
            // btn_ZoomIn
            // 
            this.btn_ZoomIn.FlatAppearance.BorderSize = 0;
            this.btn_ZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ZoomIn.Image = ((System.Drawing.Image)(resources.GetObject("btn_ZoomIn.Image")));
            this.btn_ZoomIn.Location = new System.Drawing.Point(198, 62);
            this.btn_ZoomIn.Name = "btn_ZoomIn";
            this.btn_ZoomIn.Size = new System.Drawing.Size(60, 67);
            this.btn_ZoomIn.TabIndex = 4;
            this.btn_ZoomIn.UseVisualStyleBackColor = true;
            this.btn_ZoomIn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_ZoomIn_MouseDown);
            this.btn_ZoomIn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_ZoomStop_MouseUp);
            // 
            // btn_ZoomOut
            // 
            this.btn_ZoomOut.FlatAppearance.BorderSize = 0;
            this.btn_ZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("btn_ZoomOut.Image")));
            this.btn_ZoomOut.Location = new System.Drawing.Point(198, 136);
            this.btn_ZoomOut.Name = "btn_ZoomOut";
            this.btn_ZoomOut.Size = new System.Drawing.Size(60, 67);
            this.btn_ZoomOut.TabIndex = 5;
            this.btn_ZoomOut.UseVisualStyleBackColor = true;
            this.btn_ZoomOut.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_ZoomOut_MouseDown);
            this.btn_ZoomOut.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_ZoomStop_MouseUp);
            // 
            // btn_StopPT
            // 
            this.btn_StopPT.FlatAppearance.BorderSize = 0;
            this.btn_StopPT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_StopPT.Image = ((System.Drawing.Image)(resources.GetObject("btn_StopPT.Image")));
            this.btn_StopPT.Location = new System.Drawing.Point(79, 97);
            this.btn_StopPT.Name = "btn_StopPT";
            this.btn_StopPT.Size = new System.Drawing.Size(56, 57);
            this.btn_StopPT.TabIndex = 6;
            this.btn_StopPT.UseVisualStyleBackColor = true;
            this.btn_StopPT.Click += new System.EventHandler(this.btn_StopPT_Click);
            // 
            // btn_laser
            // 
            this.btn_laser.BackColor = System.Drawing.Color.White;
            this.btn_laser.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_laser.ForeColor = System.Drawing.Color.Red;
            this.btn_laser.Location = new System.Drawing.Point(162, 14);
            this.btn_laser.Name = "btn_laser";
            this.btn_laser.Size = new System.Drawing.Size(96, 27);
            this.btn_laser.TabIndex = 104;
            this.btn_laser.Text = "laser";
            this.btn_laser.UseVisualStyleBackColor = false;
            this.btn_laser.Click += new System.EventHandler(this.btn_laser_Click);
            // 
            // CameraControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 230);
            this.Controls.Add(this.btn_laser);
            this.Controls.Add(this.btn_StopPT);
            this.Controls.Add(this.btn_ZoomOut);
            this.Controls.Add(this.btn_ZoomIn);
            this.Controls.Add(this.btn_Right);
            this.Controls.Add(this.btn_Down);
            this.Controls.Add(this.btn_Up);
            this.Controls.Add(this.btn_Left);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CameraControl";
            this.Text = "Camera Control";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Left;
        private System.Windows.Forms.Button btn_Up;
        private System.Windows.Forms.Button btn_Down;
        private System.Windows.Forms.Button btn_Right;
        private System.Windows.Forms.Button btn_ZoomIn;
        private System.Windows.Forms.Button btn_ZoomOut;
        private System.Windows.Forms.Button btn_StopPT;
        private System.Windows.Forms.Button btn_laser;
    }
}
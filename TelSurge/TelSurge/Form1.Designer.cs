namespace TelSurge
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.gb_OmniControls = new System.Windows.Forms.GroupBox();
            this.gb_SendingRight = new System.Windows.Forms.GroupBox();
            this.tb_SendingRight = new System.Windows.Forms.TextBox();
            this.gb_SendingLeft = new System.Windows.Forms.GroupBox();
            this.tb_SendingLeft = new System.Windows.Forms.TextBox();
            this.btn_ReqControl = new System.Windows.Forms.Button();
            this.tb_InControl = new System.Windows.Forms.TextBox();
            this.cb_isVPN = new System.Windows.Forms.CheckBox();
            this.cb_noOmnisAttached = new System.Windows.Forms.CheckBox();
            this.lbl_forceStrength = new System.Windows.Forms.Label();
            this.trb_forceStrength = new System.Windows.Forms.TrackBar();
            this.tb_ipAddress = new System.Windows.Forms.TextBox();
            this.lbl_myIP = new System.Windows.Forms.Label();
            this.cb_isMaster = new System.Windows.Forms.CheckBox();
            this.btn_zeroForces = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tb_forces = new System.Windows.Forms.TextBox();
            this.tb_InstanceName = new System.Windows.Forms.TextBox();
            this.NameLabel = new System.Windows.Forms.Label();
            this.ConnectToMasterButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbInk2 = new System.Windows.Forms.Label();
            this.lbButtons2 = new System.Windows.Forms.Label();
            this.lbGimbal32 = new System.Windows.Forms.Label();
            this.lbGimbal22 = new System.Windows.Forms.Label();
            this.lbGimbal12 = new System.Windows.Forms.Label();
            this.lbX2Value = new System.Windows.Forms.Label();
            this.lbY2Value = new System.Windows.Forms.Label();
            this.lbZ2Value = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lbInk1 = new System.Windows.Forms.Label();
            this.lbButtons1 = new System.Windows.Forms.Label();
            this.lbGimbal31 = new System.Windows.Forms.Label();
            this.lbGimbal21 = new System.Windows.Forms.Label();
            this.lbGimbal11 = new System.Windows.Forms.Label();
            this.lbX1value = new System.Windows.Forms.Label();
            this.lbY1value = new System.Windows.Forms.Label();
            this.lbZ1value = new System.Windows.Forms.Label();
            this.btn_Initialize = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.spRightOmni = new System.Windows.Forms.ComboBox();
            this.spLeftOmni = new System.Windows.Forms.ComboBox();
            this.gb_Telestration = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_Capture = new System.Windows.Forms.Button();
            this.btn_RedPen = new System.Windows.Forms.Button();
            this.btn_BlackPen = new System.Windows.Forms.Button();
            this.btn_BluePen = new System.Windows.Forms.Button();
            this.btn_WhitePen = new System.Windows.Forms.Button();
            this.btn_YellowPen = new System.Windows.Forms.Button();
            this.btn_GreenPen = new System.Windows.Forms.Button();
            this.btn_UndoMark = new System.Windows.Forms.Button();
            this.btn_ClearMarks = new System.Windows.Forms.Button();
            this.btn_StartAudio = new System.Windows.Forms.Button();
            this.ddl_AudioDevices = new System.Windows.Forms.ComboBox();
            this.captureImageBox = new Emgu.CV.UI.ImageBox();
            this.UnderlyingTimer = new System.Windows.Forms.Timer(this.components);
            this.ss_Connections = new System.Windows.Forms.StatusStrip();
            this.lbl_Errors = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbl_Connections = new System.Windows.Forms.ToolStripStatusLabel();
            this.errorTimer = new System.Windows.Forms.Timer(this.components);
            this.gb_OmniControls.SuspendLayout();
            this.gb_SendingRight.SuspendLayout();
            this.gb_SendingLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trb_forceStrength)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.gb_Telestration.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.captureImageBox)).BeginInit();
            this.ss_Connections.SuspendLayout();
            this.SuspendLayout();
            // 
            // gb_OmniControls
            // 
            this.gb_OmniControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_OmniControls.Controls.Add(this.gb_SendingRight);
            this.gb_OmniControls.Controls.Add(this.gb_SendingLeft);
            this.gb_OmniControls.Controls.Add(this.btn_ReqControl);
            this.gb_OmniControls.Controls.Add(this.tb_InControl);
            this.gb_OmniControls.Controls.Add(this.cb_isVPN);
            this.gb_OmniControls.Controls.Add(this.cb_noOmnisAttached);
            this.gb_OmniControls.Controls.Add(this.lbl_forceStrength);
            this.gb_OmniControls.Controls.Add(this.trb_forceStrength);
            this.gb_OmniControls.Controls.Add(this.tb_ipAddress);
            this.gb_OmniControls.Controls.Add(this.lbl_myIP);
            this.gb_OmniControls.Controls.Add(this.cb_isMaster);
            this.gb_OmniControls.Controls.Add(this.btn_zeroForces);
            this.gb_OmniControls.Controls.Add(this.groupBox3);
            this.gb_OmniControls.Controls.Add(this.tb_InstanceName);
            this.gb_OmniControls.Controls.Add(this.NameLabel);
            this.gb_OmniControls.Controls.Add(this.ConnectToMasterButton);
            this.gb_OmniControls.Controls.Add(this.groupBox2);
            this.gb_OmniControls.Controls.Add(this.groupBox4);
            this.gb_OmniControls.Controls.Add(this.btn_Initialize);
            this.gb_OmniControls.Controls.Add(this.label5);
            this.gb_OmniControls.Controls.Add(this.label4);
            this.gb_OmniControls.Controls.Add(this.spRightOmni);
            this.gb_OmniControls.Controls.Add(this.spLeftOmni);
            this.gb_OmniControls.Location = new System.Drawing.Point(826, 0);
            this.gb_OmniControls.Name = "gb_OmniControls";
            this.gb_OmniControls.Size = new System.Drawing.Size(450, 683);
            this.gb_OmniControls.TabIndex = 5;
            this.gb_OmniControls.TabStop = false;
            this.gb_OmniControls.Text = "Controls";
            // 
            // gb_SendingRight
            // 
            this.gb_SendingRight.Controls.Add(this.tb_SendingRight);
            this.gb_SendingRight.Location = new System.Drawing.Point(242, 586);
            this.gb_SendingRight.Name = "gb_SendingRight";
            this.gb_SendingRight.Size = new System.Drawing.Size(172, 91);
            this.gb_SendingRight.TabIndex = 101;
            this.gb_SendingRight.TabStop = false;
            this.gb_SendingRight.Text = "Sending Right";
            this.gb_SendingRight.Visible = false;
            // 
            // tb_SendingRight
            // 
            this.tb_SendingRight.BackColor = System.Drawing.SystemColors.Control;
            this.tb_SendingRight.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_SendingRight.Location = new System.Drawing.Point(6, 16);
            this.tb_SendingRight.Multiline = true;
            this.tb_SendingRight.Name = "tb_SendingRight";
            this.tb_SendingRight.ReadOnly = true;
            this.tb_SendingRight.Size = new System.Drawing.Size(159, 69);
            this.tb_SendingRight.TabIndex = 102;
            this.tb_SendingRight.TabStop = false;
            this.tb_SendingRight.Visible = false;
            // 
            // gb_SendingLeft
            // 
            this.gb_SendingLeft.Controls.Add(this.tb_SendingLeft);
            this.gb_SendingLeft.Location = new System.Drawing.Point(34, 586);
            this.gb_SendingLeft.Name = "gb_SendingLeft";
            this.gb_SendingLeft.Size = new System.Drawing.Size(172, 91);
            this.gb_SendingLeft.TabIndex = 100;
            this.gb_SendingLeft.TabStop = false;
            this.gb_SendingLeft.Text = "Sending Left";
            this.gb_SendingLeft.Visible = false;
            // 
            // tb_SendingLeft
            // 
            this.tb_SendingLeft.BackColor = System.Drawing.SystemColors.Control;
            this.tb_SendingLeft.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_SendingLeft.Location = new System.Drawing.Point(7, 20);
            this.tb_SendingLeft.Multiline = true;
            this.tb_SendingLeft.Name = "tb_SendingLeft";
            this.tb_SendingLeft.ReadOnly = true;
            this.tb_SendingLeft.Size = new System.Drawing.Size(159, 65);
            this.tb_SendingLeft.TabIndex = 0;
            this.tb_SendingLeft.TabStop = false;
            this.tb_SendingLeft.Visible = false;
            // 
            // btn_ReqControl
            // 
            this.btn_ReqControl.BackColor = System.Drawing.Color.LemonChiffon;
            this.btn_ReqControl.Enabled = false;
            this.btn_ReqControl.FlatAppearance.BorderSize = 0;
            this.btn_ReqControl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ReqControl.Location = new System.Drawing.Point(299, 67);
            this.btn_ReqControl.Name = "btn_ReqControl";
            this.btn_ReqControl.Size = new System.Drawing.Size(118, 36);
            this.btn_ReqControl.TabIndex = 99;
            this.btn_ReqControl.Text = "Request Control";
            this.btn_ReqControl.UseVisualStyleBackColor = false;
            this.btn_ReqControl.Click += new System.EventHandler(this.btn_ReqControl_Click);
            // 
            // tb_InControl
            // 
            this.tb_InControl.BackColor = System.Drawing.SystemColors.Control;
            this.tb_InControl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_InControl.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_InControl.Location = new System.Drawing.Point(6, 16);
            this.tb_InControl.Name = "tb_InControl";
            this.tb_InControl.ReadOnly = true;
            this.tb_InControl.Size = new System.Drawing.Size(438, 19);
            this.tb_InControl.TabIndex = 98;
            this.tb_InControl.TabStop = false;
            this.tb_InControl.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cb_isVPN
            // 
            this.cb_isVPN.AutoSize = true;
            this.cb_isVPN.Location = new System.Drawing.Point(37, 90);
            this.cb_isVPN.Name = "cb_isVPN";
            this.cb_isVPN.Size = new System.Drawing.Size(48, 17);
            this.cb_isVPN.TabIndex = 97;
            this.cb_isVPN.Text = "VPN";
            this.cb_isVPN.UseVisualStyleBackColor = true;
            // 
            // cb_noOmnisAttached
            // 
            this.cb_noOmnisAttached.AutoSize = true;
            this.cb_noOmnisAttached.Location = new System.Drawing.Point(37, 67);
            this.cb_noOmnisAttached.Name = "cb_noOmnisAttached";
            this.cb_noOmnisAttached.Size = new System.Drawing.Size(72, 17);
            this.cb_noOmnisAttached.TabIndex = 96;
            this.cb_noOmnisAttached.Text = "No Omnis";
            this.cb_noOmnisAttached.UseVisualStyleBackColor = true;
            this.cb_noOmnisAttached.CheckedChanged += new System.EventHandler(this.cb_noOmnisAttached_CheckedChanged);
            // 
            // lbl_forceStrength
            // 
            this.lbl_forceStrength.AutoSize = true;
            this.lbl_forceStrength.Enabled = false;
            this.lbl_forceStrength.Location = new System.Drawing.Point(184, 538);
            this.lbl_forceStrength.Name = "lbl_forceStrength";
            this.lbl_forceStrength.Size = new System.Drawing.Size(77, 13);
            this.lbl_forceStrength.TabIndex = 95;
            this.lbl_forceStrength.Text = "Force Strength";
            // 
            // trb_forceStrength
            // 
            this.trb_forceStrength.Enabled = false;
            this.trb_forceStrength.Location = new System.Drawing.Point(34, 547);
            this.trb_forceStrength.Name = "trb_forceStrength";
            this.trb_forceStrength.Size = new System.Drawing.Size(380, 40);
            this.trb_forceStrength.TabIndex = 9;
            // 
            // tb_ipAddress
            // 
            this.tb_ipAddress.BackColor = System.Drawing.SystemColors.Window;
            this.tb_ipAddress.Location = new System.Drawing.Point(299, 41);
            this.tb_ipAddress.Name = "tb_ipAddress";
            this.tb_ipAddress.Size = new System.Drawing.Size(118, 20);
            this.tb_ipAddress.TabIndex = 1;
            // 
            // lbl_myIP
            // 
            this.lbl_myIP.AutoSize = true;
            this.lbl_myIP.Location = new System.Drawing.Point(193, 44);
            this.lbl_myIP.Name = "lbl_myIP";
            this.lbl_myIP.Size = new System.Drawing.Size(100, 13);
            this.lbl_myIP.TabIndex = 92;
            this.lbl_myIP.Text = "Master\'s IP Address";
            // 
            // cb_isMaster
            // 
            this.cb_isMaster.AutoSize = true;
            this.cb_isMaster.Location = new System.Drawing.Point(37, 44);
            this.cb_isMaster.Name = "cb_isMaster";
            this.cb_isMaster.Size = new System.Drawing.Size(92, 17);
            this.cb_isMaster.TabIndex = 91;
            this.cb_isMaster.Text = "Set As Master";
            this.cb_isMaster.UseVisualStyleBackColor = true;
            this.cb_isMaster.CheckedChanged += new System.EventHandler(this.cb_isMaster_CheckedChanged);
            // 
            // btn_zeroForces
            // 
            this.btn_zeroForces.Enabled = false;
            this.btn_zeroForces.Location = new System.Drawing.Point(299, 420);
            this.btn_zeroForces.Name = "btn_zeroForces";
            this.btn_zeroForces.Size = new System.Drawing.Size(96, 23);
            this.btn_zeroForces.TabIndex = 7;
            this.btn_zeroForces.Text = "Zero";
            this.btn_zeroForces.UseVisualStyleBackColor = true;
            this.btn_zeroForces.Click += new System.EventHandler(this.btn_zeroForces_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tb_forces);
            this.groupBox3.Enabled = false;
            this.groupBox3.Location = new System.Drawing.Point(43, 442);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(358, 90);
            this.groupBox3.TabIndex = 84;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Forces";
            // 
            // tb_forces
            // 
            this.tb_forces.Enabled = false;
            this.tb_forces.Location = new System.Drawing.Point(7, 12);
            this.tb_forces.Multiline = true;
            this.tb_forces.Name = "tb_forces";
            this.tb_forces.ReadOnly = true;
            this.tb_forces.Size = new System.Drawing.Size(345, 72);
            this.tb_forces.TabIndex = 0;
            // 
            // tb_InstanceName
            // 
            this.tb_InstanceName.Location = new System.Drawing.Point(152, 83);
            this.tb_InstanceName.Name = "tb_InstanceName";
            this.tb_InstanceName.Size = new System.Drawing.Size(141, 20);
            this.tb_InstanceName.TabIndex = 2;
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(196, 67);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(52, 13);
            this.NameLabel.TabIndex = 87;
            this.NameLabel.Text = "My Name";
            // 
            // ConnectToMasterButton
            // 
            this.ConnectToMasterButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConnectToMasterButton.Location = new System.Drawing.Point(34, 350);
            this.ConnectToMasterButton.Name = "ConnectToMasterButton";
            this.ConnectToMasterButton.Size = new System.Drawing.Size(380, 58);
            this.ConnectToMasterButton.TabIndex = 6;
            this.ConnectToMasterButton.Text = "Connect To Master";
            this.ConnectToMasterButton.UseVisualStyleBackColor = true;
            this.ConnectToMasterButton.Visible = false;
            this.ConnectToMasterButton.Click += new System.EventHandler(this.ConnectToMasterButtonClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbInk2);
            this.groupBox2.Controls.Add(this.lbButtons2);
            this.groupBox2.Controls.Add(this.lbGimbal32);
            this.groupBox2.Controls.Add(this.lbGimbal22);
            this.groupBox2.Controls.Add(this.lbGimbal12);
            this.groupBox2.Controls.Add(this.lbX2Value);
            this.groupBox2.Controls.Add(this.lbY2Value);
            this.groupBox2.Controls.Add(this.lbZ2Value);
            this.groupBox2.Location = new System.Drawing.Point(242, 152);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(172, 138);
            this.groupBox2.TabIndex = 85;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Right Omni Stats";
            // 
            // lbInk2
            // 
            this.lbInk2.AutoSize = true;
            this.lbInk2.Location = new System.Drawing.Point(7, 108);
            this.lbInk2.Name = "lbInk2";
            this.lbInk2.Size = new System.Drawing.Size(49, 13);
            this.lbInk2.TabIndex = 9;
            this.lbInk2.Text = "InkWell :";
            // 
            // lbButtons2
            // 
            this.lbButtons2.AutoSize = true;
            this.lbButtons2.Location = new System.Drawing.Point(6, 95);
            this.lbButtons2.Name = "lbButtons2";
            this.lbButtons2.Size = new System.Drawing.Size(49, 13);
            this.lbButtons2.TabIndex = 8;
            this.lbButtons2.Text = "Buttons :";
            // 
            // lbGimbal32
            // 
            this.lbGimbal32.AutoSize = true;
            this.lbGimbal32.Location = new System.Drawing.Point(6, 82);
            this.lbGimbal32.Name = "lbGimbal32";
            this.lbGimbal32.Size = new System.Drawing.Size(54, 13);
            this.lbGimbal32.TabIndex = 7;
            this.lbGimbal32.Text = "Gimbal 3 :";
            // 
            // lbGimbal22
            // 
            this.lbGimbal22.AutoSize = true;
            this.lbGimbal22.Location = new System.Drawing.Point(6, 69);
            this.lbGimbal22.Name = "lbGimbal22";
            this.lbGimbal22.Size = new System.Drawing.Size(54, 13);
            this.lbGimbal22.TabIndex = 6;
            this.lbGimbal22.Text = "Gimbal 2 :";
            // 
            // lbGimbal12
            // 
            this.lbGimbal12.AutoSize = true;
            this.lbGimbal12.Location = new System.Drawing.Point(6, 56);
            this.lbGimbal12.Name = "lbGimbal12";
            this.lbGimbal12.Size = new System.Drawing.Size(57, 13);
            this.lbGimbal12.TabIndex = 5;
            this.lbGimbal12.Text = "Gimbal 1 : ";
            // 
            // lbX2Value
            // 
            this.lbX2Value.AutoSize = true;
            this.lbX2Value.Location = new System.Drawing.Point(6, 16);
            this.lbX2Value.Name = "lbX2Value";
            this.lbX2Value.Size = new System.Drawing.Size(20, 13);
            this.lbX2Value.TabIndex = 1;
            this.lbX2Value.Text = "X :";
            // 
            // lbY2Value
            // 
            this.lbY2Value.AutoSize = true;
            this.lbY2Value.Location = new System.Drawing.Point(6, 30);
            this.lbY2Value.Name = "lbY2Value";
            this.lbY2Value.Size = new System.Drawing.Size(20, 13);
            this.lbY2Value.TabIndex = 3;
            this.lbY2Value.Text = "Y :";
            // 
            // lbZ2Value
            // 
            this.lbZ2Value.AutoSize = true;
            this.lbZ2Value.Location = new System.Drawing.Point(6, 43);
            this.lbZ2Value.Name = "lbZ2Value";
            this.lbZ2Value.Size = new System.Drawing.Size(23, 13);
            this.lbZ2Value.TabIndex = 4;
            this.lbZ2Value.Text = "Z : ";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lbInk1);
            this.groupBox4.Controls.Add(this.lbButtons1);
            this.groupBox4.Controls.Add(this.lbGimbal31);
            this.groupBox4.Controls.Add(this.lbGimbal21);
            this.groupBox4.Controls.Add(this.lbGimbal11);
            this.groupBox4.Controls.Add(this.lbX1value);
            this.groupBox4.Controls.Add(this.lbY1value);
            this.groupBox4.Controls.Add(this.lbZ1value);
            this.groupBox4.Location = new System.Drawing.Point(34, 152);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(172, 138);
            this.groupBox4.TabIndex = 83;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Left Omni Stats";
            // 
            // lbInk1
            // 
            this.lbInk1.AutoSize = true;
            this.lbInk1.Location = new System.Drawing.Point(6, 108);
            this.lbInk1.Name = "lbInk1";
            this.lbInk1.Size = new System.Drawing.Size(52, 13);
            this.lbInk1.TabIndex = 9;
            this.lbInk1.Text = "InkWell : ";
            // 
            // lbButtons1
            // 
            this.lbButtons1.AutoSize = true;
            this.lbButtons1.Location = new System.Drawing.Point(6, 95);
            this.lbButtons1.Name = "lbButtons1";
            this.lbButtons1.Size = new System.Drawing.Size(49, 13);
            this.lbButtons1.TabIndex = 8;
            this.lbButtons1.Text = "Buttons :";
            // 
            // lbGimbal31
            // 
            this.lbGimbal31.AutoSize = true;
            this.lbGimbal31.Location = new System.Drawing.Point(6, 82);
            this.lbGimbal31.Name = "lbGimbal31";
            this.lbGimbal31.Size = new System.Drawing.Size(54, 13);
            this.lbGimbal31.TabIndex = 7;
            this.lbGimbal31.Text = "Gimbal 3 :";
            // 
            // lbGimbal21
            // 
            this.lbGimbal21.AutoSize = true;
            this.lbGimbal21.Location = new System.Drawing.Point(6, 69);
            this.lbGimbal21.Name = "lbGimbal21";
            this.lbGimbal21.Size = new System.Drawing.Size(54, 13);
            this.lbGimbal21.TabIndex = 6;
            this.lbGimbal21.Text = "Gimbal 2 :";
            // 
            // lbGimbal11
            // 
            this.lbGimbal11.AutoSize = true;
            this.lbGimbal11.Location = new System.Drawing.Point(6, 56);
            this.lbGimbal11.Name = "lbGimbal11";
            this.lbGimbal11.Size = new System.Drawing.Size(54, 13);
            this.lbGimbal11.TabIndex = 5;
            this.lbGimbal11.Text = "Gimbal 1 :";
            // 
            // lbX1value
            // 
            this.lbX1value.AutoSize = true;
            this.lbX1value.Location = new System.Drawing.Point(6, 16);
            this.lbX1value.Name = "lbX1value";
            this.lbX1value.Size = new System.Drawing.Size(23, 13);
            this.lbX1value.TabIndex = 1;
            this.lbX1value.Text = "X : ";
            // 
            // lbY1value
            // 
            this.lbY1value.AutoSize = true;
            this.lbY1value.Location = new System.Drawing.Point(6, 30);
            this.lbY1value.Name = "lbY1value";
            this.lbY1value.Size = new System.Drawing.Size(20, 13);
            this.lbY1value.TabIndex = 3;
            this.lbY1value.Text = "Y :";
            // 
            // lbZ1value
            // 
            this.lbZ1value.AutoSize = true;
            this.lbZ1value.Location = new System.Drawing.Point(6, 43);
            this.lbZ1value.Name = "lbZ1value";
            this.lbZ1value.Size = new System.Drawing.Size(20, 13);
            this.lbZ1value.TabIndex = 4;
            this.lbZ1value.Text = "Z :";
            // 
            // btn_Initialize
            // 
            this.btn_Initialize.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Initialize.Location = new System.Drawing.Point(34, 296);
            this.btn_Initialize.Name = "btn_Initialize";
            this.btn_Initialize.Size = new System.Drawing.Size(380, 54);
            this.btn_Initialize.TabIndex = 5;
            this.btn_Initialize.Text = "Initialize";
            this.btn_Initialize.UseVisualStyleBackColor = true;
            this.btn_Initialize.Click += new System.EventHandler(this.InitializeOmnis_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(296, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 81;
            this.label5.Text = "Right Omni";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(95, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 80;
            this.label4.Text = "Left Omni";
            // 
            // spRightOmni
            // 
            this.spRightOmni.FormattingEnabled = true;
            this.spRightOmni.Location = new System.Drawing.Point(265, 125);
            this.spRightOmni.Name = "spRightOmni";
            this.spRightOmni.Size = new System.Drawing.Size(121, 21);
            this.spRightOmni.TabIndex = 4;
            // 
            // spLeftOmni
            // 
            this.spLeftOmni.FormattingEnabled = true;
            this.spLeftOmni.Location = new System.Drawing.Point(63, 125);
            this.spLeftOmni.Name = "spLeftOmni";
            this.spLeftOmni.Size = new System.Drawing.Size(121, 21);
            this.spLeftOmni.TabIndex = 3;
            // 
            // gb_Telestration
            // 
            this.gb_Telestration.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gb_Telestration.Controls.Add(this.flowLayoutPanel1);
            this.gb_Telestration.Controls.Add(this.captureImageBox);
            this.gb_Telestration.Location = new System.Drawing.Point(0, 0);
            this.gb_Telestration.Name = "gb_Telestration";
            this.gb_Telestration.Size = new System.Drawing.Size(826, 683);
            this.gb_Telestration.TabIndex = 6;
            this.gb_Telestration.TabStop = false;
            this.gb_Telestration.Text = "Telestration";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel1.Controls.Add(this.btn_Capture);
            this.flowLayoutPanel1.Controls.Add(this.btn_RedPen);
            this.flowLayoutPanel1.Controls.Add(this.btn_BlackPen);
            this.flowLayoutPanel1.Controls.Add(this.btn_BluePen);
            this.flowLayoutPanel1.Controls.Add(this.btn_WhitePen);
            this.flowLayoutPanel1.Controls.Add(this.btn_YellowPen);
            this.flowLayoutPanel1.Controls.Add(this.btn_GreenPen);
            this.flowLayoutPanel1.Controls.Add(this.btn_UndoMark);
            this.flowLayoutPanel1.Controls.Add(this.btn_ClearMarks);
            this.flowLayoutPanel1.Controls.Add(this.btn_StartAudio);
            this.flowLayoutPanel1.Controls.Add(this.ddl_AudioDevices);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(820, 56);
            this.flowLayoutPanel1.TabIndex = 7;
            this.flowLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanel1_Paint);
            // 
            // btn_Capture
            // 
            this.btn_Capture.Location = new System.Drawing.Point(3, 3);
            this.btn_Capture.Name = "btn_Capture";
            this.btn_Capture.Size = new System.Drawing.Size(75, 50);
            this.btn_Capture.TabIndex = 7;
            this.btn_Capture.Text = "Start";
            this.btn_Capture.UseVisualStyleBackColor = true;
            this.btn_Capture.Visible = false;
            this.btn_Capture.Click += new System.EventHandler(this.btn_Capture_Click);
            // 
            // btn_RedPen
            // 
            this.btn_RedPen.BackColor = System.Drawing.Color.Red;
            this.btn_RedPen.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.btn_RedPen.Location = new System.Drawing.Point(84, 3);
            this.btn_RedPen.Name = "btn_RedPen";
            this.btn_RedPen.Size = new System.Drawing.Size(58, 50);
            this.btn_RedPen.TabIndex = 0;
            this.btn_RedPen.UseVisualStyleBackColor = false;
            this.btn_RedPen.Click += new System.EventHandler(this.btn_PenColor_Click);
            // 
            // btn_BlackPen
            // 
            this.btn_BlackPen.BackColor = System.Drawing.Color.Black;
            this.btn_BlackPen.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btn_BlackPen.Location = new System.Drawing.Point(148, 3);
            this.btn_BlackPen.Name = "btn_BlackPen";
            this.btn_BlackPen.Size = new System.Drawing.Size(58, 50);
            this.btn_BlackPen.TabIndex = 1;
            this.btn_BlackPen.UseVisualStyleBackColor = false;
            this.btn_BlackPen.Click += new System.EventHandler(this.btn_PenColor_Click);
            // 
            // btn_BluePen
            // 
            this.btn_BluePen.BackColor = System.Drawing.Color.Blue;
            this.btn_BluePen.FlatAppearance.BorderColor = System.Drawing.Color.Blue;
            this.btn_BluePen.Location = new System.Drawing.Point(212, 3);
            this.btn_BluePen.Name = "btn_BluePen";
            this.btn_BluePen.Size = new System.Drawing.Size(58, 50);
            this.btn_BluePen.TabIndex = 2;
            this.btn_BluePen.UseVisualStyleBackColor = false;
            this.btn_BluePen.Click += new System.EventHandler(this.btn_PenColor_Click);
            // 
            // btn_WhitePen
            // 
            this.btn_WhitePen.BackColor = System.Drawing.Color.White;
            this.btn_WhitePen.Location = new System.Drawing.Point(276, 3);
            this.btn_WhitePen.Name = "btn_WhitePen";
            this.btn_WhitePen.Size = new System.Drawing.Size(58, 50);
            this.btn_WhitePen.TabIndex = 3;
            this.btn_WhitePen.UseVisualStyleBackColor = false;
            this.btn_WhitePen.Click += new System.EventHandler(this.btn_PenColor_Click);
            // 
            // btn_YellowPen
            // 
            this.btn_YellowPen.BackColor = System.Drawing.Color.Yellow;
            this.btn_YellowPen.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.btn_YellowPen.Location = new System.Drawing.Point(340, 3);
            this.btn_YellowPen.Name = "btn_YellowPen";
            this.btn_YellowPen.Size = new System.Drawing.Size(58, 50);
            this.btn_YellowPen.TabIndex = 4;
            this.btn_YellowPen.UseVisualStyleBackColor = false;
            this.btn_YellowPen.Click += new System.EventHandler(this.btn_PenColor_Click);
            // 
            // btn_GreenPen
            // 
            this.btn_GreenPen.BackColor = System.Drawing.Color.Green;
            this.btn_GreenPen.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btn_GreenPen.Location = new System.Drawing.Point(404, 3);
            this.btn_GreenPen.Name = "btn_GreenPen";
            this.btn_GreenPen.Size = new System.Drawing.Size(58, 50);
            this.btn_GreenPen.TabIndex = 5;
            this.btn_GreenPen.UseVisualStyleBackColor = false;
            this.btn_GreenPen.Click += new System.EventHandler(this.btn_PenColor_Click);
            // 
            // btn_UndoMark
            // 
            this.btn_UndoMark.BackColor = System.Drawing.Color.White;
            this.btn_UndoMark.BackgroundImage = global::TelSurge.Properties.Resources.Undo;
            this.btn_UndoMark.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_UndoMark.Location = new System.Drawing.Point(468, 3);
            this.btn_UndoMark.Name = "btn_UndoMark";
            this.btn_UndoMark.Size = new System.Drawing.Size(58, 50);
            this.btn_UndoMark.TabIndex = 8;
            this.btn_UndoMark.UseVisualStyleBackColor = false;
            this.btn_UndoMark.Visible = false;
            this.btn_UndoMark.Click += new System.EventHandler(this.btn_UndoMark_Click);
            // 
            // btn_ClearMarks
            // 
            this.btn_ClearMarks.Location = new System.Drawing.Point(532, 3);
            this.btn_ClearMarks.Name = "btn_ClearMarks";
            this.btn_ClearMarks.Size = new System.Drawing.Size(58, 50);
            this.btn_ClearMarks.TabIndex = 6;
            this.btn_ClearMarks.Text = "Clear";
            this.btn_ClearMarks.UseVisualStyleBackColor = true;
            this.btn_ClearMarks.Click += new System.EventHandler(this.btn_ClearMarks_Click);
            // 
            // btn_StartAudio
            // 
            this.btn_StartAudio.BackColor = System.Drawing.Color.Green;
            this.btn_StartAudio.Image = global::TelSurge.Properties.Resources.mic;
            this.btn_StartAudio.Location = new System.Drawing.Point(596, 3);
            this.btn_StartAudio.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.btn_StartAudio.Name = "btn_StartAudio";
            this.btn_StartAudio.Size = new System.Drawing.Size(30, 31);
            this.btn_StartAudio.TabIndex = 0;
            this.btn_StartAudio.UseVisualStyleBackColor = false;
            this.btn_StartAudio.Click += new System.EventHandler(this.btn_StartAudio_Click);
            // 
            // ddl_AudioDevices
            // 
            this.ddl_AudioDevices.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddl_AudioDevices.FormattingEnabled = true;
            this.ddl_AudioDevices.Location = new System.Drawing.Point(626, 5);
            this.ddl_AudioDevices.Margin = new System.Windows.Forms.Padding(0, 5, 3, 3);
            this.ddl_AudioDevices.Name = "ddl_AudioDevices";
            this.ddl_AudioDevices.Size = new System.Drawing.Size(189, 28);
            this.ddl_AudioDevices.TabIndex = 9;
            // 
            // captureImageBox
            // 
            this.captureImageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.captureImageBox.BackColor = System.Drawing.SystemColors.Control;
            this.captureImageBox.Location = new System.Drawing.Point(3, 16);
            this.captureImageBox.Name = "captureImageBox";
            this.captureImageBox.Size = new System.Drawing.Size(820, 667);
            this.captureImageBox.TabIndex = 5;
            this.captureImageBox.TabStop = false;
            this.captureImageBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.captureImageBox_MouseDown);
            this.captureImageBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.captureImageBox_MouseMove);
            this.captureImageBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.captureImageBox_MouseUp);
            // 
            // UnderlyingTimer
            // 
            this.UnderlyingTimer.Interval = 10;
            this.UnderlyingTimer.Tick += new System.EventHandler(this.UnderlyingTimerTick);
            // 
            // ss_Connections
            // 
            this.ss_Connections.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbl_Errors,
            this.lbl_Connections});
            this.ss_Connections.Location = new System.Drawing.Point(0, 686);
            this.ss_Connections.Name = "ss_Connections";
            this.ss_Connections.Size = new System.Drawing.Size(1276, 22);
            this.ss_Connections.TabIndex = 7;
            this.ss_Connections.Text = "statusStrip1";
            // 
            // lbl_Errors
            // 
            this.lbl_Errors.BackColor = System.Drawing.Color.MistyRose;
            this.lbl_Errors.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.lbl_Errors.ForeColor = System.Drawing.Color.Red;
            this.lbl_Errors.Name = "lbl_Errors";
            this.lbl_Errors.Size = new System.Drawing.Size(0, 17);
            // 
            // lbl_Connections
            // 
            this.lbl_Connections.Name = "lbl_Connections";
            this.lbl_Connections.Size = new System.Drawing.Size(109, 17);
            this.lbl_Connections.Text = "Connections: None";
            // 
            // errorTimer
            // 
            this.errorTimer.Interval = 8000;
            this.errorTimer.Tick += new System.EventHandler(this.errorTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1276, 708);
            this.Controls.Add(this.ss_Connections);
            this.Controls.Add(this.gb_Telestration);
            this.Controls.Add(this.gb_OmniControls);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "TelSurge";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.gb_OmniControls.ResumeLayout(false);
            this.gb_OmniControls.PerformLayout();
            this.gb_SendingRight.ResumeLayout(false);
            this.gb_SendingRight.PerformLayout();
            this.gb_SendingLeft.ResumeLayout(false);
            this.gb_SendingLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trb_forceStrength)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.gb_Telestration.ResumeLayout(false);
            this.gb_Telestration.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.captureImageBox)).EndInit();
            this.ss_Connections.ResumeLayout(false);
            this.ss_Connections.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gb_OmniControls;
        private System.Windows.Forms.CheckBox cb_isVPN;
        private System.Windows.Forms.CheckBox cb_noOmnisAttached;
        private System.Windows.Forms.Label lbl_forceStrength;
        private System.Windows.Forms.TrackBar trb_forceStrength;
        private System.Windows.Forms.TextBox tb_ipAddress;
        private System.Windows.Forms.Label lbl_myIP;
        private System.Windows.Forms.CheckBox cb_isMaster;
        private System.Windows.Forms.Button btn_zeroForces;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tb_forces;
        private System.Windows.Forms.TextBox tb_InstanceName;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Button ConnectToMasterButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbInk2;
        private System.Windows.Forms.Label lbButtons2;
        private System.Windows.Forms.Label lbGimbal32;
        private System.Windows.Forms.Label lbGimbal22;
        private System.Windows.Forms.Label lbGimbal12;
        private System.Windows.Forms.Label lbX2Value;
        private System.Windows.Forms.Label lbY2Value;
        private System.Windows.Forms.Label lbZ2Value;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lbInk1;
        private System.Windows.Forms.Label lbButtons1;
        private System.Windows.Forms.Label lbGimbal31;
        private System.Windows.Forms.Label lbGimbal21;
        private System.Windows.Forms.Label lbGimbal11;
        private System.Windows.Forms.Label lbX1value;
        private System.Windows.Forms.Label lbY1value;
        private System.Windows.Forms.Label lbZ1value;
        private System.Windows.Forms.Button btn_Initialize;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox spRightOmni;
        private System.Windows.Forms.ComboBox spLeftOmni;
        private System.Windows.Forms.GroupBox gb_Telestration;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btn_RedPen;
        private System.Windows.Forms.Button btn_BlackPen;
        private System.Windows.Forms.Button btn_BluePen;
        private System.Windows.Forms.Button btn_WhitePen;
        private System.Windows.Forms.Button btn_YellowPen;
        private System.Windows.Forms.Button btn_GreenPen;
        private System.Windows.Forms.Button btn_ClearMarks;
        private Emgu.CV.UI.ImageBox captureImageBox;
        private System.Windows.Forms.Button btn_Capture;
        private System.Windows.Forms.Button btn_UndoMark;
        private System.Windows.Forms.Timer UnderlyingTimer;
        private System.Windows.Forms.StatusStrip ss_Connections;
        private System.Windows.Forms.ToolStripStatusLabel lbl_Connections;
        private System.Windows.Forms.ToolStripStatusLabel lbl_Errors;
        private System.Windows.Forms.ComboBox ddl_AudioDevices;
        private System.Windows.Forms.Button btn_StartAudio;
        private System.Windows.Forms.Timer errorTimer;
        private System.Windows.Forms.TextBox tb_InControl;
        private System.Windows.Forms.GroupBox gb_SendingRight;
        private System.Windows.Forms.TextBox tb_SendingRight;
        private System.Windows.Forms.GroupBox gb_SendingLeft;
        private System.Windows.Forms.TextBox tb_SendingLeft;
        private System.Windows.Forms.Button btn_ReqControl;






    }
}
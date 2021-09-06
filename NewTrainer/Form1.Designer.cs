namespace NewTrainer
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabMain = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tPmessages = new System.Windows.Forms.TabPage();
            this.dgvMessages = new System.Windows.Forms.DataGridView();
            this.MesMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MesTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tPErrors = new System.Windows.Forms.TabPage();
            this.dgvErrors = new System.Windows.Forms.DataGridView();
            this.ErrMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ErrTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkbUsingProxyWF = new System.Windows.Forms.CheckBox();
            this.cbServer = new System.Windows.Forms.ComboBox();
            this.checkbIsHost = new System.Windows.Forms.CheckBox();
            this.btnTesting = new System.Windows.Forms.Button();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblRuCaptchaBalance = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabProxy = new System.Windows.Forms.TabPage();
            this.btnResetBadProxy = new System.Windows.Forms.Button();
            this.lblProxyes = new System.Windows.Forms.Label();
            this.btnSaveProxyes = new System.Windows.Forms.Button();
            this.rtbProxyes = new System.Windows.Forms.RichTextBox();
            this.timerUIUpdater = new System.Windows.Forms.Timer(this.components);
            this.timerKeyLissener = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tPmessages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMessages)).BeginInit();
            this.tPErrors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabProxy.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabMain);
            this.tabControl1.Controls.Add(this.tabProxy);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(686, 475);
            this.tabControl1.TabIndex = 0;
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tableLayoutPanel1);
            this.tabMain.Location = new System.Drawing.Point(4, 22);
            this.tabMain.Name = "tabMain";
            this.tabMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabMain.Size = new System.Drawing.Size(678, 449);
            this.tabMain.TabIndex = 0;
            this.tabMain.Text = "Main";
            this.tabMain.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.tabControl3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(672, 443);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.tPmessages);
            this.tabControl3.Controls.Add(this.tPErrors);
            this.tabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl3.Location = new System.Drawing.Point(3, 224);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(666, 216);
            this.tabControl3.TabIndex = 3;
            // 
            // tPmessages
            // 
            this.tPmessages.Controls.Add(this.dgvMessages);
            this.tPmessages.Location = new System.Drawing.Point(4, 22);
            this.tPmessages.Name = "tPmessages";
            this.tPmessages.Padding = new System.Windows.Forms.Padding(3);
            this.tPmessages.Size = new System.Drawing.Size(658, 190);
            this.tPmessages.TabIndex = 0;
            this.tPmessages.Text = "Messages";
            this.tPmessages.UseVisualStyleBackColor = true;
            // 
            // dgvMessages
            // 
            this.dgvMessages.AllowUserToAddRows = false;
            this.dgvMessages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMessages.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MesMessage,
            this.MesTime});
            this.dgvMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMessages.Location = new System.Drawing.Point(3, 3);
            this.dgvMessages.Name = "dgvMessages";
            this.dgvMessages.RowHeadersVisible = false;
            this.dgvMessages.Size = new System.Drawing.Size(652, 184);
            this.dgvMessages.TabIndex = 0;
            // 
            // MesMessage
            // 
            this.MesMessage.HeaderText = "Message";
            this.MesMessage.Name = "MesMessage";
            this.MesMessage.ReadOnly = true;
            // 
            // MesTime
            // 
            this.MesTime.HeaderText = "Time";
            this.MesTime.Name = "MesTime";
            this.MesTime.ReadOnly = true;
            // 
            // tPErrors
            // 
            this.tPErrors.Controls.Add(this.dgvErrors);
            this.tPErrors.Location = new System.Drawing.Point(4, 22);
            this.tPErrors.Name = "tPErrors";
            this.tPErrors.Padding = new System.Windows.Forms.Padding(3);
            this.tPErrors.Size = new System.Drawing.Size(658, 190);
            this.tPErrors.TabIndex = 1;
            this.tPErrors.Text = "Errors";
            this.tPErrors.UseVisualStyleBackColor = true;
            // 
            // dgvErrors
            // 
            this.dgvErrors.AllowUserToAddRows = false;
            this.dgvErrors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvErrors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ErrMessage,
            this.ErrTime});
            this.dgvErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvErrors.Location = new System.Drawing.Point(3, 3);
            this.dgvErrors.Name = "dgvErrors";
            this.dgvErrors.RowHeadersVisible = false;
            this.dgvErrors.Size = new System.Drawing.Size(652, 184);
            this.dgvErrors.TabIndex = 0;
            // 
            // ErrMessage
            // 
            this.ErrMessage.HeaderText = "Message";
            this.ErrMessage.Name = "ErrMessage";
            this.ErrMessage.ReadOnly = true;
            // 
            // ErrTime
            // 
            this.ErrTime.HeaderText = "Time";
            this.ErrTime.Name = "ErrTime";
            this.ErrTime.ReadOnly = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.btnTesting);
            this.groupBox1.Controls.Add(this.txtServer);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtLogin);
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.lblRuCaptchaBalance);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(666, 215);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Main Controls";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkbUsingProxyWF);
            this.groupBox2.Controls.Add(this.cbServer);
            this.groupBox2.Controls.Add(this.checkbIsHost);
            this.groupBox2.Location = new System.Drawing.Point(287, 16);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(372, 193);
            this.groupBox2.TabIndex = 31;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // checkbUsingProxyWF
            // 
            this.checkbUsingProxyWF.AutoSize = true;
            this.checkbUsingProxyWF.Location = new System.Drawing.Point(7, 43);
            this.checkbUsingProxyWF.Name = "checkbUsingProxyWF";
            this.checkbUsingProxyWF.Size = new System.Drawing.Size(102, 17);
            this.checkbUsingProxyWF.TabIndex = 2;
            this.checkbUsingProxyWF.Text = "Using Proxy WF";
            this.checkbUsingProxyWF.UseVisualStyleBackColor = true;
            this.checkbUsingProxyWF.CheckedChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // cbServer
            // 
            this.cbServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbServer.FormattingEnabled = true;
            this.cbServer.Location = new System.Drawing.Point(111, 16);
            this.cbServer.Name = "cbServer";
            this.cbServer.Size = new System.Drawing.Size(78, 21);
            this.cbServer.TabIndex = 1;
            this.cbServer.SelectedIndexChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // checkbIsHost
            // 
            this.checkbIsHost.AutoSize = true;
            this.checkbIsHost.Location = new System.Drawing.Point(7, 20);
            this.checkbIsHost.Name = "checkbIsHost";
            this.checkbIsHost.Size = new System.Drawing.Size(59, 17);
            this.checkbIsHost.TabIndex = 0;
            this.checkbIsHost.Text = "Is Host";
            this.checkbIsHost.UseVisualStyleBackColor = true;
            this.checkbIsHost.CheckedChanged += new System.EventHandler(this.SettingsChanged);
            // 
            // btnTesting
            // 
            this.btnTesting.Location = new System.Drawing.Point(9, 68);
            this.btnTesting.Name = "btnTesting";
            this.btnTesting.Size = new System.Drawing.Size(75, 23);
            this.btnTesting.TabIndex = 30;
            this.btnTesting.Text = "Testing";
            this.btnTesting.UseVisualStyleBackColor = true;
            this.btnTesting.Click += new System.EventHandler(this.btnTesting_Click);
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(53, 42);
            this.txtServer.Name = "txtServer";
            this.txtServer.ReadOnly = true;
            this.txtServer.Size = new System.Drawing.Size(154, 20);
            this.txtServer.TabIndex = 29;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Server:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "Login:";
            // 
            // txtLogin
            // 
            this.txtLogin.Location = new System.Drawing.Point(53, 16);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.ReadOnly = true;
            this.txtLogin.Size = new System.Drawing.Size(154, 20);
            this.txtLogin.TabIndex = 26;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(9, 159);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 25;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblRuCaptchaBalance
            // 
            this.lblRuCaptchaBalance.AutoSize = true;
            this.lblRuCaptchaBalance.Location = new System.Drawing.Point(118, 199);
            this.lblRuCaptchaBalance.Name = "lblRuCaptchaBalance";
            this.lblRuCaptchaBalance.Size = new System.Drawing.Size(13, 13);
            this.lblRuCaptchaBalance.TabIndex = 24;
            this.lblRuCaptchaBalance.Text = "?";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 199);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "RuCaptcha Balance:";
            // 
            // tabProxy
            // 
            this.tabProxy.Controls.Add(this.btnResetBadProxy);
            this.tabProxy.Controls.Add(this.lblProxyes);
            this.tabProxy.Controls.Add(this.btnSaveProxyes);
            this.tabProxy.Controls.Add(this.rtbProxyes);
            this.tabProxy.Location = new System.Drawing.Point(4, 22);
            this.tabProxy.Name = "tabProxy";
            this.tabProxy.Padding = new System.Windows.Forms.Padding(3);
            this.tabProxy.Size = new System.Drawing.Size(678, 449);
            this.tabProxy.TabIndex = 1;
            this.tabProxy.Text = "Proxy";
            this.tabProxy.UseVisualStyleBackColor = true;
            // 
            // btnResetBadProxy
            // 
            this.btnResetBadProxy.Location = new System.Drawing.Point(480, 3);
            this.btnResetBadProxy.Name = "btnResetBadProxy";
            this.btnResetBadProxy.Size = new System.Drawing.Size(111, 23);
            this.btnResetBadProxy.TabIndex = 11;
            this.btnResetBadProxy.Text = "Reset Bad Proxy";
            this.btnResetBadProxy.UseVisualStyleBackColor = true;
            this.btnResetBadProxy.Click += new System.EventHandler(this.btnResetBadProxy_Click);
            // 
            // lblProxyes
            // 
            this.lblProxyes.AutoSize = true;
            this.lblProxyes.Location = new System.Drawing.Point(341, 39);
            this.lblProxyes.Name = "lblProxyes";
            this.lblProxyes.Size = new System.Drawing.Size(10, 13);
            this.lblProxyes.TabIndex = 10;
            this.lblProxyes.Text = ".";
            // 
            // btnSaveProxyes
            // 
            this.btnSaveProxyes.Location = new System.Drawing.Point(342, 4);
            this.btnSaveProxyes.Name = "btnSaveProxyes";
            this.btnSaveProxyes.Size = new System.Drawing.Size(115, 23);
            this.btnSaveProxyes.TabIndex = 9;
            this.btnSaveProxyes.Text = "Save Proxyes";
            this.btnSaveProxyes.UseVisualStyleBackColor = true;
            this.btnSaveProxyes.Click += new System.EventHandler(this.btnSaveProxyes_Click);
            // 
            // rtbProxyes
            // 
            this.rtbProxyes.Location = new System.Drawing.Point(3, 3);
            this.rtbProxyes.Name = "rtbProxyes";
            this.rtbProxyes.Size = new System.Drawing.Size(332, 443);
            this.rtbProxyes.TabIndex = 8;
            this.rtbProxyes.Text = "";
            // 
            // timerUIUpdater
            // 
            this.timerUIUpdater.Interval = 1000;
            this.timerUIUpdater.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timerKeyLissener
            // 
            this.timerKeyLissener.Tick += new System.EventHandler(this.timerKeyLissener_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 475);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            this.tPmessages.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMessages)).EndInit();
            this.tPErrors.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabProxy.ResumeLayout(false);
            this.tabProxy.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabMain;
        private System.Windows.Forms.TabPage tabProxy;
        private System.Windows.Forms.Button btnResetBadProxy;
        private System.Windows.Forms.Label lblProxyes;
        private System.Windows.Forms.Button btnSaveProxyes;
        private System.Windows.Forms.RichTextBox rtbProxyes;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tPmessages;
        private System.Windows.Forms.DataGridView dgvMessages;
        private System.Windows.Forms.DataGridViewTextBoxColumn MesMessage;
        private System.Windows.Forms.DataGridViewTextBoxColumn MesTime;
        private System.Windows.Forms.TabPage tPErrors;
        private System.Windows.Forms.DataGridView dgvErrors;
        private System.Windows.Forms.DataGridViewTextBoxColumn ErrMessage;
        private System.Windows.Forms.DataGridViewTextBoxColumn ErrTime;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Timer timerUIUpdater;
        private System.Windows.Forms.Label lblRuCaptchaBalance;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.Timer timerKeyLissener;
        private System.Windows.Forms.Button btnTesting;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkbIsHost;
        private System.Windows.Forms.ComboBox cbServer;
        private System.Windows.Forms.CheckBox checkbUsingProxyWF;
    }
}


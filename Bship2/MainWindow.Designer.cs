namespace Battleship
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.txtMsg = new System.Windows.Forms.TextBox();
            this.ContactList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ChatPanel = new System.Windows.Forms.Panel();
            this.ChatConsolePanel = new System.Windows.Forms.Panel();
            this.ChatContactsPanel = new System.Windows.Forms.Panel();
            this.ChatButtonPanel = new System.Windows.Forms.Panel();
            this.sendBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.RotateShip = new System.Windows.Forms.Button();
            this.ConfirmShip = new System.Windows.Forms.Button();
            this.StatusLight = new System.Windows.Forms.PictureBox();
            this.Player1Panel = new System.Windows.Forms.Panel();
            this.Player2Panel = new System.Windows.Forms.Panel();
            this.Player1Ships = new System.Windows.Forms.Panel();
            this.Player2Ships = new System.Windows.Forms.Panel();
            this.Player3Panel = new System.Windows.Forms.Panel();
            this.Player4Panel = new System.Windows.Forms.Panel();
            this.Player5Panel = new System.Windows.Forms.Panel();
            this.ChatPanel.SuspendLayout();
            this.ChatConsolePanel.SuspendLayout();
            this.ChatContactsPanel.SuspendLayout();
            this.ChatButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StatusLight)).BeginInit();
            this.SuspendLayout();
            // 
            // txtConsole
            // 
            this.txtConsole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.txtConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConsole.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsole.ForeColor = System.Drawing.Color.DarkOrange;
            this.txtConsole.Location = new System.Drawing.Point(0, 0);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ReadOnly = true;
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConsole.Size = new System.Drawing.Size(723, 130);
            this.txtConsole.TabIndex = 2;
            // 
            // txtMsg
            // 
            this.txtMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMsg.Location = new System.Drawing.Point(119, 0);
            this.txtMsg.MaxLength = 2000;
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.Size = new System.Drawing.Size(604, 20);
            this.txtMsg.TabIndex = 3;
            this.txtMsg.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMsg_KeyDown);
            // 
            // ContactList
            // 
            this.ContactList.BackColor = System.Drawing.SystemColors.Info;
            this.ContactList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContactList.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.ContactList.FormattingEnabled = true;
            this.ContactList.Location = new System.Drawing.Point(0, 16);
            this.ContactList.Name = "ContactList";
            this.ContactList.Size = new System.Drawing.Size(161, 114);
            this.ContactList.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "Player List";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ChatPanel
            // 
            this.ChatPanel.Controls.Add(this.ChatConsolePanel);
            this.ChatPanel.Controls.Add(this.ChatContactsPanel);
            this.ChatPanel.Controls.Add(this.ChatButtonPanel);
            this.ChatPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ChatPanel.Location = new System.Drawing.Point(0, 512);
            this.ChatPanel.Name = "ChatPanel";
            this.ChatPanel.Size = new System.Drawing.Size(884, 150);
            this.ChatPanel.TabIndex = 9;
            // 
            // ChatConsolePanel
            // 
            this.ChatConsolePanel.Controls.Add(this.txtConsole);
            this.ChatConsolePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChatConsolePanel.Location = new System.Drawing.Point(0, 20);
            this.ChatConsolePanel.Name = "ChatConsolePanel";
            this.ChatConsolePanel.Size = new System.Drawing.Size(723, 130);
            this.ChatConsolePanel.TabIndex = 11;
            // 
            // ChatContactsPanel
            // 
            this.ChatContactsPanel.Controls.Add(this.ContactList);
            this.ChatContactsPanel.Controls.Add(this.label1);
            this.ChatContactsPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.ChatContactsPanel.Location = new System.Drawing.Point(723, 20);
            this.ChatContactsPanel.Name = "ChatContactsPanel";
            this.ChatContactsPanel.Size = new System.Drawing.Size(161, 130);
            this.ChatContactsPanel.TabIndex = 10;
            // 
            // ChatButtonPanel
            // 
            this.ChatButtonPanel.Controls.Add(this.txtMsg);
            this.ChatButtonPanel.Controls.Add(this.sendBtn);
            this.ChatButtonPanel.Controls.Add(this.label2);
            this.ChatButtonPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ChatButtonPanel.Location = new System.Drawing.Point(0, 0);
            this.ChatButtonPanel.Name = "ChatButtonPanel";
            this.ChatButtonPanel.Size = new System.Drawing.Size(884, 20);
            this.ChatButtonPanel.TabIndex = 3;
            // 
            // sendBtn
            // 
            this.sendBtn.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.sendBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.sendBtn.Font = new System.Drawing.Font("Consolas", 8F);
            this.sendBtn.Location = new System.Drawing.Point(723, 0);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(161, 20);
            this.sendBtn.TabIndex = 0;
            this.sendBtn.Text = "Send Message";
            this.sendBtn.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.sendBtn.UseVisualStyleBackColor = false;
            this.sendBtn.Click += new System.EventHandler(this.SendMessage);
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Type a Message:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RotateShip
            // 
            this.RotateShip.BackgroundImage = global::Bship2.Properties.Resources.rotate;
            this.RotateShip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.RotateShip.Location = new System.Drawing.Point(93, 352);
            this.RotateShip.Name = "RotateShip";
            this.RotateShip.Size = new System.Drawing.Size(60, 60);
            this.RotateShip.TabIndex = 10;
            this.RotateShip.UseVisualStyleBackColor = true;
            this.RotateShip.Click += new System.EventHandler(this.RotateShip_Click);
            // 
            // ConfirmShip
            // 
            this.ConfirmShip.BackgroundImage = global::Bship2.Properties.Resources.Confirm;
            this.ConfirmShip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ConfirmShip.Location = new System.Drawing.Point(213, 352);
            this.ConfirmShip.Name = "ConfirmShip";
            this.ConfirmShip.Size = new System.Drawing.Size(60, 60);
            this.ConfirmShip.TabIndex = 11;
            this.ConfirmShip.UseVisualStyleBackColor = true;
            this.ConfirmShip.Click += new System.EventHandler(this.ConfirmShip_Click);
            // 
            // StatusLight
            // 
            this.StatusLight.BackColor = System.Drawing.Color.Transparent;
            this.StatusLight.Image = global::Bship2.Properties.Resources.Light_Red;
            this.StatusLight.Location = new System.Drawing.Point(12, 446);
            this.StatusLight.Name = "StatusLight";
            this.StatusLight.Size = new System.Drawing.Size(74, 66);
            this.StatusLight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.StatusLight.TabIndex = 12;
            this.StatusLight.TabStop = false;
            this.StatusLight.Visible = false;
            // 
            // Player1Panel
            // 
            this.Player1Panel.Location = new System.Drawing.Point(30, 30);
            this.Player1Panel.Name = "Player1Panel";
            this.Player1Panel.Size = new System.Drawing.Size(300, 300);
            this.Player1Panel.TabIndex = 13;
            this.Player1Panel.Visible = false;
            // 
            // Player2Panel
            // 
            this.Player2Panel.Location = new System.Drawing.Point(545, 30);
            this.Player2Panel.Name = "Player2Panel";
            this.Player2Panel.Size = new System.Drawing.Size(300, 300);
            this.Player2Panel.TabIndex = 14;
            this.Player2Panel.Visible = false;
            // 
            // Player1Ships
            // 
            this.Player1Ships.Location = new System.Drawing.Point(337, 30);
            this.Player1Ships.Name = "Player1Ships";
            this.Player1Ships.Size = new System.Drawing.Size(150, 150);
            this.Player1Ships.TabIndex = 15;
            this.Player1Ships.Visible = false;
            // 
            // Player2Ships
            // 
            this.Player2Ships.Location = new System.Drawing.Point(388, 180);
            this.Player2Ships.Name = "Player2Ships";
            this.Player2Ships.Size = new System.Drawing.Size(150, 150);
            this.Player2Ships.TabIndex = 16;
            this.Player2Ships.Visible = false;
            // 
            // Player3Panel
            // 
            this.Player3Panel.Location = new System.Drawing.Point(417, 376);
            this.Player3Panel.Name = "Player3Panel";
            this.Player3Panel.Size = new System.Drawing.Size(116, 116);
            this.Player3Panel.TabIndex = 15;
            this.Player3Panel.Visible = false;
            // 
            // Player4Panel
            // 
            this.Player4Panel.Location = new System.Drawing.Point(575, 376);
            this.Player4Panel.Name = "Player4Panel";
            this.Player4Panel.Size = new System.Drawing.Size(116, 116);
            this.Player4Panel.TabIndex = 16;
            this.Player4Panel.Visible = false;
            // 
            // Player5Panel
            // 
            this.Player5Panel.Location = new System.Drawing.Point(733, 376);
            this.Player5Panel.Name = "Player5Panel";
            this.Player5Panel.Size = new System.Drawing.Size(116, 116);
            this.Player5Panel.TabIndex = 17;
            this.Player5Panel.Visible = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Menu;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(884, 662);
            this.Controls.Add(this.Player5Panel);
            this.Controls.Add(this.Player4Panel);
            this.Controls.Add(this.Player3Panel);
            this.Controls.Add(this.Player2Ships);
            this.Controls.Add(this.Player1Ships);
            this.Controls.Add(this.Player2Panel);
            this.Controls.Add(this.Player1Panel);
            this.Controls.Add(this.StatusLight);
            this.Controls.Add(this.ConfirmShip);
            this.Controls.Add(this.RotateShip);
            this.Controls.Add(this.ChatPanel);
            this.Cursor = System.Windows.Forms.Cursors.Cross;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(900, 700);
            this.MinimumSize = new System.Drawing.Size(900, 700);
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Click += new System.EventHandler(this.MainWindow_Click);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainWindow_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainWindow_MouseMove);
            this.ChatPanel.ResumeLayout(false);
            this.ChatConsolePanel.ResumeLayout(false);
            this.ChatConsolePanel.PerformLayout();
            this.ChatContactsPanel.ResumeLayout(false);
            this.ChatButtonPanel.ResumeLayout(false);
            this.ChatButtonPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StatusLight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.TextBox txtMsg;
        private System.Windows.Forms.ListBox ContactList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel ChatPanel;
        private System.Windows.Forms.Panel ChatButtonPanel;
        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.Panel ChatContactsPanel;
        private System.Windows.Forms.Panel ChatConsolePanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button RotateShip;
        private System.Windows.Forms.Button ConfirmShip;
        private System.Windows.Forms.PictureBox StatusLight;
        private System.Windows.Forms.Panel Player1Panel;
        private System.Windows.Forms.Panel Player2Panel;
        private System.Windows.Forms.Panel Player1Ships;
        private System.Windows.Forms.Panel Player2Ships;
        private System.Windows.Forms.Panel Player3Panel;
        private System.Windows.Forms.Panel Player4Panel;
        private System.Windows.Forms.Panel Player5Panel;
    }
}


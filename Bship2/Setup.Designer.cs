namespace Battleship
{
    partial class SetupWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupWindow));
            this.JoinHostBox = new System.Windows.Forms.Panel();
            this.HostBtn = new System.Windows.Forms.Button();
            this.JoinBtn = new System.Windows.Forms.Button();
            this.JoinIPBox = new System.Windows.Forms.TextBox();
            this.HostBox = new System.Windows.Forms.TextBox();
            this.JoinHostBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // JoinHostBox
            // 
            this.JoinHostBox.BackColor = System.Drawing.Color.Transparent;
            this.JoinHostBox.Controls.Add(this.HostBtn);
            this.JoinHostBox.Controls.Add(this.JoinBtn);
            this.JoinHostBox.Location = new System.Drawing.Point(55, 100);
            this.JoinHostBox.Name = "JoinHostBox";
            this.JoinHostBox.Size = new System.Drawing.Size(320, 50);
            this.JoinHostBox.TabIndex = 0;
            // 
            // HostBtn
            // 
            this.HostBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.HostBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.HostBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.HostBtn.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.HostBtn.Location = new System.Drawing.Point(200, 0);
            this.HostBtn.Name = "HostBtn";
            this.HostBtn.Size = new System.Drawing.Size(120, 50);
            this.HostBtn.TabIndex = 1;
            this.HostBtn.Text = "Host New Game";
            this.HostBtn.UseVisualStyleBackColor = false;
            this.HostBtn.Click += new System.EventHandler(this.HostBtn_Click);
            // 
            // JoinBtn
            // 
            this.JoinBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.JoinBtn.Dock = System.Windows.Forms.DockStyle.Left;
            this.JoinBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.JoinBtn.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.JoinBtn.Location = new System.Drawing.Point(0, 0);
            this.JoinBtn.Name = "JoinBtn";
            this.JoinBtn.Size = new System.Drawing.Size(120, 50);
            this.JoinBtn.TabIndex = 0;
            this.JoinBtn.Text = "Join Game";
            this.JoinBtn.UseVisualStyleBackColor = false;
            this.JoinBtn.Click += new System.EventHandler(this.JoinBtn_Click);
            // 
            // JoinIPBox
            // 
            this.JoinIPBox.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JoinIPBox.Location = new System.Drawing.Point(55, 157);
            this.JoinIPBox.Name = "JoinIPBox";
            this.JoinIPBox.Size = new System.Drawing.Size(120, 32);
            this.JoinIPBox.TabIndex = 1;
            this.JoinIPBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.JoinIPBox_KeyDown);
            // 
            // HostBox
            // 
            this.HostBox.Font = new System.Drawing.Font("Consolas", 15.75F);
            this.HostBox.Location = new System.Drawing.Point(255, 157);
            this.HostBox.Name = "HostBox";
            this.HostBox.Size = new System.Drawing.Size(120, 32);
            this.HostBox.TabIndex = 2;
            this.HostBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HostBox_KeyDown);
            // 
            // SetupWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(440, 240);
            this.Controls.Add(this.HostBox);
            this.Controls.Add(this.JoinIPBox);
            this.Controls.Add(this.JoinHostBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SetupWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Join or Host Game?";
            this.JoinHostBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel JoinHostBox;
        private System.Windows.Forms.Button HostBtn;
        private System.Windows.Forms.Button JoinBtn;
        private System.Windows.Forms.TextBox JoinIPBox;
        private System.Windows.Forms.TextBox HostBox;
    }
}
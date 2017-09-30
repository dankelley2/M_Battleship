using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battleship
{
    public partial class SetupWindow : Form
    {
        public bool Host = false;
        public string JoinIP = null;
        public bool StopApplication = true;
        public int btnClicks = 0;
        public string HostName;
        System.Text.RegularExpressions.Regex IPMatch = new System.Text.RegularExpressions.Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
        public SetupWindow()
        {
            InitializeComponent();
            JoinIPBox.Hide();
            HostBox.Hide();
        }

        private void HostBtn_Click(object sender, EventArgs e)
        {
            if (btnClicks == 0)
            {
                btnClicks++;
                Host = true;
                JoinBtn.Hide();
                HostBtn.Left = 0;
                HostBtn.Width = 320;
                HostBox.Left = 55;
                HostBox.Width = 320;
                HostBox.Show();
                HostBtn.Text = "Enter your name, then click here.";
            }
            if (btnClicks == 1)
            {
                if (HostBox.Text.Trim(' ') != "")
                {
                    this.HostName = HostBox.Text.Trim(' ');
                    StopApplication = false;
                    this.Close();
                }
            }
        }

        private void JoinBtn_Click(object sender, EventArgs e)
        {
            if (btnClicks == 0)
            {
                btnClicks++;
                JoinIPBox.Select(0, 0);
                Host = false;
                HostBtn.Hide();
                JoinBtn.Width = 320;
                JoinIPBox.Width = 320;
                JoinIPBox.Show();
                JoinBtn.Text = "Enter your name, then click here.";
            }
            if (btnClicks == 1)
            {
                if (JoinIPBox.Text.Trim(' ') != "")
                {
                    btnClicks++;
                    this.HostName = JoinIPBox.Text.Trim(' ');
                    JoinBtn.Text = "Enter Host IP, then click here.";
                    JoinIPBox.Clear();
                }
            }
            if (btnClicks == 2)
            {
                if (IPMatch.IsMatch(JoinIPBox.Text))
                {
                    JoinIP = JoinIPBox.Text;
                    StopApplication = false;
                    this.Close();
                }
            }
        }

        private void JoinIPBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                JoinBtn_Click(this, e);
            }
        }

        private void HostBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                HostBtn_Click(this, e);
            }
        }
    }
}

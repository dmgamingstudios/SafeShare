using FuhrerShare.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuhrerShare.Core.Setup
{
    public partial class NetworkSetup : Form
    {
        ConnectionMethod.ConnMethod CM;
        internal NetworkSetup(ConnectionMethod.ConnMethod CM)
        {
            InitializeComponent();
            this.CM = CM;
            if (CM == ConnectionMethod.ConnMethod.Clear)
                button1.Enabled = true;
            else if (CM == ConnectionMethod.ConnMethod.I2P)
                button3.Enabled = true;
            else if (CM == ConnectionMethod.ConnMethod.Tor)
                button2.Enabled = true;
        }

        private void NetworkSetup_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Text = "Activated";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Text = "Activated";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Text = "Activated";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please wait while we set things up, DO NOT TURN OFF YOUR COMPUTER, this may take a while");
            IdentitySetup IS = new IdentitySetup(false, true, false);
            IS.Show();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Going to setup tor for you, we will download it, edit the torrc for our needs and start it everytime SafeShare starts, it wont start on windows startup. Thanks to joelverhagen for his awesome TorSharp library!");

        }
    }
}

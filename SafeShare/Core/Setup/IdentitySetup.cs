using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuhrerShare.Core.Setup
{
    public partial class IdentitySetup : Form
    {
        X509Certificate2 cert;
        bool tor, clear, i2p;
        public IdentitySetup(bool tor, bool clear, bool i2p)
        {
            InitializeComponent();
            this.tor = tor;
            this.clear = clear;
            this.i2p = i2p;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Config.LocalNode = new Nodes.SafeNode(textBox1.Text, "127.0.0.1", 5248, cert, true);
            Config.SaveLocalNode();
            Config.SetupDone = true;
            MessageBox.Show("We are done YAY");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] result = null;
            MessageBox.Show("Your keys are being requested, this may take a while and the application may seem unresponsive, DO NOT CLOSE THIS APPLICATION UNDER ANY CIRCUMSTANCES");
            if(clear)
            {

            }
            else if(tor)
            {

            }
            else if(i2p)
            {

            }
            richTextBox2.Text = result[1];
        }
    }
}
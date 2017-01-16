using FuhrerShare.Core.Certificates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FuhrerShare.Enums.ConnectionMethod;
using static FuhrerShare.Enums.ProtectionMethod;

namespace FuhrerShare.Core.Setup
{
    public partial class FirstSetup : Form
    {
        ConnMethod CM;
        PrMethod PM;
        public FirstSetup()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Low encryption means that any connection will be made even if the other party's identity is not verified, secure connections wont be used and we will go trough the clearnet. Downloaded files will not be encrypted and will stay on the system after shutdown, chat keeps logs and saves those on the system. Use only if you are mentally retarded.");
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Medium encryption means that no connection will be made if the other party cant verify its identity, secure connections are used but not required. We will use both clearnet and tor(if configured). Downloaded files will not be encrypted and will stay on the system after shutdown, chat will only keep logs if requested.");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("High encryption means that no connection will be made if the other party cant verify its identity, secure connections are required for every connection. We will use tor and i2p(if configured). Downloaded files are encrypted with a passphrase of your choice and remain on the system after shutdown, chat keeps no logs. (We advice this setting)");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Paranoid encryption means that no connection will be made if the other party cant verify its identity and the node will be auto blacklisted, secure connections are required. We will use i2p ONLY, unless tor is specifically requested. Downloaded files are encrypted with a temporary passphrase and will be deleted on shutdown(if file deletion was not possible on shutdown, no worries, the passphrase is already gone). Chat uses an otr like system and messages are encrypted with a temporary key.");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("What would this do? It means you can specify all the settings yourself ofcourse, but you clicked this.... I suggest you dont check this box lol");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text.Length <= 16)
                label3.Text = "Short pass mate";
            else if(textBox1.Text.Length >= 17)
                label3.Text = "Good pass";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("We will first install the root CA certificates in your certificate store, a warning from windows might popup about inserting an untrusted certificate, just click yes to continue importing");
            AddSecuCert.Add();
            NetworkSetup NS = new NetworkSetup(CM, PM);
            NS.Show();
            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                CM = ConnMethod.Clear;
                PM = PrMethod.low;
                button1.Enabled = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                PM = PrMethod.medium;
                CM = ConnMethod.Clear;
                button1.Enabled = true;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                PM = PrMethod.high;
                CM = ConnMethod.Tor;
                button1.Enabled = true;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox5.Checked = false;
                PM = PrMethod.paranoid;
                CM = ConnMethod.I2P;
                button1.Enabled = true;
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                groupBox2.Visible = true;
                groupBox3.Visible = true;
                groupBox4.Visible = true;
                PM = PrMethod.custom;
                button1.Enabled = true;
            }
            else
            {
                groupBox2.Visible = false;
                groupBox3.Visible = false;
                groupBox4.Visible = false;
            }
        }

        private void FirstSetup_Load(object sender, EventArgs e)
        {

        }
    }
}

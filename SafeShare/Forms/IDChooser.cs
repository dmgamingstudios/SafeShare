using FuhrerShare.Core.Nodes;
using FuhrerShare.Core.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuhrerShare.Forms
{
    public partial class IDChooser : Form
    {
        public IDChooser()
        {
            InitializeComponent();
            if (!Config.SetupDone)
            {
                FirstSetup FS = new FirstSetup();
                Hide();
                FS.ShowDialog();
            }
            else
                PopulatecomboBox1();
        }

        private void IDChooser_Load(object sender, EventArgs e)
        {

        }
        private void PopulatecomboBox1()
        {
            string[] IDS = null;
            int i = 0;
            foreach(string file in Directory.GetFiles(Application.StartupPath + "\\identities"))
            {
                FileInfo f = new FileInfo(file);
                IDS[i] = f.Name;
                i = i + 1;
            }
            comboBox1.Items.AddRange(IDS);
        }
        private void DecryptID(string pass, string idfile)
        {
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] key = UE.GetBytes(pass);
            FileStream fsCrypt = new FileStream(idfile, FileMode.Open);
            RijndaelManaged RMCrypto = new RijndaelManaged();
            CryptoStream cs = new CryptoStream(fsCrypt, RMCrypto.CreateDecryptor(key, key), CryptoStreamMode.Read);
            int data;
            MemoryStream stream = new MemoryStream();
            while ((data = cs.ReadByte()) != -1)
                stream.WriteByte((byte)data);
            string filedata = Encoding.ASCII.GetString(stream.ToArray());
            stream.Close();
            cs.Close();
            fsCrypt.Close();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DecryptID(textBox1.Text, "");
        }
    }
}
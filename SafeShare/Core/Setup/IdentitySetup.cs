using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
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
        string pub = null;
        string csr = null;
        public IdentitySetup(bool tor, bool clear, bool i2p)
        {
            InitializeComponent();
            this.tor = tor;
            this.clear = clear;
            this.i2p = i2p;
        }
        internal void GenerateCsr(out string publicKey, out string privateKey, out string eprivkey, out string csr, string name, string pass)
        {
            var rsaKeyPairGenerator = new RsaKeyPairGenerator();
            var genParam = new RsaKeyGenerationParameters(BigInteger.ValueOf(0x10001), new SecureRandom(), 4096, 128);
            rsaKeyPairGenerator.Init(genParam);
            AsymmetricCipherKeyPair pair = rsaKeyPairGenerator.GenerateKeyPair();
            privateKey = pair.Private.ToString();
            DerObjectIdentifier Do = new DerObjectIdentifier("PBEWithSHA256And256BitAES-CBC-BC");
            IDictionary attrs = new Hashtable();
            attrs.Add(X509Name.CN, name);
            attrs.Add(X509Name.ST, "SecuNet");
            attrs.Add(X509Name.C, "SN");
            var subject = new X509Name(new ArrayList(attrs.Keys), attrs);
            var pkcs10CertificationRequest = new Pkcs10CertificationRequest(PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id, subject, pair.Public, null, pair.Private);
            csr = Convert.ToBase64String(pkcs10CertificationRequest.GetEncoded());
            var pkInfo = EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(Do, pass.ToCharArray(), Encoding.ASCII.GetBytes(name), 50, pair.Private);
            eprivkey = Convert.ToBase64String(pkInfo.GetDerEncoded());
            publicKey = pair.Public.ToString();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            GenerateCsr(out pub, out string lol, out string epriv, out csr, textBox1.Text, textBox2.Text);
            richTextBox1.Text = lol;
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
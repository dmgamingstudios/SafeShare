using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuhrerShare.Forms
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Certificate Files|*.crt";
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (isSignedBySecuNetCA(new X509Certificate2(openFileDialog1.FileName)))
                {

                }
                else
                    MessageBox.Show("This certificate is NOT signed by the SecuNet CA! One more and you are banned from the network");
            }
        }
        private bool isSignedBySecuNetCA(X509Certificate2 Cert)
        {
            byte[] _data;
            Assembly _assembly = Assembly.GetExecutingAssembly();
            using (MemoryStream _mem = new MemoryStream())
            {
                _assembly.GetManifestResourceStream("SafeShare.Core.Certificates.secunetCA.pem").CopyTo(_mem);
                _data = _mem.ToArray();
            }
            X509Certificate2 authority = new X509Certificate2(_data);
            X509Certificate2 certificateToValidate = Cert;
            System.Security.Cryptography.X509Certificates.X509Chain chain = new System.Security.Cryptography.X509Certificates.X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            chain.ChainPolicy.VerificationTime = DateTime.Now;
            chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 0);
            chain.ChainPolicy.ExtraStore.Add(authority);
            bool isChainValid = chain.Build(certificateToValidate);
            if (!isChainValid)
            {
                string[] errors = chain.ChainStatus
                    .Select(x => String.Format("{0} ({1})", x.StatusInformation.Trim(), x.Status))
                    .ToArray();
                string certificateErrorsString = "Unknown errors.";
                if (errors != null && errors.Length > 0)
                {
                    certificateErrorsString = String.Join(", ", errors);
                }
                return false;
            }
            if (!chain.ChainElements
                .Cast<X509ChainElement>()
                .Any(x => x.Certificate.Thumbprint == authority.Thumbprint))
            {
                return false;
            }
            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void Settings_Load(object sender, EventArgs e)
        {

        }
    }
}
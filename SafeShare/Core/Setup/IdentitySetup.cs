using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuhrerShare.Core.Setup
{
    public partial class IdentitySetup : Form
    {
        bool tor, clear, i2p;
        string pub = null;
        string ihash = null;
        string csr = null;
        public X509Certificate2 PfxCert = null;
        public IdentitySetup(bool tor, bool clear, bool i2p)
        {
            InitializeComponent();
            this.tor = tor;
            this.clear = clear;
            this.i2p = i2p;
        }
        public void GenerateCsr(out string publicKey, out string privateKey, out string csr, string name, string pass)
        {
            var rsaKeyPairGenerator = new RsaKeyPairGenerator();
            var genParam = new RsaKeyGenerationParameters(BigInteger.ValueOf(0x10001), new SecureRandom(), 1024, 128);
            rsaKeyPairGenerator.Init(genParam);
            AsymmetricCipherKeyPair pair = rsaKeyPairGenerator.GenerateKeyPair();
            TextWriter textWriter = new StringWriter();
            PemWriter pemWriter = new PemWriter(textWriter);
            pemWriter.WriteObject(pair.Private);
            pemWriter.Writer.Flush();
            privateKey = textWriter.ToString();
            DerObjectIdentifier Do = PkcsObjectIdentifiers.PbeWithShaAnd2KeyTripleDesCbc;
            IDictionary attrs = new Hashtable();
            string idhash = new Hash().Create_Hash_For_Identity(name, privateKey);
            attrs.Add(X509Name.CN, idhash);
            attrs.Add(X509Name.ST, "SecuNet");
            attrs.Add(X509Name.C, "SN");
            var subject = new X509Name(new ArrayList(attrs.Keys), attrs);
            var pkcs10CertificationRequest = new Pkcs10CertificationRequest(PkcsObjectIdentifiers.Sha512WithRsaEncryption.Id, subject, pair.Public, null, pair.Private);
            csr = Convert.ToBase64String(pkcs10CertificationRequest.GetEncoded());
            publicKey = pair.Public.ToString();
            ihash = idhash;
            //GeneratePfx(publicKey, privateKey, pass);
        }
        public void GeneratePfx(string pubkey, string privkey, string pass)
        {
            byte[] certBuffer = Helpers.GetBytesFromPEM(pubkey, PemStringType.Certificate);
            byte[] keyBuffer = Helpers.GetBytesFromPEM(privkey, PemStringType.RsaPrivateKey);
            X509Certificate2 certificate = new X509Certificate2(certBuffer, pass);
            RSACryptoServiceProvider prov = Crypto.DecodeRsaPrivateKey(keyBuffer);
            certificate.PrivateKey = prov;
            PfxCert = certificate;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            string privkey = null;
            GenerateCsr(out pub, out privkey, out csr, textBox1.Text, textBox2.Text);
            richTextBox2.Text = privkey;
        }

        private void IdentitySetup_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Config.LocalNode = new Nodes.LocalSafeNode(textBox1.Text, "127.0.0.1", 5248, PfxCert, true, ihash);
            Config.SetupDone = true;
            new SaveIdentity(Config.LocalNode, textBox2.Text);
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
    public enum PemStringType
    {
        Certificate,
        RsaPrivateKey
    }
    public class Crypto
    {
        public static RSACryptoServiceProvider DecodeRsaPrivateKey(byte[] privateKeyBytes)
        {
            MemoryStream ms = new MemoryStream(privateKeyBytes);
            BinaryReader rd = new BinaryReader(ms);

            try
            {
                byte byteValue;
                ushort shortValue;

                shortValue = rd.ReadUInt16();

                switch (shortValue)
                {
                    case 0x8130:
                        rd.ReadByte();
                        break;
                    case 0x8230:
                        rd.ReadInt16();
                        break;
                    default:
                        Debug.Assert(false);
                        return null;
                }

                shortValue = rd.ReadUInt16();
                if (shortValue != 0x0102)
                {
                    Debug.Assert(false);
                    return null;
                }

                byteValue = rd.ReadByte();
                if (byteValue != 0x00)
                {
                    Debug.Assert(false);
                    return null;
                }
                CspParameters parms = new CspParameters();
                parms.Flags = CspProviderFlags.NoFlags;
                parms.KeyContainerName = Guid.NewGuid().ToString().ToUpperInvariant();
                parms.ProviderType = ((Environment.OSVersion.Version.Major > 5) || ((Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor >= 1))) ? 0x18 : 1;

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(parms);
                RSAParameters rsAparams = new RSAParameters();

                rsAparams.Modulus = rd.ReadBytes(Helpers.DecodeIntegerSize(rd));

                RSAParameterTraits traits = new RSAParameterTraits(rsAparams.Modulus.Length * 8);

                rsAparams.Modulus = Helpers.AlignBytes(rsAparams.Modulus, traits.size_Mod);
                rsAparams.Exponent = Helpers.AlignBytes(rd.ReadBytes(Helpers.DecodeIntegerSize(rd)), traits.size_Exp);
                rsAparams.D = Helpers.AlignBytes(rd.ReadBytes(Helpers.DecodeIntegerSize(rd)), traits.size_D);
                rsAparams.P = Helpers.AlignBytes(rd.ReadBytes(Helpers.DecodeIntegerSize(rd)), traits.size_P);
                rsAparams.Q = Helpers.AlignBytes(rd.ReadBytes(Helpers.DecodeIntegerSize(rd)), traits.size_Q);
                rsAparams.DP = Helpers.AlignBytes(rd.ReadBytes(Helpers.DecodeIntegerSize(rd)), traits.size_DP);
                rsAparams.DQ = Helpers.AlignBytes(rd.ReadBytes(Helpers.DecodeIntegerSize(rd)), traits.size_DQ);
                rsAparams.InverseQ = Helpers.AlignBytes(rd.ReadBytes(Helpers.DecodeIntegerSize(rd)), traits.size_InvQ);

                rsa.ImportParameters(rsAparams);
                return rsa;
            }
            catch (Exception)
            {
                Debug.Assert(false);
                return null;
            }
            finally
            {
                rd.Close();
            }
        }
    }
    public class RSAParameterTraits
    {
        public RSAParameterTraits(int modulusLengthInBits)
        {
            int assumedLength = -1;
            double logbase = Math.Log(modulusLengthInBits, 2);
            if (logbase == (int)logbase)
            {
                assumedLength = modulusLengthInBits;
            }
            else
            {
                assumedLength = (int)(logbase + 1.0);
                assumedLength = (int)(Math.Pow(2, assumedLength));
                System.Diagnostics.Debug.Assert(false);
            }

            switch (assumedLength)
            {
                case 1024:
                    this.size_Mod = 0x80;
                    this.size_Exp = -1;
                    this.size_D = 0x80;
                    this.size_P = 0x40;
                    this.size_Q = 0x40;
                    this.size_DP = 0x40;
                    this.size_DQ = 0x40;
                    this.size_InvQ = 0x40;
                    break;
                case 2048:
                    this.size_Mod = 0x100;
                    this.size_Exp = -1;
                    this.size_D = 0x100;
                    this.size_P = 0x80;
                    this.size_Q = 0x80;
                    this.size_DP = 0x80;
                    this.size_DQ = 0x80;
                    this.size_InvQ = 0x80;
                    break;
                case 4096:
                    this.size_Mod = 0x200;
                    this.size_Exp = -1;
                    this.size_D = 0x200;
                    this.size_P = 0x100;
                    this.size_Q = 0x100;
                    this.size_DP = 0x100;
                    this.size_DQ = 0x100;
                    this.size_InvQ = 0x100;
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }
        }

        public int size_Mod = -1;
        public int size_Exp = -1;
        public int size_D = -1;
        public int size_P = -1;
        public int size_Q = -1;
        public int size_DP = -1;
        public int size_DQ = -1;
        public int size_InvQ = -1;
    }
    public class Helpers
    {
        public static int DecodeIntegerSize(System.IO.BinaryReader rd)
        {
            byte byteValue;
            int count;

            byteValue = rd.ReadByte();
            if (byteValue != 0x02)
                return 0;

            byteValue = rd.ReadByte();
            if (byteValue == 0x81)
            {
                count = rd.ReadByte();
            }
            else if (byteValue == 0x82)
            {
                byte hi = rd.ReadByte();
                byte lo = rd.ReadByte();
                count = BitConverter.ToUInt16(new[] { lo, hi }, 0);
            }
            else
            {
                count = byteValue;
            }

            while (rd.ReadByte() == 0x00)
            {
                count -= 1;
            }
            rd.BaseStream.Seek(-1, System.IO.SeekOrigin.Current);

            return count;
        }
        public static byte[] GetBytesFromPEM(string pemString, PemStringType type)
        {
            string header; string footer;

            switch (type)
            {
                case PemStringType.Certificate:
                    header = "-----BEGIN CERTIFICATE-----";
                    footer = "-----END CERTIFICATE-----";
                    break;
                case PemStringType.RsaPrivateKey:
                    header = "-----BEGIN RSA PRIVATE KEY-----";
                    footer = "-----END RSA PRIVATE KEY-----";
                    break;
                default:
                    return null;
            }

            int start = pemString.IndexOf(header) + header.Length;
            int end = pemString.IndexOf(footer, start) - start;
            return Convert.FromBase64String(pemString.Substring(start, end));
        }
        public static byte[] AlignBytes(byte[] inputBytes, int alignSize)
        {
            int inputBytesSize = inputBytes.Length;

            if ((alignSize != -1) && (inputBytesSize < alignSize))
            {
                byte[] buf = new byte[alignSize];
                for (int i = 0; i < inputBytesSize; ++i)
                {
                    buf[i + (alignSize - inputBytesSize)] = inputBytes[i];
                }
                return buf;
            }
            else
            {
                return inputBytes;
            }
        }
    }
}
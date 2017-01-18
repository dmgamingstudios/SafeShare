using FuhrerShare.Core.Networking.Clients;
using FuhrerShare.Core.Security;
using FuhrerShare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Nodes
{
    [Serializable]
    public class SafeNode
    {
        public ConnectionMethod.ConnMethod CM;
        public bool ConnectionState = false;
        public string ip = null;
        public int port = 0;
        public string hiddenid = null;
        public Identity identity;
        public TcpClient _client;
        public SslStream ClientStream;
        RSACryptoServiceProvider csp = null;
        private X509CertificateCollection col = new X509CertificateCollection();

        public SafeNode(string name, string ip, int port, X509Certificate2 pkey, bool local, ConnectionMethod.ConnMethod CM = ConnectionMethod.ConnMethod.Clear)
        {
            this.ip = ip;
            this.port = port;
            this.CM = CM;
            identity = new Identity(pkey, "", name, local);
        }
		public SafeNode(string name, X509Certificate2 pkey, string hiddenid, ConnectionMethod.ConnMethod CM, bool local)
        {
            this.hiddenid = hiddenid;
            this.CM = CM;
            identity = new Identity(pkey, "", name, local);
        }
        public string SendNodeMsg(string msg)
        {
            if(msg.Contains("²"))
            {
                return "ERR";
            }
            SHA512Managed sha512 = new SHA512Managed();
            csp = (RSACryptoServiceProvider)Config.LocalNode.identity.pfxcert.PrivateKey;
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(msg);
                byte[] hash = sha512.ComputeHash(data);
                string signature = Convert.ToBase64String(csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA512")));
                ClientStream.Write(Encoding.ASCII.GetBytes(signature + "²" + msg));
                byte[] rmsg = new byte[2048];
                ClientStream.Read(rmsg, 0, rmsg.Length);
                return Encoding.ASCII.GetString(rmsg);
            }
            catch(Exception)
            { return "We had an exception.."; }
        }
		public void Connect()
        {
            col.Add((X509Certificate)Config.LocalNode.identity.pfxcert);
            NetworkStream _unsecure;
			if(CM == ConnectionMethod.ConnMethod.I2P && !ConnectionState)
            {

            }
			else if(CM == ConnectionMethod.ConnMethod.Clear && !ConnectionState)
            {
                _client.Connect(IPAddress.Parse(ip), port);
            }
			else if(CM == ConnectionMethod.ConnMethod.Tor && !ConnectionState)
            {

            }
            _unsecure = _client.GetStream();
            byte[] rmsg = new byte[2048];
            _unsecure.Read(rmsg, 0, rmsg.Length);
            byte[] smsg = Encoding.ASCII.GetBytes("");
            _unsecure.Write(smsg, 0, smsg.Length);
            ClientStream = new SslStream(_unsecure, false, new RemoteCertificateValidationCallback(ValCallBack), new LocalCertificateSelectionCallback(LocalCertSel));
            ClientStream.AuthenticateAsClient(Config.LocalNode.identity.hash, col, System.Security.Authentication.SslProtocols.Tls12, false);
        }

        private X509Certificate LocalCertSel(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return (X509Certificate)Config.LocalNode.identity.pfxcert;
        }

        private bool ValCallBack(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public void DisConnect()
        {
            if (!ConnectionState)
                return;
        }
		public void SendFilePieceRequest(SslStream CurrentOpenStream, File file, string RequestedLength)
        {
            if (CurrentOpenStream == null || file == null || RequestedLength == null || ConnectionState)
                return;
            CurrentOpenStream.Write(Encoding.ASCII.GetBytes(file.name + "|" + RequestedLength));
        }
    }
}
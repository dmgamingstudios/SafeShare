using FuhrerShare.Core.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Handlers
{
    public class NewClientHandler
    {
        string clientHash = null;
        TcpClient _client;
        NetworkStream _clientStream;
        SslStream _clientSslStream;
        SafeNode _node;
        public NewClientHandler(TcpClient client)
        {
            _client = client;
            _clientStream = _client.GetStream();
        }
        public void StartHandler()
        {
            if(NegotiateIdentity())
            {
                if(NegotiateSSL())
                {

                }
                if (Config.EnforceSSL)
                    _client.Close();
            }
            if (Config.VerifyIdentities == "Verify")
                _client.Close();
            else if(Config.VerifyIdentities == "Verify or Blacklist")
            {

            }
        }
        private bool NegotiateIdentity()
        {
            byte[] Msg = Encoding.ASCII.GetBytes("REQUESTIDENTITY|" + Config.LocalNode.identity.hash);
            _clientStream.Write(Msg, 0, Msg.Length);
            byte[] clientMsg = new byte[2048];
            _clientStream.Read(clientMsg, 0, clientMsg.Length);
            return ParseData(Encoding.ASCII.GetString(clientMsg));
        }
        private bool ParseData(string data)
        {
            string[] clientData = data.Split('|');
            _node = new SafeNode(clientData[0], "", 1, new X509Certificate2(Convert.FromBase64String(clientData[1])), false);
            return true;
        }
        private bool WeKnowHim(string hash)
        {
            return false;
        }
        private bool NegotiateSSL()
        {
            SslStream ssl = new SslStream(_clientStream, false, new RemoteCertificateValidationCallback(callback), new LocalCertificateSelectionCallback(select));
            ssl.AuthenticateAsServerAsync((X509Certificate)Config.LocalNode.identity.pfxcert, true, System.Security.Authentication.SslProtocols.Tls12, false);
            if (!ssl.IsAuthenticated || !ssl.IsEncrypted)
                return false;
            ssl.Write(Encoding.ASCII.GetBytes("WeDone"));
            return true;
        }

        private X509Certificate select(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return (X509Certificate)Config.LocalNode.identity.pfxcert;
        }

        private bool callback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            switch(sslPolicyErrors)
            {
                case SslPolicyErrors.RemoteCertificateChainErrors:
                    return false;
                case SslPolicyErrors.RemoteCertificateNameMismatch:
                    return false;
                case SslPolicyErrors.RemoteCertificateNotAvailable:
                    return false;
            }
            if (!certificate.Subject.Contains("CN=" + clientHash))
                return false;
            return true;
        }
    }
}
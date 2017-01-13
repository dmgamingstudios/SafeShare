using FuhrerShare.Core.Networking.Clients;
using FuhrerShare.Core.Security;
using FuhrerShare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Nodes
{
    internal class LocalSafeNode
    {
        internal ConnectionMethod.ConnMethod CM;
        internal bool ConnectionState = false;
        internal string ip = null;
        internal int port = 0;
        internal string hiddenid = null;
        internal LocalIdentity identity;
        internal SslStream ClientStream;
        RSACryptoServiceProvider csp = null;
        internal LocalSafeNode(string name, string ip, int port, X509Certificate2 pkey, bool local, string hash, ConnectionMethod.ConnMethod CM = ConnectionMethod.ConnMethod.Clear)
        {
            this.ip = ip;
            this.port = port;
            this.CM = CM;
            identity = new LocalIdentity(pkey, hash, name, local);
        }
		internal LocalSafeNode(string name, X509Certificate2 pkey, string hiddenid, ConnectionMethod.ConnMethod CM, bool local, string hash)
        {
            this.hiddenid = hiddenid;
            this.CM = CM;
            identity = new LocalIdentity(pkey, hash, name, local);
        }
        internal string SendNodeMsg(string msg)
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
		internal void Connect()
        {
			if(CM == ConnectionMethod.ConnMethod.I2P && !ConnectionState)
            {

            }
			else if(CM == ConnectionMethod.ConnMethod.Clear && !ConnectionState)
            {

            }
			else if(CM == ConnectionMethod.ConnMethod.Tor && !ConnectionState)
            {

            }
        }
		internal void DisConnect()
        {
            if (!ConnectionState)
                return;
        }
		internal void SendFilePieceRequest(SslStream CurrentOpenStream, File file, string RequestedLength)
        {
            if (CurrentOpenStream == null || file == null || RequestedLength == null || ConnectionState)
                return;
            CurrentOpenStream.Write(Encoding.ASCII.GetBytes(file.name + "|" + RequestedLength));
        }
    }
}
using FuhrerShare.Core.Networking.Clients;
using FuhrerShare.Core.Networking.ClientStreams;
using FuhrerShare.Core.Security;
using FuhrerShare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Nodes
{
    [Serializable]
    public class LocalSafeNode
    {
        public ConnectionMethod.ConnMethod CM;
        public bool ConnectionState = false;
        public string ip = null;
        public int port = 0;
        public string hiddenid = null;
        public LocalIdentity identity;
        public LocalSafeNode(string name, string ip, int port, X509Certificate2 pkey, bool local, string hash, ConnectionMethod.ConnMethod CM = ConnectionMethod.ConnMethod.Clear)
        {
            this.ip = ip;
            this.port = port;
            this.CM = CM;
            identity = new LocalIdentity(pkey, hash, name, local);
        }
        public LocalSafeNode(string name, X509Certificate2 pkey, string hiddenid, ConnectionMethod.ConnMethod CM, bool local, string hash)
        {
            this.hiddenid = hiddenid;
            this.CM = CM;
            identity = new LocalIdentity(pkey, hash, name, local);
        }
    }
}
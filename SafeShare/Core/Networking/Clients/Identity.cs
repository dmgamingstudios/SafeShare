using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Networking.Clients
{
    internal class Identity
    {
        internal X509Certificate2 PubKey;
        internal string name = "";
        internal Identity(X509Certificate2 PubKey, string name = "unknown")
        {
            this.PubKey = PubKey;
            this.name = name;
        }
    }
}
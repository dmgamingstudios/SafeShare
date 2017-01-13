using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuhrerShare.Core.Networking.Clients
{
    internal class LocalIdentity
    {
        internal X509Certificate2 pfxcert;
        internal string name = "";
        internal string hash = "";
        internal LocalIdentity(X509Certificate2 pfxcert, string hash = "unknown", string name = "unknown", bool islocal = false)
        {
            this.pfxcert = pfxcert;
            this.name = name;
            this.hash = hash;
        }
    }
}
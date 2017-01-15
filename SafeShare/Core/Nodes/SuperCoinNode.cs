using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Nodes
{
    [Serializable]
    public class SuperCoinNode
    {
        public string name = "donttrustme";
        public SuperCoinNode(X509Certificate2 SuperCert, string XmlIdFile)
        {

        }
    }
}
using FuhrerShare.Core.Networking.Clients;
using FuhrerShare.Core.Nodes;
using net.i2p.client.streaming;
using net.i2p.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Networking.Proxy.I2P
{
    public class I2PProxyServer
    {
        public void CreateI2PConnection2Node(SafeNode node, out SslStream ssl)
        {
            I2PSocketManager manager = I2PSocketManagerFactory.createManager();
            Destination D = new Destination(node.hiddenid);
            I2PSocket socket = manager.connect(D);
            ssl = null;
        }
    }
}
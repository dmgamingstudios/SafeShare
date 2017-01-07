using FuhrerShare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Nodes
{
    internal class CoinNode
    {
        internal string name = "";
        internal string connection = "";
        internal ConnectionMethod.ConnMethod CM;
        internal bool Connected = false;
        internal CoinNode(string name, string connection, ConnectionMethod.ConnMethod CM)
        {
            this.name = name;
            this.connection = connection;
            this.CM = CM;
        }
        internal void Connect()
        {

        }
    }
}
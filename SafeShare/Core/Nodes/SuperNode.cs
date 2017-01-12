using FuhrerShare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Nodes
{
    internal class SuperNode
    {
        internal string name = "";
        internal ConnectionMethod.ConnMethod CM;
        internal bool Connected = false;
        internal SslStream SuperNodeStream;
        internal SuperNode(string name, ConnectionMethod.ConnMethod CM)
        {
            this.name = name;
            this.CM = CM;
        }
        internal void Connect()
        {

        }
        internal string SendSuperNodeMsg(string msg)
        {
            if (!Connected)
                return "ERR";
            return null;
        }
    }
}
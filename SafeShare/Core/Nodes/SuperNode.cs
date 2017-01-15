using FuhrerShare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Nodes
{
    [Serializable]
    public class SuperNode
    {
        public string name = "";
        public ConnectionMethod.ConnMethod CM;
        public bool Connected = false;
        public SslStream SuperNodeStream;
        public SuperNode(string name, ConnectionMethod.ConnMethod CM)
        {
            this.name = name;
            this.CM = CM;
        }
        public void Connect()
        {

        }
        public string SendSuperNodeMsg(string msg)
        {
            if (!Connected)
                return "ERR";
            return null;
        }
    }
}
using FuhrerShare.Core.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Networking.ClientStreams
{
    public static class SafeNodeStream
    {
        private static Hashtable StreamTable = new Hashtable();
        public static SslStream GetClientStream(SafeNode node)
        {
            return (SslStream)StreamTable[node];
        }
        public static void SetClientStream(SafeNode node, SslStream ssl)
        {
            StreamTable.Add(node, ssl);
        }
    }
}
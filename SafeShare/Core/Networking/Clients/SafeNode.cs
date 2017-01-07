using FuhrerShare.Core.Security;
using FuhrerShare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Networking.Clients
{
    internal class SafeNode
    {
        internal ConnectionMethod.ConnMethod CM;
        internal bool ConnectionState = false;
        internal string ip = null;
        internal int port = 0;
        internal string hiddenid = null;
        internal Identity identity;
        internal SslStream ClientStream;
        internal SafeNode(string ip, int port, ConnectionMethod.ConnMethod CM = ConnectionMethod.ConnMethod.Clear)
        {
            this.ip = ip;
            this.port = port;
            this.CM = CM;
        }
		internal SafeNode(string hiddenid, ConnectionMethod.ConnMethod CM)
        {
            this.hiddenid = hiddenid;
            this.CM = CM;
        }
		internal void Connect()
        {
			if(CM == ConnectionMethod.ConnMethod.I2P || CM == ConnectionMethod.ConnMethod.Tor && !ConnectionState)
            {

            }
			else if(CM == ConnectionMethod.ConnMethod.Clear && !ConnectionState)
            {

            }
			else
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
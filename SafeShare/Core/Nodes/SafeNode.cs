using FuhrerShare.Core.Networking.Clients;
using FuhrerShare.Core.Security;
using FuhrerShare.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Nodes
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
        internal string SendNodeMsg(string msg)
        {
            try
            {
                ClientStream.Write(Encoding.ASCII.GetBytes(msg));
                byte[] rmsg = new byte[2048];
                ClientStream.Read(rmsg, 0, rmsg.Length);
                return Encoding.ASCII.GetString(rmsg);
            }
            catch(Exception)
            { return "We had an exception.."; }
        }
		internal void Connect()
        {
			if(CM == ConnectionMethod.ConnMethod.I2P && !ConnectionState)
            {

            }
			else if(CM == ConnectionMethod.ConnMethod.Clear && !ConnectionState)
            {

            }
			else if(CM == ConnectionMethod.ConnMethod.Tor && !ConnectionState)
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
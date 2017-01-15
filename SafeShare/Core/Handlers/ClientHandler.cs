using FuhrerShare.Core.Networking.Clients;
using FuhrerShare.Core.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Handlers
{
    public class ClientHandler
    {
        private byte[] bytes = new byte[5];
        public bool die = false;
        public SafeNode node;
        public ClientHandler(SafeNode node)
        {
            this.node = node;
        }
        public void HandleNodeMessages()
        {
            SslStream Ssl = node.ClientStream;
            while (!die)
            {
                var memStream = new MemoryStream();
                int bytesread = Ssl.Read(bytes, 0, bytes.Length);
                while (bytesread > 0)
                {
                    memStream.Write(bytes, 0, bytesread);
                    bytesread = Ssl.Read(bytes, 0, bytes.Length);
                }
                string ourResponse = new MessageHandler().HandleIncomingMsgFromNode(Encoding.ASCII.GetString(memStream.ToArray()), node);
                Ssl.Write(Encoding.ASCII.GetBytes(ourResponse));
            }
        }
        public void HandleNodeKeepAlive()
        {
            SslStream Ssl = node.ClientStream;
            while(!die)
            {
                Thread.Sleep(120000);
                Ssl.Write(Encoding.ASCII.GetBytes("PING"));
                var memStream = new MemoryStream();
                int bytesread = Ssl.Read(bytes, 0, bytes.Length);
                while (bytesread > 0)
                {
                    memStream.Write(bytes, 0, bytesread);
                    bytesread = Ssl.Read(bytes, 0, bytes.Length);
                }
                if (Encoding.ASCII.GetString(memStream.ToArray()) != "PONG")
                    die = true;
            }
        }
    }
}
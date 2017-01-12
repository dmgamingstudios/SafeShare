using FuhrerShare.Core.Networking.Clients;
using FuhrerShare.Core.Nodes;
using FuhrerShare.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Handlers
{
    internal class MessageHandler
    {
        internal void BroadcastHandler(string Message)
        {

        }
        internal string HandleIncomingMsgFromNode(string msgwsig, SafeNode node)
        {
            string[] msg = msgwsig.Split('²');
            if (!new SignatureVerifier().Verify(msg[1], Convert.FromBase64String(msg[0]), node))
                return "SIGERR";
            if (msg[1].Equals("PING"))
                return "PONG";
            string[] msgData = msg[1].Split('|');
            switch(msgData[0])
            {
                case "REQUESTSECUSITE":
                    break;
                case "HOP":
                    break;
                default:
                    return "Whatdoyoumean?";
            }
            return "";
        }
    }
}
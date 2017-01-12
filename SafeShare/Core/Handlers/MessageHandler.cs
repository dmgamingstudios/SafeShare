using FuhrerShare.Core.Networking.Clients;
using FuhrerShare.Core.Nodes;
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
        internal string HandleIncomingMsgFromNode(string msg, SafeNode node)
        {
            if (msg.Equals("PING"))
                return "PONG";
            string[] msgData = msg.Split('|');
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
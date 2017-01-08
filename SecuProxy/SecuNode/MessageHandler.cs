using FuhrerShare.Core.Networking.Clients;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace SecuProxy.SecuNode
{
    internal class MessageHandler
    {
        byte[] buff = new byte[8192];
        internal string RequestSecuNetWebSite(string HashID, string navurl, SafeNode[] Nodes)
        {
            foreach(SafeNode node in Nodes)
            {
                SslStream ssl = node.ClientStream;
                ssl.Write(Encoding.ASCII.GetBytes("REQUESTSECUSITE|" + HashID + "|" + navurl));
                var memStream = new MemoryStream();
                int bytesread = ssl.Read(buff, 0, buff.Length);
                while (bytesread > 0)
                {
                    memStream.Write(buff, 0, bytesread);
                    bytesread = ssl.Read(buff, 0, buff.Length);
                }
                string response = Encoding.ASCII.GetString(memStream.ToArray());
                if (response.StartsWith("ERR|NOPERM"))
                    return "<!DOCTYPE html>" +
                      "<html><title>Access Denied</title><body><h1>" +
                      "Access Denied" +
                      "</h1>" +
                      "<p>The host is using whitelisting and you are not on the whitelist</p>" +
                      "</body>" +
                      "</html>";
                else if(response.StartsWith("ERR|SITEUNKNOWN"))
                    return "<!DOCTYPE html>" +
                      "<html><title>Site Unknown</title><body><h1>" +
                      "Site Unknown" +
                      "</h1>" +
                      "<p>The host you are requesting is not known</p>" +
                      "</body>" +
                      "</html>";
                else
                    return response;
            }
            return "<!DOCTYPE html>" +
                      "<html><title>Error</title><body><h1>" +
                      "Unknown Error Occured" +
                      "</h1>" +
                      "<p>We were unable to connect to any node or none replied</p>" +
                      "</body>" +
                      "</html>";
        }
    }
}
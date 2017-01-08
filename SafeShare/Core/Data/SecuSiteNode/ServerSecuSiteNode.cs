using FuhrerShare.Core.Networking.Clients;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Data.SecuSiteNode
{
    internal class ServerSecuSiteNode
    {
        string name = null;
        string localip = null;
        int localport = 0;
        bool useWhiteList = false;
        string[] whitelistedNodes = null;
        internal ServerSecuSiteNode(string name, string localip, int localport, string configFile = null)
        {
            this.name = name;
            this.localip = localip;
            this.localport = localport;
        }
        internal string RequestThisSite(SafeNode Requester)
        {
            if(useWhiteList)
                if (!whitelistedNodes.Contains(Requester.identity.name))
                    return "ERR|NOPERM";
            var doc = new HtmlDocument();
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(localip + ":" + Convert.ToString(localport));
            HttpWebResponse webresponse = (HttpWebResponse)request.GetResponse();
            if (webresponse.ContentType.StartsWith("text/html"))
            {
                var resultStream = webresponse.GetResponseStream();
                doc.Load(resultStream);
            }
            return doc.ToString();
        }
    }
}
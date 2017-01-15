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
    public class DownloadHandler
    {
        public File file;
        public SafeNode[] AvailableNodes;
        public SafeNode[] UseableNodes;
        public DownloadHandler(File file, SafeNode[] AvailableNodes)
        {
            this.file = file;
            this.AvailableNodes = AvailableNodes;
        }
        public void StartDownload()
        {
            int i = 0;
            foreach(SafeNode node in AvailableNodes)
            {
                node.ClientStream.Write(Encoding.ASCII.GetBytes("Request|" + file.hash));
                byte[] buff = new byte[5];
                string response = node.ClientStream.Read(buff, 0, 0).ToString();
                if (response == "yes")
                    UseableNodes[i] = node;
                i += i;
            }
            InitiateDownload();
        }
        private void InitiateDownload()
        {

        }
    }
}
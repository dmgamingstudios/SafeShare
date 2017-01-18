using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Listeners
{
    public class SafeNodeListener
    {
        internal bool isRunning = true;
        TcpListener _SafeNodeListener;
        public void StartListener()
        {
            _SafeNodeListener = new TcpListener(IPAddress.Parse("127.0.0.1"), Config.SafeNodeListenerPort);
            _SafeNodeListener.Start();
            while(isRunning)
            {

            }
            _SafeNodeListener.Stop();
        }
        public void StopListener()
        {
            isRunning = false;
        }
    }
}
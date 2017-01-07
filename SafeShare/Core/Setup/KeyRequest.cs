﻿using FuhrerShare.Core.DataBases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static FuhrerShare.Core.Networking.ProxyEngine;

namespace FuhrerShare.Core.Setup
{
    internal class KeyRequest
    {
        internal X509Certificate2 Request(string name, string pass, bool canuse_clear = true, bool canuse_tor = false, bool canuse_i2p = false)
        {
            TcpClient client = new TcpClient();
            Socks5ProxyClient SPC;
            HttpProxyClient HPC;
            string servera = null;
            int i = 0;
            if (canuse_clear)
                servera = SuperNodeDB.ClearSuperNodes[i];
            else if (canuse_tor)
                servera = SuperNodeDB.TorSuperNodes[i];
            else if (canuse_i2p)
                servera = SuperNodeDB.I2PSuperNodes[i];
            try
            {

                if (canuse_i2p)
                {
                    HPC = new HttpProxyClient();
                    HPC.ProxyHost = "127.0.0.1";
                    HPC.ProxyPort = 4444;
                    client = HPC.CreateConnection("", 1);
                }
                else if (canuse_clear)
                    client.Connect("", 1);
                else if (canuse_tor)
                {
                    SPC = new Socks5ProxyClient();
                    SPC.ProxyHost = "127.0.0.1";
                    SPC.ProxyPort = 9050;
                    SPC.ProxyUserName = "";
                    SPC.ProxyPassword = "";
                    client = SPC.CreateConnection("", 1);
                }

            }
            catch(Exception ex)
            { }
        }
    }
}
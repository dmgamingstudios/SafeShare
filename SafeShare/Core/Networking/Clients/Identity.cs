﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuhrerShare.Core.Networking.Clients
{
    internal class Identity
    {
        internal X509Certificate2 PubKey;
        internal string name = "";
        internal string hash = "";
        internal Identity(X509Certificate2 PubKey, string hash = "Unknown", string name = "unknown", bool islocal = false)
        {
            this.PubKey = PubKey;
            this.name = name;
            this.hash = hash;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Security
{
    internal class File
    {
        internal string name = null;
        internal string hash = null;
        internal byte[] size;
        internal File(string hash, string name, int size)
        {
            this.name = name;
            this.hash = hash;
            this.size = new byte[size];
        }
    }
}
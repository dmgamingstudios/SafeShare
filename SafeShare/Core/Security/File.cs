using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Security
{
    public class File
    {
        public string name = null;
        public string hash = null;
        public byte[] size;
        public File(string hash, string name, int size)
        {
            this.name = name;
            this.hash = hash;
            this.size = new byte[size];
        }
    }
}
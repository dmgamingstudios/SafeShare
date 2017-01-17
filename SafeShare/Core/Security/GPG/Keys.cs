using Starksoft.Aspen.GnuPG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Security.GPG
{
    public class Keys
    {
        Gpg tool = new Gpg();
        public Keys()
        {
            tool.BinaryPath = Config.Gpg2exe;
        }
        public void AddPubKey(string Key)
        {
            byte[] KeyBytes = ASCIIEncoding.ASCII.GetBytes(Key);
            MemoryStream key = new MemoryStream(KeyBytes);
            tool.Import(key);
        }
        public string[] ListKeys()
        {
            GpgKeyCollection col = tool.GetKeys();
            int i = 0;
            string[] returnstring = null;
            foreach(GpgKey key in col)
            {
                returnstring[i] = key.ToString();
                i = i + 1;
            }
            return returnstring;
        }
    }
}
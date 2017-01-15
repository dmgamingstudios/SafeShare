using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuhrerShare.Core.DataBases
{
    public class CoinNodeDB
    {
        public static string[] CoindNodes;
        public CoinNodeDB()
        {
            string NodeFileData = File.ReadAllText(Application.StartupPath + "\\coinnodes.dat");

        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuhrerShare.Core.DataBases
{
    internal class CoinNodeDB
    {
        internal static string[] CoindNodes;
        internal CoinNodeDB()
        {
            string NodeFileData = File.ReadAllText(Application.StartupPath + "\\coinnodes.dat");

        }
    }
}
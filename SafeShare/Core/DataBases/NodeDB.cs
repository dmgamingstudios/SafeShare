using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuhrerShare.Core.DataBases
{
    internal class NodeDB
    {
        internal NodeDB()
        {
            if(File.Exists(Application.StartupPath + ""))
            {
                Load();
            }
        }
        internal void Load()
        {

        }
    }
}
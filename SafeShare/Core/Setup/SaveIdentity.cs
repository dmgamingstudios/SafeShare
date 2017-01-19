using FuhrerShare.Core.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace FuhrerShare.Core.Setup
{
    public class SaveIdentity
    {
        string SaveDirectory = Path.Combine(Application.StartupPath, "identities");
        public SaveIdentity(LocalSafeNode localnode)
        {
            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);
            string file = SaveDirectory + "\\" + localnode.identity.name + ".crid";
            try
            {
                using (MemoryStream mstream = new MemoryStream())
                {
                    Stream stream = File.Open(file, FileMode.Create);
                    BinaryFormatter bformatter = new BinaryFormatter();
                    bformatter.Serialize(stream, localnode);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {}
        }
    }
}
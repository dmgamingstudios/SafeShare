using FuhrerShare.Core.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            string file = SaveDirectory + "\\" + localnode.identity.name;
            string filedata = null;
            try
            {
                filedata = Convert.ToBase64String(localnode.identity.pfxcert.Export(X509ContentType.Pkcs12));
            }
            catch (Exception ex)
            {}
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.WriteLine(filedata);
                sw.WriteLine(localnode.identity.hash);
                sw.WriteLine(localnode.identity.name);
                sw.WriteLine(localnode.hiddenid);
                sw.WriteLine(localnode.ip);
                sw.WriteLine(localnode.port);
            }
        }
    }
}
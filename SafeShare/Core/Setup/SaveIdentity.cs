using FuhrerShare.Core.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace FuhrerShare.Core.Setup
{
    internal class SaveIdentity
    {
        string SaveDirectory = Path.Combine(Application.StartupPath, "identities");
        internal SaveIdentity(LocalSafeNode localnode)
        {
            string file = SaveDirectory + localnode.identity.name;
            string filedata = null;
            string nodedataxml = null;
            try
            {
                filedata += Convert.ToBase64String(localnode.identity.pfxcert.RawData);
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(localnode.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, localnode);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    using (var stringWriter = new StringWriter())
                    using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                    {
                        xmlDocument.WriteTo(xmlTextWriter);
                        xmlTextWriter.Flush();
                        nodedataxml = stringWriter.GetStringBuilder().ToString();
                    }
                }
            }
            catch (Exception ex)
            {}
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.WriteLine(nodedataxml);
                sw.WriteLine(filedata);
            }
        }
    }
}
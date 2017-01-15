using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using FuhrerShare.Core.Nodes;
using System.Collections;

namespace FuhrerShare.Core.DataBases
{
    public class NodeDB
    {
        public static Hashtable Nodes = new Hashtable();
        public NodeDB()
        {
            if (File.Exists(Application.StartupPath + "\\nodes.dat") && Nodes.Count == 0)
            {
                Load();
            }
        }
        private void Load()
        {
            string[] NodeFileData = File.ReadAllLines(Application.StartupPath + "\\nodes.dat");
            foreach(string l in NodeFileData)
            {
                SafeNode cloadednode = LoadSingle<SafeNode>(l);
                Nodes.Add(cloadednode.identity.name, cloadednode);
            }
        }
        public void Save()
        {
            string[] SerializedNodes = null;
            int s = 0;
            foreach(DictionaryEntry DE in Nodes)
            {
                if (((SafeNode)DE.Value).ConnectionState)
                    ((SafeNode)DE.Value).DisConnect();
                SerializedNodes[s] = SaveSingle<SafeNode>((SafeNode)DE.Value);
                s = s + 1;
            }
        }
        private T LoadSingle<T>(string line)
        {
            if (string.IsNullOrEmpty(line)) { return default(T); }
            T objectOut = default(T);
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(line);
                string xmlString = xmlDocument.OuterXml;
                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);
                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Close();
                    }
                    read.Close();
                }
            }
            catch (Exception ex)
            {
                
            }
            return objectOut;
        }
        private string SaveSingle<T>(T serializableObject)
        {
            if (serializableObject == null) { return ""; }
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    using (var stringWriter = new StringWriter())
                    using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                    {
                        xmlDocument.WriteTo(xmlTextWriter);
                        xmlTextWriter.Flush();
                        return stringWriter.GetStringBuilder().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                return "ERR";
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace FuhrerShare
{
    internal static class Config
    {
        private static readonly string SettingsPath = Path.Combine(Application.StartupPath, "config.xml");
        internal static string OTP = "";
        public static string UserPass
        {
            get
            {
                return ReadValueSafe("UserPass", "AVerySafeP@ssW0rd");
            }
            set
            {
                using (SHA512 shaM = SHA512.Create())
                {
                    value = Convert.ToBase64String(shaM.ComputeHash(Encoding.ASCII.GetBytes(value)));
                }
                WriteValue("UserPass", value);
            }
        }
        private static string ReadValue(string pstrValueToRead)
        {
            try
            {
                XPathDocument doc = new XPathDocument(SettingsPath);
                XPathNavigator nav = doc.CreateNavigator();
                XPathExpression expr = nav.Compile(@"/settings/" + pstrValueToRead);
                XPathNodeIterator iterator = nav.Select(expr);
                while (iterator.MoveNext())
                {
                    return iterator.Current.Value;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
        private static string ReadValueSafe(string pstrValueToRead, string defaultValue = "")
        {
            string value = ReadValue(pstrValueToRead);
            return (!string.IsNullOrEmpty(value)) ? value : defaultValue;
        }
        private static void WriteValue(string pstrValueToRead, string pstrValueToWrite)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                if (File.Exists(SettingsPath))
                {
                    using (var reader = new XmlTextReader(SettingsPath))
                    {
                        doc.Load(reader);
                    }
                }
                else
                {
                    var dir = Path.GetDirectoryName(SettingsPath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    doc.AppendChild(doc.CreateElement("settings"));
                }
                XmlElement root = doc.DocumentElement;
                XmlNode oldNode = root.SelectSingleNode(@"/settings/" + pstrValueToRead);
                if (oldNode == null)
                {
                    oldNode = doc.SelectSingleNode("settings");
                    oldNode.AppendChild(doc.CreateElement(pstrValueToRead)).InnerText = pstrValueToWrite;
                    doc.Save(SettingsPath);
                    return;
                }
                oldNode.InnerText = pstrValueToWrite;
                doc.Save(SettingsPath);
            }
            catch
            {
            }
        }
    }
}
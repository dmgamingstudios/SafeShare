﻿using FuhrerShare.Core.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using static FuhrerShare.Enums.ProtectionMethod;

namespace FuhrerShare
{
    public static class Config
    {
        private static readonly string SettingsPath = Path.Combine(Application.StartupPath, "config.xml");
        public static string OTP = "";
        public static LocalSafeNode LocalNode;
        public static string Gpg2exe
        {
            get
            {
                return ReadValueSafe("gpg2exe", "");
            }
            set
            {
                WriteValue("gpg2exe", value.ToString());
            }
        }
        public static PrMethod ProtectMethod
        {
            get
            {
                return (PrMethod)Enum.Parse(typeof(PrMethod), ReadValueSafe("PrMethod", "low"));
            }
            set
            {
                WriteValue("PrMethod", value.ToString());
            }
        }
        public static bool UseClear
        {
            get
            {
                return bool.Parse(ReadValueSafe("UseClear", "true"));
            }
            set
            {
                WriteValue("UseClear", value.ToString());
            }
        }
        public static bool SetupDone
        {
            get
            {
                return bool.Parse(ReadValueSafe("SetupDone", "false"));
            }
            set
            {
                WriteValue("SetupDone", value.ToString());
            }
        }
        public static bool UseI2P
        {
            get
            {
                return bool.Parse(ReadValueSafe("UseI2P", "false"));
            }
            set
            {
                WriteValue("UseI2P", value.ToString());
            }
        }
        public static bool UseTor
        {
            get
            {
                return bool.Parse(ReadValueSafe("UseTor", "false"));
            }
            set
            {
                WriteValue("UseTor", value.ToString());
            }
        }
        public static bool UseOTP
        {
            get
            {
                return bool.Parse(ReadValueSafe("UseOTP", "false"));
            }
            set
            {
                WriteValue("UseOTP", value.ToString());
            }
        }
        public static bool DeleteFilesOnShutdown
        {
            get
            {
                return bool.Parse(ReadValueSafe("DeleteFilesOnShutdown", "false"));
            }
            set
            {
                WriteValue("DeleteFilesOnShutdown", value.ToString());
            }
        }
        public static int SafeNodeListenerPort
        {
            get
            {
                return int.Parse(ReadValueSafe("SafeNodeListenerPort", "666"));
            }
            set
            {
                WriteValue("SafeNodeListenerPort", value.ToString());
            }
        }
        public static bool EncryptChat
        {
            get
            {
                return bool.Parse(ReadValueSafe("EncryptChat", "false"));
            }
            set
            {
                WriteValue("EncryptChat", value.ToString());
            }
        }
        public static bool KeepChatHistory
        {
            get
            {
                return bool.Parse(ReadValueSafe("KeepChatHistory", "false"));
            }
            set
            {
                WriteValue("KeepChatHistory", value.ToString());
            }
        }
        public static string VerifyIdentities
        {
            get
            {
                return ReadValueSafe("VerifyIdentities", "not");
            }
            set
            {
                WriteValue("VerifyIdentities", value.ToString());
            }
        }
        public static bool EnforceSSL
        {
            get
            {
                return bool.Parse(ReadValueSafe("EnforceSSL", "false"));
            }
            set
            {
                WriteValue("EnforceSSL", value.ToString());
            }
        }
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
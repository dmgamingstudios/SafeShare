using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using FuhrerShare.Core.Networking.Clients;

namespace FuhrerShare.Core.Data.SecuSiteNode
{
    internal class SuperSecuSiteNode
    {
        internal readonly string SiteDBFile = Application.StartupPath + "\\secusites.dat";
        internal static Hashtable SecuSites = new Hashtable();
        internal void Load()
        {
            string[] FileData = File.ReadAllLines(SiteDBFile);
            foreach(string Data in FileData)
            {
                string[] siteData = Data.Split('|');
                SecuSites.Add(siteData[0], (SafeNode)new JavaScriptSerializer().DeserializeObject(siteData[1]));
            }
        }
        internal void Save()
        {
            foreach(DictionaryEntry DE in SecuSites)
            {
                
            }
        }
    }
}
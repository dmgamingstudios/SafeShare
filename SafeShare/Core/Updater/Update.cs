using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuhrerShare.Core.Updater
{
    internal class Update
    {
        internal void Updater()
        {
            if(GitVersion() <= CurrentVersion())
                return;
            string updateloc = "";
            using (WebClient _client = new WebClient())
                updateloc = _client.DownloadString("https://raw.githubusercontent.com/dmgamingstudios/SafeShare/master/updateurl.md");
            if (MessageBox.Show("Update available, update now?", "Update available", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                UpdateForm UF = new UpdateForm(updateloc);
                UF.ShowDialog();
            }
            else
                MessageBox.Show("WARNING, not updating might result in a kick from the network, or not being able to communicate with updated users, update as soon as possible");
        }
        private Version GitVersion()
        {
            string v = "";
            using (WebClient client = new WebClient())
                v = client.DownloadString("https://raw.githubusercontent.com/dmgamingstudios/SafeShare/master/update.md");
            return new Version(v);
        }
        private Version CurrentVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return new Version(fvi.FileVersion);
        }
    }
}
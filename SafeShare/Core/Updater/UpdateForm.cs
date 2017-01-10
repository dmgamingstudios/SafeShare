using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuhrerShare.Core.Updater
{
    public partial class UpdateForm : Form
    {
        string loc;
        string uloc = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SafeShare");
        public UpdateForm(string loc)
        {
            InitializeComponent();
            this.loc = loc;
        }

        private void UpdateForm_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(uloc))
                Directory.CreateDirectory(uloc);
        }
        private void DownloadUpdate()
        {
            using (WebClient Downloader = new WebClient())
            {
                Downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
                Downloader.DownloadFileCompleted += Downloader_DownloadFileCompleted;
                Downloader.DownloadFileAsync(new Uri(loc), Path.Combine(uloc, "update.zip"));
            }
        }

        private void Downloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show("Great, the update was downloaded :D we will now open the directory where the update files are located, please move them to the directory you installed SafeShare(sorry no autoinstaller yet)");
            Process.Start("explorer.exe", uloc);
        }

        private void Downloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            label1.Text = "Downloaded " + e.BytesReceived + " of " + e.TotalBytesToReceive;
            progressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());
        }
    }
}

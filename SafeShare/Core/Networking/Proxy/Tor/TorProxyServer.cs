using Knapcode.TorSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuhrerShare.Core.Networking.Proxy.Tor
{
    public class TorProxyServer
    {
        TorSharpProxy proxy;
        public async void StartProxyAsync()
        {
            var settings = new TorSharpSettings
            {
                ZippedToolsDirectory = Path.Combine(Path.GetTempPath(), "TorZipped"),
                ExtractedToolsDirectory = Path.Combine(Path.GetTempPath(), "TorExtracted"),
                PrivoxyPort = 1337,
                TorSocksPort = 1338,
                TorControlPort = 1339,
                TorControlPassword = "foobar",
                TorrcLoc = Application.StartupPath + "\\torrc"
            };
            await new TorSharpToolFetcher(settings, new HttpClient()).FetchAsync();
            proxy = new TorSharpProxy(settings);
            var handler = new HttpClientHandler
            {
                Proxy = new WebProxy(new Uri("http://localhost:" + settings.PrivoxyPort))
            };
            var httpClient = new HttpClient(handler);
            await proxy.ConfigureAndStartAsync();
        }
        public void StopProxy()
        {
            proxy.Stop();
        }
    }
}
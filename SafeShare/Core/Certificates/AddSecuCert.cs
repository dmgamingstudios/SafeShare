using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Certificates
{
    internal static class AddSecuCert
    {
        internal static void Add()
        {
            byte[] _data;
            Assembly _assembly = Assembly.GetExecutingAssembly();
            using (MemoryStream _mem = new MemoryStream())
            {
                _assembly.GetManifestResourceStream("SafeShare.Core.Certificates.secunetCA.pem").CopyTo(_mem);
                _data = _mem.ToArray();
            }
            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindByThumbprint, "‎36 9f ad 5e 20 50 8d 72 86 4b a0 dc 4e 92 b1 68 1b 21 11 63", true);
            if (certs.Count > 0)
            {
                return;
            }
            store.Add(new X509Certificate2(_data));
            store.Close();
        }
    }
}
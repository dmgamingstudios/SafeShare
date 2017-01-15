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
    public static class AddSecuCert
    {
        public static void Add()
        {
            byte[] _data = Encoding.ASCII.GetBytes("-----BEGIN CERTIFICATE-----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-----END CERTIFICATE-----");
            X509Store store = null;
            try { store = new X509Store(StoreName.Root, StoreLocation.LocalMachine); store.Open(OpenFlags.ReadWrite); } catch (Exception) { store = new X509Store(StoreName.Root, StoreLocation.CurrentUser); store.Open(OpenFlags.ReadWrite); }
            X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindByThumbprint, "‎369fad5e20508d72864ba0dc4e92b1681b211163", true);
            if(certs.Count > 0)
            {
                return;
            }
            store.Add(new X509Certificate2(_data));
            store.Close();
        }
    }
}
using FuhrerShare.Core.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Security
{
    internal class SignatureVerifier
    {
        internal bool Verify(string msg, byte[] signature, SafeNode node)
        {
            X509Certificate2 cert = node.identity.PubKey;
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
            SHA512Managed sha512 = new SHA512Managed();
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] data = encoding.GetBytes(msg);
            byte[] hash = sha512.ComputeHash(data);
            return csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA512"), signature);
        }
    }
}
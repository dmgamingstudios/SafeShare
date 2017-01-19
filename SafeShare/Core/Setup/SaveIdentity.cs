using FuhrerShare.Core.Nodes;
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

namespace FuhrerShare.Core.Setup
{
    public class SaveIdentity
    {
        string SaveDirectory = Path.Combine(Application.StartupPath, "identities");
        public SaveIdentity(LocalSafeNode localnode, string pass)
        {
            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);
            string file = SaveDirectory + "\\" + localnode.identity.name + ".crid";
            try
            {
                using (MemoryStream mstream = new MemoryStream())
                {
                    Stream stream = File.Open(file, FileMode.Create);
                    BinaryFormatter bformatter = new BinaryFormatter();
                    bformatter.Serialize(mstream, localnode);
                    byte[] edata = EncryptedNode(mstream.ToArray(), pass);
                    stream.Write(edata, 0, edata.Length);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {}
        }
        private byte[] EncryptedNode(byte[] Mem, string pass)
        {
            try
            {
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(pass);
                MemoryStream fsCrypt = new MemoryStream();
                RijndaelManaged RMCrypto = new RijndaelManaged();
                CryptoStream cs = new CryptoStream(fsCrypt, RMCrypto.CreateEncryptor(key, key), CryptoStreamMode.Write);
                Stream fsIn = new MemoryStream(Mem);
                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);
                return fsCrypt.ToArray();
                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch
            {
                MessageBox.Show("Encryption failed!", "Error");
                return new byte[0];
            }
        }
    }
}
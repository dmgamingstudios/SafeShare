using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FuhrerShare.Core.Setup
{
    public class Hash
    {
        public string Create_Hash_For_Identity(string name, string privkey)
        {
            string result = null;
            using (SHA512Managed sha512 = new SHA512Managed())
            {
                result = Convert.ToBase64String(sha512.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(name, ComputerID(), privkey))));
            }
            return result;
        }
        private Random random = new Random((int)DateTime.Now.Ticks);
        private string RandomString(int Size, string input)
        {
            var chars = Enumerable.Range(0, Size).Select(x => input[random.Next(0, input.Length)]);
            return new string(chars.ToArray());
        }
        private string ComputerID()
        {
            return HardwareInfo.GenerateUID("SafeShare");
        }
    }
    class HardwareInfo
    {
        private static string GetDiskVolumeSerialNumber()
        {
            try
            {
                ManagementObject _disk = new ManagementObject(@"Win32_LogicalDisk.deviceid=""c:""");
                _disk.Get();
                return _disk["VolumeSerialNumber"].ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
        private static string GetProcessorId()
        {
            try
            {
                ManagementObjectSearcher _mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
                ManagementObjectCollection _mbsList = _mbs.Get();
                string _id = string.Empty;
                foreach (ManagementObject _mo in _mbsList)
                {
                    _id = _mo["ProcessorId"].ToString();
                    break;
                }
                return _id;
            }
            catch
            {
                return string.Empty;
            }
        }
        private static string GetMotherboardID()
        {
            try
            {
                ManagementObjectSearcher _mbs = new ManagementObjectSearcher("Select SerialNumber From Win32_BaseBoard");
                ManagementObjectCollection _mbsList = _mbs.Get();
                string _id = string.Empty;
                foreach (ManagementObject _mo in _mbsList)
                {
                    _id = _mo["SerialNumber"].ToString();
                    break;
                }
                return _id;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static IEnumerable<string> SplitInParts(string input, int partLength)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", "partLength");

            for (int i = 0; i < input.Length; i += partLength)
                yield return input.Substring(i, Math.Min(partLength, input.Length - i));
        }
        public static string GenerateUID(string appName)
        {
            string _id = string.Concat(appName, GetProcessorId(), GetMotherboardID(), GetDiskVolumeSerialNumber());
            byte[] _byteIds = Encoding.UTF8.GetBytes(_id);
            MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();
            byte[] _checksum = _md5.ComputeHash(_byteIds);
            string _part1Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 0));
            string _part2Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 4));
            string _part3Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 8));
            string _part4Id = BASE36.Encode(BitConverter.ToUInt32(_checksum, 12));
            return string.Format("{0}-{1}-{2}-{3}", _part1Id, _part2Id, _part3Id, _part4Id);
        }

        public static byte[] GetUIDInBytes(string UID)
        {
            string[] _ids = UID.Split('-');
            if (_ids.Length != 4) throw new ArgumentException("Wrong UID");
            byte[] _value = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(BASE36.Decode(_ids[0])), 0, _value, 0, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(BASE36.Decode(_ids[1])), 0, _value, 8, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(BASE36.Decode(_ids[2])), 0, _value, 16, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(BASE36.Decode(_ids[3])), 0, _value, 24, 8);
            return _value;
        }

        public static bool ValidateUIDFormat(string UID)
        {
            if (!string.IsNullOrWhiteSpace(UID))
            {
                string[] _ids = UID.Split('-');
                return (_ids.Length == 4);
            }
            else
            {
                return false;
            }
        }
    }
    class BASE36
    {
        private const string _charList = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly char[] _charArray = _charList.ToCharArray();
        public static long Decode(string input)
        {
            long _result = 0;
            double _pow = 0;
            for (int _i = input.Length - 1; _i >= 0; _i--)
            {
                char _c = input[_i];
                int pos = _charList.IndexOf(_c);
                if (pos > -1)
                    _result += pos * (long)Math.Pow(_charList.Length, _pow);
                else
                    return -1;
                _pow++;
            }
            return _result;
        }

        public static string Encode(ulong input)
        {
            StringBuilder _sb = new StringBuilder();
            do
            {
                _sb.Append(_charArray[input % (ulong)_charList.Length]);
                input /= (ulong)_charList.Length;
            } while (input != 0);
            return Reverse(_sb.ToString());
        }

        private static string Reverse(string s)
        {
            var charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
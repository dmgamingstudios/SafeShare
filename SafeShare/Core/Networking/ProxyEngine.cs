using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Net.Security;
using System.Security.Authentication;

namespace FuhrerShare.Core.Networking
{
    internal class ProxyEngine
    {
    public enum ProxyType
    {
        None,
        Http,
        Socks4,
        Socks4a,
        Socks5
    }
    public class ProxyClientFactory
    {
        public IProxyClient CreateProxyClient(ProxyType type)
        {
            if (type == ProxyType.None)
                throw new ArgumentOutOfRangeException("type");

            switch (type)
            {
                case ProxyType.Http:
                        return new HttpProxyClient();
                case ProxyType.Socks4:
                    throw new Exception("Not yet made");
                case ProxyType.Socks4a:
                    throw new Exception("Not yet made");
                case ProxyType.Socks5:
                    return new Socks5ProxyClient();
                default:
                    throw new ProxyException(String.Format("Unknown proxy type {0}.", type.ToString()));
            }
        }
        public IProxyClient CreateProxyClient(ProxyType type, TcpClient tcpClient)
        {
            if (type == ProxyType.None)
                throw new ArgumentOutOfRangeException("type");

            switch (type)
            {
                case ProxyType.Http:
                    return new HttpProxyClient(tcpClient);
                case ProxyType.Socks4:
                    throw new Exception("Not yet made");
                case ProxyType.Socks4a:
                    throw new Exception("Not yet made");
                case ProxyType.Socks5:
                    return new Socks5ProxyClient(tcpClient);
                default:
                    throw new ProxyException(String.Format("Unknown proxy type {0}.", type.ToString()));
            }
        }
        public IProxyClient CreateProxyClient(ProxyType type, string proxyHost, int proxyPort)
        {
            if (type == ProxyType.None)
                throw new ArgumentOutOfRangeException("type");

            switch (type)
            {
                case ProxyType.Http:
                        return new HttpProxyClient(proxyHost, proxyPort);
                    case ProxyType.Socks4:
                    throw new Exception("Not yet made");
                case ProxyType.Socks4a:
                    throw new Exception("Not yet made");
                case ProxyType.Socks5:
                    return new Socks5ProxyClient(proxyHost, proxyPort);
                default:
                    throw new ProxyException(String.Format("Unknown proxy type {0}.", type.ToString()));
            }
        }
        public IProxyClient CreateProxyClient(ProxyType type, string proxyHost, int proxyPort, string proxyUsername, string proxyPassword)
        {
            if (type == ProxyType.None)
                throw new ArgumentOutOfRangeException("type");
            switch (type)
            {
                case ProxyType.Http:
                        return new HttpProxyClient(proxyHost, proxyPort);
                    case ProxyType.Socks4:
                    throw new Exception("Not yet made");
                case ProxyType.Socks4a:
                    throw new Exception("Not yet made");
                case ProxyType.Socks5:
                    return new Socks5ProxyClient(proxyHost, proxyPort, proxyUsername, proxyPassword);
                default:
                    throw new ProxyException(String.Format("Unknown proxy type {0}.", type.ToString()));
            }
        }
        public IProxyClient CreateProxyClient(ProxyType type, TcpClient tcpClient, string proxyHost, int proxyPort, string proxyUsername, string proxyPassword)
        {
            IProxyClient c = CreateProxyClient(type, proxyHost, proxyPort, proxyUsername, proxyPassword);
            c.TcpClient = tcpClient;
            return c;
        }
    }
    public interface IProxyClient
    {
        event EventHandler<CreateConnectionAsyncCompletedEventArgs> CreateConnectionAsyncCompleted;
        string ProxyHost { get; set; }
        int ProxyPort { get; set; }
        string ProxyName { get; }
        TcpClient TcpClient { get; set; }
        TcpClient CreateConnection(string destinationHost, int destinationPort);
        void CreateConnectionAsync(string destinationHost, int destinationPort);
    }
    public class Socks5ProxyClient : IProxyClient
    {
        private string _proxyHost;
        private int _proxyPort;
        private string _proxyUserName;
        private string _proxyPassword;
        private SocksAuthentication _proxyAuthMethod;
        private TcpClient _tcpClient;
        private TcpClient _tcpClientCached;
        private const string PROXY_NAME = "SOCKS5";
        private const int SOCKS5_DEFAULT_PORT = 1080;
        private const byte SOCKS5_VERSION_NUMBER = 5;
        private const byte SOCKS5_RESERVED = 0x00;
        private const byte SOCKS5_AUTH_NUMBER_OF_AUTH_METHODS_SUPPORTED = 2;
        private const byte SOCKS5_AUTH_METHOD_NO_AUTHENTICATION_REQUIRED = 0x00;
        private const byte SOCKS5_AUTH_METHOD_GSSAPI = 0x01;
        private const byte SOCKS5_AUTH_METHOD_USERNAME_PASSWORD = 0x02;
        private const byte SOCKS5_AUTH_METHOD_IANA_ASSIGNED_RANGE_BEGIN = 0x03;
        private const byte SOCKS5_AUTH_METHOD_IANA_ASSIGNED_RANGE_END = 0x7f;
        private const byte SOCKS5_AUTH_METHOD_RESERVED_RANGE_BEGIN = 0x80;
        private const byte SOCKS5_AUTH_METHOD_RESERVED_RANGE_END = 0xfe;
        private const byte SOCKS5_AUTH_METHOD_REPLY_NO_ACCEPTABLE_METHODS = 0xff;
        private const byte SOCKS5_CMD_CONNECT = 0x01;
        private const byte SOCKS5_CMD_BIND = 0x02;
        private const byte SOCKS5_CMD_UDP_ASSOCIATE = 0x03;
        private const byte SOCKS5_CMD_REPLY_SUCCEEDED = 0x00;
        private const byte SOCKS5_CMD_REPLY_GENERAL_SOCKS_SERVER_FAILURE = 0x01;
        private const byte SOCKS5_CMD_REPLY_CONNECTION_NOT_ALLOWED_BY_RULESET = 0x02;
        private const byte SOCKS5_CMD_REPLY_NETWORK_UNREACHABLE = 0x03;
        private const byte SOCKS5_CMD_REPLY_HOST_UNREACHABLE = 0x04;
        private const byte SOCKS5_CMD_REPLY_CONNECTION_REFUSED = 0x05;
        private const byte SOCKS5_CMD_REPLY_TTL_EXPIRED = 0x06;
        private const byte SOCKS5_CMD_REPLY_COMMAND_NOT_SUPPORTED = 0x07;
        private const byte SOCKS5_CMD_REPLY_ADDRESS_TYPE_NOT_SUPPORTED = 0x08;
        private const byte SOCKS5_ADDRTYPE_IPV4 = 0x01;
        private const byte SOCKS5_ADDRTYPE_DOMAIN_NAME = 0x03;
        private const byte SOCKS5_ADDRTYPE_IPV6 = 0x04;
        private enum SocksAuthentication
        {
            None,
            UsernamePassword
        }
        public Socks5ProxyClient() { }
        public Socks5ProxyClient(TcpClient tcpClient)
        {
            if (tcpClient == null)
                throw new ArgumentNullException("tcpClient");

            _tcpClientCached = tcpClient;
        }
        public Socks5ProxyClient(string proxyHost)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");

            _proxyHost = proxyHost;
            _proxyPort = SOCKS5_DEFAULT_PORT;
        }
        public Socks5ProxyClient(string proxyHost, int proxyPort)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");
            if (proxyPort <= 0 || proxyPort > 65535)
                throw new ArgumentOutOfRangeException("proxyPort", "port must be greater than zero and less than 65535");
            _proxyHost = proxyHost;
            _proxyPort = proxyPort;
        }
        public Socks5ProxyClient(string proxyHost, string proxyUserName, string proxyPassword)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");
            if (proxyUserName == null)
                throw new ArgumentNullException("proxyUserName");
            if (proxyPassword == null)
                throw new ArgumentNullException("proxyPassword");
            _proxyHost = proxyHost;
            _proxyPort = SOCKS5_DEFAULT_PORT;
            _proxyUserName = proxyUserName;
            _proxyPassword = proxyPassword;
        }
        public Socks5ProxyClient(string proxyHost, int proxyPort, string proxyUserName, string proxyPassword)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");

            if (proxyPort <= 0 || proxyPort > 65535)
                throw new ArgumentOutOfRangeException("proxyPort", "port must be greater than zero and less than 65535");

            if (proxyUserName == null)
                throw new ArgumentNullException("proxyUserName");

            if (proxyPassword == null)
                throw new ArgumentNullException("proxyPassword");

            _proxyHost = proxyHost;
            _proxyPort = proxyPort;
            _proxyUserName = proxyUserName;
            _proxyPassword = proxyPassword;
        }
        public string ProxyHost
        {
            get { return _proxyHost; }
            set { _proxyHost = value; }
        }
        public int ProxyPort
        {
            get { return _proxyPort; }
            set { _proxyPort = value; }
        }
        public string ProxyName
        {
            get { return PROXY_NAME; }
        }
        public string ProxyUserName
        {
            get { return _proxyUserName; }
            set { _proxyUserName = value; }
        }
        public string ProxyPassword
        {
            get { return _proxyPassword; }
            set { _proxyPassword = value; }
        }
        public TcpClient TcpClient
        {
            get { return _tcpClientCached; }
            set { _tcpClientCached = value; }
        }
        public TcpClient CreateConnection(string destinationHost, int destinationPort)
        {
            if (String.IsNullOrEmpty(destinationHost))
                throw new ArgumentNullException("destinationHost");
            if (destinationPort <= 0 || destinationPort > 65535)
                throw new ArgumentOutOfRangeException("destinationPort", "port must be greater than zero and less than 65535");
            try
            {
                if (_tcpClientCached == null)
                {
                    if (String.IsNullOrEmpty(_proxyHost))
                        throw new ProxyException("ProxyHost property must contain a value.");
                    if (_proxyPort <= 0 || _proxyPort > 65535)
                        throw new ProxyException("ProxyPort value must be greater than zero and less than 65535");
                    _tcpClient = new TcpClient();
                    _tcpClient.Connect(_proxyHost, _proxyPort);
                }
                else
                {
                    _tcpClient = _tcpClientCached;
                }
                DetermineClientAuthMethod();
                NegotiateServerAuthMethod();
                SendCommand(SOCKS5_CMD_CONNECT, destinationHost, destinationPort);
                TcpClient rtn = _tcpClient;
                _tcpClient = null;
                return rtn;
            }
            catch (Exception ex)
            {
                throw new ProxyException(String.Format(CultureInfo.InvariantCulture, "Connection to proxy host {0} on port {1} failed.", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient)), ex);
            }
        }
        private void DetermineClientAuthMethod()
        {
            if (_proxyUserName != null && _proxyPassword != null)
                _proxyAuthMethod = SocksAuthentication.UsernamePassword;
            else
                _proxyAuthMethod = SocksAuthentication.None;
        }
        private void NegotiateServerAuthMethod()
        {
            NetworkStream stream = _tcpClient.GetStream();
            byte[] authRequest = new byte[4];
            authRequest[0] = SOCKS5_VERSION_NUMBER;
            authRequest[1] = SOCKS5_AUTH_NUMBER_OF_AUTH_METHODS_SUPPORTED;
            authRequest[2] = SOCKS5_AUTH_METHOD_NO_AUTHENTICATION_REQUIRED;
            authRequest[3] = SOCKS5_AUTH_METHOD_USERNAME_PASSWORD;
            stream.Write(authRequest, 0, authRequest.Length);
            byte[] response = new byte[2];
            stream.Read(response, 0, response.Length);
            byte acceptedAuthMethod = response[1];
            if (acceptedAuthMethod == SOCKS5_AUTH_METHOD_REPLY_NO_ACCEPTABLE_METHODS)
            {
                _tcpClient.Close();
                throw new ProxyException("The proxy destination does not accept the supported proxy client authentication methods.");
            }
            if (acceptedAuthMethod == SOCKS5_AUTH_METHOD_USERNAME_PASSWORD && _proxyAuthMethod == SocksAuthentication.None)
            {
                _tcpClient.Close();
                throw new ProxyException("The proxy destination requires a username and password for authentication.  If you received this error attempting to connect to the Tor network provide an string empty value for ProxyUserName and ProxyPassword.");
            }
            if (acceptedAuthMethod == SOCKS5_AUTH_METHOD_USERNAME_PASSWORD)
            {
                byte[] credentials = new byte[_proxyUserName.Length + _proxyPassword.Length + 3];
                credentials[0] = 0x01;
                credentials[1] = (byte)_proxyUserName.Length;
                Array.Copy(ASCIIEncoding.ASCII.GetBytes(_proxyUserName), 0, credentials, 2, _proxyUserName.Length);
                credentials[_proxyUserName.Length + 2] = (byte)_proxyPassword.Length;
                Array.Copy(ASCIIEncoding.ASCII.GetBytes(_proxyPassword), 0, credentials, _proxyUserName.Length + 3, _proxyPassword.Length);
                stream.Write(credentials, 0, credentials.Length);
                byte[] crResponse = new byte[2];
                stream.Read(crResponse, 0, crResponse.Length);
                if (crResponse[1] != 0)
                {
                    _tcpClient.Close();
                    throw new ProxyException("Proxy authentification failure!  The proxy server has reported that the userid and/or password is not valid.");
                }
            }
        }
        private byte GetDestAddressType(string host)
        {
            IPAddress ipAddr = null;
            bool result = IPAddress.TryParse(host, out ipAddr);
            if (!result)
                return SOCKS5_ADDRTYPE_DOMAIN_NAME;
            switch (ipAddr.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    return SOCKS5_ADDRTYPE_IPV4;
                case AddressFamily.InterNetworkV6:
                    return SOCKS5_ADDRTYPE_IPV6;
                default:
                    throw new ProxyException(String.Format(CultureInfo.InvariantCulture, "The host addess {0} of type '{1}' is not a supported address type.  The supported types are InterNetwork and InterNetworkV6.", host, Enum.GetName(typeof(AddressFamily), ipAddr.AddressFamily)));
            }
        }
        private byte[] GetDestAddressBytes(byte addressType, string host)
        {
            switch (addressType)
            {
                case SOCKS5_ADDRTYPE_IPV4:
                case SOCKS5_ADDRTYPE_IPV6:
                    return IPAddress.Parse(host).GetAddressBytes();
                case SOCKS5_ADDRTYPE_DOMAIN_NAME:
                    //  create a byte array to hold the host name bytes plus one byte to store the length
                    byte[] bytes = new byte[host.Length + 1];
                    //  if the address field contains a fully-qualified domain name.  The first
                    //  octet of the address field contains the number of octets of name that
                    //  follow, there is no terminating NUL octet.
                    bytes[0] = Convert.ToByte(host.Length);
                    Encoding.ASCII.GetBytes(host).CopyTo(bytes, 1);
                    return bytes;
                default:
                    return null;
            }
        }
        private byte[] GetDestPortBytes(int value)
        {
            byte[] array = new byte[2];
            array[0] = Convert.ToByte(value / 256);
            array[1] = Convert.ToByte(value % 256);
            return array;
        }
        private void SendCommand(byte command, string destinationHost, int destinationPort)
        {
            NetworkStream stream = _tcpClient.GetStream();
            byte addressType = GetDestAddressType(destinationHost);
            byte[] destAddr = GetDestAddressBytes(addressType, destinationHost);
            byte[] destPort = GetDestPortBytes(destinationPort);
            byte[] request = new byte[4 + destAddr.Length + 2];
            request[0] = SOCKS5_VERSION_NUMBER;
            request[1] = command;
            request[2] = SOCKS5_RESERVED;
            request[3] = addressType;
            destAddr.CopyTo(request, 4);
            destPort.CopyTo(request, 4 + destAddr.Length);
            stream.Write(request, 0, request.Length);
            byte[] response = new byte[255];
            stream.Read(response, 0, response.Length);
            byte replyCode = response[1];
            if (replyCode != SOCKS5_CMD_REPLY_SUCCEEDED)
                HandleProxyCommandError(response, destinationHost, destinationPort);
        }
        private void HandleProxyCommandError(byte[] response, string destinationHost, int destinationPort)
        {
            string proxyErrorText;
            byte replyCode = response[1];
            byte addrType = response[3];
            string addr = "";
            Int16 port = 0;
            switch (addrType)
            {
                case SOCKS5_ADDRTYPE_DOMAIN_NAME:
                    int addrLen = Convert.ToInt32(response[4]);
                    byte[] addrBytes = new byte[addrLen];
                    for (int i = 0; i < addrLen; i++)
                        addrBytes[i] = response[i + 5];
                    addr = System.Text.ASCIIEncoding.ASCII.GetString(addrBytes);
                    byte[] portBytesDomain = new byte[2];
                    portBytesDomain[0] = response[6 + addrLen];
                    portBytesDomain[1] = response[5 + addrLen];
                    port = BitConverter.ToInt16(portBytesDomain, 0);
                    break;
                case SOCKS5_ADDRTYPE_IPV4:
                    byte[] ipv4Bytes = new byte[4];
                    for (int i = 0; i < 4; i++)
                        ipv4Bytes[i] = response[i + 4];
                    IPAddress ipv4 = new IPAddress(ipv4Bytes);
                    addr = ipv4.ToString();
                    byte[] portBytesIpv4 = new byte[2];
                    portBytesIpv4[0] = response[9];
                    portBytesIpv4[1] = response[8];
                    port = BitConverter.ToInt16(portBytesIpv4, 0);
                    break;
                case SOCKS5_ADDRTYPE_IPV6:
                    byte[] ipv6Bytes = new byte[16];
                    for (int i = 0; i < 16; i++)
                        ipv6Bytes[i] = response[i + 4];
                    IPAddress ipv6 = new IPAddress(ipv6Bytes);
                    addr = ipv6.ToString();
                    byte[] portBytesIpv6 = new byte[2];
                    portBytesIpv6[0] = response[21];
                    portBytesIpv6[1] = response[20];
                    port = BitConverter.ToInt16(portBytesIpv6, 0);
                    break;
            }
            switch (replyCode)
            {
                case SOCKS5_CMD_REPLY_GENERAL_SOCKS_SERVER_FAILURE:
                    proxyErrorText = "a general socks destination failure occurred";
                    break;
                case SOCKS5_CMD_REPLY_CONNECTION_NOT_ALLOWED_BY_RULESET:
                    proxyErrorText = "the connection is not allowed by proxy destination rule set";
                    break;
                case SOCKS5_CMD_REPLY_NETWORK_UNREACHABLE:
                    proxyErrorText = "the network was unreachable";
                    break;
                case SOCKS5_CMD_REPLY_HOST_UNREACHABLE:
                    proxyErrorText = "the host was unreachable";
                    break;
                case SOCKS5_CMD_REPLY_CONNECTION_REFUSED:
                    proxyErrorText = "the connection was refused by the remote network";
                    break;
                case SOCKS5_CMD_REPLY_TTL_EXPIRED:
                    proxyErrorText = "the time to live (TTL) has expired";
                    break;
                case SOCKS5_CMD_REPLY_COMMAND_NOT_SUPPORTED:
                    proxyErrorText = "the command issued by the proxy client is not supported by the proxy destination";
                    break;
                case SOCKS5_CMD_REPLY_ADDRESS_TYPE_NOT_SUPPORTED:
                    proxyErrorText = "the address type specified is not supported";
                    break;
                default:
                    proxyErrorText = String.Format(CultureInfo.InvariantCulture, "an unknown SOCKS reply with the code value '{0}' was received", replyCode.ToString(CultureInfo.InvariantCulture));
                    break;
            }
            string responseText = response != null ? ArrayUtils.HexEncode(response) : "";
            string exceptionMsg = String.Format(CultureInfo.InvariantCulture, "proxy error: {0} for destination host {1} port number {2}.  Server response (hex): {3}.", proxyErrorText, destinationHost, destinationPort, responseText);
            throw new ProxyException(exceptionMsg);
        }
        #region "Async Methods"
        private BackgroundWorker _asyncWorker;
        private Exception _asyncException;
        bool _asyncCancelled;
        public bool IsBusy
        {
            get { return _asyncWorker == null ? false : _asyncWorker.IsBusy; }
        }
        public bool IsAsyncCancelled
        {
            get { return _asyncCancelled; }
        }
        public void CancelAsync()
        {
            if (_asyncWorker != null && !_asyncWorker.CancellationPending && _asyncWorker.IsBusy)
            {
                _asyncCancelled = true;
                _asyncWorker.CancelAsync();
            }
        }
        private void CreateAsyncWorker()
        {
            if (_asyncWorker != null)
                _asyncWorker.Dispose();
            _asyncException = null;
            _asyncWorker = null;
            _asyncCancelled = false;
            _asyncWorker = new BackgroundWorker();
        }
        public event EventHandler<CreateConnectionAsyncCompletedEventArgs> CreateConnectionAsyncCompleted;
        public void CreateConnectionAsync(string destinationHost, int destinationPort)
        {
            if (_asyncWorker != null && _asyncWorker.IsBusy)
                throw new InvalidOperationException("The Socks4 object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");
            CreateAsyncWorker();
            _asyncWorker.WorkerSupportsCancellation = true;
            _asyncWorker.DoWork += new DoWorkEventHandler(CreateConnectionAsync_DoWork);
            _asyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CreateConnectionAsync_RunWorkerCompleted);
            Object[] args = new Object[2];
            args[0] = destinationHost;
            args[1] = destinationPort;
            _asyncWorker.RunWorkerAsync(args);
        }
        private void CreateConnectionAsync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Object[] args = (Object[])e.Argument;
                e.Result = CreateConnection((string)args[0], (int)args[1]);
            }
            catch (Exception ex)
            {
                _asyncException = ex;
            }
        }
        private void CreateConnectionAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (CreateConnectionAsyncCompleted != null)
                CreateConnectionAsyncCompleted(this, new CreateConnectionAsyncCompletedEventArgs(_asyncException, _asyncCancelled, (TcpClient)e.Result));
        }
        #endregion
    }
    [Serializable()]
    public class ProxyException : Exception
    {
        public ProxyException()
        {
        }
        public ProxyException(string message)
            : base(message)
        {
        }
        public ProxyException(string message, Exception innerException)
            :
           base(message, innerException)
        {
        }
        protected ProxyException(SerializationInfo info,
           StreamingContext context)
            : base(info, context)
        {
        }
    }
    internal static class Utils
    {
        internal static string GetHost(TcpClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            string host = "";
            try
            {
                host = ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            }
            catch
            { };
            return host;
        }
        internal static string GetPort(TcpClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            string port = "";
            try
            {
                port = ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Port.ToString(CultureInfo.InvariantCulture);
            }
            catch
            { };
            return port;
        }
    }
    public class CreateConnectionAsyncCompletedEventArgs : AsyncCompletedEventArgs
    {
        private TcpClient _proxyConnection;
        public CreateConnectionAsyncCompletedEventArgs(Exception error, bool cancelled, TcpClient proxyConnection)
            : base(error, cancelled, null)
        {
            _proxyConnection = proxyConnection;
        }
        public TcpClient ProxyConnection
        {
            get { return _proxyConnection; }
        }
    }
    public enum ParityOptions
    {
        Odd,
        Even
    };
    public class ArrayUtils
    {
        static public bool Compare(byte[] array1, byte[] array2)
        {
            if (array1 == null)
                throw new ArgumentNullException("array1");
            if (array2 == null)
                throw new ArgumentNullException("array2");
            if (array1.Length != array2.Length)
            {
                return false;
            }
            int len = array1.Length;
            for (int i = 0; i < len; i++)
            {
                if (array1[i] != array2[i])
                    return false;
            }
            return true;
        }
        public static string HexEncode(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            return HexEncode(data, false, data.Length);
        }
        public static string HexEncode(byte data)
        {
            byte[] b = new byte[1];
            b[0] = data;
            return HexEncode(b, false, b.Length);
        }
        public static string HexEncode(byte[] data, bool insertColonDelimiter)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            return HexEncode(data, insertColonDelimiter, data.Length);
        }
        public static string HexEncode(byte[] data, bool insertColonDelimiter, int length)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            StringBuilder buffer = new StringBuilder(length * 2);
            int len = data.Length;
            for (int i = 0; i < len; i++)
            {
                buffer.Append(data[i].ToString("x").PadLeft(2, '0'));
                if (insertColonDelimiter && i < len - 1)
                    buffer.Append(':');
            }
            return buffer.ToString();
        }
        public static byte[] HexDecode(string s)
        {
            return HexDecode(s, 0);
        }
        public static byte[] HexDecode(string s, int paddingBytes)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            if (s.IndexOf(':') > -1)
                s = s.Replace(":", "");
            if ((s.Length % 2) != 0)
            {
                throw new FormatException("parameter 's' must have an even number of hex characters");
            }
            byte[] result = new byte[s.Length / 2 + paddingBytes];
            for (int i = 0; i < result.Length - paddingBytes; i++)
            {
                result[i] = byte.Parse(s.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier);
            }
            return result;
        }
        public static byte[] GetInt24(Int32 value)
        {
            if (value > 16777215)
                throw new ArgumentOutOfRangeException("value", "value can not be greater than 16777215 (max unsigned 24-bit integer)");
            byte[] bytes = BitConverter.GetBytes(value);
            byte[] buffer = new byte[3];
            Array.Copy(bytes, 0, buffer, 0, 3);
            return buffer;
        }
        public static Int32 GetInt32(byte[] int24)
        {
            if (int24 == null)
                throw new ArgumentNullException("int24");
            if (int24.Length != 3)
            {
                throw new ArgumentOutOfRangeException("int24", "byte size must be exactly three");
            }
            ArrayBuilder buffer = new ArrayBuilder(4);
            buffer.Append(int24);
            return BitConverter.ToInt32(buffer.GetBytes(), 0);
        }
        public static byte[] Clone(byte[] array)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            byte[] buffer = new byte[array.Length];
            Array.Copy(array, buffer, buffer.Length);
            return buffer;
        }
        public static byte[] Reverse(byte[] array)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            byte[] buffer = new byte[array.Length];
            Array.Copy(array, buffer, buffer.Length);
            Array.Reverse(buffer);
            return buffer;
        }
        public static int GetLengthSafe(byte[] array)
        {
            if (array == null)
                return 0;
            else
                return array.Length;
        }
        public static byte[] PadArrayPkcs7(byte[] array, int blockSize)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (blockSize <= 0)
                throw new ArgumentOutOfRangeException("blockSize", "must be 1 or greater");
            byte[] buffer = ArrayUtils.Clone(array);
            int rem = array.Length % blockSize;
            int pads = blockSize - rem;
            if (rem == 0)
                return buffer;
            Array.Resize<byte>(ref buffer, buffer.Length + pads);
            for (int i = array.Length; i < buffer.Length; i++)
            {
                buffer[i] = (byte)pads;
            }
            return buffer;
        }
        public static void SetOddParity(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                bytes[i] = (byte)((b & 0xfe) |
                                ((((b >> 1) ^
                                (b >> 2) ^
                                (b >> 3) ^
                                (b >> 4) ^
                                (b >> 5) ^
                                (b >> 6) ^
                                (b >> 7)) ^ 0x01) & 0x01));
            }
        }
        public static byte[] SetParity(byte[] array, ParityOptions parity)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            BitArray bits = new BitArray(array);
            byte[] buffer = Clone(array);
            for (int i = 0; i < array.Length; i++)
            {
                int oneCount = 0;
                for (int j = 0; j < 7; j++)
                {
                    if (bits[i * 8 + j] == true)
                        oneCount++;
                }
                switch (parity)
                {
                    case ParityOptions.Odd:
                        if (oneCount % 2 == 0)
                            buffer[i] |= (1 << 7);
                        else
                            buffer[i] &= unchecked((byte)(~(1 << 7)));
                        break;
                    case ParityOptions.Even:
                        if (oneCount % 2 == 0)
                            buffer[i] &= unchecked((byte)(~(1 << 7)));
                        else
                            buffer[i] |= (1 << 7);
                        break;
                }
            }
            return buffer;
        }
        public static byte[] CreateArray(int length, byte byteValue)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException("length must be greater than 0");

            byte[] buf = new byte[length];
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = byteValue;
            }
            return buf;
        }
        public static byte[] Subarray(byte[] array, int startIndex, int length)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (length > array.Length)
                throw new Exception("length exceeds size of array");
            byte[] buf = new byte[length];
            Array.Copy(array, startIndex, buf, 0, length);
            return buf;
        }
        public static byte[] TrimPadding(byte[] array)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            int j = array.Length;
            for (int i = array.Length - 1; i >= 0; i--)
            {
                if (array[i] != 0)
                    break;
                j--;
            }
            return ArrayUtils.Subarray(array, 0, j);
        }
        public static void Clear(byte[] array)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 0;
            }

            Array.Resize<byte>(ref array, 0);
        }
        public static bool IsHexValue(string data)
        {
            try
            {
                HexDecode(data);
            }
            catch { return false; }
            return true;
        }
        public static void Zero(byte[] array)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 0;
            }
        }
        public static byte[] Xor(byte[] array1, byte[] array2)
        {
            if (array1 == null)
                throw new ArgumentNullException("array1");
            if (array2 == null)
                throw new ArgumentNullException("array2");
            if (array1.Length != array2.Length)
                throw new ArgumentOutOfRangeException("array1 and array2 must have same length");

            int len = array1.Length;
            byte[] xor = new byte[len];
            for (int i = 0; i < len; i++)
            {
                xor[i] = (byte)(Convert.ToInt32(array1[i]) ^ Convert.ToInt32(array2[i]));
            }

            return xor;
        }
        public static byte[] Combine(params byte[][] list)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            int size = 0;
            foreach (byte[] item in list)
            {
                if (item == null)
                    throw new ArgumentNullException("list contains null array");
                size += item.Length;
            }
            ArrayBuilder b = new ArrayBuilder(size);
            foreach (byte[] item in list)
            {
                b.Append(item);
            }
            byte[] rtn = b.GetBytes();
            b.Clear();
            return rtn;
        }
        public static byte[] Encrypt(SymmetricAlgorithm algo, byte[] key, byte[] iv, CipherMode mode, byte[] cleartext)
        {
            if (algo == null)
                throw new ArgumentNullException("algo");
            if (key == null)
                throw new ArgumentNullException("key");
            if (cleartext == null)
                throw new ArgumentNullException("cleartext");
            algo.Mode = mode;
            ICryptoTransform c = algo.CreateEncryptor(key, iv);
            byte[] ciphertext = new byte[cleartext.Length];
            c.TransformBlock(cleartext, 0, cleartext.Length, ciphertext, 0);
            return ciphertext;
        }
        public static byte[] Decrypt(SymmetricAlgorithm algo, byte[] key, byte[] iv, CipherMode mode, byte[] ciphertext)
        {
            if (algo == null)
                throw new ArgumentNullException("algo");
            if (key == null)
                throw new ArgumentNullException("key");
            if (ciphertext == null)
                throw new ArgumentNullException("cleartext");
            algo.Mode = mode;
            ICryptoTransform d = algo.CreateDecryptor(key, iv);
            byte[] cleartext = new byte[ciphertext.Length];
            d.TransformBlock(ciphertext, 0, ciphertext.Length, cleartext, 0);
            return cleartext;
        }
        public static byte[] ZeroPad(byte[] data, int size)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (data.Length > size)
                throw new ArgumentOutOfRangeException("size");
            ArrayBuilder b = new ArrayBuilder(size);
            b.Append(data);
            byte[] pad = b.GetBytes();
            b.Clear();
            return pad;
        }
        public static byte[] PadArrayFF(byte[] data, int size)
        {
            return PadArray(data, size, 0xff);
        }
        public static byte[] PadArray(byte[] data, int size, byte padValue)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (data.Length > size)
                throw new ArgumentOutOfRangeException("size");
            ArrayBuilder b = new ArrayBuilder(size);
            b.Append(data);
            b.Append(ArrayUtils.CreateArray(size - data.Length, padValue));
            byte[] pad = b.GetBytes();
            b.Clear();
            return pad;
        }
        public static byte[] Hash(HashAlgorithm algo, params byte[][] list)
        {
            if (algo == null)
                throw new ArgumentNullException("algo");
            if (list == null)
                throw new ArgumentNullException("list");
            int size = 0;
            foreach (byte[] item in list)
            {
                if (item == null)
                    throw new ArgumentNullException("list contains null array");
                size += item.Length;
            }
            ArrayBuilder b = new ArrayBuilder(size);
            foreach (byte[] item in list)
            {
                b.Append(item);
            }
            byte[] hash = algo.ComputeHash(b.GetBytes());
            b.Clear();
            return hash;
        }
    }
    public class ArrayBuilder
    {
        private byte[] _buffer;
        private long _index;
        public ArrayBuilder(long size)
        {
            _buffer = new byte[size];
        }
        public int Length
        {
            get { return _buffer.Length; }
        }
        public void Append(params byte[] data)
        {
            Append(data, 0);
        }
        public void Append(byte[] data, long startIndex)
        {
            if (_index + data.Length - startIndex > _buffer.Length)
            {
                throw new Exception(String.Format("Data is too large to append.  Current size is {0} bytes.", _buffer.Length.ToString()));
            }
            Array.Copy(data, startIndex, _buffer, _index, data.Length - startIndex);
            _index += data.Length - startIndex;
        }
        public byte[] GetBytes()
        {
            byte[] copy = new byte[_buffer.Length];
            Array.Copy(_buffer, copy, _buffer.Length);
            return copy;
        }
        public void Clear()
        {
            Array.Clear(_buffer, 0, _buffer.Length);
            _index = 0;
        }
        public void Redim(long size)
        {
            _buffer = new byte[size];
            _index = 0;
        }
    }
    public class HttpProxyClient : IProxyClient
    {
        private string _proxyHost;
        private int _proxyPort;
        private HttpResponseCodes _respCode;
        private string _respText;
        private TcpClient _tcpClient;

        private const int HTTP_PROXY_DEFAULT_PORT = 8080;
        private const string HTTP_PROXY_CONNECT_CMD = "CONNECT {0}:{1} HTTP/1.0\r\nHost: {0}:{1}\r\n\r\n";
        private const int WAIT_FOR_DATA_INTERVAL = 50;
        private const int WAIT_FOR_DATA_TIMEOUT = 15000;
        private const string PROXY_NAME = "HTTP";

        private enum HttpResponseCodes
        {
            None = 0,
            Continue = 100,
            SwitchingProtocols = 101,
            OK = 200,
            Created = 201,
            Accepted = 202,
            NonAuthoritiveInformation = 203,
            NoContent = 204,
            ResetContent = 205,
            PartialContent = 206,
            MultipleChoices = 300,
            MovedPermanetly = 301,
            Found = 302,
            SeeOther = 303,
            NotModified = 304,
            UserProxy = 305,
            TemporaryRedirect = 307,
            BadRequest = 400,
            Unauthorized = 401,
            PaymentRequired = 402,
            Forbidden = 403,
            NotFound = 404,
            MethodNotAllowed = 405,
            NotAcceptable = 406,
            ProxyAuthenticantionRequired = 407,
            RequestTimeout = 408,
            Conflict = 409,
            Gone = 410,
            PreconditionFailed = 411,
            RequestEntityTooLarge = 413,
            RequestURITooLong = 414,
            UnsupportedMediaType = 415,
            RequestedRangeNotSatisfied = 416,
            ExpectationFailed = 417,
            InternalServerError = 500,
            NotImplemented = 501,
            BadGateway = 502,
            ServiceUnavailable = 503,
            GatewayTimeout = 504,
            HTTPVersionNotSupported = 505
        }

        public HttpProxyClient() { }

        public HttpProxyClient(TcpClient tcpClient)
        {
            if (tcpClient == null)
                throw new ArgumentNullException("tcpClient");

            _tcpClient = tcpClient;
        }

        public HttpProxyClient(string proxyHost)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");

            _proxyHost = proxyHost;
            _proxyPort = HTTP_PROXY_DEFAULT_PORT;
        }
        public HttpProxyClient(string proxyHost, int proxyPort)
        {
            if (String.IsNullOrEmpty(proxyHost))
                throw new ArgumentNullException("proxyHost");

            if (proxyPort <= 0 || proxyPort > 65535)
                throw new ArgumentOutOfRangeException("proxyPort", "port must be greater than zero and less than 65535");

            _proxyHost = proxyHost;
            _proxyPort = proxyPort;
        }

        public string ProxyHost
        {
            get { return _proxyHost; }
            set { _proxyHost = value; }
        }
        public int ProxyPort
        {
            get { return _proxyPort; }
            set { _proxyPort = value; }
        }

        public string ProxyName
        {
            get { return PROXY_NAME; }
        }

        public TcpClient TcpClient
        {
            get { return _tcpClient; }
            set { _tcpClient = value; }
        }
        public TcpClient CreateConnection(string destinationHost, int destinationPort)
        {
            try
            {
                if (_tcpClient == null)
                {
                    if (String.IsNullOrEmpty(_proxyHost))
                        throw new ProxyException("ProxyHost property must contain a value.");

                    if (_proxyPort <= 0 || _proxyPort > 65535)
                        throw new ProxyException("ProxyPort value must be greater than zero and less than 65535");
                    _tcpClient = new TcpClient();
                    _tcpClient.Connect(_proxyHost, _proxyPort);
                }
                SendConnectionCommand(destinationHost, destinationPort);

                return _tcpClient;
            }
            catch (SocketException ex)
            {
                throw new ProxyException(String.Format(CultureInfo.InvariantCulture, "Connection to proxy host {0} on port {1} failed.", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient)), ex);
            }
        }


        private void SendConnectionCommand(string host, int port)
        {
            NetworkStream stream = _tcpClient.GetStream();
            string connectCmd = String.Format(CultureInfo.InvariantCulture, HTTP_PROXY_CONNECT_CMD, host, port.ToString(CultureInfo.InvariantCulture));
            byte[] request = ASCIIEncoding.ASCII.GetBytes(connectCmd);
            stream.Write(request, 0, request.Length);

            WaitForData(stream);
            byte[] response = new byte[_tcpClient.ReceiveBufferSize];
            StringBuilder sbuilder = new StringBuilder();
            int bytes = 0;
            long total = 0;

            do
            {
                bytes = stream.Read(response, 0, _tcpClient.ReceiveBufferSize);
                total += bytes;
                sbuilder.Append(System.Text.ASCIIEncoding.UTF8.GetString(response, 0, bytes));
            } while (stream.DataAvailable);

            ParseResponse(sbuilder.ToString());

            if (_respCode != HttpResponseCodes.OK)
                HandleProxyCommandError(host, port);
        }

        private void HandleProxyCommandError(string host, int port)
        {
            string msg;

            switch (_respCode)
            {
                case HttpResponseCodes.None:
                    msg = String.Format(CultureInfo.InvariantCulture, "Proxy destination {0} on port {1} failed to return a recognized HTTP response code.  Server response: {2}", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient), _respText);
                    break;

                case HttpResponseCodes.BadGateway:

                    msg = String.Format(CultureInfo.InvariantCulture, "Proxy destination {0} on port {1} responded with a 502 code - Bad Gateway.  If you are connecting to a Microsoft ISA destination please refer to knowledge based article Q283284 for more information.  Server response: {2}", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient), _respText);
                    break;

                default:
                    msg = String.Format(CultureInfo.InvariantCulture, "Proxy destination {0} on port {1} responded with a {2} code - {3}", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient), ((int)_respCode).ToString(CultureInfo.InvariantCulture), _respText);
                    break;
            }

            throw new ProxyException(msg);
        }

        private void WaitForData(NetworkStream stream)
        {
            int sleepTime = 0;
            while (!stream.DataAvailable)
            {
                Thread.Sleep(WAIT_FOR_DATA_INTERVAL);
                sleepTime += WAIT_FOR_DATA_INTERVAL;
                if (sleepTime > WAIT_FOR_DATA_TIMEOUT)
                    throw new ProxyException(String.Format("A timeout while waiting for the proxy server at {0} on port {1} to respond.", Utils.GetHost(_tcpClient), Utils.GetPort(_tcpClient)));
            }
        }

        private void ParseResponse(string response)
        {
            string[] data = null;

            data = response.Replace('\n', ' ').Split('\r');

            ParseCodeAndText(data[0]);
        }

        private void ParseCodeAndText(string line)
        {
            int begin = 0;
            int end = 0;
            string val = null;

            if (line.IndexOf("HTTP") == -1)
                throw new ProxyException(String.Format("No HTTP response received from proxy destination.  Server response: {0}.", line));

            begin = line.IndexOf(" ") + 1;
            end = line.IndexOf(" ", begin);

            val = line.Substring(begin, end - begin);
            Int32 code = 0;

            if (!Int32.TryParse(val, out code))
                throw new ProxyException(String.Format("An invalid response code was received from proxy destination.  Server response: {0}.", line));

            _respCode = (HttpResponseCodes)code;
            _respText = line.Substring(end + 1).Trim();
        }

        private BackgroundWorker _asyncWorker;
        private Exception _asyncException;
        bool _asyncCancelled;
        public bool IsBusy
        {
            get { return _asyncWorker == null ? false : _asyncWorker.IsBusy; }
        }
        public bool IsAsyncCancelled
        {
            get { return _asyncCancelled; }
        }

        public void CancelAsync()
        {
            if (_asyncWorker != null && !_asyncWorker.CancellationPending && _asyncWorker.IsBusy)
            {
                _asyncCancelled = true;
                _asyncWorker.CancelAsync();
            }
        }

        private void CreateAsyncWorker()
        {
            if (_asyncWorker != null)
                _asyncWorker.Dispose();
            _asyncException = null;
            _asyncWorker = null;
            _asyncCancelled = false;
            _asyncWorker = new BackgroundWorker();
        }
        public event EventHandler<CreateConnectionAsyncCompletedEventArgs> CreateConnectionAsyncCompleted;
        public void CreateConnectionAsync(string destinationHost, int destinationPort)
        {
            if (_asyncWorker != null && _asyncWorker.IsBusy)
                throw new InvalidOperationException("The HttpProxy object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

            CreateAsyncWorker();
            _asyncWorker.WorkerSupportsCancellation = true;
            _asyncWorker.DoWork += new DoWorkEventHandler(CreateConnectionAsync_DoWork);
            _asyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CreateConnectionAsync_RunWorkerCompleted);
            Object[] args = new Object[2];
            args[0] = destinationHost;
            args[1] = destinationPort;
            _asyncWorker.RunWorkerAsync(args);
        }

        private void CreateConnectionAsync_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Object[] args = (Object[])e.Argument;
                e.Result = CreateConnection((string)args[0], (int)args[1]);
            }
            catch (Exception ex)
            {
                _asyncException = ex;
            }
        }

        private void CreateConnectionAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (CreateConnectionAsyncCompleted != null)
                CreateConnectionAsyncCompleted(this, new CreateConnectionAsyncCompletedEventArgs(_asyncException, _asyncCancelled, (TcpClient)e.Result));
        }
    }
}
}

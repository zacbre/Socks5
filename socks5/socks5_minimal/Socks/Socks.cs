using socks5.TCP;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace socks5.Socks
{
    class Socks5
    {
        public static List<AuthTypes> RequestAuth(SocksClient client)
        {
            byte[] buff = Receive(client.Client);

            if (buff == null || (HeaderTypes)buff[0] != HeaderTypes.Socks5) return new List<AuthTypes>();

            int methods = Convert.ToInt32(buff[1]);
            List<AuthTypes> types = new List<AuthTypes>();
            for (int i = 2; i < methods + 2; i++)
            {
                switch ((AuthTypes)buff[i])
                {
                    case AuthTypes.Login:
                        types.Add(AuthTypes.Login);
                        break;
                    case AuthTypes.None:
                        types.Add(AuthTypes.None);
                        break;
                }
            }
            return types;
        }

        public static User RequestLogin(SocksClient client)
        {
            //request authentication.
            client.Client.Send(new byte[] { (byte)HeaderTypes.Socks5, (byte)HeaderTypes.Authentication });
            byte[] buff = Receive(client.Client);

            if (buff == null || buff[0] != 0x01) return null;

            int numusername = Convert.ToInt32(buff[1]);
            int numpassword = Convert.ToInt32(buff[(numusername + 2)]);
            string username = Encoding.ASCII.GetString(buff, 2, numusername);
            string password = Encoding.ASCII.GetString(buff, numusername + 3, numpassword);

            return new User(username, password, (IPEndPoint)client.Client.Sock.RemoteEndPoint);
        }

        public static SocksRequest RequestTunnel(SocksClient client)
        {
            byte[] buff = Receive(client.Client);
            if (buff == null || (HeaderTypes)buff[0] != HeaderTypes.Socks5) return null;
            switch ((StreamTypes)buff[1])
            {
                case StreamTypes.Stream:
                    {
                        int fwd = 4;
                        string address = "";
                        switch ((AddressType)buff[3])
                        {
                            case AddressType.IP:
                                {
                                    for (int i = 4; i < 8; i++)
                                    {
                                        //grab IP.
                                        address += Convert.ToInt32(buff[i]).ToString() + (i != 7 ? "." : "");
                                    }
                                    fwd += 4;
                                }
                                break;
                            case AddressType.Domain:
                                {
                                    int domainlen = Convert.ToInt32(buff[4]);
                                    address += Encoding.ASCII.GetString(buff, 5, domainlen);
                                    fwd += domainlen + 1;
                                }
                                break;
                            case AddressType.IPv6:
                                //can't handle IPV6 traffic just yet.
                                return null;
                        }
                        byte[] po = new byte[2];
                        Array.Copy(buff, fwd, po, 0, 2);
                        Int16 x = BitConverter.ToInt16(po, 0);
                        int port = Convert.ToInt32(IPAddress.NetworkToHostOrder(x));
                        port = (port < 1 ? port + 65536 : port);
                        return new SocksRequest(StreamTypes.Stream, (AddressType)buff[3], address, port);
                    }
                default:
                    //not supported.
                    return null;

            }
        }

        public static byte[] Receive(Client client)
        {
            byte[] buffer = new byte[2048];
            int received = client.Receive(buffer, 0, buffer.Length);
            if (received != -1)
            {
                return buffer;
            }
            else
                return null;
        }
    }

    public class SocksRequest
    {
        public AddressType Type { get; set; }
        public StreamTypes StreamType { get; private set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public SocksError Error { get; set; }
        public SocksRequest(StreamTypes type, AddressType addrtype, string address, int port)
        {
            Type = addrtype;
            StreamType = type;
            Address = address;
            Port = port;
        }
        public IPAddress IP
        {
            get
            {
                if (Type == AddressType.IP)
                {
                    try
                    {
                        return IPAddress.Parse(Address);
                    }
                    catch { Error = SocksError.NotSupported; return null; }
                }
                else if (Type == AddressType.Domain)
                {
                    try
                    {
                        foreach (IPAddress p in Dns.GetHostAddresses(Address))
                            if (p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                return p;
                        return null;
                    }
                    catch
                    {
                        Error = SocksError.HostUnreachable;
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        public byte[] GetData()
        {
            byte[] data;
            var port = IPAddress.NetworkToHostOrder(Port);
            if (Type == AddressType.IP)
            {
                data = new byte[10];
                string[] content = IP.ToString().Split('.');
                for (int i = 4; i < content.Length + 4; i++)
                    data[i] = Convert.ToByte(Convert.ToInt32(content[i - 4]));
                Buffer.BlockCopy(BitConverter.GetBytes(port), 0, data, 8, 2);
            }
            else if (Type == AddressType.Domain)
            {
                data = new byte[Address.Length + 7];
                data[4] = Convert.ToByte(Address.Length);
                Buffer.BlockCopy(Encoding.ASCII.GetBytes(Address), 0, data, 5, Address.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(port), 0, data, data.Length - 2, 2);
            }
            else return null;
            data[0] = 0x05;                
            data[1] = (byte)Error;
            data[2] = 0x00;
            data[3] = (byte)Type;
            return data;
        }
    }

    public enum AuthTypes
    {
        Login = 0x02,
        None = 0x00
    }

    public enum HeaderTypes
    {
        Socks5 = 0x05,
        Authentication = 0x02,
        Zero = 0x00
    }

    public enum StreamTypes
    {
        Stream = 0x01,
        Bind = 0x02,
        UDP = 0x03
    }

    public enum AddressType
    {
        IP = 0x01,
        Domain = 0x03,
        IPv6 = 0x04
    }

    public enum SocksError
    {
        Granted = 0x00,
        Failure = 0x01,
        NotAllowed = 0x02,
        Unreachable = 0x03,
        HostUnreachable = 0x04,
        Refused = 0x05,
        Expired = 0x06,
        NotSupported = 0x07,
        AddressNotSupported = 0x08
    }
}

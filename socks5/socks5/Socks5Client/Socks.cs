using socks5.Socks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace socks5.Socks5Client
{
    public class Socks
    {
        public static int Greet(Socks5Client client)
        {
            if (client.reqPass)
                client.Client.Send(new byte[] { 0x05, Convert.ToByte(2), 0x00, 0x02 });
            else
                client.Client.Send(new byte[] { 0x05, Convert.ToByte(1), 0x00});
            byte[] buffer = new byte[512];
            int received = client.Client.Receive(buffer, 0, buffer.Length);
            if(received > 0)
            {
                //check for server version.
                if (buffer[0] == 0x05)
                {
                    switch (buffer[1])
                    {
                        case 0x00:
                            //doesnt require a password.
                            return 1;
                        case 0x02:
                            //requires a password.
                            return 2;
                        case 0xFF:
                            //no supported login methods.
                            break;
                    }
                }
            }
            return 0;
        }

        public static bool SendLogin(Socks5Client cli, string Username, string Password)
        {
            byte[] x = new byte[Username.Length + Password.Length + 3];
            int total = 0;
            x[total++] = 0x01;
            x[total++] = Convert.ToByte(Username.Length);
            Buffer.BlockCopy(Encoding.ASCII.GetBytes(Username), 0, x, 2, Username.Length);
            total += Username.Length;
            x[total++] = Convert.ToByte(Password.Length); 
            Buffer.BlockCopy(Encoding.ASCII.GetBytes(Password), 0, x, total, Password.Length);
            //send request.
            cli.Client.Send(x);
            byte[] buffer = new byte[512];
            cli.Client.Receive(buffer, 0, buffer.Length);
            switch (buffer[1])
            {
                case 0x00:
                    //success.
                    return true;
                default:
                    return false;
            }
        }

        public static socks5.Socks.SocksError SendRequest(Socks5Client cli, string ipOrDomain, int port)
        {
            AddressType type;
            IPAddress ipAddress;
            if (!IPAddress.TryParse(ipOrDomain, out ipAddress))
                //it's a domain. :D (hopefully).
                type = AddressType.Domain;
            else
                type = AddressType.IP;
            SocksRequest sr = new SocksRequest(StreamTypes.Stream, type, ipOrDomain, port);
            //send data.
            byte[] p = sr.GetData(false);
            p[1] = 0x01;
            cli.Client.Send(p);
            byte[] buffer = new byte[512];
            cli.Client.Receive(buffer, 0, buffer.Length);
            return (SocksError)buffer[1];
        }
    }
}
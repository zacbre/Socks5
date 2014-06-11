using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using socks5.TCP;
namespace socks5.Socks
{
    public class SocksClient
    {
        public event EventHandler<SocksClientEventArgs> onClientDisconnected = delegate { };

        public Client Client;
        public bool Authenticated { get; private set; }
        public SocksClient(Client cli)
        {
            Client = cli;
        }
        private SocksRequest req1;
        public SocksRequest Destination { get { return req1; } }
        public void Begin(int PacketSize, int Timeout)
        {
            Client.onClientDisconnected += Client_onClientDisconnected;
            List<AuthTypes> authtypes = Socks5.RequestAuth(this);
            if (authtypes.Count <= 0)
            {
                Client.Send(new byte[] { 0x00, 0xFF });
                Client.Disconnect();
                return;
            }
            //Request Site Data.
            if (!Authenticated)
            {//no username/password required?
                Authenticated = true;
                Client.Send(new byte[] { (byte)HeaderTypes.Socks5, (byte)HeaderTypes.Zero });
            }

            SocksRequest req = Socks5.RequestTunnel(this);
            if (req == null) { Client.Disconnect(); return; }
            SocksTunnel x = new SocksTunnel(this, req, PacketSize, Timeout);
            x.Open();
        }

        void Client_onClientDisconnected(object sender, ClientEventArgs e)
        {
            this.onClientDisconnected(this, new SocksClientEventArgs(this));
        }
    }
    public class User
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public IPEndPoint IP { get; private set; }
        public User(string un, string pw, IPEndPoint ip)
        {
            Username = un;
            Password = pw;
            IP = ip;
        }
    }
}

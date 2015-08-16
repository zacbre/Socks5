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
        public delegate LoginStatus Authenticate(object sender, SocksAuthenticationEventArgs e);
        public event Authenticate OnClientAuthenticating = null;

        public Client Client;
        public bool Authenticated { get; private set; }
        public bool Authentication = false;


        private SocksRequest req1 = null;
        public SocksRequest Destination { get { return req1; } }

        public SocksClient(Client cli)
        {
            Client = cli;
        }

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
            else if(Authentication && this.OnClientAuthenticating != null)
            {
                //request login.
                User user = Socks5.RequestLogin(this);
                if (user == null)
                {
                    Client.Disconnect();
                    return;
                }
                LoginStatus status = this.OnClientAuthenticating(this, new SocksAuthenticationEventArgs(user));
                Client.Send(new byte[] { (byte)HeaderTypes.Socks5, (byte)status });
                if (status == LoginStatus.Denied)
                {
                    Client.Disconnect();
                    return;
                }
                else if (status == LoginStatus.Correct)
                {
                    Authenticated = true;
                }
                //read password and invoke.
                //this.OnClientAuthenticating(this, new SocksAuthenticationEventArgs(..));
            }
            else
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
    public enum LoginStatus
    {
        Denied = 0xFF,
        Correct = 0x00
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

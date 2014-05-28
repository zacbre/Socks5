using System;
using System.Collections.Generic;
using System.Text;
using socks5.Socks5;
using socks5.Plugin;
namespace socks5.TCP
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

        public void Begin(int PacketSize, int Timeout)
        {
            Client.onClientDisconnected += Client_onClientDisconnected;
            List<AuthTypes> authtypes = Socks.RequestAuth(this);
            if (authtypes.Count <= 0)
            {
                Client.Send(new byte[] { 0x00, 0xFF });
                Client.Disconnect();
                return;
            }
            foreach (LoginHandler lh in PluginLoader.LoadPlugin(typeof(LoginHandler)))
            {
                if (lh.LoginRequired)
                {
                    if(!authtypes.Contains(AuthTypes.Login)) //disconnect.
                    {
                        Client.Send(new byte[] { (byte)HeaderTypes.Socks5, 0xFF });
                        Client.Disconnect();
                        return;
                    }
                    //request login.
                    User user = Socks.RequestLogin(this);
                    if (user == null)
                    {
                        Client.Disconnect();
                        return;
                    }
                    LoginStatus status = lh.HandleLogin(user);
                    Client.Send(new byte[] { (byte)HeaderTypes.Socks5, (byte)status });
                    if (status == LoginStatus.Denied)
                    {
                        Client.Disconnect();
                        return;
                    }
                    else if (status == LoginStatus.Correct)
                    {
                        Authenticated = true;
                        break;
                    }
                }
            }
            //Request Site Data.
            if (!Authenticated)
            {//no username/password required?
                Authenticated = true;
                Client.Send(new byte[] { (byte)HeaderTypes.Socks5, (byte)HeaderTypes.Zero });
            }
            SocksRequest req = Socks.RequestTunnel(this);
            if (req == null) { Client.Disconnect(); return; }
            SocksRequest req1 = new SocksRequest(req.StreamType, req.Type, req.Address, req.Port);
            //call on plugins for connect callbacks.
            foreach (ConnectHandler conn in PluginLoader.LoadPlugin(typeof(ConnectHandler)))
            {
                if(conn.Enabled)
                    if (conn.OnConnect(req1) == false)
                    {
                        req.Error = SocksError.Failure;
                        Client.Send(req.GetData());
                        Client.Disconnect();
                        return;
                    }
            }
            //Send Tunnel Data back.
            SocksTunnel x = new SocksTunnel(this, req, req1, PacketSize, Timeout);
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
        public User(string un, string pw)
        {
            Username = un;
            Password = pw;
        }
    }
}

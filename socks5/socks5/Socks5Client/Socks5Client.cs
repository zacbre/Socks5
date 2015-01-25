using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using socks5.TCP;
using System.Net.Sockets;
using socks5.Socks;

namespace socks5.Socks5Client
{
    public class Socks5Client
    {
        private IPAddress ipAddress;
        public Client Client;

        private Socket p;
        private int Port;
        public bool reqPass = false;

        private string Username;
        private string Password;
        private string Dest;
        private int Destport;

        public event EventHandler<Socks5ClientArgs> OnConnected = delegate { };
        public event EventHandler<Socks5ClientDataArgs> OnDataReceived = delegate { };
        public event EventHandler<Socks5ClientDataArgs> OnDataSent = delegate { };
        public event EventHandler<Socks5ClientArgs> OnDisconnected = delegate { };

        public Socks5Client(string ipOrDomain, int port, string dest, int destport, string username = null, string password = null)
        {
            //Parse IP?
            if (!IPAddress.TryParse(ipOrDomain, out ipAddress))
            {
                //not connected.
                try
                {
                    foreach (IPAddress p in Dns.GetHostAddresses(ipOrDomain))
                        if (p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            DoSocks(p, port, dest, destport, username, password);
                            return;
                        }
                }
                catch
                {
                    throw new NullReferenceException();
                }
            }           
            DoSocks(ipAddress, port, dest, destport, username, password);
        }
        public Socks5Client(IPAddress ip, int port, string dest, int destport, string username = null, string password = null)
        {
            DoSocks(ip, port, dest, destport, username, password);
        }

        private void DoSocks(IPAddress ip, int port, string dest, int destport, string username = null, string password = null)
        {
            ipAddress = ip;
            Port = port;
            //check for username & pw.
            if(username != null && password != null)
            {
                Username = username;
                Password = password;
                reqPass = true;
            }
            Dest = dest;
            Destport = destport;
        }
        void Client_onDataReceived(object sender, DataEventArgs e)
        {
            e.Client.ReceiveAsync();
        }

        public void ConnectAsync()
        {
            //
            p = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Client = new Client(p, 2048);
            Client.Sock.BeginConnect(new IPEndPoint(ipAddress, Port), new AsyncCallback(onConnected), Client);
            //return status?
        }

        public bool Connect()
        {
            try
            {
                p = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Client = new Client(p, 2048);
                Client.Sock.Connect(new IPEndPoint(ipAddress, Port));
                //try the greeting.
                //Client.onDataReceived += Client_onDataReceived;
                switch (Socks.Greet(this))
                {
                    case 0:
                        //disconnect.
                        return false;
                    case 1:
                        //no login, continue.
                        break;
                    case 2:
                        //requires login. send login.
                        if (!Socks.SendLogin(this, Username, Password))
                        {
                            return false;
                        }
                        break;
                }
                if (Socks.SendRequest(this, Dest, Destport) == SocksError.Granted)
                    return true;
                else return false;
            }
            catch
            {
                return false;
            }
        }

        private void onConnected(IAsyncResult res)
        {
            Client = (Client)res.AsyncState;
            try
            {
                Client.Sock.EndConnect(res);
            }
            catch
            {
                this.OnConnected(this, new Socks5ClientArgs(null, SocksError.Failure));
                return;
            }
            //Client.onDataReceived += Client_onDataReceived;
            switch (Socks.Greet(this))
            {
                case 0:
                    //disconnect.
                    this.OnConnected(this, new Socks5ClientArgs(null, SocksError.Failure));
                    return;
                case 1:
                    //no login, continue.
                    break;
                case 2:
                    //requires login. send login.
                    if(!Socks.SendLogin(this, Username, Password))
                    {
                        this.OnConnected(this, new Socks5ClientArgs(null, SocksError.LoginRequired));
                        return;
                    }
                    break;
            }
            this.OnConnected(this, new Socks5ClientArgs(Client, Socks.SendRequest(this, Dest, Destport)));
        }

        public bool Connected
        {
            get { return Client.Sock.Connected; }
        }
        //send.

    }
}

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using socks5.Socks;
namespace socks5.TCP
{
    public class ClientEventArgs : EventArgs
    {
        public Client Client { get; private set; }
        public ClientEventArgs(Client client)
        {
            Client = client;
        }
    }
    public class SocksClientEventArgs : EventArgs
    {
        public SocksClient Client { get; private set; }
        public SocksClientEventArgs(SocksClient client)
        {
            Client = client;
        }
    }
}

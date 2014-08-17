using socks5.Socks;
using socks5.TCP;
using System;
using System.Collections.Generic;
using System.Text;

namespace socks5.Socks5Client
{
    public class Socks5ClientArgs : EventArgs
    {
        public Socks5ClientArgs(Client p, SocksError x)
        {
            sock = p;
            status = x;
        }
        private Client sock = null;
        private SocksError status = SocksError.Failure;
        public SocksError Status { get { return status; } }
        public Client Client { get { return sock; } }
    }
}

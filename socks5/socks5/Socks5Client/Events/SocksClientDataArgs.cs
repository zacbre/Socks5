using socks5.TCP;
using System;
using System.Collections.Generic;
using System.Text;

namespace socks5.Socks5Client
{
    public class Socks5ClientDataArgs : EventArgs
    {
        public Socks5ClientDataArgs(Client client, byte[] buff, int count, int offset)
        {
            cli = client;
            Buffer = buff;
            Count = count;
            Offset = offset;
        }
        //Data Buffer
        //Socks Client
        private Client cli = null;
        public Client Client { get { return cli; } }
        public byte[] Buffer { get; set; }
        public int Count { get; set; }
        public int Offset { get; set; }
    }
}

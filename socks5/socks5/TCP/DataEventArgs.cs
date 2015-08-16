using socks5.Socks;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace socks5.TCP
{
    public class DataEventArgs : EventArgs
    {
        public Client Client { get; set; }
        public byte[] Buffer { get; set; }
        public int Count { get; set; }
        public int Offset { get; set; }
        public SocksRequest Request { get; set; }
        public DataEventArgs(Client client, byte[] buffer, int count, SocksRequest req = null)
        {
            Client = client;
            Buffer = buffer;
            Count = count;
            Offset = 0;
            Request = req;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace socks5.TCP
{
    public class Stats
    {
        public Stats()
        {
        }
        public ulong NetworkReceived { get; set; }
        public ulong NetworkSent { get; set; }

        public int TotalClients { get; set; }
        public int ClientsSinceRun { get; set; }

        public ulong PacketsSent { get; set; }
        public ulong PacketsReceived { get; set; }
    }
}

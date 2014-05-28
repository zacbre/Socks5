using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace socks5.TCP
{
    public class Stats
    {
        BandwidthCounter sc;
        BandwidthCounter rc;
        public Stats()
        {
            sc = new BandwidthCounter();
            rc = new BandwidthCounter();
        }
        public void AddClient()
        {
            TotalClients++;
            ClientsSinceRun++;
        }

        public void ResetClients(int count)
        {
            TotalClients = count;
        }

        public void AddBytes(int bytes, ByteType typ)
        {
            if(typ != ByteType.Sent)
            {
                rc.AddBytes((uint)bytes);
                NetworkReceived += (ulong)bytes;
                return;
            }
            sc.AddBytes((uint)bytes);
            NetworkSent += (ulong)bytes;
        }

        public void AddPacket(PacketType pkt)
        {
            if (pkt != PacketType.Sent)
                PacketsReceived++;
            else
                PacketsSent++;
        }

        public int TotalClients { get; private set; }
        public int ClientsSinceRun { get; private set; }

        public ulong NetworkReceived { get; private set; }
        public ulong NetworkSent { get; private set; }

        public ulong PacketsSent { get; private set; }
        public ulong PacketsReceived { get; private set; }
        //per sec.
        public string BytesReceivedPerSec { get { return rc.GetPerSecond(); } }
        public string BytesSentPerSec
        {
            get { return sc.GetPerSecond(); }
        }
    }
    public enum PacketType
    {
        Sent,
        Received
    }
    public enum ByteType
    {
        Sent,
        Received
    }
}

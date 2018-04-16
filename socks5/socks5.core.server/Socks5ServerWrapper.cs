using System;
using System.Net;
using System.Threading;
using socks5.Plugin;
using Socks5Test;

namespace socks5.core.server
{
    public class Socks5ServerWrapper
    {
        private Socks5Server x;

        public void Start(int port, string userName, string password)
        {
            x = new Socks5Server(IPAddress.Any, port);
            Auth.Initialize(userName, password);
            PluginLoader.ChangePluginStatus(true, typeof(Auth));
            x.Start();

            while (true)
            {
                //Console.Clear();
                Console.Write("Total Clients: \t{0}\nTotal Recvd: \t{1:0.00##}MB\nTotal Sent: \t{2:0.00##}MB\n", x.Stats.TotalClients, ((x.Stats.NetworkReceived / 1024f) / 1024f), ((x.Stats.NetworkSent / 1024f) / 1024f));
                Console.Write("Receiving/sec: \t{0}\nSending/sec: \t{1}", x.Stats.SBytesReceivedPerSec, x.Stats.SBytesSentPerSec);
                Thread.Sleep(TimeSpan.FromHours(1));
            }
        }

        public void Stop()
        {
            x.Stop();
        }
    }
}
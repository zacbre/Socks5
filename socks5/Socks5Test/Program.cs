using System;
using System.Collections.Generic;
using System.Text;
using socks5;
using System.Net;
using System.Threading;
using socks5.Plugin;
using socks5.ExamplePlugins;
namespace Socks5Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Socks5Server x = new Socks5Server(IPAddress.Any, 1080);
            x.Start();
            //enable plugin.
            /*foreach (object p in PluginLoader.GetPlugins)
            {
                if (p.GetType() == typeof(LoginHandlerExample))
                {
                    //enable it.
                    PluginLoader.ChangePluginStatus(true, p.GetType());
                    Console.WriteLine("Enabled {0}.", p.GetType().ToString());
                }
            }*/
            //Start showing network stats.
            while (true)
            {
                Console.Clear();
                Console.Write("Total Clients: \t{0}\nTotal Recvd: \t{1:0.00##}MB\nTotal Sent: \t{2:0.00##}MB\n", x.Stats.TotalClients, ((x.Stats.NetworkReceived / 1024f) / 1024f), ((x.Stats.NetworkSent / 1024f) / 1024f));
                Console.Write("Receiving/sec: \t{0}\nSending/sec: \t{1}", x.Stats.BytesReceivedPerSec, x.Stats.BytesSentPerSec);
                Thread.Sleep(1000);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using socks5;
using System.Net;
using System.Threading;
using socks5.Plugin;
using socks5.ExamplePlugins;
using socks5.Socks5Client;
namespace Socks5Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Socks5Server x = new Socks5Server(IPAddress.Any, 1084);
            x.Start();
            PluginLoader.ChangePluginStatus(false, typeof(socks5.ExamplePlugins.DataHandlerExample));
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
            //Socks5Client p = new Socks5Client("142.4.208.185", 3128, "unexposedihope.speedresolve.com", 9000); //"yolo", "swag");
            //p.OnConnected += p_OnConnected;
            //p.ConnectAsync();
            while (true)
            {
                Console.Clear();
                Console.Write("Total Clients: \t{0}\nTotal Recvd: \t{1:0.00##}MB\nTotal Sent: \t{2:0.00##}MB\n", x.Stats.TotalClients, ((x.Stats.NetworkReceived / 1024f) / 1024f), ((x.Stats.NetworkSent / 1024f) / 1024f));
                Console.Write("Receiving/sec: \t{0}\nSending/sec: \t{1}", x.Stats.BytesReceivedPerSec, x.Stats.BytesSentPerSec);
                Thread.Sleep(1000);
            }
        }
        static byte[] m;
        static void p_OnConnected(object sender, Socks5ClientArgs e)
        {
            if (e.Status == socks5.Socks.SocksError.Granted)
            {
                e.Client.OnDataReceived += Client_OnDataReceived;
                m = new byte[10] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 };
                e.Client.Send(m, 0, m.Length);
                e.Client.ReceiveAsync();
            }
            else
            {
                Console.WriteLine("Failed to connect: {0}.", e.Status.ToString());
            }
        }

        static void Client_OnDataReceived(object sender, Socks5ClientDataArgs e)
        {
            Console.WriteLine("Received {0} bytes from server.", e.Count);
            e.Client.Send(m, 0, m.Length);
            e.Client.ReceiveAsync();
        }
    }
}

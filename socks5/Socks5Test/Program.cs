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
            //PluginLoader.ChangePluginStatus(true, typeof(socks5.ExamplePlugins.DataHandlerExample));
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
            //socks5 client.
            /*socks5.Socks5Client.Socks5Client cli = new socks5.Socks5Client.Socks5Client("localhost", 1080, "www.thrdu.de", 80, "thrdev", "testing1234");
            cli.OnConnected += cli_OnConnected;
            cli.Connect();*/
            //Start showing network stats.
            while (true)
            {
                Console.Clear();
                Console.Write("Total Clients: \t{0}\nTotal Recvd: \t{1:0.00##}MB\nTotal Sent: \t{2:0.00##}MB\n", x.Stats.TotalClients, ((x.Stats.NetworkReceived / 1024f) / 1024f), ((x.Stats.NetworkSent / 1024f) / 1024f));
                Console.Write("Receiving/sec: \t{0}\nSending/sec: \t{1}", x.Stats.BytesReceivedPerSec, x.Stats.BytesSentPerSec);
                Thread.Sleep(1000);
            }
        }
        static void cli_OnConnected(object sender, socks5.Socks5Client.Socks5ClientArgs e)
        {
            //Console.WriteLine("Connected to Socket! Status: {0}.", e.Status.ToString());
            if (e.Status == socks5.Socks.SocksError.Granted)
            {
                e.Client.onDataReceived += Client_onDataReceived;
                e.Client.onClientDisconnected += Client_onClientDisconnected;
                e.Client.Send(Encoding.ASCII.GetBytes("GET /lorem.txt HTTP/1.1\r\nHost: www.thrdu.de\r\n\r\n"));
                e.Client.ReceiveAsync();
            }
            else e.Client.Disconnect();
        }

        static void Client_onClientDisconnected(object sender, socks5.TCP.ClientEventArgs e)
        {
            Console.WriteLine("Client Disconnected.");
        }

        static void Client_onDataReceived(object sender, socks5.TCP.DataEventArgs e)
        {
            //Console.Write(Encoding.ASCII.GetString(e.Buffer, 0, e.Count));
            e.Client.ReceiveAsync();
            //e.Client.Send(Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\nHost: mcflix.com\r\n\r\n"));
        }
    }
}

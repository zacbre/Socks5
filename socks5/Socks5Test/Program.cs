using System;
using System.Collections.Generic;
using System.Text;
using socks5;
using System.Net;
using System.Threading;
namespace Socks5Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Socks5Server x = new Socks5Server(IPAddress.Any, 1080);
            x.onDataReceived += x_onDataReceived;
            x.onDataSent += x_onDataSent;
            x.Start();
            //Start showing network stats.
            while (true)
            {
                Console.Clear();
                Console.Write("Total Clients: \t{0}\nTotal Recvd: \t{1:0.00##}MB\nTotal Sent: \t{2:0.00##}MB\n", x.Stats.TotalClients, ((x.Stats.NetworkReceived / 1024f) / 1024f), ((x.Stats.NetworkSent / 1024f) / 1024f));
                Console.Write("Receiving/sec: \t{0}\nSending/sec: \t{1}", x.Stats.BytesReceivedPerSec, x.Stats.BytesSentPerSec);
                Thread.Sleep(1000);
            }
        }

        static void x_onDataSent(object sender, socks5.TCP.DataEventArgs e)
        {
            //e.Buffer = new byte[] { 120, 121, 122, (byte)'\n' };
            //e.Count = e.Buffer.Length;

            //e.Buffer[0] = (byte)'E';

            //Console.WriteLine("Sent: {0}", Encoding.ASCII.GetString(e.Buffer, 0, e.Count));
        }

        static void x_onDataReceived(object sender, socks5.TCP.DataEventArgs e)
        {
            //Console.WriteLine("Received: {0}", Encoding.ASCII.GetString(e.Buffer, 0, e.Count));
        }
    }
}

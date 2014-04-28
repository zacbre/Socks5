using System;
using System.Collections.Generic;
using System.Text;
using socks5;
using System.Net;
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
                Console.Write("\rTotal Clients: {0}\tTotal Received: {1}MB\tTotal Sent: {2}MB\t", x._server.Stats.TotalClients, ((x._server.Stats.NetworkReceived / 1024) / 1024) + "." + ((x._server.Stats.NetworkReceived / 1024) % 1024), ((x._server.Stats.NetworkSent / 1024) / 1024) + "." + ((x._server.Stats.NetworkSent / 1024) % 1024));
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

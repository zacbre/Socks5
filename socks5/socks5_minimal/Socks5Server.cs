using System;
using System.Collections.Generic;
using System.Text;
using socks5.TCP;
using System.Net;
using System.Threading;
using socks5.Socks;
namespace socks5
{
    public class Socks5Server
    {
        public int Timeout { get; set; }
        public int PacketSize { get; set; }
        public bool LoadPluginsFromDisk { get; set; }

        public TcpServer _server;
        private Thread NetworkStats;

        public List<SocksClient> Clients = new List<SocksClient>();
        
        private bool started;

        public Socks5Server(IPAddress ip, int port)
        {
            Timeout = 1000;
            PacketSize = 128;
            _server = new TcpServer(ip, port);
            _server.onClientConnected += _server_onClientConnected;
        }

        public void Start()
        {
            if (started) return;
            _server.PacketSize = PacketSize;
            _server.Start();
            started = true;
        }

        public void Stop()
        {
            if (!started) return;
            _server.Stop();
            for (int i = 0; i < Clients.Count; i++)
            {
                Clients[i].Client.Disconnect();
            }
            Clients.Clear();
            started = false;
        }

        void _server_onClientConnected(object sender, ClientEventArgs e)
        {
            //Console.WriteLine("Client connected.");
            SocksClient client = new SocksClient(e.Client);
            client.onClientDisconnected += client_onClientDisconnected;
            Clients.Add(client);
            client.Begin(this.PacketSize, this.Timeout);
        }

        void client_onClientDisconnected(object sender, SocksClientEventArgs e)
        {
            e.Client.onClientDisconnected -= client_onClientDisconnected;
            this.Clients.Remove(e.Client);
        }
    }
}

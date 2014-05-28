using System;
using System.Collections.Generic;
using System.Text;
using socks5.TCP;
using System.Net;
using System.Threading;
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
        public Stats Stats;

        public event EventHandler<DataEventArgs> onDataReceived = delegate { };
        public event EventHandler<DataEventArgs> onDataSent = delegate { };

        private bool started;

        public Socks5Server(IPAddress ip, int port)
        {
            Timeout = 1000;
            PacketSize = 128;
            LoadPluginsFromDisk = false;
            Stats = new Stats();
            _server = new TcpServer(ip, port);
            _server.onClientConnected += _server_onClientConnected;
        }

        public void Start()
        {
            if (started) return;
            Plugin.PluginLoader.LoadPluginsFromDisk = LoadPluginsFromDisk;
            _server.PacketSize = PacketSize;
            _server.Start();
            started = true;
            //start thread.
            NetworkStats = new Thread(new ThreadStart(delegate()
            {
                while (started)
                {
                    if (this.Clients.Contains(null))
                        this.Clients.Remove(null);
                    Stats.ResetClients(this.Clients.Count);
                    Thread.Sleep(1000);
                }
            }));
            NetworkStats.Start();
        }

        public void Stop()
        {
            if (!started) return;
            _server.Stop();
            started = false;
        }

        void _server_onClientConnected(object sender, ClientEventArgs e)
        {
            //Console.WriteLine("Client connected.");
            SocksClient client = new SocksClient(e.Client);
            client.Client.onDataReceived += Client_onDataSent;
            client.Client.onDataSent += Client_onDataReceived;
            client.onClientDisconnected += client_onClientDisconnected;
            client.Begin(this.PacketSize, this.Timeout);
            Clients.Add(client);
        }

        void client_onClientDisconnected(object sender, SocksClientEventArgs e)
        {
            e.Client.onClientDisconnected -= client_onClientDisconnected;
            e.Client.Client.onDataReceived -= onDataReceived;
            e.Client.Client.onDataSent -= onDataSent;
            this.Clients.Remove(e.Client);
        }

        void Client_onDataSent(object sender, DataEventArgs e)
        {
            this.Stats.AddBytes(e.Count, ByteType.Sent);
            this.Stats.AddPacket(PacketType.Sent);
            onDataSent(this, e);
        }

        void Client_onDataReceived(object sender, DataEventArgs e)
        {
            this.Stats.AddBytes(e.Count, ByteType.Received);
            this.Stats.AddPacket(PacketType.Received);
            onDataReceived(this, e);
        }
    }
}

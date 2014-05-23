using System;
using System.Collections.Generic;
using System.Text;
using socks5.TCP;
using System.Net;
namespace socks5
{
    public class Socks5Server
    {
        public int Timeout { get; set; }
        public int PacketSize { get; set; }
        public bool LoadPluginsFromDisk { get; set; }

        public TcpServer _server;

        public List<SocksClient> Clients = new List<SocksClient>();
        public Stats Stats;

        public event EventHandler<DataEventArgs> onDataReceived = delegate { };
        public event EventHandler<DataEventArgs> onDataSent = delegate { };

        public Socks5Server(IPAddress ip, int port)
        {
            Timeout = 1000;
            PacketSize = 65535;
            LoadPluginsFromDisk = false;
            Stats = new Stats();
            _server = new TcpServer(ip, port);
            _server.onClientConnected += _server_onClientConnected;
            _server.onClientDisconnected += _server_onClientDisconnected;
        }

        public void Start()
        {
            Plugin.PluginLoader.LoadPluginsFromDisk = LoadPluginsFromDisk;
            _server.PacketSize = PacketSize;
            _server.Start();
        }

        public void Stop()
        {
            _server.Stop();
        }

        void _server_onClientDisconnected(object sender, ClientEventArgs e)
        {
            e.Client.onDataReceived -= onDataReceived;
            e.Client.onDataSent -= onDataSent;
            //Console.WriteLine("Client disconnected.");
            Stats.ResetClients(_server.Clients.Count);
        }

        void _server_onClientConnected(object sender, ClientEventArgs e)
        {
            //Console.WriteLine("Client connected.");
            SocksClient client = new SocksClient(e.Client);
            Clients.Add(client);
            client.Client.onDataReceived += Client_onDataSent;
            client.Client.onDataSent += Client_onDataReceived;
            client.onClientDisconnected += client_onClientDisconnected;
            this.Stats.AddClient();
            client.Begin(this.PacketSize, this.Timeout);
        }

        void client_onClientDisconnected(object sender, SocksClientEventArgs e)
        {
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

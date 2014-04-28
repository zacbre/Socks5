using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace socks5.TCP
{
    public class TcpServer
    {
        private TcpListener p;
        private List<Thread> acceptthreads = new List<Thread>();
        private bool accept = false;
        public int PacketSize{get;set;}

        public List<Client> Clients = new List<Client>();

        public event EventHandler<ClientEventArgs> onClientConnected = delegate { };
        public event EventHandler<ClientEventArgs> onClientDisconnected = delegate { };

        public event EventHandler<DataEventArgs> onDataReceived = delegate { };
        public event EventHandler<DataEventArgs> onDataSent = delegate { };

        public Stats Stats;
        public TcpServer(IPAddress ip, int port)
        {
            p = new TcpListener(ip, port);
            onDataReceived += TcpServer_onDataReceived;
            onDataSent += TcpServer_onDataSent;
            Stats = new Stats();
        }

        void TcpServer_onDataSent(object sender, DataEventArgs e)
        {
            //
            Stats.NetworkSent += (ulong)e.Count;
            Stats.PacketsSent++;
        }

        void TcpServer_onDataReceived(object sender, DataEventArgs e)
        {
            //Update data stats.
            Stats.NetworkReceived += (ulong)e.Count;
            Stats.PacketsReceived++;
        }

        private void AcceptConnections()
        {
            while(accept)
            {
                try
                {
                    Socket x = p.AcceptSocket();
                    //New Client.
                    Client f = new Client(x, PacketSize);
                    f.onClientDisconnected += onClientDisconnected;
                    f.onDataReceived += onDataReceived;
                    f.onDataSent += onDataSent;
                    f.onClientDisconnected += f_onClientDisconnected;
                    onClientConnected(this, new ClientEventArgs(f));
                    this.Stats.ClientsSinceRun++;
                    this.Stats.TotalClients++;
                    this.Clients.Add(f);
                }
                catch { //error, most likely server shutdown.
                }
            }
        }

        void f_onClientDisconnected(object sender, ClientEventArgs e)
        {
            this.Clients.Remove(e.Client);
            this.Stats.TotalClients--;
        }

        public void Start()
        {
            if (!accept)
            {
                accept = true;
                p.Start(10000);
                //start 4 new threads for pooled listening.
                for (int i = 0; i < 4; i++)
                {
                    acceptthreads.Add(new Thread(new ThreadStart(AcceptConnections)));
                    acceptthreads[acceptthreads.Count - 1].Start();
                }
            }
        }

        public void Stop()
        {
            if (accept)
            {
                accept = false;
                p.Stop();
            }
        }
    }
}

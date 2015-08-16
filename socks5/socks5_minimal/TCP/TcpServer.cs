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
        private bool accept = false;
        public int PacketSize{get;set;}

        public event EventHandler<ClientEventArgs> onClientConnected = delegate { };
        public event EventHandler<ClientEventArgs> onClientDisconnected = delegate { };

        public TcpServer(IPAddress ip, int port)
        {
            p = new TcpListener(ip, port);
        }

        private ManualResetEvent Task = new ManualResetEvent(false);

        private void AcceptConnections()
        {
            while(accept)
            {
                try
                {
                    Task.Reset();
                    p.BeginAcceptSocket(new AsyncCallback(AcceptClient), p);
                    Task.WaitOne();
                }
                catch { //error, most likely server shutdown.
                }
            }
        }

        void AcceptClient(IAsyncResult res)
        {
            try
            {
                TcpListener px = (TcpListener)res.AsyncState;
                Socket x = px.EndAcceptSocket(res);
                Task.Set();
                Client f = new Client(x, PacketSize);
                f.onClientDisconnected += onClientDisconnected;
                onClientConnected(this, new ClientEventArgs(f));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
         }

        public void Start()
        {
            if (!accept)
            {
                accept = true;
                p.Start(10000);
                new Thread(new ThreadStart(AcceptConnections)).Start();
            }
        }

        public void Stop()
        {
            if (accept)
            {
                accept = false;
                p.Stop();
                Task.Set();
            }
        }
    }
}

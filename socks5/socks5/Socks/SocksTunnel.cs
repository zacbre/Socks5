using socks5.Plugin;
using socks5.Socks;
using socks5.TCP;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace socks5
{
    class SocksTunnel
    {
        public SocksRequest Req;
        public SocksRequest ModifiedReq;

        public SocksClient Client;
        public Client RemoteClient;

        private List<DataHandler> Plugins = new List<DataHandler>();

        private int Timeout = 10000;
        private int PacketSize = 128;

        public SocksTunnel(SocksClient p, SocksRequest req, SocksRequest req1, int packetSize, int timeout)
        {
            RemoteClient = new Client(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), PacketSize);
            Client = p;
            Req = req;
            ModifiedReq = req1;
            PacketSize = packetSize;
            Timeout = timeout;
        }

        public void Open()
        {
            if (ModifiedReq.Address == null || ModifiedReq.Port <= -1) { Client.Client.Disconnect(); return; }
#if DEBUG
            Console.WriteLine("{0}:{1}", ModifiedReq.Address, ModifiedReq.Port);
#endif
            var socketArgs = new SocketAsyncEventArgs { RemoteEndPoint = new IPEndPoint(ModifiedReq.IP, ModifiedReq.Port) };
            socketArgs.Completed += socketArgs_Completed;
            RemoteClient.Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (!RemoteClient.Sock.ConnectAsync(socketArgs))
                ConnectHandler(socketArgs);
        }

        void socketArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            byte[] request = Req.GetData(true); // Client.Client.Send(Req.GetData());
            if (e.SocketError != SocketError.Success)
            {
                Console.WriteLine("Error while connecting: {0}", e.SocketError.ToString());
                request[1] = (byte)SocksError.Unreachable;
            }
            else
            {
                request[1] = 0x00;
            }

            Client.Client.Send(request);

            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    //connected;
                    ConnectHandler(e);
                    break;               
            }
        }

        private void ConnectHandler(SocketAsyncEventArgs e)
        {
            //start receiving from both endpoints.
            try
            {
                //all plugins get the event thrown.
                foreach (DataHandler data in PluginLoader.LoadPlugin(typeof(DataHandler)))
                    Plugins.Push(data);
                Client.Client.onDataReceived += Client_onDataReceived;
                RemoteClient.onDataReceived += RemoteClient_onDataReceived;
                RemoteClient.onClientDisconnected += RemoteClient_onClientDisconnected;
                Client.Client.ReceiveAsync();
                RemoteClient.ReceiveAsync();
            }
            catch
            {
            }
        }

        void RemoteClient_onClientDisconnected(object sender, ClientEventArgs e)
        {
#if DEBUG
            Console.WriteLine("Remote DC'd");
#endif
            Client.Client.Disconnect();
        }

        void RemoteClient_onDataReceived(object sender, DataEventArgs e)
        {
            e.Request = this.ModifiedReq;
            foreach (DataHandler f in Plugins)
                if(f.Enabled)
                    f.OnDataReceived(this, e);
            Client.Client.SendAsync(e.Buffer, e.Offset, e.Count);
            RemoteClient.ReceiveAsync();
        }

        void Client_onDataReceived(object sender, DataEventArgs e)
        {
            e.Request = this.ModifiedReq;
            foreach (DataHandler f in Plugins)
                if(f.Enabled)
                    f.OnDataSent(this, e);
            RemoteClient.SendAsync(e.Buffer, e.Offset, e.Count);
            Client.Client.ReceiveAsync();
        }
    }
}

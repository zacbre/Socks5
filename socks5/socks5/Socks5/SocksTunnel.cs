using socks5.Plugin;
using socks5.Socks5;
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

        public SocksClient Client;
        public Client RemoteClient;

        private List<SocksDataPluginHandler> Plugins = new List<SocksDataPluginHandler>();

        public SocksTunnel(SocksClient p, SocksRequest req)
        {
            RemoteClient = new Client(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
            Client = p;
            Req = req;
        }

        public void Open()
        {
            if (Req.Address == null || Req.Port == null) { Client.Client.Disconnect(); return; }
            Console.WriteLine("{0}:{1}", Req.Address, Req.Port);
            var socketArgs = new SocketAsyncEventArgs { RemoteEndPoint = new IPEndPoint(Req.IP, Req.Port) };
            socketArgs.Completed += socketArgs_Completed;
            RemoteClient.Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (!RemoteClient.Sock.ConnectAsync(socketArgs))
                ConnectHandler(socketArgs);
        }

        void socketArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            byte[] request = Req.GetData(); // Client.Client.Send(Req.GetData());
            if (e.SocketError != SocketError.Success)
            {
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
                foreach (SocksDataPluginHandler data in PluginLoader.LoadPlugin(typeof(SocksDataPluginHandler)))
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
            Client.Client.Disconnect();
        }

        void RemoteClient_onDataReceived(object sender, DataEventArgs e)
        {
            foreach (SocksDataPluginHandler f in Plugins)
                if(f.Enabled)
                    f.OnDataReceived(this, e);
            Client.Client.SendAsync(e.Buffer, e.Offset, e.Count);
            RemoteClient.ReceiveAsync();
        }

        void Client_onDataReceived(object sender, DataEventArgs e)
        {
            foreach (SocksDataPluginHandler f in Plugins)
                if(f.Enabled)
                    f.OnDataSent(this, e);
            RemoteClient.SendAsync(e.Buffer, e.Offset, e.Count);
            Client.Client.ReceiveAsync();
        }
    }
}

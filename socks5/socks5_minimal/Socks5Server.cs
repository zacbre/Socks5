/*
    Socks5 - A full-fledged high-performance socks5 proxy server written in C#. Plugin support included.
    Copyright (C) 2016 ThrDev

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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
        public bool Authentication { get { return Authenticate; } set { Authenticate = value; } }

        public event SocksClient.Authenticate OnAuthentication = null;

        public TcpServer _server;
        private bool Authenticate = false;

        public List<SocksClient> Clients = new List<SocksClient>();
        
        private bool started;

        public Socks5Server(IPAddress ip, int port)
        {
            this.Timeout = 1000;
            this.PacketSize = 65535;
            this._server = new TcpServer(ip, port);
            this._server.onClientConnected += _server_onClientConnected;
        }

        public void Start()
        {
            if (this.started) return;
            this._server.PacketSize = PacketSize;
            this._server.Start();
            this.started = true;
        }

        public void Stop()
        {
            if (!started) return;
            this._server.Stop();
            for (int i = 0; i < this.Clients.Count; i++)
            {
                this.Clients[i].Client.Disconnect();
            }
            this.Clients.Clear();
            this.started = false;
        }

        void _server_onClientConnected(object sender, ClientEventArgs e)
        {
            //Console.WriteLine("Client connected.");
            SocksClient client = new SocksClient(e.Client);
            client.onClientDisconnected += client_onClientDisconnected;
            client.OnClientAuthenticating += client_OnClientAuthenticating;
            Clients.Add(client);
            client.Authentication = this.Authentication;
            client.Begin(this.PacketSize, this.Timeout);
        }

        LoginStatus client_OnClientAuthenticating(object sender, SocksAuthenticationEventArgs e)
        {
            if (this.Authenticate)
            {
                return OnAuthentication(sender, e);
            }
            else
            {
                return LoginStatus.Correct;
            }
        }

        void client_onClientDisconnected(object sender, SocksClientEventArgs e)
        {
            e.Client.onClientDisconnected -= client_onClientDisconnected;
            this.Clients.Remove(e.Client);
        }
    }
}

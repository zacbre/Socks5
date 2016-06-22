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
using System.Net;
using socks5.TCP;
namespace socks5.Socks
{
    public class SocksClient
    {
        public event EventHandler<SocksClientEventArgs> onClientDisconnected = delegate { };
        public delegate LoginStatus Authenticate(object sender, SocksAuthenticationEventArgs e);
        public event Authenticate OnClientAuthenticating = null;

        public Client Client;
        public bool Authenticated { get; private set; }
        public bool Authentication = false;


        private SocksRequest req1 = null;
        public SocksRequest Destination { get { return req1; } }

        public SocksClient(Client cli)
        {
            Client = cli;
        }

        public void Begin(int PacketSize, int Timeout)
        {
            Client.onClientDisconnected += Client_onClientDisconnected;
            List<AuthTypes> authtypes = Socks5.RequestAuth(this);
            if (authtypes.Count <= 0)
            {
                Client.Send(new byte[] { 0x00, 0xFF });
                Client.Disconnect();
                return;
            }
            else if(Authentication && this.OnClientAuthenticating != null)
            {
                //request login.
                User user = Socks5.RequestLogin(this);
                if (user == null)
                {
                    Client.Disconnect();
                    return;
                }
                LoginStatus status = this.OnClientAuthenticating(this, new SocksAuthenticationEventArgs(user));
                Client.Send(new byte[] { (byte)HeaderTypes.Socks5, (byte)status });
                if (status == LoginStatus.Denied)
                {
                    Client.Disconnect();
                    return;
                }
                else if (status == LoginStatus.Correct)
                {
                    Authenticated = true;
                }
                //read password and invoke.
                //this.OnClientAuthenticating(this, new SocksAuthenticationEventArgs(..));
            }
            else
            {//no username/password required?
                Authenticated = true;
                Client.Send(new byte[] { (byte)HeaderTypes.Socks5, (byte)HeaderTypes.Zero });
            }
            SocksRequest req = Socks5.RequestTunnel(this);
            if (req == null) { Client.Disconnect(); return; }
            SocksTunnel x = new SocksTunnel(this, req, PacketSize, Timeout);
            x.Open();
        }

        void Client_onClientDisconnected(object sender, ClientEventArgs e)
        {
            this.onClientDisconnected(this, new SocksClientEventArgs(this));
        }
    }
    public enum LoginStatus
    {
        Denied = 0xFF,
        Correct = 0x00
    }
    public class User
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public IPEndPoint IP { get; private set; }
        public User(string un, string pw, IPEndPoint ip)
        {
            Username = un;
            Password = pw;
            IP = ip;
        }
    }
}

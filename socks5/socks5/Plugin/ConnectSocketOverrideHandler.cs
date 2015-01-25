using socks5.Socks;
using socks5.TCP;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace socks5.Plugin
{
    public abstract class ConnectSocketOverrideHandler : GenericPlugin
    {
        public abstract bool Enabled { get; set; }
        /// <summary>
        /// Override the connection, to do whatever you want with it. Client is a wrapper around a socket.
        /// </summary>
        /// <param name="sr">The original request params.</param>
        /// <returns></returns>
        public abstract Client OnConnectOverride(SocksRequest sr);
    }
}

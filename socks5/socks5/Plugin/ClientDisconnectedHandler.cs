using socks5.TCP;
using System;
using System.Collections.Generic;
using System.Text;

namespace socks5.Plugin
{
    public abstract class ClientDisconnectedHandler : GenericPlugin
    {
        /// <summary>
        /// Handle client disconnected callback. Useful for keeping track of connected clients.
        /// </summary>
        public abstract bool OnDisconnected(object sender, SocksClientEventArgs e);
        public abstract bool Enabled { get; set; }
    }
}

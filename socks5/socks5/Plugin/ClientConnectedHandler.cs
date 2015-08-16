using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace socks5.Plugin
{
    public abstract class ClientConnectedHandler : GenericPlugin
    {
        /// <summary>
        /// Handle client connected callback. Useful for IPblocking.
        /// </summary>
        /// <param name="Client"></param>
        /// <returns>Return true to allow the connection, return false to deny it.</returns>
        public abstract bool OnConnect(socks5.TCP.Client Client, IPEndPoint IP);
        public abstract bool Enabled { get; set; }
    }
}

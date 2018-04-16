using System;
using System.Net;
using socks5.Plugin;
using Socks5Test;

namespace socks5.core.server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Socks5Server(IPAddress.Any, 1080);
            PluginLoader.ChangePluginStatus(true, typeof(Auth));
            server.Start();
        }
    }
}

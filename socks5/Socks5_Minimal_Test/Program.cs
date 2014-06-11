using System;
using System.Collections.Generic;
using System.Text;
using socks5;
using System.Net;
namespace Socks5_Minimal_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Socks5Server f = new Socks5Server(IPAddress.Any, 1080);
            f.Start();
        }
    }
}

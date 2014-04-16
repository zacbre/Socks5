using socks5.Plugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace socks5.ExamplePlugins
{
    class ConnectHandlerExample : ConnectHandler
    {
        public override bool OnConnect(Socks5.SocksRequest Request)
        {
            //Compare data.
            if (Request.Address.Contains("74.125.224")) //Google.com IP
            {
                Console.WriteLine("Redirecting traffic from {0} to yahoo.com.", Request.Address);
                Request.Address = "www.yahoo.com";
                Request.Type = Socks5.AddressType.Domain;
            }
            //Allow the connection.
            return true;
        }

        public override bool Enabled
        {
            get { return false; }
        }
    }
}

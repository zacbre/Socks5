using socks5.Plugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace socks5.ExamplePlugins
{
    public class ConnectHandlerExample : ConnectHandler
    {
        public override bool OnConnect(Socks.SocksRequest Request)
        {
            //Compare data.
            if (Request.Address.Contains("74.125.224")) //Google.com IP
            {
                Console.WriteLine("Redirecting traffic from {0} to yahoo.com.", Request.Address);
                Request.Address = "www.yahoo.com";
                Request.Type = Socks.AddressType.Domain;
            }
            //Allow the connection.
            return true;
        }
        private bool enabled = false;
        public override bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
    }
}

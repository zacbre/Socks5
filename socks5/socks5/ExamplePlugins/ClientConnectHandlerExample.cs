using System;
using System.Collections.Generic;
using System.Text;

namespace socks5.ExamplePlugins
{
    class ClientConnectHandlerExample : socks5.Plugin.ClientConnectedHandler
    {
        public override bool OnConnect(TCP.Client Client, System.Net.IPEndPoint IP)
        {
            if (IP.Address.ToString() != "127.0.0.1")
                //deny the connection.
                return false;
            return true;
            //With this function you can also Modify the Socket, as it's stored in e.Client.Sock.
        }
        private bool enabled = false;
        public override bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }
    }
}

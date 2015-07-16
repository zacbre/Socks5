using System;
using System.Collections.Generic;
using System.Text;
using socks5;
using socks5.HTTP;
namespace Socks5Test
{
    class HTTPRewriter : socks5.Plugin.DataHandler
    {
        public override void OnServerDataReceived(object sender, socks5.TCP.DataEventArgs e)
        {
            if (e.Buffer.FindString("HTTP/1.") != -1 && e.Buffer.FindString("\r\n") != -1)
            {
                e.Buffer = e.Buffer.ReplaceString("\r\n", "\r\nX-Served-By: Socks5Server\r\n");
                e.Count = e.Count + "X-Served-By: Socks5Server\r\n".Length;
            }
        }

        public override void OnClientDataReceived(object sender, socks5.TCP.DataEventArgs e)
        {
           
        }

        private bool enabled = true;
        public override bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
    }
}

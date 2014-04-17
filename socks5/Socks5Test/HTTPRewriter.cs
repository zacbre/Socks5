using System;
using System.Collections.Generic;
using System.Text;
using socks5;
namespace Socks5Test
{
    class HTTPRewriter : socks5.Plugin.DataHandler
    {
        public override void OnDataReceived(object sender, socks5.TCP.DataEventArgs e)
        {
            if (e.Buffer.FindString("HTTP/1.") != -1 && e.Buffer.FindString("\r\n\r\n") != -1)
            {
                e.Buffer = e.Buffer.ReplaceString("\r\n", "\r\nX-Served-By: Socks5Server\r\n");
                e.Count = e.Count + "X-Served-By: Socks5Server\r\n".Length;
            }
        }

        public override void OnDataSent(object sender, socks5.TCP.DataEventArgs e)
        {
            
        }

        public override bool Enabled
        {
            get { return true; }
        }
    }
}

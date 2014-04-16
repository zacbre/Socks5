using socks5.Plugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace socks5.ExamplePlugins
{
    class DataHandlerExample : SocksDataPluginHandler
    {
        private string httpString = "HTTP/1.1";

        public override void OnDataReceived(object sender, TCP.DataEventArgs e)
        {
            //Modify data.
            int Location = e.Buffer.FindString(httpString);
            if (Location != -1)
            {
                //find end of location.
                int EndHTTP = e.Buffer.FindString(" ", Location + 1);
                //replace between these two values.
                if (EndHTTP != -1)
                {
                    e.Buffer = e.Buffer.ReplaceBetween(Location, EndHTTP, Encoding.ASCII.GetBytes("HTTP/0.1"));
                    Console.WriteLine(Encoding.ASCII.GetString(e.Buffer, 0, e.Count));
                    //convert sender.
                }
            }
        }

        public override void OnDataSent(object sender, TCP.DataEventArgs e)
        {
            //
        }

        public override bool Enabled
        {
            get { return true; }
        }
    }
}

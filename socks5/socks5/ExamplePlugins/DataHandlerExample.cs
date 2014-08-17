using socks5.Plugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace socks5.ExamplePlugins
{
    public class DataHandlerExample : DataHandler
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
                    e.Buffer = e.Buffer.ReplaceBetween(Location, EndHTTP, Encoding.ASCII.GetBytes("HTTP/1.0"));
                    Console.WriteLine(Encoding.ASCII.GetString(e.Buffer, 0, e.Count));
                    //convert sender.
                }
            }
        }

        public override void OnDataSent(object sender, TCP.DataEventArgs e)
        {
            //
        }

        private bool enabled = false;
        public override bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
    }
}

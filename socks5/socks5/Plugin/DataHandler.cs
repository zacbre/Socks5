using socks5.TCP;
using System;
using System.Collections.Generic;
using System.Text;

namespace socks5.Plugin
{
    public abstract class DataHandler : GenericPlugin
    {
        /// <summary>
        /// Allows you to grab data before it's sent to the end user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void OnDataReceived(object sender, DataEventArgs e);

        /// <summary>
        /// Allows you to grab/modify data before it's sent to the end server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public abstract void OnDataSent(object sender, DataEventArgs e);

        public abstract bool Enabled { get; set; }
    }
}

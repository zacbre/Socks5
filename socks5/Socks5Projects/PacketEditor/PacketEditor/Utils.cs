using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using socks5;
namespace PacketEditor
{
    class Utils
    {
        public static List<DataCapture.Data> CapturedData = new List<DataCapture.Data>();
        public static void Add(DataCapture.Data item)
        {
            if (DataAdded != null)
            {
                CapturedData.Push<DataCapture.Data>(item);
                DataAdded.Invoke(null, EventArgs.Empty);
            }
        }
        //need to invoke form.
        public static event EventHandler DataAdded;
    }
}

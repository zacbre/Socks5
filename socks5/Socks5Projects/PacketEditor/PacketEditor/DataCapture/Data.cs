using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketEditor.DataCapture
{
    class Data
    {
        public Data(socks5.Socks.SocksRequest p, byte[] buffer, int count, DataType dtype)
        {
            Buffer = new byte[count];
            Array.Copy(buffer, Buffer, count);
            Request = p;
            DataType = dtype;
        }
        public byte[] Buffer { get; private set; }
        public socks5.Socks.SocksRequest Request { get; private set; }
        public DataType DataType { get; private set; }
    }
    enum DataType
    {
        Received,
        Sent
    }
}

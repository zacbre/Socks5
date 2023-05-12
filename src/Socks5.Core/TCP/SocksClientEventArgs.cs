using Socks5.Core.Socks;

namespace Socks5.Core.TCP;

public class SocksClientEventArgs : EventArgs
{
    public SocksClientEventArgs(SocksClient client)
    {
        Client = client;
    }

    public SocksClient Client { get; private set; }
}
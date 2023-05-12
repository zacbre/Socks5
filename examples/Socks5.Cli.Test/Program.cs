using System.Net;
using Socks5.Core.Plugin;
using Socks5.Core.Socks;
using Socks5.Core.SocksServer;

var socks5Server = new Socks5Server(IPAddress.Any, 4444);
socks5Server.Start();
//PluginLoader.ChangePluginStatus(true, typeof(Auth));

Console.WriteLine("Server Started!");

while (true)
{
    Console.Write("Total Clients: \t{0}\nTotal Recvd: \t{1:0.00##}MB\nTotal Sent: \t{2:0.00##}MB\n", socks5Server.Stats.TotalClients, ((socks5Server.Stats.NetworkReceived / 1024f) / 1024f), ((socks5Server.Stats.NetworkSent / 1024f) / 1024f));
    Console.Write("Receiving/sec: \t{0}\nSending/sec: \t{1}", socks5Server.Stats.ReceivedBytesPerSecond(), socks5Server.Stats.SentBytesPerSecond());
    Thread.Sleep(1000);
    Console.Clear();
}

class Auth : LoginHandler
{
    public override bool OnStart() => true;

    public override bool Enabled { get; set; } = false;

    public override LoginStatus HandleLogin(User user)
    {
        return (user is { Username: "test", Password: "1234" } ? LoginStatus.Correct : LoginStatus.Denied);
    }
}
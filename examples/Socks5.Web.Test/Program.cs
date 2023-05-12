
using System.Net;
using Socks5.Core.SocksServer;

var socks5Server = new Socks5Server(IPAddress.Any, 4444);
socks5Server.Start();

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Lifetime.ApplicationStopping.Register(() =>
{
    Console.WriteLine("Shutting down...");
    socks5Server.Stop();
});

app.MapGet("/", () =>
{
    var totalString = "";
    foreach (var item in socks5Server.Clients)
    {
        if (item?.Client?.Sock == null)
        {
            continue;
        }

        try
        {
            totalString += $"{item.Client.Sock.RemoteEndPoint}\n";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    return totalString;
});

app.Run();
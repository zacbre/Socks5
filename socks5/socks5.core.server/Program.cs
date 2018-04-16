using System;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;

namespace socks5.core.server
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "socks5.core.server",
                Description = ".NET Core socks5 server."
            };

            app.HelpOption("-?|-h|--help");

            var portOption = app.Option("-p|--port", "Incomnig packages port", CommandOptionType.SingleValue);
            var userOption = app.Option("-u|--user", "User name", CommandOptionType.SingleValue);
            var pwdOption = app.Option("-pwd|--password", "Password", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                if (portOption.HasValue() && int.TryParse(portOption.Value(), out var port)
                    && userOption.HasValue() 
                    && pwdOption.HasValue())
                {
                    Console.WriteLine($"Server listen port: {port}");
                    new Socks5ServerWrapper().Start(port, userOption.Value(), pwdOption.Value());
                }
                else
                {
                    app.ShowHint();
                }
                return 0;
            });

            app.Execute(args);
        }
    }
}

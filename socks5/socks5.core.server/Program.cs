using System;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace socks5.core.server
{
    class Program
    {
        static Socks5ServerWrapper server;
        private static TaskCompletionSource<object> taskToWait;

        static void Main(string[] args)
        {
            taskToWait = new TaskCompletionSource<object>();
            AssemblyLoadContext.Default.Unloading += SigTermEventHandler;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelHandler);

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
                    server = new Socks5ServerWrapper();
                    server.Start(port, userOption.Value(), pwdOption.Value());
                    taskToWait.Task.Wait();
                    server.Stop();
                    AssemblyLoadContext.Default.Unloading -= SigTermEventHandler;
                    Console.CancelKeyPress -= new ConsoleCancelEventHandler(CancelHandler);
                }
                else
                {
                    app.ShowHint();
                }
                return 0;
            });

            app.Execute(args);
        }

        private static void CancelHandler(object sender, ConsoleCancelEventArgs e)
        {
            System.Console.WriteLine("Exiting...");
            taskToWait.TrySetResult(null);
        }

        private static void SigTermEventHandler(AssemblyLoadContext obj)
        {
            System.Console.WriteLine("Unloading...");
            taskToWait.TrySetResult(null);
        }
    }
}

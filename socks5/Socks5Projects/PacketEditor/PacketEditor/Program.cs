using socks5;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace PacketEditor
{
    static class Program
    {
        public static socks5.Socks5Server sock5;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {           
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            sock5 = new Socks5Server(IPAddress.Any, 7777);
            sock5.PacketSize = 65535;
            sock5.Start();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            sock5.Stop();
            //output to console instead.
            Environment.Exit(0);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}

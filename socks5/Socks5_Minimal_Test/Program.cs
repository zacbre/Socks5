using System;
using System.Collections.Generic;
using System.Text;
using socks5;
using System.Net;
namespace Socks5_Minimal_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Socks5Server f = new Socks5Server(IPAddress.Any, 1080);
            f.Authentication = true;
            f.OnAuthentication += f_OnAuthentication;
            f.Start();
        }

        static socks5.Socks.LoginStatus f_OnAuthentication(object sender, socks5.TCP.SocksAuthenticationEventArgs e)
        {
            if(e.User.Username == "test" && e.User.Password == "test123")
                return socks5.Socks.LoginStatus.Correct;
            return socks5.Socks.LoginStatus.Denied;
        }
    }
}

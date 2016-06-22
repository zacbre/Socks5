/*
    Socks5 - A full-fledged high-performance socks5 proxy server written in C#. Plugin support included.
    Copyright (C) 2016 ThrDev

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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

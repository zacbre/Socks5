using socks5.Socks;
using System;
using System.Collections.Generic;
using System.Text;

namespace socks5.Plugin
{
    public enum LoginStatus
    {
        Denied = 0xFF,
        Correct = 0x00
    }
    public abstract class LoginHandler : GenericPlugin
    {
        public abstract LoginStatus HandleLogin(User user);
        public abstract bool Enabled { get; set; }
        //
    }
}

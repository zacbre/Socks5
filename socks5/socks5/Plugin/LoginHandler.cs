using socks5.TCP;
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
    public abstract class LoginHandler
    {
        public abstract LoginStatus HandleLogin(User user);
        public abstract bool LoginRequired { get; }
        public abstract bool Enabled { get; }
    }
}

using socks5.Plugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace socks5.ExamplePlugins
{
    class LoginHandlerExample : LoginHandler
    {
        public override LoginStatus HandleLogin(TCP.User user)
        {
            return (user.Username == "thrdev" && user.Password == "testing1234" ? LoginStatus.Correct : LoginStatus.Denied);
        }

        public override bool LoginRequired
        {
            get { return true; }
        }

        public override bool Enabled 
        {
            get { return true; }
        }
    }
}

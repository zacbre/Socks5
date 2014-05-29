using socks5.Plugin;
using System;
using System.Collections.Generic;
using System.Text;

namespace socks5.ExamplePlugins
{
    public class LoginHandlerExample : LoginHandler
    {
        public override LoginStatus HandleLogin(TCP.User user)
        {
            return (user.Username == "thrdev" && user.Password == "testing1234" ? LoginStatus.Correct : LoginStatus.Denied);
        }

        private bool enabled = false;
        public override bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
    }
}

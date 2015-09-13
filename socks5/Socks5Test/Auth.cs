using System;
using System.Collections.Generic;
using System.Text;
using socks5.Plugin;
namespace Socks5Test
{
    class Auth : LoginHandler
    {
        public override LoginStatus HandleLogin(socks5.Socks.User user)
        {
            return (user.Username == "test" && user.Password == "1234" ? LoginStatus.Correct : LoginStatus.Denied);
        }

        private bool enabled = false;
        public override bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                this.enabled = value;
            }
        }
    }
}

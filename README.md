#Plugin Information

With plugins, you can currently do the following:
* Handle connections to the socks5 server and block by IP.
* Handle/require a login for the Socks5 proxy.
* Handle/modify incoming and outgoing data.
* Handle/modify incoming connections, and rewrite them to different domains/addresses & ports.
* Handle raw socket connections and override them.

#Included In This Branch

Socks5 and Socks5_Minimal.
Socks5 Minimal is a socks5 server with no plugin support, and is a lot better performing as well as smaller in size.

#Security

Built into the Socks5Client is a small encryption protocol that interfaces perfectly with the Socks5Server. This is exclusive between the client and server and uses a special authentication type for compatibility. The Socks5Client will prefer SocksEncrypt mode on connection but for reverse compatibility, it still has regular Socks5 support.

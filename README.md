# Update: May 2023
Fear not! This repo is not dead! This project has undergone a massive overhaul to support .net 7 and fix several bugs and make several performance optimizations.
Much work is still needed (i.e a github actions runner that updates a nuget package, etc) on this project to keep it up to date.

# About Socks5
Socks5 is a Socks5 proxy server/client written in C#. The server is both high performance and low latency, with maximum throughput thought through.

Socks5 includes massive plugin support, for doing things such as sniffing data, modifying inbound/outbound connections, and even giving the server firewall-like functionality.

# Plugin Information

The current plugin list has the following functionality and examples included:

* Handle connections to the socks5 server and allow/block by IP.
* Handle/require a login for the Socks5 proxy.
* Handle/modify incoming and outgoing data.
* Handle/modify incoming connections, and rewrite them to different domains/addresses & ports.
* Handle raw socket connections and override them.

# Included In This Branch

Just the standard Socks5 library.
Socks5Minimal is no longer supported.

# Security

Built into the Socks5Client is a small encryption protocol that interfaces perfectly with the Socks5Server. This is exclusive between the client and server and uses a special authentication type for compatibility. The Socks5Client will prefer SocksEncrypt mode on connection but for reverse compatibility, it still has regular Socks5 support.

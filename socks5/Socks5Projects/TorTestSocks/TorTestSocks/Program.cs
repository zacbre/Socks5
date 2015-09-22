using System;
using socks5;
using System.Net;
using socks5.Socks5Client;
using socks5.Plugin;

namespace TorTestSocks
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//to test this - use something like proxifier or configure firefox for this proxy.
			Socks5Server s = new Socks5Server (IPAddress.Any, 1080);
			s.Start ();
		}
	}

	class TorSocks : socks5.Plugin.ConnectSocketOverrideHandler 
	{
		public override socks5.TCP.Client OnConnectOverride (socks5.Socks.SocksRequest sr)
		{
			//use a socks5client to connect to it and passthru the data.
			//port 9050 is where torsocks is running (linux & windows)
			Socks5Client s = new Socks5Client ("localhost", 9050, sr.Address, sr.Port, "un", "pw");
			if (s.Connect ()) {
				//connect synchronously to block the thread.
				return s.Client;
			} else {
				Console.WriteLine ("Failed!");
				return null;
			}
		}
		private bool enabled = true;
		public override bool Enabled {
			get {
				return this.enabled;
			}
			set {
				this.enabled = value;
			}
		}
	}
}

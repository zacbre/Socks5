using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using socks5.TCP;
using System.Net.Sockets;
using socks5.Socks;
using socks5.Encryption;

namespace socks5.Socks5Client
{
    public class Socks5Client
    {
        private IPAddress ipAddress;
        public Client Client;

        private Socket p;
        private int Port;
        public bool reqPass = false;

        private string Username;
        private string Password;
        private string Dest;
        private int Destport;

        public Encryption.SocksEncryption enc;

        public event EventHandler<Socks5ClientArgs> OnConnected = delegate { };
        public event EventHandler<Socks5ClientDataArgs> OnDataReceived = delegate { };
        public event EventHandler<Socks5ClientDataArgs> OnDataSent = delegate { };
        public event EventHandler<Socks5ClientArgs> OnDisconnected = delegate { };

        public Socks5Client(string ipOrDomain, int port, string dest, int destport, string username = null, string password = null)
        {
            //Parse IP?
            if (!IPAddress.TryParse(ipOrDomain, out ipAddress))
            {
                //not connected.
                try
                {
                    foreach (IPAddress p in Dns.GetHostAddresses(ipOrDomain))
                        if (p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            DoSocks(p, port, dest, destport, username, password);
                            return;
                        }
                }
                catch
                {
                    throw new NullReferenceException();
                }
            }           
            DoSocks(ipAddress, port, dest, destport, username, password);
        }
        public Socks5Client(IPAddress ip, int port, string dest, int destport, string username = null, string password = null)
        {
            DoSocks(ip, port, dest, destport, username, password);
        }

        private void DoSocks(IPAddress ip, int port, string dest, int destport, string username = null, string password = null)
        {
            ipAddress = ip;
            Port = port;
            //check for username & pw.
            if(username != null && password != null)
            {
                Username = username;
                Password = password;
                reqPass = true;
            }
            Dest = dest;
            Destport = destport;
        }

        public void ConnectAsync()
        {
            //
            p = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Client = new Client(p, 2048);
            Client.onClientDisconnected += Client_onClientDisconnected;
            Client.Sock.BeginConnect(new IPEndPoint(ipAddress, Port), new AsyncCallback(onConnected), Client);
            //return status?
        }

        void Client_onClientDisconnected(object sender, ClientEventArgs e)
        {
            this.OnDisconnected(this, new Socks5ClientArgs(this, SocksError.Expired));
        }

        public bool Send(byte[] buffer, int offset, int length)
        {
            try
            {
                //buffer sending.
                int offst = 0;
                while(true)
                {
                    byte[] outputdata = enc.ProcessOutputData(buffer, offst, (length - offst > 4096 ? 4096 : length - offst));
                    offst += (length - offst > 4096 ? 4096 : length - offst);
                    //craft headers & shit.
                    //send outputdata's length firs.t
                    if (enc.GetAuthType() != AuthTypes.Login && enc.GetAuthType() != AuthTypes.None)
                    {
                        Client.Send(BitConverter.GetBytes(outputdata.Length));
                    }
                    Client.Send(outputdata, 0, outputdata.Length);
                    if (offst >= buffer.Length)
                    {
                        //exit;
                        return true;
                    }
                }
                return true;
            }
            catch
            {
                throw new Exception();
            }
        }

        public bool Send(byte[] buffer)
        {
            return Send(buffer, 0, buffer.Length);
        }

        public int Receive(out byte[] data)
        {
            //this should be packet header.
            try
            {
                //if we're using special encryptions, get size first.
                int recv = 0;
                byte[] newbuff;
                int torecv = 0;
                if (enc.GetAuthType() != AuthTypes.Login && enc.GetAuthType() != AuthTypes.None)
                {
                    byte[] buffer = new byte[sizeof(int)];
                    recv = Client.Receive(buffer, 0, buffer.Length);
                    //get total number of bytes.
                    torecv = BitConverter.ToInt32(buffer, 0);
                    newbuff = new byte[torecv];
                }
                else 
                {
                    newbuff = new byte[4096];
                }
                recv = Client.Receive(newbuff, 0, newbuff.Length);
                if (recv <= 0)
                {
                    throw new Exception();
                }
                if (enc.GetAuthType() != AuthTypes.Login && enc.GetAuthType() != AuthTypes.None)
                {
                    if (recv == torecv)
                    {
                        //yey
                        //process packet.
                        byte[] output = enc.ProcessInputData(newbuff, 0, recv);
                        //receive full packet.
                        data = output;
                        return recv;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    byte[] output = enc.ProcessInputData(newbuff, 0, recv);
                    data = output;
                    return recv;
                }
            }
            catch
            {
                //disconnect.
                Client.Disconnect();
                throw new Exception();
            }
        }

        public byte[] Receive()
        {
            byte[] m;
            this.Receive(out m);
            return m;
        }

        public void ReceiveAsync()
        {
            if (enc.GetAuthType() != AuthTypes.Login && enc.GetAuthType() != AuthTypes.None)
            {
                Client.ReceiveAsync(sizeof(int));
            }
            else
            {
                Client.ReceiveAsync(65535);
            }
        }


        void Client_onDataReceived(object sender, DataEventArgs e)
        {
            //this should be packet header.
            try
            {
                if (enc.GetAuthType() != AuthTypes.Login && enc.GetAuthType() != AuthTypes.None)
                {
                    //get total number of bytes.
                    int torecv = BitConverter.ToInt32(e.Buffer, e.Offset);

                    byte[] newbuff = new byte[torecv];
                    int recv = Client.Receive(newbuff, 0, newbuff.Length);
                    
                    if (recv == torecv)
                    {
                        //yey
                        //process packet.
                        byte[] output = enc.ProcessInputData(newbuff, 0, recv);
                        //receive full packet.
                        e.Buffer = output;
                        e.Offset = 0;
                        e.Count = output.Length;
                        this.OnDataReceived(this, new Socks5ClientDataArgs(this, e.Buffer, e.Count, e.Offset));
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    this.OnDataReceived(this, new Socks5ClientDataArgs(this, e.Buffer, e.Count, e.Offset));
                }
            }
            catch
            {
                //disconnect.
                Client.Disconnect();
                throw new Exception();
            }
        }

        public bool Connect()
        {
            try
            {
                p = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Client = new Client(p, 65535);
                Client.Sock.Connect(new IPEndPoint(ipAddress, Port));
                //try the greeting.
                //Client.onDataReceived += Client_onDataReceived;
                if(Socks.DoSocksAuth(this, Username, Password))
                    if (Socks.SendRequest(Client, enc, Dest, Destport) == SocksError.Granted)
                        return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        private void onConnected(IAsyncResult res)
        {
            Client = (Client)res.AsyncState;
            try
            {
                Client.Sock.EndConnect(res);
            }
            catch
            {
                this.OnConnected(this, new Socks5ClientArgs(null, SocksError.Failure));
                return;
            }
            if (Socks.DoSocksAuth(this, Username, Password))
            {
                SocksError p = Socks.SendRequest(Client, enc, Dest, Destport);
                Client.onDataReceived += Client_onDataReceived;
                this.OnConnected(this, new Socks5ClientArgs(this, p));
                
            }
            else
                this.OnConnected(this, new Socks5ClientArgs(this, SocksError.Failure));
        }


        public bool Connected
        {
            get { return Client.Sock.Connected; }
        }
        //send.

    }
}

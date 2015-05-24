using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

using iControlInterfaces;

namespace iControlServerApplication {
    class iControlClient : IiControlClient  {

        private TcpClient _client;
        public TcpClient TCP { get { return _client; } set { _client = value; _ip = GetIP(value); } }

        private String _ip;
        public String IPAddress { get { return _ip; } }

        private Thread _thread;
        public Thread Thread { get { return _thread; } set { _thread = value; } }
        
        private bool _keepConnected = true;
        public bool keepConnected { get { return _keepConnected; } set {_keepConnected = value; } }

        public iControlClient() { }

        public iControlClient(TcpClient client) {
            this._client = client;
            this._ip = GetIP(client);
        }

        private String GetIP(TcpClient client) {
            IPEndPoint ipep = (IPEndPoint)client.Client.RemoteEndPoint;
            return ipep.Address.ToString();
        }
    }
}

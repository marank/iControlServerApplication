using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace iControlInterfaces {
    public interface IiControlClient {

        TcpClient TCP { get; }
        string IPAddress { get; }
        
        bool keepConnected { get; set; }

    }
}

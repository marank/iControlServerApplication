using System.Net.Sockets;

namespace iControlInterfaces {
    public interface IiControlClient {

        TcpClient TCP { get; }
        string IPAddress { get; }
        
        bool keepConnected { get; set; }

    }
}

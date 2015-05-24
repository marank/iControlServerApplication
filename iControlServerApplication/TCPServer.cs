using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using System.Net;

namespace iControlServerApplication {
    class TCPServer {

        public int Port = 31313;

        public delegate void CommandReceivedEventHandler(object source, CommandReceivedEventArgs e);
        public event CommandReceivedEventHandler CommandReceived;
        public class CommandReceivedEventArgs : EventArgs {
            private string _command;
            private string[] _splittedCommands;
            private iControlClient _client;
            public CommandReceivedEventArgs(string command, iControlClient client) {
                _command = command;
                _splittedCommands = command.Split(new Char[]{' '});
                _client = client;
            }
            public string Command { get { return _command; } }
            public string[] SplittedCommands { get { return _splittedCommands; } }
            public iControlClient Client { get { return _client; } }
        }

        private TcpListener tcpListener;
        private Thread listenThread;
        private Dictionary<String, iControlClient> icClients;

        private bool keepListening = true;
        private bool keepReceiving = true;

        public TCPServer() {
            this.tcpListener = new TcpListener(IPAddress.Any, Port);
            this.icClients = new Dictionary<String, iControlClient>();
        }

        public Boolean Start() {
            if (PortIsAvailable(Port)) {
                this.listenThread = new Thread(new ThreadStart(ListenForClients));
                this.listenThread.Start();
                Program.Log("ListeningThread started.");
                return true;
            } else {
                return false;
            }
        }

        public void Stop() {
            if (this.listenThread.IsAlive) {
                this.tcpListener.Stop();
                this.listenThread.Abort();
            }

            foreach (KeyValuePair<String, iControlClient> client in this.icClients) {
                client.Value.keepConnected = false;
                client.Value.Thread.Abort();
            }
            this.keepReceiving = false;
            this.keepListening = false;

            foreach (KeyValuePair<String, iControlClient> client in this.icClients) {
                if (client.Value.Thread.IsAlive) client.Value.Thread.Abort();
            }
        }

        private void ListenForClients() {
            this.tcpListener.Start();

            TcpClient client;

            while (this.keepListening) {
                try {
                    client = this.tcpListener.AcceptTcpClient();
                } catch {
                    break;
                }

                iControlClient icClient = new iControlClient(client);
                this.icClients.Add(icClient.IPAddress, icClient);

                Program.Log("[" + icClient.IPAddress + "] Connection accepted. Starting client thread.");

                icClient.Thread = new Thread(new ParameterizedThreadStart(HandleClientCommunication));
                icClient.Thread.Start(icClient);
            }

            Program.Log("Stop listening.");
        }

        private void HandleClientCommunication(object client) {
            iControlClient icClient = (iControlClient)client;
            NetworkStream clientStream = icClient.TCP.GetStream();

            clientStream.ReadTimeout = 10;

            int bufflen = 4096;
            byte[] message = new byte[bufflen];
            int bytesRead;

            while (this.keepReceiving && icClient.keepConnected) {
                bytesRead = 0;

                try {
                    bytesRead = clientStream.Read(message, 0, bufflen);
                } catch {
                    break;
                }

                if (bytesRead == 0) {
                    break;
                }
                ProcessReceivedData(icClient, ParseData(message, bytesRead));
            }

            Program.Log("[" + icClient.IPAddress + "] Connection closed.");

            icClient.TCP.Close();
            this.icClients.Remove(icClient.IPAddress);
        }

        private String[] ParseData(byte[] message, int bytesRead) {
            ASCIIEncoding encoder = new ASCIIEncoding();
            String dataString = encoder.GetString(message, 0, bytesRead);
            return dataString.Split(new Char[] { ' ' });
        }

        private void ProcessReceivedData(iControlClient icClient, String[] commands) {
            Program.Log("[" + icClient.IPAddress + "] >> " + String.Join(" ", commands));

            if (this.CommandReceived != null) {
                CommandReceived(this, new CommandReceivedEventArgs(String.Join(" ", commands), icClient));
            }

            NetworkStream clientStream = icClient.TCP.GetStream();
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] buffer = encoder.GetBytes("::ok");
            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();

            icClient.keepConnected = false;
        }

        private Boolean PortIsAvailable(int port) {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] endPoints = ipGlobalProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endPoint in endPoints) {
                if (endPoint.Port == port) {
                    return false;
                }
            }
            return true;
        }
    }
}
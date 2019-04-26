using System;
using System.Net;
using System.Net.Sockets;

namespace LocalFileSharing.Network.Sockets
{
    public class TcpServer : TcpSocketBase
    {
        protected readonly Socket listener;
        protected TcpClient connectedClient;

        public TcpServer(IPAddress ipAddress, int port)
        {
            if (ipAddress is null)
            {
                throw new ArgumentNullException(nameof(ipAddress));
            }

            if (port >= MinAllowedPort && port <= MaxAllowedPort)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(port),
                    port,
                    $"The port have to be between {MinAllowedPort} and {MaxAllowedPort}."
                );
            }

            listener = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            listener.Bind(ipEndPoint);
        }

        public void StartListening(int backlog)
        {
            listener.Listen(backlog);
        }

        public TcpClient AcceptTcpClient()
        {
            Socket acceptedClient = listener.Accept();
            connectedClient = new TcpClient(acceptedClient);

            return connectedClient;
        }

        public void StopListening()
        {
            connectedClient?.Close();

            listener.Shutdown(SocketShutdown.Both);
            listener.Close();
        }
    }
}

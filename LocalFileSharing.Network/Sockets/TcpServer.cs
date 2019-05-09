using System;
using System.Net;
using System.Net.Sockets;

namespace LocalFileSharing.Network.Sockets {
    public class TcpServer : TcpSocketBase {
        protected int _backlog = 1;

        public TcpClient ConnectedClient { get; protected set; }

        public TcpServer(IPEndPoint ipEndPoint) {
            if (ipEndPoint is null) {
                throw new ArgumentNullException(nameof(ipEndPoint));
            }

            if (!(ipEndPoint.Port >= MinAllowedPort && ipEndPoint.Port <= MaxAllowedPort)) {
                throw new ArgumentOutOfRangeException(
                    nameof(ipEndPoint.Port),
                    ipEndPoint.Port,
                    $"The port have to be between {MinAllowedPort} and {MaxAllowedPort}."
                );
            }

            _socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            _socket.Bind(ipEndPoint);
            _socket.Listen(_backlog);
        }

        public TcpClient AcceptTcpClient() {
            Socket connectedClient = _socket.Accept();
            ConnectedClient = new TcpClient(connectedClient);
            return ConnectedClient;
        }

        public override void Disconnect() {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            ConnectedClient.Close();
        }
    }
}

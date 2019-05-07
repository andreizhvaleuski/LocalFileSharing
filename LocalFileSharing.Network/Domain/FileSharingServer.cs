using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using LocalFileSharing.Network.Sockets;

using Socs = System.Net.Sockets;

namespace LocalFileSharing.Network.Domain {
    public sealed class FileSharingServer {
        public FileSharingClient FileSharingClient { get; private set; }

        private TcpServer _tcpServer;

        public async Task<IPEndPoint> GetServerIPEndPointAsync(
            CancellationToken cancellationToken
        ) {
            IPEndPoint ipEndPoint = null;

            await Task.Run(() => {
                using (Socs.Socket udpClient =
                        new Socs.Socket(
                        Socs.AddressFamily.InterNetwork,
                        Socs.SocketType.Dgram,
                        Socs.ProtocolType.Udp)) {
                    udpClient.Connect(IPAddress.Parse("8.8.8.8"), 65_535);
                    ipEndPoint = udpClient.LocalEndPoint as IPEndPoint;
                }
            });

            return ipEndPoint;
        }

        public async Task<FileSharingClient> ListenAsync(
            IPEndPoint ipEndPoint
        ) {
            if (ipEndPoint is null) {
                throw new ArgumentNullException(nameof(ipEndPoint));
            }

            if (FileSharingClient != null) {
                throw new InvalidOperationException(
                    $"Can not start listening while client exists."
                );
            }

            if (_tcpServer is null) {
                _tcpServer = new TcpServer(ipEndPoint.Address, ipEndPoint.Port);
                _tcpServer.StartListening(1);
            }

            await Task.Run(() => {
                TcpClient tcpClient = _tcpServer.AcceptTcpClient();
                FileSharingClient = new FileSharingClient(tcpClient);
            });

            return FileSharingClient;
        }

        public void Close() {
            _tcpServer.StopListening();
        }
    }
}

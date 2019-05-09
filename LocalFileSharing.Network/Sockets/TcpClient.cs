using System;
using System.Net;
using System.Net.Sockets;

namespace LocalFileSharing.Network.Sockets {
    public class TcpClient : TcpSocketBase {
        public TcpClient(IPEndPoint ipEndPoint) {
            if (ipEndPoint is null) {
                throw new ArgumentNullException(nameof(ipEndPoint));
            }

            if (!(ipEndPoint.Port >= MinAllowedPort && ipEndPoint.Port <= MaxAllowedPort)) {
                throw new ArgumentOutOfRangeException(
                    nameof(ipEndPoint.Port),
                    ipEndPoint.Port,
                    $"The ip end point port have to be between {MinAllowedPort} and {MaxAllowedPort}."
                );
            }

            _socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            _socket.Connect(ipEndPoint);
        }

        public TcpClient(Socket connectedClient) {
            if (connectedClient is null) {
                throw new ArgumentNullException(nameof(connectedClient));
            }

            if (!connectedClient.Connected) {
                throw new ArgumentException(
                    $"The client have to be connected.",
                    nameof(connectedClient)
                );
            }

            _socket = connectedClient;
        }

        public virtual void SendBytes(byte[] buffer) {
            if (buffer is null) {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (buffer.Length == 0) {
                throw new ArgumentException(
                    $"The buffer can not be empty.",
                    nameof(buffer)
                );
            }

            _socket.Send(buffer);
        }

        public virtual byte[] ReceiveBytes(int bytesNumber) {
            if (bytesNumber <= 0) {
                throw new ArgumentOutOfRangeException(
                    $"The bytes number must be greater than zero.",
                    nameof(bytesNumber)
                );
            }

            byte[] buffer = new byte[bytesNumber];
            int totalBytesReceivedNumber = 0;
            do {
                int currentBytesReceivedNumber = _socket.Receive(
                    buffer,
                    totalBytesReceivedNumber,
                    bytesNumber - totalBytesReceivedNumber,
                    SocketFlags.None
                );

                if (currentBytesReceivedNumber <= 0) {
                    throw new SocketException();
                }

                totalBytesReceivedNumber += currentBytesReceivedNumber;
            } while (totalBytesReceivedNumber != bytesNumber);

            return buffer;
        }

        public override void Disconnect() {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        public virtual void Close() {
            _socket.Close();
        }
    }
}

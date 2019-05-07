using System;
using System.Net;
using System.Net.Sockets;

namespace LocalFileSharing.Network.Sockets {
    public class TcpClient : TcpSocketBase {
        protected readonly Socket client;

        public TcpClient(IPAddress ipAddress, int port) {
            if (ipAddress is null) {
                throw new ArgumentNullException(nameof(ipAddress));
            }

            if (!(port >= MinAllowedPort && port <= MaxAllowedPort)) {
                throw new ArgumentOutOfRangeException(
                    nameof(port),
                    port,
                    $"The port have to be between {MinAllowedPort} and {MaxAllowedPort}."
                );
            }

            client = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            client.Connect(ipEndPoint);
        }

        public TcpClient(Socket connectedClient) {
            if (!connectedClient.Connected) {
                throw new ArgumentException(
                    $"The client is not connected.",
                    nameof(connectedClient)
                );
            }

            client = connectedClient;
        }

        public virtual void SendBytes(byte[] buffer) {
            if (buffer is null) {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (buffer.Length == 0) {
                throw new ArgumentException(
                    $"The buffer cannot be empty.",
                    nameof(buffer)
                );
            }

            client.Send(buffer);
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
                int currentBytesReceivedNumber = client.Receive(
                    buffer,
                    totalBytesReceivedNumber,
                    bytesNumber - totalBytesReceivedNumber,
                    SocketFlags.None
                );

                if (currentBytesReceivedNumber <= 0) {
                    return null;
                }

                totalBytesReceivedNumber += currentBytesReceivedNumber;
            } while (totalBytesReceivedNumber != bytesNumber);

            return buffer;
        }

        public virtual void Close() {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}

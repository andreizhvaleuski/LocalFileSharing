using System.Net;
using System.Net.Sockets;

namespace LocalFileSharing.Network.Sockets {
    public abstract class TcpSocketBase {
        public const int MinAllowedPort = 61001;
        public const int MaxAllowedPort = 65535;

        protected Socket _socket;

        public virtual IPEndPoint GetLocalIPEndPoint() {
            return _socket?.LocalEndPoint as IPEndPoint;
        }

        public virtual IPEndPoint GetRemoteIPEndPoint() {
            return _socket?.RemoteEndPoint as IPEndPoint;
        }

        public abstract void Disconnect();
    }
}

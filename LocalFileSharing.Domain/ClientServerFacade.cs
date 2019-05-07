using System;

using LocalFileSharing.Network.Framing;
using LocalFileSharing.Network.Sockets;

namespace LocalFileSharing.Domain {
    public class ClientServerFacade {
        private readonly ILengthPrefixWrapper lengthPrefixWrapper;
        private readonly ITypePrefixWrapper typePrefixWrapper;

        private readonly TcpServer server;
        private readonly TcpClient client;

        public ClientServerFacade(ILengthPrefixWrapper lengthPrefixWrapper, ITypePrefixWrapper typePrefixWrapper) {
            if (lengthPrefixWrapper is null) {
                throw new ArgumentNullException(nameof(lengthPrefixWrapper));
            }

            if (typePrefixWrapper is null) {
                throw new ArgumentNullException(nameof(typePrefixWrapper));
            }

            this.lengthPrefixWrapper = lengthPrefixWrapper;
            this.typePrefixWrapper = typePrefixWrapper;
        }
    }
}

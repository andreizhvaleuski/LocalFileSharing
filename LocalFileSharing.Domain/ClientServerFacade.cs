using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalFileSharing.Network.Framing;
using LocalFileSharing.Network.Sockets;

namespace LocalFileSharing.Domain
{
    public class ClientServerFacade
    {
        private readonly ILengthPrefixWrapper lengthPrefixWrapper;
        private readonly ITypePrefixWrapper typePrefixWrapper;

        private TcpServer server;
        private TcpClient client;

        public ClientServerFacade(ILengthPrefixWrapper lengthPrefixWrapper, ITypePrefixWrapper typePrefixWrapper)
        {
            if (lengthPrefixWrapper is null)
            {
                throw new ArgumentNullException(nameof(lengthPrefixWrapper));
            }

            if (typePrefixWrapper is null)
            {
                throw new ArgumentNullException(nameof(typePrefixWrapper));
            }

            this.lengthPrefixWrapper = lengthPrefixWrapper;
            this.typePrefixWrapper = typePrefixWrapper;
        }


    }
}

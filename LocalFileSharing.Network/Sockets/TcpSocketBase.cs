using System;
using LocalFileSharing.Network.Framing;

namespace LocalFileSharing.Network.Sockets
{
    public abstract class TcpSocketBase
    {
        protected readonly ILengthPrefixWrapper lengthPrefixWrapper;

        protected readonly ITypePrefixWrapper typePrefixWrapper;

        public TcpSocketBase(
            ILengthPrefixWrapper lengthPrefixWrapper,
            ITypePrefixWrapper typePrefixWrapper
        ) {
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

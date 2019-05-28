using System;

namespace LocalFileSharing.Network.Framing.Wrappers {
    public interface ITransferIDPrefixWrapper : IPrefixWrapper {
        byte[] Wrap(byte[] unwrappedBuffer, Guid transferID);

        Guid Unwrap(byte[] wrappedBuffer);
    }
}

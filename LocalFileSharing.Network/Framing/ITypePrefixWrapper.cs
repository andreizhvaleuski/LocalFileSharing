using LocalFileSharing.Network.Framing;

namespace LocalFileSharing.Network.Framing {
    public interface ITypePrefixWrapper {
        int TypePrefixSize { get; }

        byte[] Wrap(byte[] unwrappedBuffer, MessageType type);

        MessageType GetTypePrefixValue(byte[] wrappedBuffer);
    }
}

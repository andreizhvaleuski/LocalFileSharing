namespace LocalFileSharing.Network.Framing.Wrappers {
    public interface ITypePrefixWrapper {
        int TypePrefixSize { get; }

        byte[] Wrap(byte[] unwrappedBuffer, MessageType type);

        MessageType GetTypePrefixValue(byte[] wrappedBuffer);
    }
}

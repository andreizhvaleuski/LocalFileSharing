namespace LocalFileSharing.Network.Framing.Wrappers {
    public interface ITypePrefixWrapper : IPrefixWrapper {
        byte[] Wrap(byte[] unwrappedBuffer, MessageType type);

        MessageType Unwrap(byte[] wrappedBuffer);
    }
}

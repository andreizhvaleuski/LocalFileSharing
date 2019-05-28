namespace LocalFileSharing.Network.Framing.Wrappers {
    public interface ILengthPrefixWrapper : IPrefixWrapper {
        byte[] Wrap(byte[] unwrappedBuffer);

        int Unwrap(byte[] wrappedBuffer);
    }
}

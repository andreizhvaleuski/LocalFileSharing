namespace LocalFileSharing.Network.Framing.Wrappers {
    public interface ILengthPrefixWrapper {
        int LengthPrefixSize { get; }

        byte[] Wrap(byte[] unwrappedBuffer);

        int GetLengthPrefixValue(byte[] wrappedBuffer);
    }
}

namespace LocalFileSharing.Network.Framing
{
    public interface ILengthPrefixWrapper
    {
        int LengthPrefixSize { get; }

        byte[] Wrap(byte[] unwrappedBuffer);

        int GetLengthPrefixValue(byte[] wrappedBuffer);
    }
}

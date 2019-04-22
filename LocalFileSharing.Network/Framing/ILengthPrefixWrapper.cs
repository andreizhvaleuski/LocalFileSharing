namespace LocalFileSharing.Network.Framing
{
    public interface ILengthPrefixWrapper : IWrapper
    {
        int LengthPrefixSize { get; }

        int GetLengthPrefixValue(byte[] wrappedBuffer);
    }
}

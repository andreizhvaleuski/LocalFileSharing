namespace LocalFileSharing.Network.Framing
{
    public interface ILengthPrefixWrapper : IWrapper
    {
        int LengthPrefixBytes { get; }

        int Unwrap(byte[] buffer);
    }
}

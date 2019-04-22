using LocalFileSharing.Network.Common;

namespace LocalFileSharing.Network.Framing
{
    public interface ITypePrefixWrapper : IWrapper
    {
        int TypePrefixSize { get; }

        MessageType GetTypePrefixValue(byte[] wrappedBuffer);
    }
}

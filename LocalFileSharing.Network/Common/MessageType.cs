namespace LocalFileSharing.Network.Common
{
    public enum MessageType
    {
        Unknown = -1,
        Unspecified = 0,
        Keepalive = 1,
        Utf16LittleEndian = 2,
        Utf16BigEndian = 3
    }
}

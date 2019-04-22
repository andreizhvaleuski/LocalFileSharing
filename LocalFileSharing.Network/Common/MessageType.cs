namespace LocalFileSharing.Network.Common
{
    public enum MessageType
    {
        Unknown = -1,
        Unspecified = 0,
        Keepalive = 1,
        SendFileInitial = 2,
        SendFileRegular = 3 ,
        SendFileEnd = 4
    }
}

namespace LocalFileSharing.Network.Common
{
    public enum MessageType
    {
        Unknown = -1,
        Unspecified = 0,
        Keepalive = 1,
        SendFileInitial = 2,
        SendFile = 3 ,
        SendFileEnd = 4
    }
}

namespace LocalFileSharing.Network.Common
{
    public enum MessageType
    {
        Unspecified = 0,
        Keepalive = 1,
        SendFileInitial = 2,
        SendFileRegular = 3 ,
        SendFileEnd = 4
    }
}

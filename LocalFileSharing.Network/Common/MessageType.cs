namespace LocalFileSharing.Network.Common
{
    public enum MessageType
    {
        Unspecified = 0,
        Keepalive = 1,
        SendFileInitial = 2,
        SendFileRegular = 3,
        SendFileEnd = 4,
        SendFileCancel = 5,
        ReceiveFileInitialize = 6,
        ReciveFileRegular = 7,
        ReceiveFileEnd = 8,
        ReceiveFileCancel = 9
    }
}

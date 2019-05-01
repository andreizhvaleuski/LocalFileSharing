namespace LocalFileSharing.Network.Common
{
    public enum ResponseType
    {
        Unspecified = 0,
        ReceiveFileInitial = 1,
        ReceiveFileRegular = 2,
        ReceiveFileEnd = 3,
        ReceiveFileCancel = 4
    }
}

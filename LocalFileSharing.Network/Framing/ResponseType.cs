namespace LocalFileSharing.Network.Framing {
    public enum ResponseType {
        Unspecified = 0,
        Keepalive = 1,
        ReceiveFileInitial = 2,
        ReceiveFileRegular = 3,
        ReceiveFileCancel = 4
    }
}

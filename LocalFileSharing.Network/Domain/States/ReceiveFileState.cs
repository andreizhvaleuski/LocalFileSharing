namespace LocalFileSharing.Network.Domain.States {
    public enum ReceiveFileState {
        Unspecified = 0,
        Initializing = 1,
        Receiving = 2,
        Received = 3,
        Hashing = 4,
        Completed = 5,
        Cancelled = 6,
        Failed = 7
    }
}

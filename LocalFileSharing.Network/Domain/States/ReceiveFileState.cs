namespace LocalFileSharing.Network.Domain.States {
    public enum ReceiveFileState {
        Unspecified = 0,
        Initializing = 1,
        Receiving = 2,
        Hashing = 3,
        Completed = 4,
        Cancelled = 5,
        Failed = 6
    }
}

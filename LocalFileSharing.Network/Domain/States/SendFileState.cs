namespace LocalFileSharing.Network.Domain.States {
    public enum SendFileState {
        Unspecified = 0,
        Hashing = 1,
        Initializing = 2,
        Sending = 3,
        Completed = 4,
        Cancelled = 5,
        Failed = 6
    }
}

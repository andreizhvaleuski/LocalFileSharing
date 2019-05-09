namespace LocalFileSharing.Network.Domain.States {
    public enum SendFileState {
        Unspecified = 0,
        Hashing = 1,
        Initializing = 2,
        Sending = 3,
        Sent = 4,
        Completed = 5,
        Cancelled = 6,
        Failed = 7
    }
}

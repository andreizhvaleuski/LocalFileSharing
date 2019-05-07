namespace LocalFileSharing.Network.Domain.States {
    public enum ReceiveFileState {
        Unspecified = 0,
        Initializing = 1,
        Sending = 2,
        Ending = 3,
        Cancelled = 4,
        Hashing = 5,
        HashChecked = 6,
        Failed = 7,
        Completed = 8
    }
}

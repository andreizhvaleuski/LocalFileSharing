namespace LocalFileSharing.Domain.Infrastructure
{
    public enum ReceiveFileState
    {
        Unspecified,
        Initializing,
        Sending,
        Ending,
        Canceling,
        Cancelled,
        Hashing,
        HashCheck,

    }
}

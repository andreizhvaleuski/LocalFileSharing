namespace LocalFileSharing.Domain.Infrastructure
{
    public enum SendFileState
    {
        Unspecified = 0,
        Hashing = 1,
        Initializing = 2,
        Sending = 3,
        Ending = 4
    }
}

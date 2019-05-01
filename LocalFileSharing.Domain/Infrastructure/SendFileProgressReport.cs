namespace LocalFileSharing.Domain.Infrastructure
{
    public class SendFileProgressReport
    {
        public SendFileState SendFileState { get; private set; }

        public FileData FileData { get; private set; }

        public long BytesSent { get; set; }
    }
}

namespace LocalFileSharing.Domain.Infrastructure {
    public class SendFileProgressReport {
        public SendFileState SendFileState { get; set; }

        public FileData FileData { get; set; }

        public long BytesSent { get; set; }
    }
}

namespace LocalFileSharing.Domain.Infrastructure {
    public class ReceiveFileProgressReport {
        public ReceiveFileState ReceiveFileState { get; set; }

        public FileData FileData { get; set; }

        public long BytesRecived { get; set; }
    }
}

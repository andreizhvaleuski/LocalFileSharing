using LocalFileSharing.Network.Domain.States;

namespace LocalFileSharing.Network.Domain.Progress {
    public class SendFileProgressReport {
        public SendFileState SendFileState { get; set; }

        public FileData FileData { get; set; }

        public long BytesSent { get; set; }
    }
}

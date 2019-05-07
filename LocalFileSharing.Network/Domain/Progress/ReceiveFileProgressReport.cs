using LocalFileSharing.Network.Domain.States;

namespace LocalFileSharing.Network.Domain.Progress {
    public class ReceiveFileProgressReport {
        public ReceiveFileState ReceiveFileState { get; set; }

        public FileData FileData { get; set; }

        public long BytesRecived { get; set; }
    }
}

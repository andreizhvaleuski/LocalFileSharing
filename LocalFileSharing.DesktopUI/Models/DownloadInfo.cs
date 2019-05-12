using LocalFileSharing.Network.Domain.States;

namespace LocalFileSharing.DesktopUI.Models {
    public class DownloadInfo : TransferInfoBase {
        private long _bytesReceived;
        private ReceiveFileState _state;

        public long BytesReceived {
            get { return _bytesReceived; }
            set {
                if (!Set(ref _bytesReceived, value, nameof(BytesReceived))) {
                    return;
                }
                NotifyOfPropertyChange(nameof(Progress));
            }
        }
        public override long Progress {
            get {
                return BytesReceived * 100 / FileSize;
            }
        }
        public ReceiveFileState State {
            get { return _state; }
            set {
                Set(ref _state, value, nameof(State));
            }
        }
    }
}

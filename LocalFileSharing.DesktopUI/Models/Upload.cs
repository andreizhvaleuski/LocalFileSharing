using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalFileSharing.Network.Domain.States;

namespace LocalFileSharing.DesktopUI.Models {
    public class Upload : TransferBase {
        private long _bytesSent;
        private SendFileState _state;

        public long BytesSent {
            get { return _bytesSent; }
            set {
                if (!Set(ref _bytesSent, value, nameof(BytesSent))) {
                    return;
                }
                NotifyOfPropertyChange(nameof(Progress));
            }
        }
        public override long Progress {
            get {
                return BytesSent * 100 / FileSize;
            }
        }
        public SendFileState State {
            get { return _state; }
            set {
                Set(ref _state, value, nameof(State));
            }
        }
    }
}

using System;

namespace LocalFileSharing.Network.Domain.Context {
    public abstract class FileTransferContextBase {
        private bool _isCancelled;
        private bool _isFailed;

        public bool Cancelled => _isCancelled;
        public bool Failed => _isFailed;

        public abstract void Close();

        public virtual void Cancel() {
            if (Cancelled) {
                throw new InvalidOperationException();
            }
            _isCancelled = true;
        }
    }
}

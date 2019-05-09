using System;

namespace LocalFileSharing.Network.Domain.Context {
    public abstract class FileTransferContextBase
        : IComparable<FileTransferContextBase>, IEquatable<FileTransferContextBase> {
        public Guid TransferID { get; protected set; }

        private bool _isCancelled;

        public bool Cancelled => _isCancelled;

        public FileTransferContextBase(Guid transferID) {
            if (transferID == Guid.Empty) {
                throw new ArgumentException(
                    $"The transfer id can not be empty.",
                    nameof(transferID)
                );
            }
        }

        public int CompareTo(FileTransferContextBase other) {
            if (other is null) {
                return 1;
            }
            return TransferID.CompareTo(other.TransferID);
        }

        public bool Equals(FileTransferContextBase other) {
            if (other is null) {
                return false;
            }
            return TransferID.Equals(other.TransferID);
        }

        public override bool Equals(object obj) {
            if (obj != null && obj is SendFileContext context) {
                return Equals(context);
            }
            return false;
        }

        public override int GetHashCode() {
            return TransferID.GetHashCode();
        }

        public abstract void Close();

        public virtual void Cancel() {
            if (Cancelled) {
                throw new InvalidOperationException();
            }

            _isCancelled = true;
        }
    }
}

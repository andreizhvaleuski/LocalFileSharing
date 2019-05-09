using System;

using LocalFileSharing.Network.Domain.States;

namespace LocalFileSharing.Network.Domain.Progress {
    public class ReceiveFileEventArgs : FileTransferBaseEventArgs {
        public ReceiveFileState ReceiveState { get; protected set; }
        public long BytesRecived { get; protected set; }

        public ReceiveFileEventArgs(
            Guid transferID,
            string filePath,
            long fileSize,
            ReceiveFileState receiveState,
            long bytesRecived
        ) : base(transferID, filePath, fileSize) {
            if (receiveState == ReceiveFileState.Unspecified) {
                throw new ArgumentException(
                    $"The receive state can not be with {ReceiveFileState.Unspecified} value.",
                    nameof(receiveState)
                );
            }

            if (bytesRecived < 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(bytesRecived),
                    bytesRecived,
                    $"The bytes received number can not be negative."
                );
            }

            ReceiveState = receiveState;
            BytesRecived = bytesRecived;
        }
    }
}

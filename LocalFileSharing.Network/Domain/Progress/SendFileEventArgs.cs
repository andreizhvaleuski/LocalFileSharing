using System;

using LocalFileSharing.Network.Domain.States;

namespace LocalFileSharing.Network.Domain.Progress {
    public class SendFileEventArgs : FileTransferBaseEventArgs {
        public SendFileState SendState { get; set; }
        public long BytesSent { get; set; }

        public SendFileEventArgs(
            Guid transferID,
            string filePath,
            long fileSize,
            SendFileState sendState,
            long bytesSent
        ) : base(transferID, filePath, fileSize) {
            if (sendState == SendFileState.Unspecified) {
                throw new ArgumentException(
                    $"The send state can not be with {SendFileState.Unspecified} value.",
                    nameof(sendState)
                );
            }

            if (bytesSent < 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(bytesSent),
                    bytesSent,
                    $"The bytes sent number can not be negative."
                );
            }

            SendState = sendState;
            BytesSent = bytesSent;
        }
    }
}

using System;
using System.IO;

namespace LocalFileSharing.Network.Domain.Progress {
    public class FileTransferBaseEventArgs : EventArgs {
        public Guid TransferID { get; protected set; }
        public string FilePath { get; protected set; }
        public long FileSize { get; protected set; }

        public FileTransferBaseEventArgs(Guid transferID, string filePath, long fileSize) {
            if (transferID == Guid.Empty) {
                throw new ArgumentException(
                    $"The transfer id can not be empty.",
                    nameof(transferID)
                );
            }

            if (string.IsNullOrWhiteSpace(filePath)) {
                throw new ArgumentException(
                    $"The file path can not be empty or null.",
                    nameof(filePath)
                );
            }

            //if (!File.Exists(filePath)) {
            //    throw new FileNotFoundException($"File not found.", filePath);
            //}

            if (fileSize <= 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(fileSize),
                    fileSize,
                    $"The file size have to be positive number."
                );
            }

            TransferID = transferID;
            FilePath = filePath;
            FileSize = fileSize;
        }
    }
}

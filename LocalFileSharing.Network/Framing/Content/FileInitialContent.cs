using System;

namespace LocalFileSharing.Network.Framing.Content {
    [Serializable]
    public class FileInitialContent : FileContentBase {
        public string FileName { get; protected set; }
        public long FileSize { get; protected set; }
        public byte[] FileHash { get; protected set; }

        public FileInitialContent(
            string fileName,
            long fileSize,
            byte[] fileHash
        ) {
            if (string.IsNullOrWhiteSpace(fileName)) {
                throw new ArgumentException(
                    $"The file name can not be null or whitespace.",
                    nameof(fileName)
                );
            }

            if (fileSize <= 0) {
                throw new ArgumentOutOfRangeException(
                    $"The file size must be greater than 0 bytes.",
                    nameof(fileSize)
                );
            }

            if (fileHash is null) {
                throw new ArgumentNullException(nameof(fileHash));
            }

            if (fileHash.Length == 0) {
                throw new ArgumentException(
                    $"File hash can not be empty.",
                    nameof(fileHash)
                );
            }

            FileName = fileName;
            FileSize = fileSize;
            FileHash = fileHash;
        }
    }
}

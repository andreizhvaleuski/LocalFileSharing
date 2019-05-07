using System;

namespace LocalFileSharing.Network.Framing.Content {
    [Serializable]
    public class FileInitialContent : FileContentBase {
        public string FileName { get; protected set; }

        public long FileSize { get; protected set; }

        public FileInitialContent(
            Guid opeartionID,
            string fileName,
            long fileSize,
            byte[] fileHash
        ) : base(opeartionID, fileHash) {
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

            FileName = fileName;
            FileSize = fileSize;
        }

        public FileInitialContent(string fileName, long fileSize, byte[] sha256FileHash)
            : this(Guid.NewGuid(), fileName, fileSize, sha256FileHash) { }
    }
}

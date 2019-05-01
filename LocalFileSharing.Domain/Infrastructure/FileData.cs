using System;

namespace LocalFileSharing.Domain.Infrastructure
{
    public class FileData
    {
        public Guid FileId { get; private set; }

        public string FilePath { get; private set; }

        public long FileSize { get; private set; }

        public FileData(Guid fileId, string filePath, long fileSize)
        {
            if (fileId == Guid.Empty)
            {
                throw new ArgumentException(nameof(fileId));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException(nameof(filePath));
            }

            if (fileSize <= 0)
            {
                throw new ArgumentNullException(nameof(fileSize));
            }

            FileId = fileId;
            FilePath = filePath;
            FileSize = fileSize;
        }
    }
}

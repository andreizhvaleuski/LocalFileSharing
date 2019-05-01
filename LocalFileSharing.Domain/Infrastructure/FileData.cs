using System;

namespace LocalFileSharing.Domain.Infrastructure
{
    public class FileData
    {
        public Guid FileId { get; set; }

        public string FilePath { get; set; }

        public long FileSize { get; set; }

        public byte[] FileSha256Hash { get; set; }
    }
}

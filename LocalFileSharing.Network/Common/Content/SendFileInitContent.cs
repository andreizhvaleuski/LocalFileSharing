using System;
using System.Collections.Generic;

namespace LocalFileSharing.Network.Common.Content
{
    public class SendFileInitContent
    {
        public SendFileInitContent(string fileName, long fileSize, byte[] sha256FileHash)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException(
                    $"The file name can not be null or whitespace.",
                    nameof(fileName)
                );
            }

            if (fileSize <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    $"The file size must be greater than 0 bytes.",
                    nameof(fileSize)
                );
            }

            if (sha256FileHash is null)
            {
                throw new ArgumentNullException(nameof(sha256FileHash));
            }

            if (sha256FileHash.Length <= 0)
            {
                throw new ArgumentException(
                    $"The file hash length must be greater than 0.",
                    nameof(sha256FileHash)
                );
            }

            FileName = fileName;
            FileSize = fileSize;
            Sha256FileHash = sha256FileHash;
        }

        public string FileName { get; private set; }

        public long FileSize { get; private set; }

        public IEnumerable<byte> Sha256FileHash { get; private set; }
    }
}

using System;

namespace LocalFileSharing.Network.Common.Content
{
    public class FileContent
    {
        public Guid FileId { get; private set; }

        public FileContent(Guid fileId)
        {
            if (fileId == Guid.Empty)
            {
                throw new ArgumentException(
                    $"The file id can not be empty.",
                    nameof(fileId)
                );
            }

            FileId = fileId;
        }
    }
}

using System;

namespace LocalFileSharing.Network.Common.Content
{
    public abstract class FileBaseContent
    {
        public Guid FileId { get; private set; }

        public FileBaseContent(Guid fileId)
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

using System;

namespace LocalFileSharing.Network.Common.Content
{
    [Serializable]
    public abstract class FileBaseContent
    {
        public Guid FileId { get; protected set; }

        public FileBaseContent(Guid fileId)
        {
            if (fileId == Guid.Empty)
            {
                throw new ArgumentNullException();
            }

            FileId = fileId;
        }
    }
}

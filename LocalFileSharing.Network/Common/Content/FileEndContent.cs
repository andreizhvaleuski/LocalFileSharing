using System;

namespace LocalFileSharing.Network.Common.Content
{
    [Serializable]
    public class FileEndContent : FileBaseContent
    {
        public FileEndContent(Guid fileId)
            : base(fileId)
        {
        }
    }
}

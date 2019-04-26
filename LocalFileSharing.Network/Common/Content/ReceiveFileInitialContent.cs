using System;

namespace LocalFileSharing.Network.Common.Content
{
    public class ReceiveFileInitialContent : FileContent
    {
        public ReceiveFileInitialContent(Guid fileId)
            : base(fileId)
        {
        }
    }
}

using System;

namespace LocalFileSharing.Network.Common.Content
{
    public class ReceiveFileInitialContent : FileBaseContent
    {
        public ReceiveFileInitialContent(Guid fileId)
            : base(fileId)
        {
        }
    }
}

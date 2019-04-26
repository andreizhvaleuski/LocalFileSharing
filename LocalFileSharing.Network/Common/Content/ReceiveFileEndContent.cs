using System;

namespace LocalFileSharing.Network.Common.Content
{
    public class ReceiveFileEndContent : FileBaseContent
    {
        public ReceiveFileEndContent(Guid fileId)
            : base(fileId)
        {
        }
    }
}

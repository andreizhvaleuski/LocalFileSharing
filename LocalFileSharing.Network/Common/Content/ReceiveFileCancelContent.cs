using System;

namespace LocalFileSharing.Network.Common.Content
{
    public class ReceiveFileCancelContent : FileBaseContent
    {
        public ReceiveFileCancelContent(Guid fileId)
            : base(fileId)
        {
        }
    }
}

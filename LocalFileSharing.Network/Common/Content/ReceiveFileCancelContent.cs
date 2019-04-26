using System;

namespace LocalFileSharing.Network.Common.Content
{
    public class ReceiveFileCancelContent : FileContent
    {
        public ReceiveFileCancelContent(Guid fileId)
            : base(fileId)
        {
        }
    }
}

using System;

namespace LocalFileSharing.Network.Common.Content
{
    public class ReceiveFileEndContent : FileContent
    {
        public ReceiveFileEndContent(Guid fileId)
            : base(fileId)
        {
        }
    }
}

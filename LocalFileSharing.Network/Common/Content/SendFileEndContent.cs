using System;

namespace LocalFileSharing.Network.Common.Content
{
    public class SendFileEndContent : FileBaseContent
    {
        public SendFileEndContent(Guid fileId)
            : base(fileId)
        {
        }
    }
}

using System;

namespace LocalFileSharing.Network.Common.Content
{
    public class SendFileEndContent : FileContent
    {
        public SendFileEndContent(Guid fileId)
            : base(fileId)
        {
        }
    }
}

using System;

namespace LocalFileSharing.Network.Common.Content
{
    public class SendFileCancelContent : FileContent
    {
        public SendFileCancelContent(Guid fileId)
            : base(fileId)
        {
        }
    }
}

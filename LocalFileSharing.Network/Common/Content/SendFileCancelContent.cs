using System;

namespace LocalFileSharing.Network.Common.Content
{
    public class SendFileCancelContent : FileBaseContent
    {
        public SendFileCancelContent(Guid fileId)
            : base(fileId)
        {
        }
    }
}

using System;
using System.Collections.Generic;

namespace LocalFileSharing.Network.Common.Content
{
    public class SendFileRegularContent : FileContent
    {
        public SendFileRegularContent(Guid fileId, byte[] block)
            : base(fileId)
        {
            if (block is null)
            {
                throw new ArgumentNullException(nameof(block));
            }

            if (block.Length <= 0)
            {
                throw new ArgumentException(
                    $"The block length must be greater than 0.",
                    nameof(block)
                );
            }

            Block = block;
        }

        public IReadOnlyCollection<byte> Block { get; private set; }
    }
}

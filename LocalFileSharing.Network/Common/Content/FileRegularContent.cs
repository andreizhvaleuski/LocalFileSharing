using System;

namespace LocalFileSharing.Network.Common.Content {
    [Serializable]
    public class FileRegularContent : FileBaseContent {
        public FileRegularContent(Guid fileId, byte[] block)
            : base(fileId) {
            if (block is null) {
                throw new ArgumentNullException(nameof(block));
            }

            if (block.Length <= 0) {
                throw new ArgumentException(
                    $"The block length must be greater than 0.",
                    nameof(block)
                );
            }

            Block = block;
        }

        public byte[] Block { get; private set; }
    }
}

using System;

namespace LocalFileSharing.Network.Framing.Content {
    [Serializable]
    public class FileRegularContent : FileContentBase {
        public byte[] FileBlock { get; protected set; }

        public FileRegularContent(
            Guid opeartionID, 
            byte[] fileHash,
            byte[] fileBlock
        ) : base(opeartionID, fileHash) {
            if (fileBlock is null) {
                throw new ArgumentNullException(nameof(fileBlock));
            }

            if (fileBlock.Length == 0) {
                throw new ArgumentException(
                    $"The file block can not be empty.",
                    nameof(fileBlock)
                );
            }

            FileBlock = fileBlock;
        }
    }
}

using System;

namespace LocalFileSharing.Network.Framing.Content {
    [Serializable]
    public abstract class FileContentBase : ContentBase {
        public byte[] FileHash { get; protected set; }

        public FileContentBase(Guid operationID, byte[] fileHash)
            : base(operationID) {
            if (fileHash is null) {
                throw new ArgumentNullException(nameof(fileHash));
            }

            if (fileHash.Length == 0) {
                throw new ArgumentException(
                    $"File hash can not be empty.",
                    nameof(fileHash)
                );
            }

            FileHash = fileHash;
        }
    }
}

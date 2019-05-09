using System.IO;

namespace LocalFileSharing.Network.Domain.Context {
    public class ReceiveFileContext : FileTransferContextBase {
        public long BytesReceived { get; set; }
        public BinaryWriter Writer { get; set; }
    }
}

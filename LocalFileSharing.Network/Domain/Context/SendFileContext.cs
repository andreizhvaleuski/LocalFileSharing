using System.IO;

namespace LocalFileSharing.Network.Domain.Context {
    public class SendFileContext : FileTransferContextBase {
        public long BytesSent { get; set; }
        public BinaryReader Reader { get; set; }
    }
}

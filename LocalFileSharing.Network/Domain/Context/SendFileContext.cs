using System.IO;

namespace LocalFileSharing.Network.Domain.Context {
    public class SendFileContext : FileTransferContextBase {
        public long BytesSent { get; set; }
        public BinaryReader Reader { get; protected set; }

        public override void Initialize() {
            if (Initialized) {
                return;
            }
            Reader = new BinaryReader(File.OpenRead(FilePath));
            base.Initialize();
        }

        public override void End() {
            Reader?.Close();
        }
    }
}

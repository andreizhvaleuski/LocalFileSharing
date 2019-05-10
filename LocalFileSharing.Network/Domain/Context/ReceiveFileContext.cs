using System.IO;

namespace LocalFileSharing.Network.Domain.Context {
    public class ReceiveFileContext : FileTransferContextBase {
        public long BytesReceived { get; set; }
        public BinaryWriter Writer { get; protected set; }

        public override void Initialize() {
            if (Initialized) {
                return;
            }
            Writer = new BinaryWriter(File.Create(FilePath));
            base.Initialize();
        }

        public override void End() {
            Writer.Close();
        }
    }
}

using System.IO;

namespace LocalFileSharing.Network.Domain.Context {
    public class ReceiveFileContext : FileTransferContextBase {
        public long BytesReceived { get; set; }
        public BinaryWriter Writer { get; protected set; }

        public override void Initialize(string filePath) {
            if (Initialized) {
                return;
            }
            if (!File.Exists(filePath)) {
                throw new FileNotFoundException();
            }
            FilePath = filePath;
            Writer = new BinaryWriter(File.Create(FilePath));
            base.Initialize();
        }

        public override void End() {
            Writer?.Close();
        }
    }
}

using System;

namespace LocalFileSharing.Network.Domain {
    public class FileData {
        public Guid OperationID { get; set; }

        public string FilePath { get; set; }

        public long FileSize { get; set; }

        public byte[] FileHash { get; set; }
    }
}

using System;

namespace LocalFileSharing.Network.Common.Content {
    [Serializable]
    public class FileCancelContent : FileBaseContent {
        public FileCancelContent(Guid fileId)
            : base(fileId) {
        }
    }
}

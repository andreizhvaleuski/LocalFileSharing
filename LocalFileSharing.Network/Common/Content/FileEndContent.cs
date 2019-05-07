using System;

namespace LocalFileSharing.Network.Common.Content {
    [Serializable]
    public class FileEndContent : ContentBase {
        public FileEndContent(Guid fileId)
            : base(fileId) {
        }
    }
}

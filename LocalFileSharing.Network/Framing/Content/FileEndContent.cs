using System;

namespace LocalFileSharing.Network.Framing.Content {
    [Serializable]
    public class FileEndContent : ContentBase {
        public FileEndContent(Guid fileId)
            : base(fileId) {
        }
    }
}

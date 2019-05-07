using System;

namespace LocalFileSharing.Network.Framing.Content {
    [Serializable]
    public class FileCancelContent : ContentBase {
        public FileCancelContent(Guid fileId)
            : base(fileId) {
        }
    }
}

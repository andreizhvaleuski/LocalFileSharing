using System;

namespace LocalFileSharing.Network.Common.Content {
    [Serializable]
    public class FileCancelContent : ContentBase {
        public FileCancelContent(Guid fileId)
            : base(fileId) {
        }
    }
}

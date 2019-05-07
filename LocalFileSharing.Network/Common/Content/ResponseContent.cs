using System;

namespace LocalFileSharing.Network.Common.Content {
    [Serializable]
    public class ResponseContent : ContentBase {
        public ResponseType Response { get; private set; }

        public ResponseContent(Guid fileId, ResponseType type)
            : base(fileId) {
            Response = type;
        }
    }
}

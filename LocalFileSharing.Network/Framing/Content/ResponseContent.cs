using System;

namespace LocalFileSharing.Network.Framing.Content {
    [Serializable]
    public class ResponseContent : ContentBase {
        public ResponseType ResponseType { get; protected set; }

        public ResponseContent(ResponseType responseType) {
            if (responseType == ResponseType.Unspecified) {
                throw new ArgumentException(
                    $"Response type can not be unspecified.",
                    nameof(responseType)
                );
            }
            ResponseType = responseType;
        }
    }
}

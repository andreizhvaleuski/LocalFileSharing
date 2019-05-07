using System;

namespace LocalFileSharing.Network.Framing.Content {
    [Serializable]
    public class ResponseContent : ContentBase {
        public ResponseType ResponseType { get; protected set; }

        public ResponseContent(Guid operationID, ResponseType responseType)
            : base(operationID) {
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

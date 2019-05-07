using System;

namespace LocalFileSharing.Network.Framing.Content {
    [Serializable]
    public abstract class ContentBase {
        public Guid OperationID { get; protected set; }

        public ContentBase(Guid operationID) {
            if (operationID == Guid.Empty) {
                throw new ArgumentException(
                    $"Operation ID can not be empty.", 
                    nameof(operationID)
                );
            }

            OperationID = operationID;
        }
    }
}

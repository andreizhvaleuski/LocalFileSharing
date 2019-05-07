using System;

namespace LocalFileSharing.Network.Framing.Content {
    [Serializable]
    public abstract class FileContentBase : ContentBase {
        public FileContentBase(Guid operationID)
            : base(operationID) { }
    }
}

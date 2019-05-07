﻿using System;

namespace LocalFileSharing.Network.Framing.Content {
    [Serializable]
    public class FileEndContent : FileContentBase {
        public FileEndContent(
            Guid operationID,
            byte[] fileHash
        ) : base(operationID, fileHash) {}
    }
}

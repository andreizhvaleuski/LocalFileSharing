using System;

using LocalFileSharing.Network.Framing.Content;

namespace LocalFileSharing.Network.Framing {
    public interface IMessageFramer {
        byte[] Frame(Guid transferID, MessageType messageType, ContentBase content);

        void GetFrameComponents(byte[] buffer, out Guid transferID, out MessageType messageType, out ContentBase content);
    }
}

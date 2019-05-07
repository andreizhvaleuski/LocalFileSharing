using LocalFileSharing.Network.Framing.Content;

namespace LocalFileSharing.Network.Framing {
    public interface IContentConverter {
        byte[] GetBytes(ContentBase content);

        ContentBase GetContent(byte[] contentBuffer);
    }
}

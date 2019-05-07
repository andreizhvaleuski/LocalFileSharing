using LocalFileSharing.Network.Common.Content;

namespace LocalFileSharing.Network.Common {
    public interface IContentConverter {
        byte[] GetBytes(ContentBase content);

        ContentBase GetContent(byte[] contentBuffer);
    }
}

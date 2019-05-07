using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using LocalFileSharing.Network.Common.Content;

namespace LocalFileSharing.Network.Common {
    public class ContentConverter : IContentConverter {
        public byte[] GetBytes(ContentBase content) {
            if (content is null) {
                throw new ArgumentNullException(nameof(content));
            }

            byte[] buffer = null;

            using (MemoryStream stream = new MemoryStream()) {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, content);
                buffer = stream.ToArray();
            }

            return buffer;
        }

        public ContentBase GetContent(byte[] contentBuffer) {
            if (contentBuffer is null) {
                throw new ArgumentNullException(nameof(contentBuffer));
            }

            if (contentBuffer.Length == 0) {
                throw new ArgumentException(
                    $"The buffer can not be empty.",
                    nameof(contentBuffer)
                );
            }

            ContentBase content = null;

            using (MemoryStream stream = new MemoryStream(contentBuffer)) {
                IFormatter formatter = new BinaryFormatter();
                content = formatter.Deserialize(stream) as ContentBase;
            }

            return content;
        }
    }
}

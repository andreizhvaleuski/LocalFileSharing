using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalFileSharing.Network.Common.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace LocalFileSharing.Network.Common
{
    public static class FileContentConverter
    {
        public static byte[] GetBytes(FileContent content)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            byte[] buffer = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (BsonDataWriter writer = new BsonDataWriter(ms))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, content);
                }
                buffer = ms.ToArray();
            }

            return buffer;
        }

        public static FileContent GetFileContent(byte[] contentBuffer)
        {
            if (contentBuffer is null)
            {
                throw new ArgumentNullException(nameof(contentBuffer));
            }

            if (contentBuffer.Length <= 0)
            {
                throw new ArgumentException(
                    $"The content buffer cannot be empty.",
                    nameof(contentBuffer)
                );
            }

            FileContent content = null;

            using (MemoryStream ms = new MemoryStream(contentBuffer))
            {
                using (BsonDataReader reader = new BsonDataReader(ms))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    content = serializer.Deserialize<FileContent>(reader);
                }
            }

            return content;
        }
    }
}

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using LocalFileSharing.Network.Common.Content;

namespace LocalFileSharing.Network.Common {
    public static class ContentConverter {
        public static byte[] GetBytes(FileBaseContent content) {
            if (content is null) {
                throw new ArgumentNullException(nameof(content));
            }

            byte[] buffer = null;

            using (MemoryStream ms = new MemoryStream()) {
                IFormatter formatter = new BinaryFormatter();

                formatter.Serialize(ms, content);

                buffer = ms.ToArray();
            }

            return buffer;
        }

        public static FileInitialContent GetSendFileInitialContent(byte[] contentBuffer) {
            return GetFileContent(contentBuffer) as FileInitialContent;
        }

        public static FileRegularContent GetSendFileRegularContent(byte[] contentBuffer) {
            return GetFileContent(contentBuffer) as FileRegularContent;
        }

        public static FileEndContent GetSendFileEndContent(byte[] contentBuffer) {
            return GetFileContent(contentBuffer) as FileEndContent;
        }

        public static FileCancelContent GetSendFileCancelContent(byte[] contentBuffer) {
            return GetFileContent(contentBuffer) as FileCancelContent;
        }

        public static ResponseContent GetResponseContent(byte[] contentBuffer) {
            return GetFileContent(contentBuffer) as ResponseContent;
        }

        public static FileBaseContent GetFileContent(byte[] contentBuffer) {
            if (contentBuffer is null) {
                throw new ArgumentNullException(nameof(contentBuffer));
            }

            if (contentBuffer.Length <= 0) {
                throw new ArgumentException(
                    $"The content buffer cannot be empty.",
                    nameof(contentBuffer)
                );
            }

            FileBaseContent content = null;

            using (MemoryStream ms = new MemoryStream(contentBuffer)) {
                IFormatter formatter = new BinaryFormatter();

                content = formatter.Deserialize(ms) as FileBaseContent;
            }

            return content;
        }
    }
}

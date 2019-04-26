using System;
using System.IO;

using LocalFileSharing.Network.Common;
using LocalFileSharing.Network.Common.Content;

using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

using NUnit.Framework;

namespace LocalFileSharing.UnitTests.Network
{
    [TestFixture]
    public class FileContentConverterTests
    {
        [Test]
        public void GetBytes_ValidFileContent_ReturnsValidBuffer()
        {
            FileContent content = new ReceiveFileCancelContent(Guid.NewGuid());
            MemoryStream ms = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, content);
            }
            byte[] expectedBuffer = ms.ToArray();
            byte[] actualBuffer = FileContentConverter.GetBytes(content);

            Assert.AreEqual(expectedBuffer, actualBuffer);
        }

        [Test]
        public void GetBytes_NullFileContent_ThrowsArgumentNullException()
        {
            FileContent content = null;

            var ex = Assert.Throws<ArgumentNullException>(() => FileContentConverter.GetBytes(content));
            Assert.That(ex.ParamName, Is.EqualTo("content"));
        }

        [Test]
        public void GetFileContent_ValidBuffer_ReturnsValidObject()
        {
            FileContent expectedContent = new ReceiveFileCancelContent(Guid.NewGuid());
            MemoryStream ms = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, expectedContent);
            }
            byte[] buffer = ms.ToArray();

            FileContent actualContent = FileContentConverter.GetFileContent(buffer);

            Assert.AreEqual(expectedContent.FileId, actualContent.FileId);
        }

        [Test]
        public void GetFileContent_NullBuffer_ThrowsArgumentNullException()
        {
            byte[] buffer = null;

            var ex = Assert.Throws<ArgumentNullException>(() => FileContentConverter.GetFileContent(buffer));
            Assert.That(ex.ParamName, Is.EqualTo("contentBuffer"));
        }

        [Test]
        public void GetFileContent_BufferWithInvalidLength_ThrowsArgumentException()
        {
            byte[] buffer = new byte[0];

            var ex = Assert.Throws<ArgumentException>(() => FileContentConverter.GetFileContent(buffer));
            Assert.That(ex.ParamName, Is.EqualTo("contentBuffer"));
        }
    }
}

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using LocalFileSharing.Network.Framing;
using LocalFileSharing.Network.Framing.Content;

using NUnit.Framework;

namespace LocalFileSharing.UnitTests.Network {
    [TestFixture]
    public class ContentConverterTests {
        private ContentConverter contentConverter;

        [OneTimeSetUp]
        public void Init() {
            contentConverter = new ContentConverter();
        }

        [Test]
        public void GetBytes_NullContent_ThrowsArgumentNullException() {
            ContentBase content = null;

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() =>
                contentConverter.GetBytes(content)
            );
            Assert.That(ex.ParamName, Is.EqualTo("content"));
        }

        [Test]
        public void GetBytes_ValidContent_ReturnsBuffer() {
            ContentBase content = new ResponseContent(
                Guid.NewGuid(),
                ResponseType.ReceiveFileInitial
            );

            byte[] actualContentBuffer = contentConverter.GetBytes(content);
            Assert.NotNull(actualContentBuffer);
        }

        [Test]
        public void GetContent_ValidContentBuffer_ReturnsValidContent() {
            ResponseContent expectedContent = new ResponseContent(
                Guid.NewGuid(),
                ResponseType.ReceiveFileInitial
            );
            byte[] contentBuffer = contentConverter.GetBytes(expectedContent);

            ContentBase actualContent = contentConverter.GetContent(contentBuffer);
            Assert.AreEqual(expectedContent, actualContent);
            Assert.IsInstanceOf(expectedContent.GetType(), actualContent);
        }

        [Test]
        public void GetContent_InvalidContentBuffer_ThrowsInvalidCastException() {
            object obj = "hello!";
            byte[] contentBuffer = null;
            using (MemoryStream stream = new MemoryStream()) {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                contentBuffer = stream.ToArray();
            }

            Assert.Throws<InvalidCastException>(() =>
                contentConverter.GetContent(contentBuffer)
            );
        }

        [Test]
        public void GetContent_NullContentBuffer_ThrowsArgumentNullException() {
            byte[] contentBuffer = null;

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() =>
                contentConverter.GetContent(contentBuffer)
            );
            Assert.That(ex.ParamName, Is.EqualTo("contentBuffer"));
        }

        [Test]
        public void GetContent_ContentBufferWithInvalidLength_ThrowsArgumentException() {
            byte[] contentBuffer = new byte[0];

            ArgumentException ex = Assert.Throws<ArgumentException>(() =>
                contentConverter.GetContent(contentBuffer)
            );
            Assert.That(ex.ParamName, Is.EqualTo("contentBuffer"));
        }
    }
}

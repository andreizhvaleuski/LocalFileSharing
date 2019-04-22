using System.Text;

using LocalFileSharing.Network.Common;

using NUnit.Framework;

namespace LocalFileSharing.UnitTests.Network
{
    [TestFixture]
    public class MessageTypeConverterTests
    {
        [TestCase(MessageType.Keepalive, MessageTypeConverter.Keepalive)]
        [TestCase(MessageType.SendFileInitial, MessageTypeConverter.SendFileInitial)]
        [TestCase(MessageType.SendFileRegular, MessageTypeConverter.SendFileRegular)]
        [TestCase(MessageType.SendFileEnd, MessageTypeConverter.SendFileEnd)]
        public void GetBytes_ValidMessageType_ReturnsValidBuffer(MessageType type, string typeShortName)
        {
            byte[] expectedTypeBuffer = Encoding.Unicode.GetBytes(typeShortName);

            byte[] actualTypeBuffer = MessageTypeConverter.GetBytes(type);

            Assert.AreEqual(actualTypeBuffer, expectedTypeBuffer);
        }

        [Test]
        public void GetBytes_InvalidMessageType_ReturnsNullBuffer()
        {
            MessageType type = MessageType.Unspecified;

            byte[] actualTypeBuffer = MessageTypeConverter.GetBytes(type);

            Assert.IsNull(actualTypeBuffer);
        }
    }
}

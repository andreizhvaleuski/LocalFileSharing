using System;
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

        [TestCase(MessageTypeConverter.Keepalive, MessageType.Keepalive)]
        [TestCase(MessageTypeConverter.SendFileInitial, MessageType.SendFileInitial)]
        [TestCase(MessageTypeConverter.SendFileRegular, MessageType.SendFileRegular)]
        [TestCase(MessageTypeConverter.SendFileEnd, MessageType.SendFileEnd)]
        public void GetMessageType_ValidBuffer_ReturnsValidMessageType(string typeShortName, MessageType expectedType)
        {
            byte[] typeBuffer = Encoding.Unicode.GetBytes(typeShortName);

            MessageType actualType = MessageTypeConverter.GetMessageType(typeBuffer);

            Assert.AreEqual(expectedType, actualType);
        }

        [Test]
        public void GetMessageType_NullBuffer_ThrowsArgumentNullException()
        {
            byte[] typeBuffer = null;

            var ex = Assert.Throws<ArgumentNullException>(() => MessageTypeConverter.GetMessageType(typeBuffer));
            Assert.That(ex.ParamName, Is.EqualTo("typeBuffer"));
        }

        [TestCase(0)]
        [TestCase(MessageTypeConverter.MessageTypeLength - 1)]
        [TestCase(MessageTypeConverter.MessageTypeLength + 1)]
        public void GetMessageType_BufferWithInvalidLength_ThrowsArgumentException(int typeBufferLength)
        {
            byte[] typeBuffer = new byte[typeBufferLength];

            var ex = Assert.Throws<ArgumentException>(() => MessageTypeConverter.GetMessageType(typeBuffer));
            Assert.That(ex.ParamName, Is.EqualTo("typeBuffer"));
        }
    }
}

using System;
using System.Text;

using LocalFileSharing.Network.Common;
using LocalFileSharing.Network.Framing.Wrappers;

using NUnit.Framework;

namespace LocalFileSharing.UnitTests.Network {
    [TestFixture]
    public class TypePrefixWrapperTests {
        private TypePrefixWrapper typePrefixWrapper;

        [OneTimeSetUp]
        public void Init() {
            typePrefixWrapper = new TypePrefixWrapper();
        }

        [TestCase("message", MessageType.Keepalive)]
        [TestCase("", MessageType.SendFileEnd)]
        public void Wrap_ValidBuffer_ReturnsWrappedBuffer(string message, MessageType type) {
            byte[] unwrappedBuffer = Encoding.Unicode.GetBytes(message);
            byte[] expectedTypePrefixBuffer = MessageTypeConverter.GetBytes(type);
            byte[] expectedWrappedBuffer =
                new byte[unwrappedBuffer.Length + expectedTypePrefixBuffer.Length];
            expectedTypePrefixBuffer.CopyTo(expectedWrappedBuffer, 0);
            unwrappedBuffer.CopyTo(expectedWrappedBuffer, expectedTypePrefixBuffer.Length);

            byte[] actualWrappedBuffer = typePrefixWrapper.Wrap(unwrappedBuffer, type);

            Assert.AreEqual(expectedWrappedBuffer, actualWrappedBuffer);
        }

        [Test]
        public void Wrap_NullBuffer_ThrowsArgumentNullException() {
            byte[] buffer = null;

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() =>
                typePrefixWrapper.Wrap(buffer, MessageType.Unspecified));
            Assert.That(ex.ParamName, Is.EqualTo("unwrappedBuffer"));
        }

        [Test]
        public void Wrap_MessageTypeUnspecified_ThrowsArgumentException() {
            byte[] buffer = new byte[1];

            ArgumentException ex = Assert.Throws<ArgumentException>(() =>
                typePrefixWrapper.Wrap(buffer, MessageType.Unspecified));
            Assert.That(ex.ParamName, Is.EqualTo("type"));
        }

        [TestCase(MessageType.Keepalive)]
        [TestCase(MessageType.SendFileEnd)]
        [TestCase(MessageType.SendFileRegular)]
        [TestCase(MessageType.SendFileInitial)]
        [TestCase(MessageType.SendFileCancel)]
        [TestCase(MessageType.Response)]
        public void GetTypePrefixValue_ValidBuffer_ReturnsMessageType(MessageType expectedType) {
            byte[] wrappedBuffer = new byte[typePrefixWrapper.TypePrefixSize];
            byte[] typeBuffer = MessageTypeConverter.GetBytes(expectedType);
            typeBuffer.CopyTo(wrappedBuffer, 0);

            MessageType actualType = typePrefixWrapper.GetTypePrefixValue(wrappedBuffer);

            Assert.AreEqual(expectedType, actualType);
        }

        [TestCase(sizeof(int) - 1)]
        [TestCase(0)]
        public void GetTypePrefixValue_BufferWithInvalidLength_ThrowsArgumentNullException(
            int wrappedBufferLength) {
            byte[] wrappedBuffer = new byte[wrappedBufferLength];

            ArgumentException ex = Assert.Throws<ArgumentException>(() =>
                typePrefixWrapper.GetTypePrefixValue(wrappedBuffer));
            Assert.That(ex.ParamName, Is.EqualTo("wrappedBuffer"));
        }

        [Test]
        public void GetTypePrefixValue_NullBuffer_ThrowsArgumentNullException() {
            byte[] wrappedBuffer = null;

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() =>
                typePrefixWrapper.GetTypePrefixValue(wrappedBuffer));
            Assert.That(ex.ParamName, Is.EqualTo("wrappedBuffer"));
        }
    }
}

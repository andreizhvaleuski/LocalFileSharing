using System;
using System.Text;

using LocalFileSharing.Network.Framing.Wrappers;

using NUnit.Framework;

namespace LocalFileSharing.UnitTests.Network {
    [TestFixture]
    public class LengthPrefixWrapperTests {
        private LengthPrefixWrapper lengthPrefixWrapper;

        [OneTimeSetUp]
        public void Init() {
            lengthPrefixWrapper = new LengthPrefixWrapper();
        }

        [TestCase("messsage")]
        [TestCase("")]
        public void Wrap_ValidBuffer_ReturnsWrappedBuffer(string message) {
            byte[] unwrappedBuffer = Encoding.Unicode.GetBytes(message);
            byte[] lengthPrefixBuffer = BitConverter.GetBytes(unwrappedBuffer.Length);
            byte[] expectedWrappedMessageBuffer =
                new byte[unwrappedBuffer.Length + lengthPrefixBuffer.Length];
            lengthPrefixBuffer.CopyTo(expectedWrappedMessageBuffer, 0);
            unwrappedBuffer.CopyTo(expectedWrappedMessageBuffer, lengthPrefixBuffer.Length);

            byte[] actualWrappedMessageBuffer = lengthPrefixWrapper.Wrap(unwrappedBuffer);

            Assert.AreEqual(expectedWrappedMessageBuffer, actualWrappedMessageBuffer);
        }

        [Test]
        public void Wrap_NullBuffer_ThrowsArgumentNullException() {
            byte[] buffer = null;

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() =>
                lengthPrefixWrapper.Wrap(buffer));
            Assert.That(ex.ParamName, Is.EqualTo("unwrappedBuffer"));
        }

        [TestCase(sizeof(int))]
        [TestCase(sizeof(int) + 1)]
        public void Unwrap_ValidBuffer_ReturnsUnwrappedBufferLength(
            int expectedLength) {
            byte[] wrappedBuffer = new byte[sizeof(int)];
            byte[] lengthBuffer = BitConverter.GetBytes(expectedLength);
            lengthBuffer.CopyTo(wrappedBuffer, 0);

            int actualLength = lengthPrefixWrapper.Unwrap(wrappedBuffer);

            Assert.AreEqual(expectedLength, actualLength);
        }

        [TestCase(sizeof(int) - 1)]
        [TestCase(0)]
        public void Unwrap_BufferWithInvalidLength_ThrowsArgumentException(
            int wrappedBufferLength) {
            byte[] wrappedBuffer = new byte[wrappedBufferLength];

            ArgumentException ex = Assert.Throws<ArgumentException>(() =>
                lengthPrefixWrapper.Unwrap(wrappedBuffer));
            Assert.That(ex.ParamName, Is.EqualTo("wrappedBuffer"));
        }

        [Test]
        public void Unwrap_NullBuffer_ThrowsArgumentNullException() {
            byte[] wrappedBuffer = null;

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() =>
                lengthPrefixWrapper.Unwrap(wrappedBuffer));
            Assert.That(ex.ParamName, Is.EqualTo("wrappedBuffer"));
        }
    }
}

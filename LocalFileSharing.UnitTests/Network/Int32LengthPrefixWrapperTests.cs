using System;
using System.Text;

using LocalFileSharing.Network.Framing.Wrappers;

using NUnit.Framework;

namespace LocalFileSharing.UnitTests.Network
{
    [TestFixture]
    public class Int32LengthPrefixWrapperTests
    {
        private LengthPrefixWrapper lengthPrefixWrapper;

        [OneTimeSetUp]
        public void Init()
        {
            lengthPrefixWrapper = new LengthPrefixWrapper();
        }

        [TestCase("messsage")]
        [TestCase("")]
        public void Wrap_ValidBuffer_ReturnsWrappedBuffer(string message)
        {
            byte[] messageBuffer = Encoding.Unicode.GetBytes(message);
            int expectedLength = messageBuffer.Length;

            byte[] wrappedMessageBuffer = lengthPrefixWrapper.Wrap(messageBuffer);
            int actualLength = BitConverter.ToInt32(wrappedMessageBuffer, 0);
            Assert.AreEqual(expectedLength, actualLength);
        }

        [Test]
        public void Wrap_NullBuffer_ThrowsArgumentNullException()
        {
            byte[] buffer = null;

            var ex = Assert.Throws<ArgumentNullException>(() => lengthPrefixWrapper.Wrap(buffer));
            Assert.That(ex.ParamName, Is.EqualTo("unwrappedBuffer"));
        }

        [TestCase(sizeof(int))]
        [TestCase(sizeof(int) + 1)]
        public void GetLengthPrefixValue_ValidBuffer_ReturnsUnwrappedBufferLength(int expectedLength)
        {
            byte[] wrappedBuffer = new byte[sizeof(int)];
            byte[] lengthBuffer = BitConverter.GetBytes(expectedLength);
            lengthBuffer.CopyTo(wrappedBuffer, 0);

            int actualLength = lengthPrefixWrapper.GetLengthPrefixValue(wrappedBuffer);

            Assert.AreEqual(expectedLength, actualLength);
        }

        [TestCase(sizeof(int) - 1)]
        [TestCase(0)]
        public void GetLengthPrefixValue_BufferWithInvalidLength_ThrowsArgumentNullException(int wrappedBufferLength)
        {
            byte[] wrappedBuffer = new byte[wrappedBufferLength];

            var ex = Assert.Throws<ArgumentException>(() => lengthPrefixWrapper.GetLengthPrefixValue(wrappedBuffer));
            Assert.That(ex.ParamName, Is.EqualTo("wrappedBuffer"));
        }

        [Test]
        public void GetLengthPrefixValue_NullBuffer_ThrowsArgumentNullException()
        {
            byte[] wrappedBuffer = null;

            var ex = Assert.Throws<ArgumentNullException>(() => lengthPrefixWrapper.GetLengthPrefixValue(wrappedBuffer));
            Assert.That(ex.ParamName, Is.EqualTo("wrappedBuffer"));
        }
    }
}

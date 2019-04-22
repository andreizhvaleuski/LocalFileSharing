using System;
using System.Collections.Generic;
using System.Text;
using LocalFileSharing.Network.Common;
using LocalFileSharing.Network.Framing.Wrappers;
using NUnit.Framework;

namespace LocalFileSharing.UnitTests.Network
{
    [TestFixture]
    public class TypePrefixWrapperTests
    {
        private TypePrefixWrapper typePrefixWrapper;

        [OneTimeSetUp]
        public void Init()
        {
            typePrefixWrapper = new TypePrefixWrapper();
        }

        [TestCase("message", MessageType.Keepalive)]
        [TestCase("", MessageType.SendFileEnd)]
        public void Wrap_ValidBuffer_ReturnsWrappedBuffer(string message, MessageType type)
        {
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
        public void Wrap_NullBuffer_ThrowsArgumentNullException()
        {
            byte[] buffer = null;

            var ex = Assert.Throws<ArgumentNullException>(() =>
                typePrefixWrapper.Wrap(buffer, MessageType.Unspecified));
            Assert.That(ex.ParamName, Is.EqualTo("unwrappedBuffer"));
        }

        [Test]
        public void Wrap_MessageTypeUnspecified_ThrowsArgumentException()
        {
            byte[] buffer = new byte[1];

            var ex = Assert.Throws<ArgumentException>(() =>
                typePrefixWrapper.Wrap(buffer, MessageType.Unspecified));
            Assert.That(ex.ParamName, Is.EqualTo("type"));
        }
    }
}

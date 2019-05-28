using LocalFileSharing.Network.Framing.Wrappers;

using NUnit.Framework;

namespace LocalFileSharing.UnitTests.Network {
    [TestFixture]
    public class TransferIDPrefixWrapperTests {
        private TransferIDPrefixWrapper _prefixWrapper;

        [OneTimeSetUp]
        public void Init() {
            _prefixWrapper = new TransferIDPrefixWrapper();
        }
    }
}

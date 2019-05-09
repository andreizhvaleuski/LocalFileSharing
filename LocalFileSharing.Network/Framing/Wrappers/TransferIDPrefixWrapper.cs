using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalFileSharing.Network.Framing.Wrappers {
    public class TransferIDPrefixWrapper : ITransferIDPrefixWrapper {
        public int PrefixLength { get; }

        public Guid Unwrap(byte[] wrappedBuffer) {
            throw new NotImplementedException();
        }

        public byte[] Wrap(byte[] unwrappedBuffer, Guid transferID) {
            throw new NotImplementedException();
        }
    }
}

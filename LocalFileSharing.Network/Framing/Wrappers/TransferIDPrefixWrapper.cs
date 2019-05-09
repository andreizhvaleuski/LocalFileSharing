using System;

namespace LocalFileSharing.Network.Framing.Wrappers {
    public class TransferIDPrefixWrapper : ITransferIDPrefixWrapper {
        public int PrefixLength { get; protected set; }

        public TransferIDPrefixWrapper() {
            PrefixLength = Guid.Empty.ToByteArray().Length;
        }

        public byte[] Wrap(byte[] unwrappedBuffer, Guid transferID) {
            if (unwrappedBuffer is null) {
                throw new ArgumentNullException(nameof(unwrappedBuffer));
            }

            if (transferID == Guid.Empty) {
                throw new ArgumentException(
                    $"The transfer id can not be empty.",
                    nameof(transferID)
                );
            }

            byte[] transferIDBuffer = transferID.ToByteArray();

            byte[] wrappedBuffer = new byte[transferIDBuffer.Length + unwrappedBuffer.Length];
            transferIDBuffer.CopyTo(wrappedBuffer, 0);
            unwrappedBuffer.CopyTo(wrappedBuffer, transferIDBuffer.Length);

            return wrappedBuffer;
        }

        public Guid Unwrap(byte[] wrappedBuffer) {
            if (wrappedBuffer is null) {
                throw new ArgumentNullException(nameof(wrappedBuffer));
            }

            if (wrappedBuffer.Length < PrefixLength) {
                throw new ArgumentException(
                    $"The {nameof(wrappedBuffer)} length cannot be less than {PrefixLength} bytes.",
                    nameof(wrappedBuffer)
                );
            }

            Guid transferID = new Guid(wrappedBuffer);
            return transferID;
        }
    }
}

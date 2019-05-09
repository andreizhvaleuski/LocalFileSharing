using System;

namespace LocalFileSharing.Network.Framing.Wrappers {
    public class LengthPrefixWrapper : ILengthPrefixWrapper {
        public int PrefixLength { get; protected set; }

        public LengthPrefixWrapper() {
            PrefixLength = sizeof(int);
        }

        public byte[] Wrap(byte[] unwrappedBuffer) {
            if (unwrappedBuffer is null) {
                throw new ArgumentNullException(nameof(unwrappedBuffer));
            }

            byte[] lengthBuffer = BitConverter.GetBytes(unwrappedBuffer.Length);

            byte[] wrappedBuffer = new byte[lengthBuffer.Length + unwrappedBuffer.Length];
            lengthBuffer.CopyTo(wrappedBuffer, 0);
            unwrappedBuffer.CopyTo(wrappedBuffer, lengthBuffer.Length);

            return wrappedBuffer;
        }

        public int Unwrap(byte[] wrappedBuffer) {
            if (wrappedBuffer is null) {
                throw new ArgumentNullException(nameof(wrappedBuffer));
            }

            if (wrappedBuffer.Length != PrefixLength) {
                throw new ArgumentException(
                    $"The buffer length have to be {PrefixLength} bytes long.",
                    nameof(wrappedBuffer)
                );
            }

            int length = BitConverter.ToInt32(wrappedBuffer, 0);
            return length;
        }
    }
}

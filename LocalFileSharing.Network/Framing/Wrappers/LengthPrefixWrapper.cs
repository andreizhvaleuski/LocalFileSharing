using System;

namespace LocalFileSharing.Network.Framing.Wrappers {
    public class LengthPrefixWrapper : ILengthPrefixWrapper {
        public int LengthPrefixSize => sizeof(int);

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

        public int GetLengthPrefixValue(byte[] wrappedBuffer) {
            if (wrappedBuffer is null) {
                throw new ArgumentNullException(nameof(wrappedBuffer));
            }

            if (wrappedBuffer.Length < LengthPrefixSize) {
                throw new ArgumentException(
                    $"The {nameof(wrappedBuffer)} length cannot be less than {LengthPrefixSize} bytes.",
                    nameof(wrappedBuffer)
                );
            }

            int length = BitConverter.ToInt32(wrappedBuffer, 0);
            return length;
        }
    }
}

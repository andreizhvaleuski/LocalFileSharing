using System;

namespace LocalFileSharing.Network.Framing.Wrappers
{
    public class Int32LengthPrefixWrapper : ILengthPrefixWrapper
    {
        public int LengthPrefixBytes
        {
            get
            {
                return sizeof(Int32);
            }
        }

        public byte[] Wrap(byte[] buffer)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            byte[] lengthBuffer = BitConverter.GetBytes(buffer.Length);

            byte[] wrappedMessage = new byte[lengthBuffer.Length + buffer.Length];
            lengthBuffer.CopyTo(wrappedMessage, 0);
            buffer.CopyTo(wrappedMessage, lengthBuffer.Length);

            return wrappedMessage;
        }

        public int Unwrap(byte[] buffer)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (buffer.Length != LengthPrefixBytes)
            {
                throw new ArgumentException(
                    $"The {nameof(buffer)} must be {LengthPrefixBytes} bytes long.",
                    nameof(buffer)
                );
            }

            int length = BitConverter.ToInt32(buffer, 0);
            return length;
        }
    }
}

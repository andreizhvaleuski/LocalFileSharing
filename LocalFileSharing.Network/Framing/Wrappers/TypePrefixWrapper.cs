using System;

namespace LocalFileSharing.Network.Framing.Wrappers {
    public class TypePrefixWrapper : ITypePrefixWrapper {
        public int PrefixLength { get; protected set; }

        public TypePrefixWrapper() {
            PrefixLength = MessageTypeConverter.MessageTypeLength;
        }

        public byte[] Wrap(byte[] unwrappedBuffer, MessageType type) {
            if (unwrappedBuffer is null) {
                throw new ArgumentNullException(nameof(unwrappedBuffer));
            }

            if (type == MessageType.Unspecified) {
                throw new ArgumentException(
                    $"The argument {nameof(type)} cannot be with {MessageType.Unspecified} value.",
                    nameof(type)
                );
            }

            byte[] typePrefixBuffer = MessageTypeConverter.GetBytes(type);
            byte[] wrappedBuffer = new byte[unwrappedBuffer.Length + typePrefixBuffer.Length];
            typePrefixBuffer.CopyTo(wrappedBuffer, 0);
            unwrappedBuffer.CopyTo(wrappedBuffer, typePrefixBuffer.Length);

            return wrappedBuffer;
        }

        public MessageType Unwrap(byte[] wrappedBuffer) {
            if (wrappedBuffer is null) {
                throw new ArgumentNullException(nameof(wrappedBuffer));
            }

            if (wrappedBuffer.Length < PrefixLength) {
                throw new ArgumentException(
                    $"The {nameof(wrappedBuffer)} length cannot be less than {PrefixLength} bytes.",
                    nameof(wrappedBuffer)
                );
            }

            MessageType type = MessageTypeConverter.GetMessageType(wrappedBuffer);
            return type;
        }
    }
}

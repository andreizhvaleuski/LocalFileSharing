using System;

using LocalFileSharing.Network.Common;

namespace LocalFileSharing.Network.Framing.Wrappers {
    public class TypePrefixWrapper : ITypePrefixWrapper {
        public int TypePrefixSize => MessageTypeConverter.MessageTypeLength;

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

        public MessageType GetTypePrefixValue(byte[] wrappedBuffer) {
            if (wrappedBuffer is null) {
                throw new ArgumentNullException(nameof(wrappedBuffer));
            }

            if (wrappedBuffer.Length < TypePrefixSize) {
                throw new ArgumentException(
                    $"The {nameof(wrappedBuffer)} length cannot be less than {TypePrefixSize} bytes.",
                    nameof(wrappedBuffer)
                );
            }

            MessageType type = MessageTypeConverter.GetMessageType(wrappedBuffer);

            return type;
        }
    }
}

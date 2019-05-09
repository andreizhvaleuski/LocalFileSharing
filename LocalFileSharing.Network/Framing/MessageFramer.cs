using System;

using LocalFileSharing.Network.Framing.Content;
using LocalFileSharing.Network.Framing.Wrappers;

namespace LocalFileSharing.Network.Framing {
    public class MessageFramer : IMessageFramer {
        public ILengthPrefixWrapper LengthPrefixWrapper { get; protected set; }
        public ITransferIDPrefixWrapper TransferIDPrefixWrapper { get; protected set; }
        public ITypePrefixWrapper TypePrefixWrapper { get; protected set; }
        public IContentConverter ContentConverter { get; protected set; }

        public MessageFramer(
            ILengthPrefixWrapper lengthPrefixWrapper,
            ITransferIDPrefixWrapper transferIDPrefixWrapper,
            ITypePrefixWrapper typePrefixWrapper,
            IContentConverter contentConverter
        ) {
            if (lengthPrefixWrapper is null) {
                throw new ArgumentNullException(nameof(lengthPrefixWrapper));
            }

            if (transferIDPrefixWrapper is null) {
                throw new ArgumentNullException(nameof(transferIDPrefixWrapper));
            }

            if (typePrefixWrapper is null) {
                throw new ArgumentNullException(nameof(typePrefixWrapper));
            }

            if (contentConverter is null) {
                throw new ArgumentNullException(nameof(contentConverter));
            }

            LengthPrefixWrapper = lengthPrefixWrapper;
            TransferIDPrefixWrapper = transferIDPrefixWrapper;
            TypePrefixWrapper = typePrefixWrapper;
            ContentConverter = contentConverter;
        }

        public byte[] Frame(Guid transferID, MessageType messageType, ContentBase content) {
            if (transferID == Guid.Empty) {
                throw new ArgumentException(
                    $"The transfer id can not be empty.",
                    nameof(transferID)
                );
            }

            if (messageType == MessageType.Unspecified) {
                throw new ArgumentException(
                    $"The message type can not be with {MessageType.Unspecified} value.",
                    nameof(messageType)
                );
            }

            byte[] wrappedBuffer = new byte[0];
            if (content is null) {
                wrappedBuffer = ContentConverter.GetBytes(content);
            }
            wrappedBuffer = TypePrefixWrapper.Wrap(wrappedBuffer, messageType);
            wrappedBuffer = TransferIDPrefixWrapper.Wrap(wrappedBuffer, transferID);
            wrappedBuffer = LengthPrefixWrapper.Wrap(wrappedBuffer);

            return wrappedBuffer;
        }

        public void GetFrameComponents(
            byte[] buffer,
            out Guid transferID,
            out MessageType messageType,
            out ContentBase content
        ) {
            if (buffer is null) {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (buffer.Length == 0) {
                throw new ArgumentException(
                    $"The buffer can not be empty.",
                    nameof(buffer)
                );
            }

            byte[] transferIDBuffer = new byte[TransferIDPrefixWrapper.PrefixLength];
            Array.Copy(buffer, transferIDBuffer, TransferIDPrefixWrapper.PrefixLength);
            transferID = TransferIDPrefixWrapper.Unwrap(transferIDBuffer);

            byte[] messageTypeBuffer = new byte[TypePrefixWrapper.PrefixLength];
            Array.Copy(
                buffer,
                TransferIDPrefixWrapper.PrefixLength,
                messageTypeBuffer,
                0,
                TypePrefixWrapper.PrefixLength
            );
            messageType = TypePrefixWrapper.Unwrap(messageTypeBuffer);

            int contentBufferLength = buffer.Length - transferIDBuffer.Length - messageTypeBuffer.Length;
            if (contentBufferLength == 0) {
                content = null;
                return;
            }
            byte[] contentBuffer = new byte[contentBufferLength];
            Array.Copy(
                buffer,
                TransferIDPrefixWrapper.PrefixLength + TypePrefixWrapper.PrefixLength,
                contentBuffer,
                0,
                contentBufferLength
            );
            content = ContentConverter.GetContent(contentBuffer);
        }
    }
}

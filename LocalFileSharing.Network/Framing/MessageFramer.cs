using System;

using LocalFileSharing.Network.Framing.Content;
using LocalFileSharing.Network.Framing.Wrappers;

namespace LocalFileSharing.Network.Framing {
    public class MessageFramer : IMessageFramer {
        protected readonly ILengthPrefixWrapper _lengthPrefixWrapper;
        protected readonly ITransferIDPrefixWrapper _transferIDPrefixWrapper;
        protected readonly ITypePrefixWrapper _typePrefixWrapper;
        protected readonly IContentConverter _contentConverter;

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

            _lengthPrefixWrapper = lengthPrefixWrapper;
            _transferIDPrefixWrapper = transferIDPrefixWrapper;
            _typePrefixWrapper = typePrefixWrapper;
            _contentConverter = contentConverter;
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

            if (content == null) {
                throw new ArgumentNullException(nameof(content));
            }

            byte[] wrappedBuffer = _contentConverter.GetBytes(content);
            wrappedBuffer = _typePrefixWrapper.Wrap(wrappedBuffer, messageType);
            wrappedBuffer = _transferIDPrefixWrapper.Wrap(wrappedBuffer, transferID);
            wrappedBuffer = _lengthPrefixWrapper.Wrap(wrappedBuffer);

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

            byte[] transferIDBuffer = new byte[_transferIDPrefixWrapper.PrefixLength];
            Array.Copy(buffer, transferIDBuffer, _transferIDPrefixWrapper.PrefixLength);
            transferID = _transferIDPrefixWrapper.Unwrap(transferIDBuffer);

            byte[] messageTypeBuffer = new byte[_typePrefixWrapper.PrefixLength];
            Array.Copy(
                buffer,
                _transferIDPrefixWrapper.PrefixLength,
                messageTypeBuffer,
                0,
                _typePrefixWrapper.PrefixLength
            );
            messageType = _typePrefixWrapper.Unwrap(messageTypeBuffer);

            int contentBufferLength = buffer.Length - transferIDBuffer.Length - messageTypeBuffer.Length;
            byte[] contentBuffer = new byte[contentBufferLength];
            Array.Copy(
                buffer,
                _transferIDPrefixWrapper.PrefixLength + _typePrefixWrapper.PrefixLength,
                contentBuffer,
                0,
                contentBufferLength
            );
            content = _contentConverter.GetContent(contentBuffer);
        }
    }
}

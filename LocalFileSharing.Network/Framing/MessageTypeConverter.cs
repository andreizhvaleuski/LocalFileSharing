using System;
using System.Text;

namespace LocalFileSharing.Network.Framing {
    public static class MessageTypeConverter {
        public const int MessageTypeLength = 6;

        public const string Keepalive = "KPA";
        public const string SendFileInitial = "SFI";
        public const string SendFileRegular = "SFR";
        public const string SendFileEnd = "SFE";
        public const string SendFileCancel = "SFC";
        public const string Response = "RSP";

        public static byte[] GetBytes(MessageType type) {
            byte[] typeBuffer = null;

            if (type == MessageType.Keepalive) {
                typeBuffer = Encoding.Unicode.GetBytes(Keepalive);
            }
            else if (type == MessageType.SendFileInitial) {
                typeBuffer = Encoding.Unicode.GetBytes(SendFileInitial);
            }
            else if (type == MessageType.SendFileRegular) {
                typeBuffer = Encoding.Unicode.GetBytes(SendFileRegular);
            }
            else if (type == MessageType.SendFileEnd) {
                typeBuffer = Encoding.Unicode.GetBytes(SendFileEnd);
            }
            else if (type == MessageType.SendFileCancel) {
                typeBuffer = Encoding.Unicode.GetBytes(SendFileCancel);
            }
            else if (type == MessageType.Response) {
                typeBuffer = Encoding.Unicode.GetBytes(Response);
            }

            return typeBuffer;
        }

        public static MessageType GetMessageType(byte[] typeBuffer) {
            if (typeBuffer is null) {
                throw new ArgumentNullException(nameof(typeBuffer));
            }

            if (typeBuffer.Length != MessageTypeLength) {
                throw new ArgumentException(
                    $"The {typeBuffer} must be {MessageTypeLength} bytes long.",
                    nameof(typeBuffer)
                );
            }

            string message = Encoding.Unicode.GetString(typeBuffer);
            MessageType type = MessageType.Unspecified;

            if (message.Equals(Keepalive, StringComparison.InvariantCultureIgnoreCase)) {
                type = MessageType.Keepalive;
            }
            else if (message.Equals(SendFileInitial, StringComparison.InvariantCultureIgnoreCase)) {
                type = MessageType.SendFileInitial;
            }
            else if (message.Equals(SendFileRegular, StringComparison.InvariantCultureIgnoreCase)) {
                type = MessageType.SendFileRegular;
            }
            else if (message.Equals(SendFileEnd, StringComparison.InvariantCultureIgnoreCase)) {
                type = MessageType.SendFileEnd;
            }
            else if (message.Equals(SendFileCancel, StringComparison.InvariantCultureIgnoreCase)) {
                type = MessageType.SendFileCancel;
            }
            else if (message.Equals(Response, StringComparison.InvariantCultureIgnoreCase)) {
                type = MessageType.Response;
            }

            return type;
        }
    }
}

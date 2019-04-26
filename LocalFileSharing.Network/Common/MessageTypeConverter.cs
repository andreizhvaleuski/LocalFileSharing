using System;
using System.Text;

namespace LocalFileSharing.Network.Common
{
    public static class MessageTypeConverter
    {
        public const int MessageTypeLength = 6;

        public const string Keepalive = "KPA";
        public const string SendFileInitial = "SFI";
        public const string SendFileRegular = "SFR";
        public const string SendFileEnd = "SFE";
        public const string SendFileCancel = "SFC";
        public const string ReceiveFileInitial = "RFI";
        public const string ReceiveFileRegular = "RFR";
        public const string ReceiveFileEnd = "RFE";
        public const string ReceiveFileCancel = "RFC";

        public static byte[] GetBytes(MessageType type)
        {
            byte[] typeBuffer = null;

            if (type == MessageType.Keepalive)
            {
                typeBuffer = Encoding.Unicode.GetBytes(Keepalive);
            }
            else if (type == MessageType.SendFileInitial)
            {
                typeBuffer = Encoding.Unicode.GetBytes(SendFileInitial);
            }
            else if (type == MessageType.SendFileRegular)
            {
                typeBuffer = Encoding.Unicode.GetBytes(SendFileRegular);
            }
            else if (type == MessageType.SendFileEnd)
            {
                typeBuffer = Encoding.Unicode.GetBytes(SendFileEnd);
            }
            else if (type == MessageType.SendFileCancel)
            {
                typeBuffer = Encoding.Unicode.GetBytes(SendFileCancel);
            }
            else if (type == MessageType.ReceiveFileInitial)
            {
                typeBuffer = Encoding.Unicode.GetBytes(ReceiveFileInitial);
            }
            else if (type == MessageType.ReceiveFileRegular)
            {
                typeBuffer = Encoding.Unicode.GetBytes(ReceiveFileRegular);
            }
            else if (type == MessageType.ReceiveFileEnd)
            {
                typeBuffer = Encoding.Unicode.GetBytes(ReceiveFileEnd);
            }
            else if (type == MessageType.ReceiveFileCancel)
            {
                typeBuffer = Encoding.Unicode.GetBytes(ReceiveFileCancel);
            }

            return typeBuffer;
        }

        public static MessageType GetMessageType(byte[] typeBuffer)
        {
            if (typeBuffer is null)
            {
                throw new ArgumentNullException(nameof(typeBuffer));
            }

            if (typeBuffer.Length != MessageTypeLength)
            {
                throw new ArgumentException(
                    $"The {typeBuffer} must be {MessageTypeLength} bytes long.",
                    nameof(typeBuffer)
                );
            }

            string message = Encoding.Unicode.GetString(typeBuffer);
            MessageType type = MessageType.Unspecified;

            if (message.Equals(Keepalive, StringComparison.InvariantCultureIgnoreCase))
            {
                type = MessageType.Keepalive;
            }
            else if (message.Equals(SendFileInitial, StringComparison.InvariantCultureIgnoreCase))
            {
                type = MessageType.SendFileInitial;
            }
            else if (message.Equals(SendFileRegular, StringComparison.InvariantCultureIgnoreCase))
            {
                type = MessageType.SendFileRegular;
            }
            else if (message.Equals(SendFileEnd, StringComparison.InvariantCultureIgnoreCase))
            {
                type = MessageType.SendFileEnd;
            }
            else if (message.Equals(SendFileCancel, StringComparison.InvariantCultureIgnoreCase))
            {
                type = MessageType.SendFileCancel;
            }
            else if (message.Equals(ReceiveFileInitial, StringComparison.InvariantCultureIgnoreCase))
            {
                type = MessageType.ReceiveFileInitial;
            }
            else if (message.Equals(ReceiveFileRegular, StringComparison.InvariantCultureIgnoreCase))
            {
                type = MessageType.ReceiveFileRegular;
            }
            else if (message.Equals(ReceiveFileEnd, StringComparison.InvariantCultureIgnoreCase))
            {
                type = MessageType.ReceiveFileEnd;
            }
            else if (message.Equals(ReceiveFileCancel, StringComparison.InvariantCultureIgnoreCase))
            {
                type = MessageType.ReceiveFileCancel;
            }

            return type;
        }
    }
}

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using LocalFileSharing.Network.Content;
using LocalFileSharing.Network.Framing;

namespace LocalFileSharing.Network.Sockets
{
    public class TcpClient : TcpSocketBase
    {
        protected readonly Socket client;

        public TcpClient(
            IPEndPoint endPoint,
            ILengthPrefixWrapper lengthPrefixWrapper,
            ITypePrefixWrapper typePrefixWrapper
        ) : base(lengthPrefixWrapper, typePrefixWrapper) {
            if (endPoint is null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            client = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            client.Connect(endPoint);
        }

        public void StartReceiveMessages()
        {
            while (true)
            {
                byte[] lengthBuffer = ReceiveBytes(4);
                int length = BitConverter.ToInt32(lengthBuffer, 0);
                byte[] buffer = ReceiveBytes(length);
                DemoType type = (DemoType)BytesToObject(buffer);
                Console.WriteLine($"FileName: {type.FileName}; BlockNumber: {type.BlocksNumber}");
            }
        }

        protected int ReceiveMessageLength()
        {
            byte[] messageLengthBuffer = ReceiveBytes(4);
            int messageLength = BitConverter.ToInt32(messageLengthBuffer, 0);
            return messageLength;
        }

        protected string ReceiveMessageType()
        {
            byte[] messageTypeBuffer = ReceiveBytes(4);
            string messageType = Encoding.Unicode.GetString(messageTypeBuffer);
            return messageType;
        }

        protected byte[] ReceiveBytes(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            byte[] buffer = new byte[count];
            int totalReceivedBytesNumber = 0;
            do
            {
                int bytesReceivedNumber = client.Receive(
                    buffer,
                    totalReceivedBytesNumber,
                    count - totalReceivedBytesNumber,
                    SocketFlags.None
                );
                if (bytesReceivedNumber <= 0)
                {
                    return null;
                }
                totalReceivedBytesNumber += bytesReceivedNumber;
            } while (totalReceivedBytesNumber < count);

            return buffer;
        }

        protected object BytesToObject(byte[] buffer)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            IFormatter formatter = new BinaryFormatter();

            object deserializedObj = null;
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                deserializedObj = formatter.Deserialize(stream);
            }

            return deserializedObj;
        }
    }
}

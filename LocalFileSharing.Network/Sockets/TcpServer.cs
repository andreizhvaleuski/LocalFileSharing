using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

using LocalFileSharing.Network.Content;
using LocalFileSharing.Network.Framing;

namespace LocalFileSharing.Network.Sockets
{
    public class TcpServer : TcpSocketBase
    {
        public const int MinAllowedPort = 61001;

        public const int MaxAllowedPort = 65535;

        protected readonly Socket listener;

        protected Socket connectedClient;

        public TcpServer(
            IPAddress localIPAddress, 
            int port,
            ILengthPrefixWrapper lengthPrefixWrapper,
            ITypePrefixWrapper typePrefixWrapper
        ) : base(lengthPrefixWrapper, typePrefixWrapper)
        {
            if (localIPAddress is null)
            {
                throw new ArgumentNullException(nameof(localIPAddress));
            }

            if (port >= MinAllowedPort && port <= MaxAllowedPort)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(port),
                    port,
                    $"Port number have to be between {MinAllowedPort} and {MaxAllowedPort}."
                );
            }

            listener = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );
            listener.Bind(new IPEndPoint(localIPAddress, port));
        }

        public void StartListening()
        {
            listener.Listen(1);

            connectedClient = listener.Accept();
        }

        public void StopListening()
        {
            connectedClient?.Close();
            listener.Close();
        }

        public void StartSendingMessages()
        {
            while (true)
            {
                Thread.Sleep(2000);
                DemoType type = new DemoType()
                {
                    FileName = DateTime.Now.ToLongTimeString(),
                    BlocksNumber = DateTime.Now.Ticks
                };
                SendObject(type);
            }
        }

        public void SendObject(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            IFormatter formatter = new BinaryFormatter();

            byte[] buffer = null;
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                buffer = stream.ToArray();
            }
            connectedClient.Send(MessageFraming.WrapMessage(buffer));
        }
    }
}

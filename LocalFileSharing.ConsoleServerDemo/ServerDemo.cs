using System;
using System.Net;

using LocalFileSharing.Network.Sockets;

namespace LocalFileSharing.ConsoleServerDemo
{
    class ServerDemo
    {
        static void Main(string[] args)
        {
            TcpServer server = new TcpServer(
                new IPEndPoint(
                    IPAddress.Loopback,
                    60000
            ));

            Console.WriteLine("Listening...");
            server.StartListening();

            Console.WriteLine("Sending...");
            server.StartSendingMessages();
            server.StopListening();
        }
    }
}

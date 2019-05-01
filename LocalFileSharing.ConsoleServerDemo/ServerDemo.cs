using System;
using System.Net;
using LocalFileSharing.Domain;
using LocalFileSharing.Network.Sockets;

namespace LocalFileSharing.ConsoleServerDemo
{
    class ServerDemo
    {
        static void Main(string[] args)
        {
            TcpServer server = new TcpServer(
                IPAddress.Loopback,
                60000
            );

            Console.WriteLine("Listening...");
            server.StartListening(1);
            TcpClient client = server.AcceptTcpClient();
            FileSharingClient fileSharing = new FileSharingClient(client);
            Console.WriteLine("Sending...");
            fileSharing.SendFile(@"D:\Downloads\yennefer_by_shalizeh_d96qk0j.jpg");
            while (true) ;
            server.StopListening();
        }
    }
}

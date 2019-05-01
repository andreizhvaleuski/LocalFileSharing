using System;
using System.Net;
using LocalFileSharing.Domain;
using LocalFileSharing.Network.Sockets;

namespace LocalFileSharing.ConsoleServerDemo
{
    class ServerDemo
    {
        const string Path = @"D:\Downloads\Torrents\Series\Game of Thrones - Игра престолов. Сезон 8. Amedia. 1080p\Game.of.Thrones.s08e01.WEB-DL.1080p.Amedia.mkv";

        static void Main(string[] args)
        {
            Start();
            while (true) ;
        }

        private static async void Start()
        {
             TcpServer server = new TcpServer(
                IPAddress.Any,
                60000
            );
            Console.WriteLine("Listening...");
            server.StartListening(1);
            TcpClient client = server.AcceptTcpClient();
            FileSharingClient fileSharing = new FileSharingClient(client);
            Console.WriteLine("Sending...");
            await fileSharing.SendFileAsync(Path, null);
            Console.WriteLine("Completed");
            server.StopListening();
            Console.WriteLine("Stopped");
        }
    }
}

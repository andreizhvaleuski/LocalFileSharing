using System;
using System.Net;
using System.Threading;
using LocalFileSharing.Domain;
using LocalFileSharing.Domain.Infrastructure;
using LocalFileSharing.Network.Sockets;

namespace LocalFileSharing.ConsoleServerDemo
{
    class ServerDemo
    {
        readonly static CancellationTokenSource cancellationSource = new CancellationTokenSource();

        const string Path = @"D:\Downloads\stephen-king.jpg";

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
            var progress = new Progress<SendFileProgressReport>();
            progress.ProgressChanged += ProgressReport;
            await fileSharing.SendFileAsync(Path, progress, cancellationSource.Token);
            Console.WriteLine("Completed");
            server.StopListening();
            Console.WriteLine("Stopped");
        }

        private static void ProgressReport(object sender, SendFileProgressReport e)
        {
            if (e.SendFileState == SendFileState.Initializing)
            {
                Console.WriteLine($"{e.SendFileState} :: {e.FileData.FileId} :: {e.FileData.FilePath} :: {e.FileData.FileSize}");
            }
            else if (e.SendFileState == SendFileState.Hashing)
            {
                Console.WriteLine($"{e.SendFileState} :: {e?.FileData.FileSha256Hash}");
            }
            else
            {
                Console.WriteLine($"{e.SendFileState} :: {e?.BytesSent}");
            }
        }
    }
}

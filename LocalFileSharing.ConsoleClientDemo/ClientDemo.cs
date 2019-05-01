using System;
using System.Net;
using System.Threading;
using LocalFileSharing.Domain;
using LocalFileSharing.Domain.Infrastructure;

namespace LocalFileSharing.ConsoleClientDemo
{
    class ClientDemo
    {
        static CancellationTokenSource tokenSource = new CancellationTokenSource();

        static void Main(string[] args)
        {
            Start();

            while (true) ;
        }

        static async void Start()
        {
            FileSharingClient fileSharing = new FileSharingClient(IPAddress.Parse("192.168.100.164"), 60000);

            var progress = new Progress<ReceiveFileProgressReport>();
            progress.ProgressChanged += ProgressReport;

            await fileSharing.ReceiveFileAsync(@"F:\", progress, tokenSource.Token);
        }


        private static void ProgressReport(object sender, ReceiveFileProgressReport e)
        {
            if (e.ReceiveFileState == ReceiveFileState.Initializing)
            {
                Console.WriteLine($"{e.ReceiveFileState} :: {e.FileData.FileId} :: {e.FileData.FilePath} :: {e.FileData.FileSize}");
            }
            else if (e.ReceiveFileState == ReceiveFileState.Hashing)
            {
                Console.WriteLine($"{e.ReceiveFileState} :: {e?.FileData.FileSha256Hash}");
            }
            else
            {
                Console.WriteLine($"{e.ReceiveFileState} :: {e?.ReceiveFileState}");
            }
        }
    }
}

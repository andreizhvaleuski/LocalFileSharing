using System.Net;
using LocalFileSharing.Domain;

namespace LocalFileSharing.ConsoleClientDemo
{
    class ClientDemo
    {
        static void Main(string[] args)
        {
            FileSharingClient fileSharing = new FileSharingClient(IPAddress.Loopback, 60000);

            fileSharing.ReceiveFile(@"D:\");
        }
    }
}

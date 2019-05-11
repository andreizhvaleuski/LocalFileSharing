using System;
using System.Net;
using System.Threading.Tasks;

using LocalFileSharing.Network.Domain;
using LocalFileSharing.Network.Domain.Progress;
using LocalFileSharing.Network.Domain.States;

namespace ClientReceive {
    class Program {
        static FileSharingClient  client;
        static void Main(string[] args) {
            Start();
            while(true) ;
        }

        static async void Start() {
            await Task.Run(() => {
                client = new FileSharingClient(new IPEndPoint(IPAddress.Parse("192.168.100.164"), 61001));
                client.SetDownloadDirectory(@"D:\LFS-Tests\Server");
                client.FileReceive += Output;
            });
        }

        private static void Output(object sender, ReceiveFileEventArgs e) {
            Console.WriteLine($"{e.TransferID} :: {e.ReceiveState} :: {e.FilePath} :: {e.FileSize} :: {e.BytesRecived}");
            if (e.ReceiveState == ReceiveFileState.Initializing) {
                client.InitializeReceive(e.TransferID);
            }
            else if (e.ReceiveState == ReceiveFileState.Receiving) {
            }
            else if (e.ReceiveState == ReceiveFileState.Hashing) {

            }
            else if (e.ReceiveState == ReceiveFileState.Completed) {

            }
            else if (e.ReceiveState == ReceiveFileState.Cancelled) {

            }
            else if (e.ReceiveState == ReceiveFileState.Failed) {

            }
        }
    }
}

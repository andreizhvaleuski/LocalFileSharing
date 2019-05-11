using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using LocalFileSharing.Network.Domain;
using LocalFileSharing.Network.Domain.Progress;
using LocalFileSharing.Network.Domain.States;

namespace ClientReceive {
    class Program {
        static ConcurrentDictionary<Guid, ConsoleColor> consColors = new ConcurrentDictionary<Guid, ConsoleColor>();
        static int s = 1;

        static FileSharingClient  client;
        static void Main(string[] args) {
            Start();
            while(true) ;
        }

        static async void Start() {
            await Task.Run(() => {
                client = new FileSharingClient(new IPEndPoint(IPAddress.Parse("192.168.100.164"), 61001));
                client.SetDownloadDirectory(@"D:\LFS-Tests\Receive");
                client.FileReceive += Output;
            });
        }

        private static void Output(object sender, ReceiveFileEventArgs e) {
            if (!consColors.ContainsKey(e.TransferID)) {
                client.InitializeReceive(e.TransferID);
                consColors.TryAdd(e.TransferID, (ConsoleColor)s);
                s++;
            }
            if (e.ReceiveState == ReceiveFileState.Initializing) {
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
            Console.ForegroundColor = consColors[e.TransferID];
            Console.WriteLine($"{e.TransferID} :: {e.ReceiveState} :: {e.FilePath} :: {e.FileSize} :: {e.BytesRecived}");
        }
    }
}

using System;
using System.Collections.Generic;
using LocalFileSharing.Network.Domain;
using LocalFileSharing.Network.Domain.Progress;
using LocalFileSharing.Network.Domain.States;

namespace ClientSend {
    class Program {
        static Dictionary<Guid, ConsoleColor> consColors = new Dictionary<Guid, ConsoleColor>();
        static int s = 1;

        static FileSharingServer server;
        static FileSharingClient client;
        static void Main(string[] args) {
            Start();
            while (true) ;
        }

        static async void Start() {
            server = new FileSharingServer();
            var x = await server.GetServerIPEndPointAsync();
            x.Port = 61001;
            client = await server.ListenAsync(x);
            client.FileSend += Output;
            client.SendFileAsync(@"D:\LFS-Tests\Send\ndp48-devpack-enu.exe");
            client.SendFileAsync(@"D:\LFS-Tests\Send\dotnet-sdk-3.0.100-preview5-011568-win-x64.exe");
            client.SendFileAsync(@"D:\LFS-Tests\Send\VSCodeUserSetup-x64-1.33.1.exe");
            client.SendFileAsync(@"D:\LFS-Tests\Send\jetbrains-toolbox-1.14.5179.exe");
        }

        private static void Output(object sender, SendFileEventArgs e) {
            if (e.SendState == SendFileState.Initializing) {
                consColors.Add(e.TransferID, (ConsoleColor)s);
                s++;
            }
            else if (e.SendState == SendFileState.Sending) {
            }
            else if (e.SendState == SendFileState.Hashing) {

            }
            else if (e.SendState == SendFileState.Failed) {

            }
            else if (e.SendState == SendFileState.Cancelled) {

            }
            else if (e.SendState == SendFileState.Completed) {

            }
            Console.ForegroundColor = consColors[e.TransferID];
            Console.WriteLine($"{e.TransferID} :: {e.SendState} :: {e.FilePath} :: {e.FileSize} :: {e.BytesSent}");
        }
    }
}

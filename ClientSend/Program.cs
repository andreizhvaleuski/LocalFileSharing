using LocalFileSharing.Network.Domain;

namespace ClientSend {
    class Program {
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
            client.SetDownloadDirectory(@"D:\LFS-Tests\Client");
            await client.SendFile(@"D:\Downloads\qbittorrent_4.1.6_x64_setup.exe");
        }
    }
}

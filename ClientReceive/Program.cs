using System.Net;
using System.Threading.Tasks;
using LocalFileSharing.Network.Domain;

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
            });
        }
    }
}

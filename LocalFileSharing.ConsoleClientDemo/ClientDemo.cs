using System.Net;

using LocalFileSharing.Network.Sockets;

namespace LocalFileSharing.ConsoleClientDemo
{
    class ClientDemo
    {
        static void Main(string[] args)
        {
            TcpClientSocket client = new TcpClientSocket(
                new IPEndPoint(
                    IPAddress.Loopback, 
                    60000
            ));

            client.StartReceiveMessages();
        }
    }
}

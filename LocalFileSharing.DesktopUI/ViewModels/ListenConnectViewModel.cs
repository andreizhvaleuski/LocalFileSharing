using System.Net;
using System.Net.Sockets;

using Caliburn.Micro;

namespace LocalFileSharing.DesktopUI.ViewModels {
    public class ListenConnectViewModel : Screen {
        private string _listenIP;

        private int _listenPort;

        private string _connectIP;

        private int _connectPort;

        public ListenConnectViewModel() {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0)) {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            ListenIP = localIP;
            ListenPort = 65000;
        }

        public int ListenPort {
            get => _listenPort;
            private set {
                if (_listenPort == value) {
                    return;
                }

                _listenPort = value;
                NotifyOfPropertyChange(() => ListenPort);
                NotifyOfPropertyChange(() => CanListen);
            }
        }

        public string ListenIP {
            get => _listenIP;
            private set {
                if (_listenIP == value) {
                    return;
                }

                _listenIP = value;
                NotifyOfPropertyChange(() => ListenIP);
                NotifyOfPropertyChange(() => CanListen);
            }
        }

        public string ConnectIP {
            get => _connectIP;
            set {
                if (_connectIP == value) {
                    return;
                }

                _connectIP = value;
                NotifyOfPropertyChange(() => ConnectIP);
                NotifyOfPropertyChange(() => CanConnect);
                NotifyOfPropertyChange(() => CanClear);
            }
        }

        public int ConnectPort {
            get => _connectPort;
            set {
                if (_connectPort == value) {
                    return;
                }

                _connectPort = value;
                NotifyOfPropertyChange(() => ConnectPort);
                NotifyOfPropertyChange(() => CanConnect);
                NotifyOfPropertyChange(() => CanClear);
            }
        }

        public bool CanListen => true;

        public void Listen() {

        }

        public bool CanConnect => false;

        public void Connect() {

        }

        public bool CanClear => !string.IsNullOrWhiteSpace(ConnectIP) && ConnectPort != 0;

        public void Clear() {
            ConnectIP = IPAddress.Any.ToString();
            ConnectPort = 0;
        }
    }
}

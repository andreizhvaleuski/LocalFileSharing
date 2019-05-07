using System.Net;

using Caliburn.Micro;
using LocalFileSharing.Network.Domain;
using LocalFileSharing.Network.Sockets;

namespace LocalFileSharing.DesktopUI.ViewModels {
    public class ListenConnectViewModel : Screen {
        private string _listenIP = IPAddress.Any.ToString();
        private int _listenPort = TcpSocketBase.MinAllowedPort;
        private bool _listening;

        private string _connectIP;
        private int _connectPort;
        private bool _connecting;

        private FileSharingServer _listenFileSharingClient;
        private FileSharingClient _connectFileSharingClient;

        public ListenConnectViewModel() {
            _listenFileSharingClient = new FileSharingServer();

            GetListenIPEndPoint();
        }

        private async void GetListenIPEndPoint() {
            IPEndPoint endPoint = await _listenFileSharingClient.GetServerIPEndPointAsync(default);
            ListenIP = endPoint.Address.ToString();
            ListenPort = endPoint.Port;
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
            }
        }

        public bool CanListen => 
            !_connecting;

        public void Listen() {
            _listening = true;

            _listening = false;
        }

        public bool CanConnect => 
            IPAddress.TryParse(_connectIP, out _) && 
            ConnectPort >= TcpSocketBase.MinAllowedPort && 
            ConnectPort <= TcpSocketBase.MaxAllowedPort &&
            !_listening;

        public void Connect() {
            _connecting = true;

            _connecting = false;
        }

        public void Clear() {
            ConnectIP = IPAddress.Any.ToString();
            ConnectPort = TcpSocketBase.MinAllowedPort;
        }
    }
}

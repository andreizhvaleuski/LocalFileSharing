using System.Net;
using System.Threading.Tasks;

using Caliburn.Micro;

using LocalFileSharing.DesktopUI.Messages;
using LocalFileSharing.Network.Domain;
using LocalFileSharing.Network.Sockets;

using Socs = System.Net.Sockets;

namespace LocalFileSharing.DesktopUI.ViewModels {
    public class ListenConnectViewModel : Screen {
        private IPAddress _listenIP = IPAddress.Any;
        private int _listenPort = TcpSocketBase.MinAllowedPort;
        private bool _listening;

        private IPAddress _connectIP = IPAddress.Loopback;
        private int _connectPort = TcpSocketBase.MinAllowedPort;
        private bool _connecting;

        private FileSharingServer _listenFileSharingClient;
        private FileSharingClient _connectFileSharingClient;

        private readonly IEventAggregator _eventAgregator;

        public ListenConnectViewModel(IEventAggregator eventAggregator) {
            _listenFileSharingClient = new FileSharingServer();
            _eventAgregator = eventAggregator;
            GetListenIPEndPoint();
        }

        private async void GetListenIPEndPoint() {
            IPEndPoint endPoint = await _listenFileSharingClient.GetServerIPEndPointAsync();
            ListenIP = endPoint.Address;
            ListenPort = TcpSocketBase.MinAllowedPort;
            _connectIP = IPAddress.Parse(ListenIP.ToString());
        }

        public IPAddress ListenIP {
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

        public bool Listening {
            get => _listening;
            private set {
                if (_listening == value) {
                    return;
                }

                _listening = value;
                NotifyOfPropertyChange(() => Listening);
                NotifyOfPropertyChange(() => CanListen);
                NotifyOfPropertyChange(() => CanConnect);
                NotifyOfPropertyChange(() => CanClear);
            }
        }

        public IPAddress ConnectIP {
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
                NotifyOfPropertyChange(() => CanClear);
                NotifyOfPropertyChange(() => CanListen);
            }
        }

        public bool Connecting {
            get => _connecting;
            private set {
                if (_connecting == value) {
                    return;
                }

                _connecting = value;
                NotifyOfPropertyChange(() => Connecting);
                NotifyOfPropertyChange(() => CanConnect);
                NotifyOfPropertyChange(() => CanClear);
                NotifyOfPropertyChange(() => CanListen);
            }
        }

        public bool CanListen => !Connecting && !Listening;

        public async void Listen() {
            Listening = true;
            FileSharingClient fileSharingClient =
                await _listenFileSharingClient.ListenAsync(new IPEndPoint(ListenIP, ListenPort));
            _eventAgregator.PublishOnUIThread(new ConnectedMessage(fileSharingClient));
            Listening = false;
        }

        public bool CanConnect =>
            ConnectIP.AddressFamily == Socs.AddressFamily.InterNetwork &&
            ConnectPort >= TcpSocketBase.MinAllowedPort &&
            ConnectPort <= TcpSocketBase.MaxAllowedPort &&
            !Listening && !Connecting;

        public async void Connect() {
            Connecting = true;
            await Task.Run(() => {
                _connectFileSharingClient = new FileSharingClient(new IPEndPoint(ConnectIP, ConnectPort));
            });
            _eventAgregator.PublishOnUIThread(new ConnectedMessage(_connectFileSharingClient));
            Connecting = false;
        }

        public bool CanClear => !(Listening || Connecting);

        public void Clear() {
            ConnectIP = IPAddress.Any;
            ConnectPort = TcpSocketBase.MinAllowedPort;
        }

        public void ClearServer() {
            _listenFileSharingClient.Disconnect();
        }
    }
}

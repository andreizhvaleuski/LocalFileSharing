using System.Diagnostics;

using Caliburn.Micro;

using LocalFileSharing.DesktopUI.Messages;

namespace LocalFileSharing.DesktopUI.ViewModels {
    public class ShellViewModel : Conductor<IScreen>, IHandle<ConnectedMessage> {
        private readonly IEventAggregator _eventAggregator;

        private readonly ListenConnectViewModel _listenConnectVM;
        private readonly ConnectedViewModel _connectedVM;

        public ShellViewModel(
            IEventAggregator eventAggregator,
            ListenConnectViewModel listenConnectVM,
            ConnectedViewModel connectedVM) {
            _eventAggregator = eventAggregator;
            _listenConnectVM = listenConnectVM;
            _connectedVM = connectedVM;

            DisplayName = "Local File Sharing";

            ListenOrConnect();
        }

        public void Exit() {
            TryClose();
        }

        public bool CanListenOrConnect => !(ActiveItem == _listenConnectVM);

        public void ListenOrConnect() {
            ActivateItem(_listenConnectVM);
            NotifyOfPropertyChange(() => CanListenOrConnect);
        }

        public void Options() {
            Debug.WriteLine(nameof(Options));
        }

        public void Handle(ConnectedMessage message) {
            ActiveItem.TryClose();

            ActivateItem(_connectedVM);
        }
    }
}

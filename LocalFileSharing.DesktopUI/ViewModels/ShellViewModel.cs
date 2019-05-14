using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Caliburn.Micro;

using LocalFileSharing.DesktopUI.Messages;
using LocalFileSharing.DesktopUI.Services;

namespace LocalFileSharing.DesktopUI.ViewModels {
    public class ShellViewModel
        : Conductor<IScreen>, IHandle<ConnectedMessage>, IHandle<ErrorMessage> {
        private readonly IEventAggregator _eventAggregator;

        private ListenConnectViewModel _listenConnectVM;
        private ConnectedViewModel _connectedVM;
        private ErrorViewModel _errorVM;

        public ShellViewModel(IEventAggregator eventAggregator, ListenConnectViewModel listenConnectVM, ErrorViewModel errorVM) {
            DisplayName = "Local File Sharing";

            _listenConnectVM = listenConnectVM;
            _errorVM = errorVM;

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            ListenOrConnect();
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
            if (message is null) {
                throw new ArgumentNullException(nameof(message));
            }

            _connectedVM = new ConnectedViewModel(message.FileSharingClient, new DialogService(), _eventAggregator);

            if (ActiveItem == _connectedVM) {
                return;
            }

            ActiveItem.TryClose();
            ActivateItem(_connectedVM);
        }

        public void Handle(ErrorMessage message) {
            if (message is null) {
                throw new ArgumentNullException(nameof(message));
            }

            ActiveItem.TryClose();
            ActivateItem(_listenConnectVM);
        }
    }
}

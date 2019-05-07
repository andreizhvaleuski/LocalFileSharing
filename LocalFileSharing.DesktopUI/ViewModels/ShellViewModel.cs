using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Caliburn.Micro;

using LocalFileSharing.DesktopUI.Messages;

namespace LocalFileSharing.DesktopUI.ViewModels {
    public class ShellViewModel
        : Conductor<IScreen>, IHandle<ConnectedMessage>, IHandle<ErrorMessage> {
        private readonly IEventAggregator _eventAggregator;

        private readonly ListenConnectViewModel _listenConnectVM;
        private readonly ConnectedViewModel _connectedVM;
        private readonly ErrorViewModel _errorVM;

        public ShellViewModel(
            IEventAggregator eventAggregator,
            ListenConnectViewModel listenConnectVM,
            ConnectedViewModel connectedVM,
            ErrorViewModel errorVM
        ) {
            _eventAggregator = eventAggregator;

            _listenConnectVM = listenConnectVM;
            _connectedVM = connectedVM;
            _errorVM = errorVM;

            DisplayName = "Local File Sharing";

            _eventAggregator.Subscribe(this);

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
            if (message is null) {
                throw new ArgumentNullException(nameof(message));
            }

            _connectedVM.FileSharingClient = message.FileSharingClient;

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

            _errorVM.Title = message.Title;
            _errorVM.Description = message.Description;

            if (ActiveItem == _errorVM) {
                return;
            }

            ActiveItem.TryClose();
            ActivateItem(_errorVM);
        }
    }
}

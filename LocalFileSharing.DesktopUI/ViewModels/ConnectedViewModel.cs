using Caliburn.Micro;
using LocalFileSharing.Network.Domain;

namespace LocalFileSharing.DesktopUI.ViewModels {
    public class ConnectedViewModel : Screen {
        public FileSharingClient FileSharingClient { get; set; }

    }
}

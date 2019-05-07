using Caliburn.Micro;

namespace LocalFileSharing.DesktopUI.ViewModels {
    public class ErrorViewModel : Screen {
        private string _title;

        private string _description;

        public string Title {
            get => _title;
            set => _title = value;
        }
        public string Description {
            get => _description;
            set => _description = value;
        }
    }
}

namespace LocalFileSharing.DesktopUI.Services {
    public interface IDialogService {
        string[] OpenFilePaths { get; }
        string SaveFilePath { get; }
        bool OpenFileDialog();
        bool SaveFileDialog(string fileName);
        void ShowErrorMessage(string errorMessage);
        bool ShowTryCloseMessage(string message);
    }
}

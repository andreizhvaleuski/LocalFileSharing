namespace LocalFileSharing.DesktopUI.Services {
    public interface IDialogService {
        string[] OpenFilePaths { get; }
        string SaveFilePath { get; }
        bool OpenFileDialog();
        bool SaveFileDialog(string fileName);
    }
}

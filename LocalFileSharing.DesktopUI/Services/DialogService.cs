using System;
using System.IO;
using System.Windows;

using Microsoft.Win32;

namespace LocalFileSharing.DesktopUI.Services {
    public class DialogService : IDialogService {
        public string[] OpenFilePaths { get; private set; }
        public string SaveFilePath { get; private set; }

        public bool OpenFileDialog() {
            OpenFileDialog openFileDialog = new OpenFileDialog() {
                Multiselect = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (openFileDialog.ShowDialog() == true) {
                OpenFilePaths = openFileDialog.FileNames;
                return true;
            }
            return false;
        }

        public bool SaveFileDialog(string fileName) {
            if (string.IsNullOrWhiteSpace(fileName)) {
                return false;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog {
                FileName = fileName,
                DefaultExt = Path.GetExtension(fileName),
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (saveFileDialog.ShowDialog() == true) {
                SaveFilePath = saveFileDialog.FileName;
                return true;
            }
            return false;
        }

        public void ShowErrorMessage(string errorMessage) {
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool ShowTryCloseMessage(string message) {
            MessageBoxResult result = MessageBox.Show(
                message,
                "Error",
                MessageBoxButton.YesNo,
                MessageBoxImage.Error
            );
            return result == MessageBoxResult.Yes;
        }
    }
}

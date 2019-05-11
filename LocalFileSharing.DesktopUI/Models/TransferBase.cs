using System;
using System.IO;

using Caliburn.Micro;

namespace LocalFileSharing.DesktopUI.Models {
    public abstract class TransferBase : PropertyChangedBase {
        private Guid _transferID;
        private string _filePath;
        private long _fileSize;

        public Guid TransferID {
            get { return _transferID; }
            set {
                Set(ref _transferID, value, nameof(TransferID));
            }
        }
        public string FilePath {
            get { return _filePath; }
            set {
                if (!Set(ref _filePath, value, nameof(FilePath))) {
                    return;
                }
                NotifyOfPropertyChange(nameof(FileName));
            }
        }
        public string FileName {
            get {
                FileInfo fileInfo = new FileInfo(FilePath);
                return fileInfo.Name;
            }
        }
        public long FileSize {
            get { return _fileSize; }
            set {
                if (!Set(ref _fileSize, value, nameof(FileSize))) {
                    return;
                }
                NotifyOfPropertyChange(nameof(Progress));
            }
        }
        public abstract long Progress { get; }
    }
}

using System;
using System.Linq;

using Caliburn.Micro;
using LocalFileSharing.DesktopUI.Messages;
using LocalFileSharing.DesktopUI.Models;
using LocalFileSharing.DesktopUI.Services;
using LocalFileSharing.Network.Domain;
using LocalFileSharing.Network.Domain.Progress;
using LocalFileSharing.Network.Domain.States;

namespace LocalFileSharing.DesktopUI.ViewModels {
    public class ConnectedViewModel : Screen {
        public BindableCollection<DownloadInfo> Downloads { get; private set; }
        public BindableCollection<UploadInfo> Uploads { get; private set; }

        private DownloadInfo _downloadsSelectedItem;
        private UploadInfo _uploadsSelectedItem;

        private bool _acceptingDownload;
        private bool _cancellingDownload;
        private bool _uploadingFile;
        private bool _cancellingUpload;

        private readonly FileSharingClient _fileSharingClient;
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;

        public DownloadInfo DownloadsSelectedItem {
            get { return _downloadsSelectedItem; }
            set {
                Set(ref _downloadsSelectedItem, value, nameof(DownloadsSelectedItem));
                NotifyOfPropertyChange(nameof(CanAcceptDownload));
                NotifyOfPropertyChange(nameof(CanCancelDownload));
            }
        }

        public UploadInfo UploadsSelectedItem {
            get { return _uploadsSelectedItem; }
            set {
                Set(ref _uploadsSelectedItem, value, nameof(UploadsSelectedItem));
                NotifyOfPropertyChange(nameof(CanCancelUpload));
            }
        }

        public bool AcceptingDownload {
            get { return _acceptingDownload; }
            set {
                Set(ref _acceptingDownload, value, nameof(AcceptDownload));
                NotifyOfPropertyChange(nameof(CanAcceptDownload));
            }
        }

        public bool CancellingDownload {
            get { return _cancellingDownload; }
            set {
                Set(ref _cancellingDownload, value, nameof(CancellingDownload));
                NotifyOfPropertyChange(nameof(CanCancelDownload));
            }
        }

        public bool UploadingFile {
            get { return _uploadingFile; }
            set {
                Set(ref _uploadingFile, value, nameof(UploadingFile));
                NotifyOfPropertyChange(nameof(CanUploadFile));
            }
        }

        public bool CancellingUpload {
            get { return _cancellingUpload; }
            set {
                Set(ref _cancellingUpload, value, nameof(CancellingUpload));
                NotifyOfPropertyChange(nameof(CanCancelUpload));
            }
        }

        public bool CanAcceptDownload {
            get {
                return !AcceptingDownload && 
                    DownloadsSelectedItem != null &&
                    DownloadsSelectedItem.State == ReceiveFileState.Initializing;
            }
        }

        public bool CanCancelDownload {
            get {
                return !CancellingDownload &&
                    DownloadsSelectedItem != null &&
                    DownloadsSelectedItem.State != ReceiveFileState.Cancelled &&
                    DownloadsSelectedItem.State != ReceiveFileState.Hashing &&
                    DownloadsSelectedItem.State != ReceiveFileState.Completed &&
                    DownloadsSelectedItem.State != ReceiveFileState.Failed;
            }
        }

        public bool CanUploadFile {
            get {
                return !UploadingFile;
            }
        }

        public bool CanCancelUpload {
            get {
                return !CancellingUpload &&
                    UploadsSelectedItem != null &&
                    UploadsSelectedItem.State != SendFileState.Completed &&
                    UploadsSelectedItem.State != SendFileState.Cancelled &&
                    UploadsSelectedItem.State != SendFileState.Failed;
            }
        }

        public ConnectedViewModel(FileSharingClient client, IDialogService dialogService, IEventAggregator eventAggregator) {
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;

            _fileSharingClient = client;
            _fileSharingClient.FileReceive += ProcessDownloadInfo;
            _fileSharingClient.FileSend += ProcessUploadInfo;
            _fileSharingClient.ConnectionLost += ProcessConnectionLost;

            Downloads = new BindableCollection<DownloadInfo>();
            Uploads = new BindableCollection<UploadInfo>();
        }

        private void ProcessConnectionLost(object sender, ConnectionLostEventArgs e) {
            _dialogService.ShowErrorMessage(e.Exception.Message);
            _eventAggregator.PublishOnUIThread(new ErrorMessage(null, null));
        }

        public void AcceptDownload() {
            AcceptingDownload = true;
            if (_dialogService.SaveFileDialog(DownloadsSelectedItem.FileName) == true) {
                _fileSharingClient.InitializeReceive(DownloadsSelectedItem.TransferID, _dialogService.SaveFilePath);
            }
            AcceptingDownload = false;
        }

        public void CancelDownload() {
            _fileSharingClient.CancellReceive(DownloadsSelectedItem.TransferID);
        }

        public async void UploadFile() {
            UploadingFile = true;
            if (_dialogService.OpenFileDialog() == true) {
                foreach (string filePath in _dialogService.OpenFilePaths) {
                    await _fileSharingClient.SendFileAsync(filePath);
                }
            }
            UploadingFile = false;
        }

        public void CancelUpload() {
            _fileSharingClient.CancellSend(UploadsSelectedItem.TransferID);
        }

        private void ProcessUploadInfo(object sender, SendFileEventArgs e) {
            if (e.SendState == SendFileState.Hashing) {
                UploadInfo upload = new UploadInfo() {
                    TransferID = e.TransferID,
                    FilePath = e.FilePath,
                    FileSize = e.FileSize,
                    State = e.SendState
                };
                Uploads.Add(upload);
            }
            else {
                UploadInfo upload = Uploads.First(u => e.TransferID == u.TransferID);
                upload.BytesSent = e.BytesSent;
                upload.State = e.SendState;
            }
        }

        private void ProcessDownloadInfo(object sender, ReceiveFileEventArgs e) {
            if (e.ReceiveState == ReceiveFileState.Initializing) {
                DownloadInfo download = new DownloadInfo() {
                    TransferID = e.TransferID,
                    FilePath = e.FilePath,
                    FileSize = e.FileSize,
                    State = e.ReceiveState
                };
                Downloads.Add(download);
            }
            else {
                DownloadInfo download = Downloads.First(d => e.TransferID == d.TransferID);
                download.BytesReceived = e.BytesRecived;
                download.State = e.ReceiveState;
            }
        }
    }
}

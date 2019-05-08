using System;
using Caliburn.Micro;
using LocalFileSharing.Network.Domain;
using LocalFileSharing.Network.Domain.Progress;

namespace LocalFileSharing.DesktopUI.ViewModels {
    public class ConnectedViewModel : Screen {
        public FileSharingClient FileSharingClient { get; set; }

        private long _download;

        public long Download {
            get { return _download; }
            set {
                if (_download == value) {
                    return;
                }

                _download = value;
                NotifyOfPropertyChange(() => Download);
            }
        }

        private long _upload;

        public long Upload {
            get { return _upload; }
            set {
                if (_upload == value) {
                    return;
                }

                _upload = value;
                NotifyOfPropertyChange(() => Upload);
            }
        }

        public async void Receive() {
            Progress<ReceiveFileProgressReport> progress = new Progress<ReceiveFileProgressReport>();
            progress.ProgressChanged += DownloadReport;
            await FileSharingClient.ReceiveFileAsync(@"D:\Downloads", progress, default);
        }

        public async void Send() {
            string path =
                @"D:\Downloads\Torrents\Series\Game of Thrones - Игра престолов. Сезон 8. Amedia. 1080p\Game.of.Thrones.s08e04.WEB-DL.1080p.Amedia.mkv";
            Progress<SendFileProgressReport> progress = new Progress<SendFileProgressReport>();
            progress.ProgressChanged += UploadReport;
            await FileSharingClient.SendFileAsync(path, progress, default);
        }

        private void DownloadReport(object sender, ReceiveFileProgressReport e) {
            Download = e.FileData.FileSize / e.BytesRecived;
        }

        private void UploadReport(object sender, SendFileProgressReport e) {
            Upload = e.FileData.FileSize / e.BytesSent;
        }
    }
}

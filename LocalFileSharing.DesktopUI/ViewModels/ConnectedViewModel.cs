using System;
using System.Diagnostics;
using Caliburn.Micro;
using LocalFileSharing.Network.Domain;
using LocalFileSharing.Network.Domain.Progress;

namespace LocalFileSharing.DesktopUI.ViewModels {
    public class ConnectedViewModel : Screen {
        private bool _sending;
        private bool _receiving;
        private long _download;
        private long _upload;

        private readonly Progress<SendFileEventArgs> sendProgress;
        private readonly Progress<ReceiveFileEventArgs> receiveProgress;

        public ConnectedViewModel() {
            sendProgress = new Progress<SendFileEventArgs>();
            sendProgress.ProgressChanged += UploadReport;

            receiveProgress = new Progress<ReceiveFileEventArgs>();
            receiveProgress.ProgressChanged += DownloadReport;
        }

        public FileSharingClient FileSharingClient { get; set; }

        public bool Sending {
            get { return _sending; }
            set {
                if (_sending == value) {
                    return;
                }

                _sending = value;
                NotifyOfPropertyChange(() => CanReceive);
                NotifyOfPropertyChange(() => CanSend);
            }
        }

        public bool Receiving {
            get { return _receiving; }
            set {
                if (_receiving == value) {
                    return;
                }

                _receiving = value;
                NotifyOfPropertyChange(() => CanReceive);
                NotifyOfPropertyChange(() => CanSend);
            }
        }

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

        public bool CanReceive => !(Sending || Receiving);

        public async void Receive() {
            await FileSharingClient.ReceiveFileAsync(@"F:\Downloads", receiveProgress, default);
        }

        public bool CanSend => !(Sending || Receiving);

        public async void Send() {
            string path =
                @"D:\Downloads\Torrents\Programs\ubuntu-19.04-desktop-amd64.iso";
            await FileSharingClient.SendFileAsync(path, sendProgress, default);
        }

        private void DownloadReport(object sender, ReceiveFileEventArgs e) {
            Debug.WriteLine(e.ReceiveState);
            if (e.ReceiveState != Network.Domain.States.ReceiveFileState.Receiving) {
                return;
            }
            Download = e.BytesRecived * 100 / e.FileData.FileSize;
        }

        private void UploadReport(object sender, SendFileEventArgs e) {
            Debug.WriteLine(e.SendState);
            if (e.SendState != Network.Domain.States.SendFileState.Sending) {
                return;
            }
            Upload = e.BytesSent * 100 / e.FileData.FileSize;
        }
    }
}

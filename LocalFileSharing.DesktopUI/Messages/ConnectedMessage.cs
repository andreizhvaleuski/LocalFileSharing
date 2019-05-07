using System;

using LocalFileSharing.Network.Domain;

namespace LocalFileSharing.DesktopUI.Messages {
    public class ConnectedMessage {
        public FileSharingClient FileSharingClient { get; protected set; }

        public ConnectedMessage(FileSharingClient fileSharingClient) {
            //if (fileSharingClient is null) {
            //    throw new ArgumentNullException(nameof(fileSharingClient));
            //}

            FileSharingClient = fileSharingClient;
        }
    }
}

using System;

namespace LocalFileSharing.Network.Domain.Progress {
    public class ConnectionLostEventArgs : EventArgs {
        public Exception Exception { get; protected set; }

        public ConnectionLostEventArgs(Exception exception) {
            if (exception is null) {
                throw new ArgumentNullException(nameof(exception));
            }

            Exception = exception;
        }
    }
}

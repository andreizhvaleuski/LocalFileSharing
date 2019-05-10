namespace LocalFileSharing.Network.Domain.Context {
    public abstract class FileTransferContextBase {
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public byte[] FileHash { get; set; }
        public bool Initialized { get; protected set; }
        public bool Cancelled { get; protected set; }

        public virtual void Initialize() {

            Initialized = true;
        }

        public abstract void End();

        public virtual void Cancel() {
            End();
            Cancelled = true;
        }
    }
}

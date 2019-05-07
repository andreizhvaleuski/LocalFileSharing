namespace LocalFileSharing.Domain.Infrastructure {
    public interface IFileHash {
        byte[] ComputeHash(string path);
    }
}

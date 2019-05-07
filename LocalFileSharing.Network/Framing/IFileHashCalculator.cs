namespace LocalFileSharing.Network.Framing {
    public interface IFileHashCalculator {
        byte[] Calculate(string path);
    }
}

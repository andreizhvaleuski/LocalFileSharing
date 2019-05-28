using System.IO;
using System.Security.Cryptography;

namespace LocalFileSharing.Network.Framing {
    public class SHA256FileHashCalculator : IFileHashCalculator {
        public byte[] Calculate(string path) {
            if (!File.Exists(path)) {
                throw new FileNotFoundException(
                    $"File was not found.",
                    path
                );
            }

            byte[] sha256Hash = null;

            using (FileStream strem = File.OpenRead(path))
            using (SHA256 sha256 = SHA256.Create()) {
                sha256Hash = sha256.ComputeHash(strem);
            }

            return sha256Hash;
        }
    }
}

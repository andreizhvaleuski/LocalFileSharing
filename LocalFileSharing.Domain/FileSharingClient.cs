using LocalFileSharing.Domain.Infrastructure;
using LocalFileSharing.Network.Common;
using LocalFileSharing.Network.Common.Content;
using LocalFileSharing.Network.Framing;
using LocalFileSharing.Network.Framing.Wrappers;
using LocalFileSharing.Network.Sockets;

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace LocalFileSharing.Domain
{
    public sealed class FileSharingClient
    {
        public const int BlockBufferSize = 512_000;

        private readonly ILengthPrefixWrapper lengthPrefixWrapper;
        private readonly ITypePrefixWrapper typePrefixWrapper;
        private readonly IFileHash fileHash;

        private readonly TcpClient client;

        public FileSharingClient(IPAddress ipAddress, int port)
            : this(new TcpClient(ipAddress, port))
        { }

        public FileSharingClient(TcpClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            this.client = client;

            lengthPrefixWrapper = new LengthPrefixWrapper();
            typePrefixWrapper = new TypePrefixWrapper();
            fileHash = new SHA256FileHash();
        }

        public async Task SendFileAsync(
            string path,
            IProgress<SendFileProgressReport> progress,
            CancellationToken cancellationToken
        ) {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(
                    "The path must be provided.",
                    nameof(path)
                );
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File was not found.", path);
            }

            await Task.Run(() =>
            {
                FileInfo fileInfo = new FileInfo(path);

                FileData fileData = new FileData()
                {
                    FileId = Guid.NewGuid(),
                    FilePath = path,
                    FileSize = fileInfo.Length
                };

                SendFileProgressReport progressReport = new SendFileProgressReport()
                {
                    SendFileState = SendFileState.Unspecified,
                    FileData = fileData,
                    BytesSent = 0
                };

                if (cancellationToken.IsCancellationRequested)
                {
                    progressReport.SendFileState = SendFileState.Cancelled;
                    progress.Report(progressReport);
                    return;
                }

                progressReport.FileData.FileSha256Hash = = fileHash.ComputeHash(path); ;
                progressReport.SendFileState = SendFileState.Hashing;
                progress.Report(progressReport);

                FileInitialContent initialContent =
                    new FileInitialContent(fileData.FileId, fileInfo.Name, fileData.FileSize, sha256Hash);
                SendFileInitialContent(initialContent);
                progressReport.SendFileState = SendFileState.Initializing;
                progress.Report(progressReport);

                ResponseType responseToInitial = ReceiveResponse();
                if (responseToInitial == ResponseType.ReceiveFileCancel)
                {
                    progressReport.SendFileState = SendFileState.Cancelled;
                    progress.Report(progressReport);
                    return;
                }
                else if (responseToInitial != ResponseType.ReceiveFileInitial)
                {
                    progressReport.SendFileState = SendFileState.Failed;
                    progress.Report(progressReport);
                    throw new ArgumentException();
                }

                using (BinaryReader stream = new BinaryReader(File.OpenRead(path)))
                {
                    byte[] fileBuffer = new byte[BlockBufferSize];
                    long bytesSent = 0;
                    do
                    {
                        int bytesReadNumber = stream.Read(fileBuffer, 0, fileBuffer.Length);
                        if (bytesReadNumber == 0)
                        {
                            break;
                        }

                        byte[] block = new byte[bytesReadNumber];
                        Array.Copy(fileBuffer, block, bytesReadNumber);
                        FileRegularContent regularContent = new FileRegularContent(fileData.FileId, block);
                        SendRegularContent(regularContent);

                        ResponseType responseToRegular = ReceiveResponse();
                        if (responseToRegular == ResponseType.ReceiveFileCancel)
                        {
                            progressReport.SendFileState = SendFileState.Cancelled;
                            progress.Report(progressReport);
                            return;
                        }
                        else if (responseToRegular != ResponseType.ReceiveFileRegular)
                        {
                            progressReport.SendFileState = SendFileState.Failed;
                            progress.Report(progressReport);
                            throw new ArgumentException();
                        }

                        bytesSent += bytesReadNumber;
                        progressReport.BytesSent = bytesSent;
                        progressReport.SendFileState = SendFileState.Sending;
                        progress.Report(progressReport);
                    } while (true);
                }

                FileEndContent endContent = new FileEndContent(fileData.FileId);
                SendFileEndContent(endContent);

                ResponseType responseToEnd = ReceiveResponse();
                if (responseToEnd == ResponseType.ReceiveFileCancel)
                {
                    return;
                }
                else if (responseToEnd != ResponseType.ReceiveFileEnd)
                {
                    progressReport.SendFileState = SendFileState.Failed;
                    progress.Report(progressReport);
                    throw new Exception();
                }

                progressReport.SendFileState = SendFileState.Sent;
                progress.Report(progressReport);
            });
        }

        public void SendFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(
                    "The path must be provided.",
                    nameof(path)
                );
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File was not found: { path }.", path);
            }

            FileInfo fileInfo = new FileInfo(path);
            string fileName = fileInfo.Name;
            long fileSize = fileInfo.Length;
            Guid fileId = Guid.NewGuid();
            byte[] sha256Hash = fileHash.ComputeHash(path);

            FileInitialContent initialContent =
                new FileInitialContent(fileId, fileName, fileSize, sha256Hash);
            SendFileInitialContent(initialContent);

            ResponseType responseToInitial = ReceiveResponse();
            if (responseToInitial == ResponseType.ReceiveFileCancel)
            {
                return;
            }
            else if (responseToInitial != ResponseType.ReceiveFileInitial)
            {
                throw new ArgumentException();
            }

            using (BinaryReader stream = new BinaryReader(File.OpenRead(path)))
            {
                byte[] fileBuffer = new byte[BlockBufferSize];
                do
                {
                    int bytesReadNumber = stream.Read(fileBuffer, 0, fileBuffer.Length);
                    if (bytesReadNumber == 0)
                    {
                        break;
                    }
                    SendRegularContent(Guid.NewGuid(), fileBuffer, bytesReadNumber);

                    ResponseType responseToRegular = ReceiveResponse();
                    if (responseToRegular == ResponseType.ReceiveFileCancel)
                    {
                        break;
                    }
                    else if (responseToRegular != ResponseType.ReceiveFileRegular)
                    {
                        throw new ArgumentException();
                    }
                } while (true);
            }

            FileEndContent endContent = new FileEndContent(Guid.NewGuid());
            SendFileEndContent(endContent);

            ResponseType responseToEnd = ReceiveResponse();
            if (responseToEnd == ResponseType.ReceiveFileCancel)
            {
                return;
            }
            else if (responseToEnd != ResponseType.ReceiveFileEnd)
            {
                throw new Exception();
            }
        }

        private ResponseType ReceiveResponse()
        {
            int length = ReceiveLength();
            byte[] buffer = client.ReceiveBytes(length);
            UnwrapMessageData(buffer, out MessageType type, out FileBaseContent content);

            if (type != MessageType.Response)
            {
                throw new ArgumentException();
            }

            ResponseContent response = content as ResponseContent;
            if (response is null)
            {
                throw new InvalidCastException();
            }

            return response.Response;
        }

        private void SendFileInitialContent(FileInitialContent initialContent)
        {
            if (initialContent is null)
            {
                throw new ArgumentNullException(nameof(initialContent));
            }

            byte[] buffer = WrapContent(initialContent, MessageType.SendFileInitial);

            client.SendBytes(buffer);
        }

        private void SendRegularContent(Guid fileId, byte[] block, int count)
        {
            byte[] newBlock = new byte[count];
            Array.Copy(block, newBlock, count);

            FileRegularContent regularContent =
                new FileRegularContent(fileId, newBlock);

            byte[] buffer = ContentConverter.GetBytes(regularContent);
            buffer = typePrefixWrapper.Wrap(buffer, MessageType.SendFileRegular);
            buffer = lengthPrefixWrapper.Wrap(buffer);

            client.SendBytes(buffer);
        }

        private void SendRegularContent(FileRegularContent regularContent)
        {
            if (regularContent is null)
            {
                throw new ArgumentNullException(nameof(regularContent));
            }

            byte[] buffer = WrapContent(regularContent, MessageType.SendFileRegular);

            client.SendBytes(buffer);
        }

        private void SendFileEndContent(FileEndContent endContent)
        {
            if (endContent is null)
            {
                throw new ArgumentNullException(nameof(endContent));
            }

            byte[] buffer = WrapContent(endContent, MessageType.SendFileEnd);

            client.SendBytes(buffer);
        }

        private byte[] WrapContent(FileBaseContent content, MessageType message)
        {
            if (message == MessageType.Unspecified)
            {
                throw new ArgumentException();
            }

            byte[] buffer = ContentConverter.GetBytes(content);
            buffer = typePrefixWrapper.Wrap(buffer, message);
            buffer = lengthPrefixWrapper.Wrap(buffer);

            return buffer;
        }

        public void ReceiveFile(string downloadDirectory)
        {
            bool initialized = false;
            string fileName = string.Empty;
            FileStream stream = null;
            do
            {
                int length = ReceiveLength();
                byte[] buffer = client.ReceiveBytes(length);
                UnwrapMessageData(buffer, out MessageType type, out FileBaseContent content);
                if (type == MessageType.SendFileInitial)
                {
                    FileInitialContent inititalContent = (FileInitialContent)content;
                    initialized = true;
                    fileName = inititalContent.FileName;
                    stream = File.OpenWrite(Path.Combine(downloadDirectory, fileName));

                    SendResponse(Guid.NewGuid(), ResponseType.ReceiveFileInitial);
                }
                else if (type == MessageType.SendFileRegular && initialized)
                {
                    FileRegularContent regularContent = (FileRegularContent)content;
                    stream.Write(regularContent.Block, 0, regularContent.Block.Length);

                    SendResponse(Guid.NewGuid(), ResponseType.ReceiveFileRegular);
                }
                else if (type == MessageType.SendFileEnd && initialized)
                {
                    //SendFileEndContent initialContent = (SendFileEndContent)content;

                    SendResponse(Guid.NewGuid(), ResponseType.ReceiveFileEnd);

                    break;
                }
                else if (type == MessageType.SendFileCancel && initialized)
                {
                    //SendFileCancelContent initialContent = (SendFileCancelContent)content;
                    File.Delete(Path.Combine(downloadDirectory, fileName));

                    SendResponse(Guid.NewGuid(), ResponseType.ReceiveFileCancel);
                    break;
                }
            } while (true);
            stream?.Close();
        }

        private void SendResponse(Guid fileId, ResponseType response)
        {
            if (response == ResponseType.Unspecified)
            {
                throw new ArgumentException();
            }

            ResponseContent content = new ResponseContent(fileId, response);
            byte[] buffer = ContentConverter.GetBytes(content);
            buffer = typePrefixWrapper.Wrap(buffer, MessageType.Response);
            buffer = lengthPrefixWrapper.Wrap(buffer);

            client.SendBytes(buffer);
        }

        private int ReceiveLength()
        {
            byte[] bufferLength = client.ReceiveBytes(lengthPrefixWrapper.LengthPrefixSize);
            int length = BitConverter.ToInt32(bufferLength, 0);
            return length;
        }

        private void UnwrapMessageData(byte[] buffer, out MessageType type, out FileBaseContent content)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            byte[] typeBuffer = new byte[typePrefixWrapper.TypePrefixSize];
            Array.Copy(buffer, 0, typeBuffer, 0, typeBuffer.Length);
            type = typePrefixWrapper.GetTypePrefixValue(typeBuffer);
            byte[] contentBuffer = new byte[buffer.Length - typePrefixWrapper.TypePrefixSize];
            Array.Copy(buffer, typePrefixWrapper.TypePrefixSize, contentBuffer,
                0, contentBuffer.Length);
            content = ContentConverter.GetFileContent(contentBuffer);
        }
    }
}

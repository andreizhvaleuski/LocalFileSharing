using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using LocalFileSharing.Network.Domain.Context;
using LocalFileSharing.Network.Domain.Progress;
using LocalFileSharing.Network.Domain.States;
using LocalFileSharing.Network.Framing;
using LocalFileSharing.Network.Framing.Content;
using LocalFileSharing.Network.Framing.Wrappers;
using LocalFileSharing.Network.Sockets;

namespace LocalFileSharing.Network.Domain {
    public sealed class FileSharingClient {
        public const int BlockBufferSize = 1024 * 1024 * 4;

        private readonly ILengthPrefixWrapper _lengthPrefixWrapper;
        private readonly ITypePrefixWrapper _typePrefixWrapper;
        private readonly IFileHashCalculator _fileHashCalculator;
        private readonly IContentConverter _contentConverter;

        private readonly ConcurrentDictionary<Guid, SendFileContext> _sendFileContexts;
        private readonly ConcurrentDictionary<Guid, ReceiveFileContext> _receiveFileContexts;

        private readonly ConcurrentQueue<byte[]> _sendMessages;
        private readonly ConcurrentQueue<byte[]> _receiveMessages;

        private readonly TcpClient client;

        public FileSharingClient(IPAddress ipAddress, int port)
            : this(new TcpClient(ipAddress, port)) { }

        public FileSharingClient(TcpClient client) {
            if (client is null) {
                throw new ArgumentNullException(nameof(client));
            }

            this.client = client;

            _lengthPrefixWrapper = new LengthPrefixWrapper();
            _typePrefixWrapper = new TypePrefixWrapper();
            _fileHashCalculator = new SHA256FileHashCalculator();
            _contentConverter = new ContentConverter();

            _sendFileContexts = new ConcurrentDictionary<Guid, SendFileContext>();
            _receiveFileContexts = new ConcurrentDictionary<Guid, ReceiveFileContext>();
        }

        public async Task SendFileAsync(
            string path,
            IProgress<SendFileProgressReport> progress,
            CancellationToken cancellationToken
        ) {
            if (string.IsNullOrWhiteSpace(path)) {
                throw new ArgumentException(
                    "The path must be provided.",
                    nameof(path)
                );
            }

            if (!File.Exists(path)) {
                throw new FileNotFoundException($"File was not found.", path);
            }

            await Task.Run(() => {
                FileInfo fileInfo = new FileInfo(path);

                FileData fileData = new FileData() {
                    OperationID = Guid.NewGuid(),
                    FilePath = path,
                    FileSize = fileInfo.Length
                };

                if (cancellationToken.IsCancellationRequested) {
                    ReportSendProgress(progress, fileData, 0, SendFileState.Cancelled);
                    return;
                }

                fileData.FileHash = _fileHashCalculator.Calculate(path);
                ReportSendProgress(progress, fileData, 0, SendFileState.Hashing);

                if (cancellationToken.IsCancellationRequested) {
                    ReportSendProgress(progress, fileData, 0, SendFileState.Cancelled);
                    return;
                }

                FileInitialContent initialContent =
                    new FileInitialContent(fileData.OperationID, fileInfo.Name, fileData.FileSize, fileData.FileHash);
                SendFileInitialContent(initialContent);
                ReportSendProgress(progress, fileData, 0, SendFileState.Initializing);

                if (cancellationToken.IsCancellationRequested) {
                    ReportSendProgress(progress, fileData, 0, SendFileState.Cancelled);
                    return;
                }

                ResponseType responseToInitial = ReceiveResponse();
                if (responseToInitial == ResponseType.ReceiveFileCancel) {
                    ReportSendProgress(progress, fileData, 0, SendFileState.Cancelled);
                    return;
                }
                else if (responseToInitial != ResponseType.ReceiveFileInitial) {
                    ReportSendProgress(progress, fileData, 0, SendFileState.Failed);
                    throw new ArgumentException();
                }

                using (BinaryReader stream = new BinaryReader(File.OpenRead(path))) {
                    byte[] fileBuffer = new byte[BlockBufferSize];
                    long bytesSent = 0;
                    do {
                        int bytesReadNumber = stream.Read(fileBuffer, 0, fileBuffer.Length);
                        if (bytesReadNumber == 0) {
                            break;
                        }

                        byte[] block = new byte[bytesReadNumber];
                        Array.Copy(fileBuffer, block, bytesReadNumber);
                        FileRegularContent regularContent = new FileRegularContent(fileData.OperationID, block);
                        SendRegularContent(regularContent);

                        ResponseType responseToRegular = ReceiveResponse();
                        if (responseToRegular == ResponseType.ReceiveFileCancel) {
                            ReportSendProgress(progress, fileData, 0, SendFileState.Cancelled);
                            return;
                        }
                        else if (responseToRegular != ResponseType.ReceiveFileRegular) {
                            ReportSendProgress(progress, fileData, 0, SendFileState.Failed);
                            throw new ArgumentException();
                        }

                        bytesSent += bytesReadNumber;
                        ReportSendProgress(progress, fileData, bytesSent, SendFileState.Sending);

                        if (cancellationToken.IsCancellationRequested) {
                            ReportSendProgress(progress, fileData, 0, SendFileState.Cancelled);
                            return;
                        }
                    } while (true);
                }

                FileEndContent endContent = new FileEndContent(fileData.OperationID);
                SendFileEndContent(endContent);

                ResponseType responseToEnd = ReceiveResponse();
                if (responseToEnd == ResponseType.ReceiveFileCancel) {
                    ReportSendProgress(progress, fileData, 0, SendFileState.Cancelled);
                    return;
                }
                else if (responseToEnd != ResponseType.ReceiveFileEnd) {
                    ReportSendProgress(progress, fileData, 0, SendFileState.Failed);
                    throw new Exception();
                }

                ReportSendProgress(progress, fileData, 0, SendFileState.Sent);
            });
        }

        private void ReportSendProgress(
            IProgress<SendFileProgressReport> progress,
            FileData fileData,
            long bytesSent,
            SendFileState sendFileState) {
            SendFileProgressReport report = new SendFileProgressReport {
                SendFileState = sendFileState,
                FileData = fileData,
                BytesSent = bytesSent
            };
            progress?.Report(report);
        }

        private void ReportReceiveProgress(
            IProgress<ReceiveFileProgressReport> progress,
            FileData fileData,
            long bytesReceived,
            ReceiveFileState receiveFileState) {
            ReceiveFileProgressReport report = new ReceiveFileProgressReport {
                ReceiveFileState = receiveFileState,
                FileData = fileData,
                BytesRecived = bytesReceived
            };
            progress?.Report(report);
        }

        private ResponseType ReceiveResponse() {
            int length = ReceiveLength();
            byte[] buffer = client.ReceiveBytes(length);
            UnwrapMessageData(buffer, out MessageType type, out ContentBase content);

            if (type != MessageType.Response) {
                throw new ArgumentException();
            }

            ResponseContent response = content as ResponseContent;
            if (response is null) {
                throw new InvalidCastException();
            }

            return response.ResponseType;
        }

        private void SendFileInitialContent(FileInitialContent initialContent) {
            if (initialContent is null) {
                throw new ArgumentNullException(nameof(initialContent));
            }

            byte[] buffer = WrapContent(initialContent, MessageType.SendFileInitial);

            client.SendBytes(buffer);
        }

        private void SendRegularContent(FileRegularContent regularContent) {
            if (regularContent is null) {
                throw new ArgumentNullException(nameof(regularContent));
            }

            byte[] buffer = WrapContent(regularContent, MessageType.SendFileRegular);

            client.SendBytes(buffer);
        }

        private void SendFileEndContent(FileEndContent endContent) {
            if (endContent is null) {
                throw new ArgumentNullException(nameof(endContent));
            }

            byte[] buffer = WrapContent(endContent, MessageType.SendFileEnd);

            client.SendBytes(buffer);
        }

        private byte[] WrapContent(ContentBase content, MessageType message) {
            if (content is null) {
                throw new ArgumentNullException(nameof(content));
            }

            if (message == MessageType.Unspecified) {
                throw new ArgumentException();
            }

            byte[] buffer = _contentConverter.GetBytes(content);
            buffer = _typePrefixWrapper.Wrap(buffer, message);
            buffer = _lengthPrefixWrapper.Wrap(buffer);

            return buffer;
        }

        public async Task ReceiveFileAsync(
            string downloadDirectory,
            IProgress<ReceiveFileProgressReport> progress,
            CancellationToken cancellationToken
        ) {
            if (string.IsNullOrWhiteSpace(downloadDirectory)) {
                throw new ArgumentException();
            }

            await Task.Run(() => {
                FileData fileData = new FileData();

                bool initialized = false;
                bool received = true;
                string fileName = string.Empty;
                BinaryWriter stream = null;
                long bytesReceived = 0;
                do {
                    int length = ReceiveLength();
                    byte[] buffer = client.ReceiveBytes(length);
                    UnwrapMessageData(buffer, out MessageType type, out ContentBase content);
                    if (type == MessageType.SendFileInitial) {
                        FileInitialContent initialContent = (FileInitialContent)content;

                        fileData.OperationID = initialContent.OperationID;
                        fileData.FileSize = initialContent.FileSize;
                        fileData.FileHash = initialContent.FileHash.ToArray();
                        fileData.FilePath = Path.Combine(downloadDirectory, initialContent.FileName);

                        stream = new BinaryWriter(File.Create(fileData.FilePath));

                        ReportReceiveProgress(progress, fileData, 0, ReceiveFileState.Initializing);
                        SendResponse(fileData.OperationID, ResponseType.ReceiveFileInitial);

                        initialized = true;
                    }
                    else if (type == MessageType.SendFileRegular && initialized) {
                        FileRegularContent regularContent = (FileRegularContent)content;
                        stream.Write(regularContent.FileBlock, 0, regularContent.FileBlock.Length);
                        bytesReceived += regularContent.FileBlock.Length;

                        ReportReceiveProgress(progress, fileData, bytesReceived, ReceiveFileState.Receiving);

                        SendResponse(fileData.OperationID, ResponseType.ReceiveFileRegular);
                    }
                    else if (type == MessageType.SendFileEnd && initialized) {
                        //SendFileEndContent initialContent = (SendFileEndContent)content;
                        received = true;

                        ReportReceiveProgress(progress, fileData, 0, ReceiveFileState.Received);

                        SendResponse(fileData.OperationID, ResponseType.ReceiveFileEnd);
                        break;
                    }
                    else if (type == MessageType.SendFileCancel && initialized) {
                        //SendFileCancelContent initialContent = (SendFileCancelContent)content;

                        ReportReceiveProgress(progress, fileData, 0, ReceiveFileState.Cancelled);

                        SendResponse(fileData.OperationID, ResponseType.ReceiveFileCancel);
                        break;
                    }
                    if (cancellationToken.IsCancellationRequested) {
                        ReportReceiveProgress(progress, fileData, 0, ReceiveFileState.Cancelled);
                        break;
                    }
                } while (true);
                stream?.Close();

                if (!received) {
                    return;
                }

                ReportReceiveProgress(progress, fileData, 0, ReceiveFileState.Hashing);
                byte[] receivedFileSha256Hash = _fileHashCalculator.Calculate(fileData.FilePath);

                if (!fileData.FileHash.SequenceEqual(receivedFileSha256Hash)) {
                    ReportReceiveProgress(progress, fileData, 0, ReceiveFileState.Failed);
                    return;
                }

                ReportReceiveProgress(progress, fileData, 0, ReceiveFileState.HashChecked);

                ReportReceiveProgress(progress, fileData, 0, ReceiveFileState.Completed);
            });
        }

        private void SendResponse(Guid fileId, ResponseType response) {
            if (response == ResponseType.Unspecified) {
                throw new ArgumentException();
            }

            ResponseContent content = new ResponseContent(fileId, response);
            byte[] buffer = _contentConverter.GetBytes(content);
            buffer = _typePrefixWrapper.Wrap(buffer, MessageType.Response);
            buffer = _lengthPrefixWrapper.Wrap(buffer);

            client.SendBytes(buffer);
        }

        private int ReceiveLength() {
            byte[] bufferLength = client.ReceiveBytes(_lengthPrefixWrapper.PrefixLength);
            int length = BitConverter.ToInt32(bufferLength, 0);
            return length;
        }

        private void UnwrapMessageData(
            byte[] buffer,
            out MessageType type,
            out ContentBase content
        ) {
            if (buffer is null) {
                throw new ArgumentNullException(nameof(buffer));
            }

            byte[] typeBuffer = new byte[_typePrefixWrapper.PrefixLength];
            Array.Copy(buffer, 0, typeBuffer, 0, typeBuffer.Length);
            type = _typePrefixWrapper.Unwrap(typeBuffer);
            byte[] contentBuffer = new byte[buffer.Length - _typePrefixWrapper.PrefixLength];
            Array.Copy(buffer, _typePrefixWrapper.PrefixLength, contentBuffer,
                0, contentBuffer.Length);
            content = _contentConverter.GetContent(contentBuffer);
        }
    }
}

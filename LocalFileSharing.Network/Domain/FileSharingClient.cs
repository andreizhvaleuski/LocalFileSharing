using System;
using System.Collections.Concurrent;
using System.Diagnostics;
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
        public const int FileBlockSize = 1024 * 1024 * 2;

        public event EventHandler<SendFileEventArgs> FileSend;
        public event EventHandler<ReceiveFileEventArgs> FileReceive;
        public event EventHandler<ConnectionLostEventArgs> ConnectionLost;

        public string DownloadPath { get; private set; }

        private readonly IMessageFramer _messageFramer;
        private readonly IFileHashCalculator _fileHashCalculator;

        private readonly ConcurrentDictionary<Guid, SendFileContext> _sendFileContexts;
        private readonly ConcurrentDictionary<Guid, ReceiveFileContext> _receiveFileContexts;

        private readonly ConcurrentQueue<byte[]> _messagesToSend;
        private readonly ConcurrentQueue<byte[]> _receivedMessages;

        private readonly TcpClient _client;

        public readonly CancellationTokenSource ConnectionCancellationTokenSource;

        public FileSharingClient(IPEndPoint ipEndPoint)
            : this(new TcpClient(ipEndPoint)) { }

        public FileSharingClient(TcpClient client) {
            if (client is null) {
                throw new ArgumentNullException(nameof(client));
            }

            _client = client;

            _messageFramer = new MessageFramer(
                new LengthPrefixWrapper(),
                new TransferIDPrefixWrapper(),
                new TypePrefixWrapper(),
                new ContentConverter()
            );
            _fileHashCalculator = new SHA256FileHashCalculator();

            _sendFileContexts = new ConcurrentDictionary<Guid, SendFileContext>();
            _receiveFileContexts = new ConcurrentDictionary<Guid, ReceiveFileContext>();

            _messagesToSend = new ConcurrentQueue<byte[]>();
            _receivedMessages = new ConcurrentQueue<byte[]>();

            ConnectionCancellationTokenSource = new CancellationTokenSource();

            Initialize(ConnectionCancellationTokenSource.Token);
        }

        private void Initialize(CancellationToken cancellationToken) {
            StartSendAsync(cancellationToken);
            StartReceiveAsync(cancellationToken);
            StartProcessReceivedMessagesAsync(cancellationToken);
        }

        private async void StartReceiveAsync(CancellationToken cancellationToken) {
            await Task.Run(() => StartReceive(cancellationToken));
        }

        private void StartReceive(CancellationToken cancellationToken) {
            while (!cancellationToken.IsCancellationRequested) {
                try {
                    byte[] messageBuffer = ReceiveMessage();
                    _receivedMessages.Enqueue(messageBuffer);
                }
                catch (Exception e) {
                    ConnectionLostEventArgs connectionLostEA = new ConnectionLostEventArgs(e);
                    OnConnectionLost(connectionLostEA);
                    break;
                }
            }
        }

        private async void StartProcessReceivedMessagesAsync(CancellationToken cancellationToken) {
            await Task.Run(() => StartProcessingReceivedMessages(cancellationToken));
        }

        private void StartProcessingReceivedMessages(CancellationToken cancellationToken) {
            while (!cancellationToken.IsCancellationRequested) {
                bool result = _receivedMessages.TryDequeue(out byte[] messageBuffer);
                if (!result) {
                    continue;
                }

                _messageFramer.GetFrameComponents(
                    messageBuffer,
                    out Guid transferID,
                    out MessageType messageType,
                    out ContentBase content
                );
                ProcessReceivedMessage(transferID, messageType, content);
            }
        }

        private void ProcessReceivedMessage(Guid transferID, MessageType messageType, ContentBase content) {
            if (transferID == Guid.Empty) {
                throw new ArgumentException(
                    $"The transfer id can not be empty.",
                    nameof(transferID)
                );
            }

            if (messageType == MessageType.Unspecified) {
                throw new ArgumentException(
                    $"The message type can not be with {MessageType.Unspecified} value.",
                    nameof(transferID)
                );
            }

            if (messageType == MessageType.SendFileInitial) {
                FileInitialContent initialContent = content as FileInitialContent;
                if (initialContent is null) {
                    throw new InvalidOperationException();
                }

                ProcessSendFileInitialMessage(transferID, initialContent);
            }
            else if (messageType == MessageType.SendFileRegular) {
                FileRegularContent regularContent = content as FileRegularContent;
                if (regularContent is null) {
                    throw new InvalidOperationException();
                }

                ProcessSendFileRegularMessage(transferID, regularContent);
            }
            else if (messageType == MessageType.SendFileEnd) {
                ProcessSendFileEndMessage(transferID);
            }
            else if (messageType == MessageType.SendFileCancel) {
                ProcessSendFileCancelMessage(transferID);
            }
            else if (messageType == MessageType.Response) {
                ResponseContent responseContent = content as ResponseContent;
                if (responseContent is null) {
                    throw new InvalidOperationException();
                }

                ProcessResponseMessage(transferID, responseContent.ResponseType);
            }
        }

        private void ProcessSendFileInitialMessage(Guid transferID, FileInitialContent content) {
            ReceiveFileContext context = new ReceiveFileContext() {
                FilePath = content.FileName,
                FileSize = content.FileSize,
                FileHash = content.FileHash
            };
            bool result = _receiveFileContexts.TryAdd(transferID, context);
            if (!result) {
                return;
            }

            ReceiveFileEventArgs recvFileEventArgs = new ReceiveFileEventArgs(
                transferID,
                context.FilePath,
                context.FileSize,
                ReceiveFileState.Initializing,
                context.BytesReceived
            );
            OnFileReceive(recvFileEventArgs);

            DebugInfo(transferID, context);
        }

        private void ProcessSendFileRegularMessage(Guid transferID, FileRegularContent content) {
            bool result = _receiveFileContexts.TryGetValue(
                transferID,
                out ReceiveFileContext recvContext
            );
            if (!result) {
                return;
            }

            if (recvContext.Cancelled) {
                recvContext.End();
                AddResponseToSendQueue(transferID, ResponseType.ReceiveFileCancel);
                _receiveFileContexts.TryRemove(transferID, out _);
                return;
            }

            recvContext.Writer.Write(content.FileBlock);
            recvContext.BytesReceived += content.FileBlock.LongLength;

            ReceiveFileEventArgs recvFileEventArgs = new ReceiveFileEventArgs(
                transferID,
                recvContext.FilePath,
                recvContext.FileSize,
                ReceiveFileState.Receiving,
                recvContext.BytesReceived
            );
            OnFileReceive(recvFileEventArgs);

            DebugInfo(transferID, recvContext);
            AddResponseToSendQueue(transferID, ResponseType.ReceiveFileRegular);
        }

        private void ProcessSendFileEndMessage(Guid transferID) {
            bool result = _receiveFileContexts.TryGetValue(
                transferID,
                out ReceiveFileContext context
            );
            if (!result) {
                return;
            }

            context.End();
            DebugInfo(transferID, context);

            CheckHashAsync(transferID);
        }

        private void ProcessSendFileCancelMessage(Guid transferID) {
            bool result = _receiveFileContexts.TryGetValue(
                transferID,
                out ReceiveFileContext context
            );
            if (!result) {
                return;
            }

            context.Cancel();

            ReceiveFileEventArgs recvFileEventArgs = new ReceiveFileEventArgs(
                transferID,
                context.FilePath,
                context.FileSize,
                ReceiveFileState.Cancelled,
                context.BytesReceived
            );
            OnFileReceive(recvFileEventArgs);

            _receiveFileContexts.TryRemove(transferID, out _);
        }

        private void ProcessResponseMessage(Guid transferID, ResponseType responseType) {
            bool result = _sendFileContexts.TryGetValue(
                transferID,
                out SendFileContext sendContext
            );
            if (!result) {
                return;
            }

            bool initialized = false;
            if (responseType == ResponseType.ReceiveFileInitial) {
                sendContext.Initialize();
                SendFileEventArgs sendFileEventArgs = new SendFileEventArgs(
                    transferID,
                    sendContext.FilePath,
                    sendContext.FileSize,
                    SendFileState.Initializing,
                    sendContext.BytesSent
                );
                OnFileSend(sendFileEventArgs);
                initialized = true;
            }
            if (responseType == ResponseType.ReceiveFileRegular || initialized) {
                if (sendContext.BytesSent == sendContext.FileSize) {
                    SendFileEventArgs sendFileEventArgs = new SendFileEventArgs(
                        transferID,
                        sendContext.FilePath,
                        sendContext.FileSize,
                        SendFileState.Completed,
                        sendContext.BytesSent
                    );
                    OnFileSend(sendFileEventArgs);

                    DebugInfo(transferID, sendContext);
                    AddMessageToSendQueue(transferID, MessageType.SendFileEnd);

                    _sendFileContexts.TryRemove(transferID, out _);
                }
                else {
                    byte[] fileBlock = sendContext.Reader.ReadBytes(FileBlockSize);
                    sendContext.BytesSent += fileBlock.Length;
                    FileRegularContent content = new FileRegularContent(fileBlock);

                    SendFileEventArgs sendFileEventArgs = new SendFileEventArgs(
                        transferID,
                        sendContext.FilePath,
                        sendContext.FileSize,
                        SendFileState.Sending,
                        sendContext.BytesSent
                    );
                    OnFileSend(sendFileEventArgs);

                    DebugInfo(transferID, sendContext);
                    AddMessageToSendQueue(transferID, MessageType.SendFileRegular, content);
                }
            }
            else if (responseType == ResponseType.ReceiveFileCancel) {
                sendContext.Cancel();
                DebugInfo(transferID, sendContext);

                SendFileEventArgs sendFileEventArgs = new SendFileEventArgs(
                    transferID,
                    sendContext.FilePath,
                    sendContext.FileSize,
                    SendFileState.Cancelled,
                    sendContext.BytesSent
                );
                OnFileSend(sendFileEventArgs);

                _sendFileContexts.TryRemove(transferID, out _);
            }
        }

        private void AddMessageToSendQueue(Guid transferID, MessageType messageType) {
            AddMessageToSendQueue(transferID, messageType, null);
        }

        private void AddMessageToSendQueue(Guid transferID, MessageType messageType, ContentBase content) {
            byte[] messageBuffer = _messageFramer.Frame(transferID, messageType, content);
            _messagesToSend.Enqueue(messageBuffer);
        }

        private void AddResponseToSendQueue(Guid transferID, ResponseType responseType) {
            byte[] responseBuffer = _messageFramer.Frame(
                transferID,
                MessageType.Response,
                new ResponseContent(responseType)
            );
            _messagesToSend.Enqueue(responseBuffer);
        }

        public bool InitializeReceive(Guid transferID, string newFilePath) {
            bool result = _receiveFileContexts.TryGetValue(
                transferID,
                out ReceiveFileContext recvContext
            );
            if (!result) {
                return false;
            }
            if (recvContext.Initialized) {
                return true;
            }

            recvContext.Initialize(newFilePath);

            ReceiveFileEventArgs recvFileEventArgs = new ReceiveFileEventArgs(
                transferID,
                recvContext.FilePath,
                recvContext.FileSize,
                ReceiveFileState.Receiving,
                recvContext.BytesReceived
            );
            OnFileReceive(recvFileEventArgs);

            DebugInfo(transferID, recvContext);
            AddResponseToSendQueue(transferID, ResponseType.ReceiveFileInitial);
            return true;
        }

        public bool CancellSend(Guid transferID) {
            bool result = _sendFileContexts.TryGetValue(transferID, out SendFileContext context);
            if (!result) {
                return false;
            }
            context.Cancel();
            SendFileEventArgs sendFileEventArgs = new SendFileEventArgs(
                transferID,
                context.FilePath,
                context.FileSize,
                SendFileState.Cancelled,
                context.BytesSent
            );
            OnFileSend(sendFileEventArgs);
            AddMessageToSendQueue(transferID, MessageType.SendFileCancel);
            return true;
        }

        public bool CancellReceive(Guid transferID) {
            bool result = _receiveFileContexts.TryGetValue(transferID, out ReceiveFileContext context);
            if (!result) {
                return false;
            }
            context.Cancel();
            ReceiveFileEventArgs recvFileEventArgs = new ReceiveFileEventArgs(
                transferID,
                context.FilePath,
                context.FileSize,
                ReceiveFileState.Cancelled,
                context.BytesReceived
            );
            OnFileReceive(recvFileEventArgs);
            AddResponseToSendQueue(transferID, ResponseType.ReceiveFileCancel);
            return true;
        }

        private async void StartSendAsync(CancellationToken cancellationToken) {
            await Task.Run(() => StartSend(cancellationToken));
        }

        private void StartSend(CancellationToken cancellationToken) {
            while (!cancellationToken.IsCancellationRequested) {
                bool result = _messagesToSend.TryDequeue(out byte[] messageBuffer);
                if (!result) {
                    continue;
                }

                _client.SendBytes(messageBuffer);
            }
        }

        private int ReceiveLength() {
            byte[] bufferLength =
                _client.ReceiveBytes(((MessageFramer)_messageFramer).LengthPrefixWrapper.PrefixLength);
            int length = BitConverter.ToInt32(bufferLength, 0);
            return length;
        }

        private byte[] ReceiveMessage() {
            int messageLength = ReceiveLength();
            byte[] messageBuffer = _client.ReceiveBytes(messageLength);
            return messageBuffer;
        }

        private void OnFileSend(SendFileEventArgs e) {
            FileSend?.Invoke(this, e);
        }

        private void OnFileReceive(ReceiveFileEventArgs e) {
            FileReceive?.Invoke(this, e);
        }

        private void OnConnectionLost(ConnectionLostEventArgs e) {
            ConnectionLost?.Invoke(this, e);
        }

        private async void CheckHashAsync(Guid transferID) {
            await Task.Run(() => {
                bool result = _receiveFileContexts.TryGetValue(
                    transferID,
                    out ReceiveFileContext recvContext
                );
                if (!result) {
                    return;
                }

                ReceiveFileEventArgs recvFileEventArgs = new ReceiveFileEventArgs(
                    transferID,
                    recvContext.FilePath,
                    recvContext.FileSize,
                    ReceiveFileState.Hashing,
                    recvContext.BytesReceived
                );
                OnFileReceive(recvFileEventArgs);
                byte[] hash = _fileHashCalculator.Calculate(recvContext.FilePath);
                if (hash.SequenceEqual(recvContext.FileHash)) {
                    recvFileEventArgs = new ReceiveFileEventArgs(
                        transferID,
                        recvContext.FilePath,
                        recvContext.FileSize,
                        ReceiveFileState.Completed,
                        recvContext.BytesReceived
                    );
                    OnFileReceive(recvFileEventArgs);
                }
                else {
                    recvFileEventArgs = new ReceiveFileEventArgs(
                        transferID,
                        recvContext.FilePath,
                        recvContext.FileSize,
                        ReceiveFileState.Failed,
                        recvContext.BytesReceived
                    );
                    OnFileReceive(recvFileEventArgs);
                }

                _receiveFileContexts.TryRemove(transferID, out _);
            });
        }

        public async Task SendFileAsync(string path) {
            await Task.Run(() => {
                FileInfo fileInfo = new FileInfo(path);

                SendFileContext sendContext = new SendFileContext() {
                    FilePath = path,
                    FileSize = fileInfo.Length
                };

                Guid transferID = Guid.NewGuid();
                SendFileEventArgs sendFileEventArgs = new SendFileEventArgs(
                    transferID,
                    sendContext.FilePath,
                    sendContext.FileSize,
                    SendFileState.Hashing,
                    sendContext.BytesSent
                );
                OnFileSend(sendFileEventArgs);

                sendContext.FileHash = _fileHashCalculator.Calculate(path);

                _sendFileContexts.TryAdd(transferID, sendContext);
                sendFileEventArgs = new SendFileEventArgs(
                    transferID,
                    sendContext.FilePath,
                    sendContext.FileSize,
                    SendFileState.Initializing,
                    sendContext.BytesSent
                );
                OnFileSend(sendFileEventArgs);
                FileInitialContent initialContent =
                    new FileInitialContent(fileInfo.Name, sendContext.FileSize, sendContext.FileHash);
                DebugInfo(transferID, sendContext);
                AddMessageToSendQueue(transferID, MessageType.SendFileInitial, initialContent);
            });
        }

        private void DebugInfo(Guid transferID, SendFileContext context) {
            Debug.WriteLine($"TransferID: {transferID}, FilePath: {context.FilePath}, " +
                $"FileSize: {context.FileSize}, BytesSent: {context.BytesSent}, " +
                $"Initialized: {context.Initialized}, Cancelled: {context.Cancelled}");
        }

        private void DebugInfo(Guid transferID, ReceiveFileContext context) {
            Debug.WriteLine($"TransferID: {transferID}, FilePath: {context.FilePath}, " +
                $"FileSize: {context.FileSize}, BytesReceived: {context.BytesReceived}",
                $"Initialized: {context.Initialized}, Cancelled: {context.Cancelled}");
        }

        public async Task Clean() {
            await Task.Run(() => {
                foreach (var c in _sendFileContexts) {
                    c.Value?.Reader.Close();
                }
                foreach (var c in _receiveFileContexts) {
                    c.Value?.Writer.Close();
                }
            });
        }

        public void Disconnect() {
            _client.Disconnect();
        }
    }
}

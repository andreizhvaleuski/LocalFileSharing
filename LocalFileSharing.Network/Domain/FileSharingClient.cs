using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using LocalFileSharing.Network.Domain.Context;
using LocalFileSharing.Network.Domain.States;
using LocalFileSharing.Network.Framing;
using LocalFileSharing.Network.Framing.Content;
using LocalFileSharing.Network.Framing.Wrappers;
using LocalFileSharing.Network.Sockets;

namespace LocalFileSharing.Network.Domain {
    public sealed class FileSharingClient {
        public const int FileBlockSize = 1024 * 1024 * 2;

        private readonly IMessageFramer _messageFramer;
        private readonly IFileHashCalculator _fileHashCalculator;

        private readonly ConcurrentDictionary<Guid, SendFileContext> _sendFileContexts;
        private readonly ConcurrentDictionary<Guid, ReceiveFileContext> _receiveFileContexts;

        private readonly ConcurrentQueue<byte[]> _messagesToSend;
        private readonly ConcurrentQueue<byte[]> _receivedMessages;

        private readonly TcpClient _client;

        private readonly CancellationTokenSource _connectionCancellationTokenSource;

        public event EventHandler FileSend;
        public event EventHandler FileReceive;

        public string DownloadPath { get; private set; }

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

            _connectionCancellationTokenSource = new CancellationTokenSource();

            DownloadPath = @"D:\Downloads";

            Initialize(_connectionCancellationTokenSource.Token);
        }

        private void Initialize(CancellationToken cancellationToken) {
            StartSendAsync(cancellationToken);
            StartReceiveAsync(cancellationToken);
            StartProcessReceivedMessagesAsync(cancellationToken);
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

        private void ProcessReceivedMessage(
            Guid transferID,
            MessageType messageType,
            ContentBase content
        ) {
            if (messageType == MessageType.SendFileInitial) {
                FileInitialContent initialContent = content as FileInitialContent;
                if (initialContent is null) {
                    throw new ArgumentException();
                }

                ReceiveFileContext context = new ReceiveFileContext() {
                    FilePath = Path.Combine(DownloadPath, initialContent.FileName),
                    ReceiveState = ReceiveFileState.Initializing
                };
                context.Writer = new BinaryWriter(File.Create(context.FilePath));
                bool result = _receiveFileContexts.TryAdd(transferID, context);
                if (result) {
                    // TO DO:
                    // Add transferID to ignored.
                    return;
                }

                byte[] responseBuffer = _messageFramer.Frame(
                    transferID,
                    MessageType.Response,
                    new ResponseContent(ResponseType.ReceiveFileInitial)
                );
                _messagesToSend.Enqueue(responseBuffer);
            }
            else if (messageType == MessageType.SendFileRegular) {
                FileRegularContent regularContent = content as FileRegularContent;
                if (regularContent is null) {
                    throw new ArgumentException();
                }

                bool result = _receiveFileContexts.TryGetValue(
                    transferID,
                    out ReceiveFileContext context
                );
                if (!result) {
                    // TODO;

                    return;
                }
                context.Writer.Write(regularContent.FileBlock);

                AddResponseToSendQueue(transferID, ResponseType.ReceiveFileRegular);
            }
            else if (messageType == MessageType.SendFileEnd) {
                bool result = _receiveFileContexts.TryGetValue(
                    transferID,
                    out ReceiveFileContext context
                );
                if (!result) {
                    // TODO;

                    return;
                }
                context.Writer.Close();

                byte[] hash = _fileHashCalculator.Calculate(context.FilePath);
                if (!hash.SequenceEqual(context.FileHash)) {
                    context.ReceiveState = ReceiveFileState.Failed;
                }

                _receiveFileContexts.TryRemove(transferID, out _);

                AddResponseToSendQueue(transferID, ResponseType.ReceiveFileEnd);
            }
            else if (messageType == MessageType.SendFileCancel) {
                bool result = _receiveFileContexts.TryGetValue(
                    transferID,
                    out ReceiveFileContext context
                );
                if (!result) {
                    // TODO;

                    return;
                }
                context.Writer.Close();

                _receiveFileContexts.TryRemove(transferID, out _);

                AddResponseToSendQueue(transferID, ResponseType.ReceiveFileCancel);
            }
            else if (messageType == MessageType.Response) {
                ResponseContent responseContent = content as ResponseContent;
                if (responseContent is null) {
                    throw new ArgumentException();
                }

                ProcessResponseMessage(transferID, responseContent.ResponseType);
            }
        }

        private void ProcessResponseMessage(
            Guid transferID,
            ResponseType responseType
        ) {
            bool result = _sendFileContexts.TryGetValue(
                transferID,
                out SendFileContext context
            );
            if (!result) {
                // TODO:
            }

            if (responseType == ResponseType.ReceiveFileInitial) {
                context.Reader = new BinaryReader(File.OpenRead(context.FilePath));

                byte[] fileBlock = context.Reader.ReadBytes(FileBlockSize);
                FileRegularContent content = new FileRegularContent(fileBlock);
                AddMessageToSendQueue(transferID, MessageType.SendFileRegular, content);
            }
            else if (responseType == ResponseType.ReceiveFileRegular) {
                if (context.BytesSent == context.FileSize) {
                    AddMessageToSendQueue(transferID, MessageType.SendFileEnd);
                }
                else {
                    byte[] fileBlock = context.Reader.ReadBytes(FileBlockSize);
                    FileRegularContent content = new FileRegularContent(fileBlock);
                    AddMessageToSendQueue(transferID, MessageType.SendFileRegular, content);
                }
            }
        }

        private void AddMessageToSendQueue(
            Guid transferID,
            MessageType messageType
        ) {
            AddMessageToSendQueue(transferID, messageType, null);
        }

        private void AddMessageToSendQueue(
            Guid transferID,
            MessageType messageType,
            ContentBase content
        ) {
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

        public bool InitializeReceive(Guid transferID) {
            bool result = _receiveFileContexts.TryGetValue(transferID, out ReceiveFileContext context);
            if (!result) {
                return false;
            }
            context.Initialize();
            AddResponseToSendQueue(transferID, ResponseType.ReceiveFileInitial);
            return true;
        }

        public bool CancellSend(Guid transferID) {
            bool result = _sendFileContexts.TryGetValue(transferID, out SendFileContext context);
            if (!result) {
                return false;
            }
            context.Cancel();
            return true;
        }

        public bool CancellReceive(Guid transferID) {
            bool result = _receiveFileContexts.TryGetValue(transferID, out ReceiveFileContext context);
            if (!result) {
                return false;
            }
            context.Cancel();
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

        private async void StartReceiveAsync(CancellationToken cancellationToken) {
            await Task.Run(() => StartReceive(cancellationToken));
        }

        private void StartReceive(CancellationToken cancellationToken) {
            while (!cancellationToken.IsCancellationRequested) {
                byte[] messageBuffer = ReceiveMessage();
                _receivedMessages.Enqueue(messageBuffer);
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
    }
}

using System;
using System.IO;

using LocalFileSharing.Network.Domain.States;

namespace LocalFileSharing.Network.Domain.Context {
    public class ReceiveFileContext : FileTransferContextBase {
        public ReceiveFileState State { get; protected set; }

        public BinaryWriter Writer { get; protected set; }

        public ReceiveFileContext(ReceiveFileState state, BinaryWriter writer) {
            if (state == ReceiveFileState.Unspecified) {
                throw new ArgumentException(
                    $"The state value can not be unspecified.",
                    nameof(state)
                );
            }

            if (writer is null) {
                throw new ArgumentNullException(nameof(writer));
            }

            State = state;
            Writer = writer;
        }

        public override void Close() {
            Writer.Close();
        }
    }
}

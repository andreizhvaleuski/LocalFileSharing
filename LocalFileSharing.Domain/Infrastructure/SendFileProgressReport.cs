using LocalFileSharing.Network.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalFileSharing.Domain.Infrastructure
{
    public class SendFileProgressReport
    {
        public SendFileState SendFileState { get; private set; }

        public string FilePath { get; private set; }

        public long FileSize { get; private set; }

        public long SentSize { get; private set; }

        public SendFileProgressReport(SendFileState sendFileState)
        {
            SendFileState = sendFileState;
        }
    }
}

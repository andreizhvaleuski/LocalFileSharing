using System;

namespace LocalFileSharing.Network.Content
{
    [Serializable]
    public class DemoType
    {
        public string FileName { get; set; }

        public long BlocksNumber { get; set; }
    }
}

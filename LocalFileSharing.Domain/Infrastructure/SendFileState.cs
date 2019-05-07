﻿namespace LocalFileSharing.Domain.Infrastructure {
    public enum SendFileState {
        Unspecified = 0,
        Hashing = 1,
        Initializing = 2,
        Sending = 3,
        Sent = 4,
        Cancelled = 6,
        Failed = 7
    }
}

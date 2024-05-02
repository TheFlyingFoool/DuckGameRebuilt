using System;
using System.Threading;

namespace XnaToFna
{
    public sealed class SyncResult : IAsyncResult
    {
        private object _AsyncState;

        public object AsyncState => _AsyncState;

        public WaitHandle AsyncWaitHandle => null;

        public bool CompletedSynchronously => true;

        public bool IsCompleted => true;

        public SyncResult(object state) => _AsyncState = state;
    }
}

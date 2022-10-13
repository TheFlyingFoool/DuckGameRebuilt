// Decompiled with JetBrains decompiler
// Type: XnaToFna.SyncResult
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System;
using System.Threading;

namespace XnaToFna
{
    public sealed class SyncResult : IAsyncResult
    {
        private object _AsyncState;

        public object AsyncState => this._AsyncState;

        public WaitHandle AsyncWaitHandle => null;

        public bool CompletedSynchronously => true;

        public bool IsCompleted => true;

        public SyncResult(object state) => this._AsyncState = state;
    }
}

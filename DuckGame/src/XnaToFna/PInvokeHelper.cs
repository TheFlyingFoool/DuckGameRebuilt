// Decompiled with JetBrains decompiler
// Type: XnaToFna.PInvokeHelper
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using MonoMod.Utils;
using System;
using System.Runtime.InteropServices;

namespace XnaToFna
{
    public static class PInvokeHelper
    {
        private static IntPtr _PThread;
        private static d_pthread_self pthread_self;

        public static IntPtr PThread
        {
            get
            {
                if (_PThread != IntPtr.Zero)
                    return _PThread;
                if (!DynDll.DllMap.ContainsKey("pthread"))
                    DynDll.DllMap["pthread"] = (PlatformHelper.Current & Platform.MacOS) != Platform.MacOS ? "libpthread.so" : "libpthread.dylib";
                return _PThread = DynDll.OpenLibrary("pthread");
            }
        }

        [DllImport("kernel32")]
        private static extern uint GetCurrentThreadId();

        public static ulong CurrentThreadId
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    return GetCurrentThreadId();
                d_pthread_self dPthreadSelf;
                pthread_self = dPthreadSelf = pthread_self ?? PThread.GetFunction("pthread_self").AsDelegate<d_pthread_self>();
                return dPthreadSelf == null ? 0UL : dPthreadSelf();
            }
        }

        private delegate ulong d_pthread_self();
    }
}

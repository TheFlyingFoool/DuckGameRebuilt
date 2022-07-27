// Decompiled with JetBrains decompiler
// Type: DbMon.NET.DebugMonitor
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace DbMon.NET
{
    /// <summary>
    /// This class captures all strings passed to <c>OutputDebugString</c> when
    /// the application is not debugged.
    /// </summary>
    /// <remarks>
    /// This class is a port of Microsofts Visual Studio's C++ example "dbmon", which
    /// can be found at <c>http://msdn.microsoft.com/library/default.asp?url=/library/en-us/vcsample98/html/vcsmpdbmon.asp</c>.
    /// </remarks>
    /// <remarks>
    /// 	<code>
    /// 		public static void Main(string[] args) {
    /// 			DebugMonitor.Start();
    /// 			DebugMonitor.OnOutputDebugString += new OnOutputDebugStringHandler(OnOutputDebugString);
    /// 			Console.WriteLine("Press 'Enter' to exit.");
    /// 			Console.ReadLine();
    /// 			DebugMonitor.Stop();
    /// 		}
    /// 
    /// 		private static void OnOutputDebugString(int pid, string text) {
    /// 			Console.WriteLine(DateTime.Now + ": " + text);
    /// 		}
    /// 	</code>
    /// </remarks>
    public sealed class DebugMonitor
    {
        private const int WAIT_OBJECT_0 = 0;
        private const uint INFINITE = 4294967295;
        private const int ERROR_ALREADY_EXISTS = 183;
        private const uint SECURITY_DESCRIPTOR_REVISION = 1;
        private const uint SECTION_MAP_READ = 4;
        /// <summary>Event handle for slot 'DBWIN_BUFFER_READY'</summary>
        private static IntPtr m_AckEvent = IntPtr.Zero;
        /// <summary>Event handle for slot 'DBWIN_DATA_READY'</summary>
        private static IntPtr m_ReadyEvent = IntPtr.Zero;
        /// <summary>Handle for our shared file</summary>
        private static IntPtr m_SharedFile = IntPtr.Zero;
        /// <summary>Handle for our shared memory</summary>
        private static IntPtr m_SharedMem = IntPtr.Zero;
        /// <summary>Our capturing thread</summary>
        private static Thread m_Capturer = null;
        /// <summary>Our synchronization root</summary>
        private static object m_SyncRoot = new object();
        /// <summary>Mutex for singleton check</summary>
        private static Mutex m_Mutex = null;

        /// <summary>
        /// Private constructor so no one can create a instance
        /// of this static class
        /// </summary>
        private DebugMonitor()
        {
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr MapViewOfFile(
          IntPtr hFileMappingObject,
          uint dwDesiredAccess,
          uint dwFileOffsetHigh,
          uint dwFileOffsetLow,
          uint dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool InitializeSecurityDescriptor(
          ref DebugMonitor.SECURITY_DESCRIPTOR sd,
          uint dwRevision);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetSecurityDescriptorDacl(
          ref DebugMonitor.SECURITY_DESCRIPTOR sd,
          bool daclPresent,
          IntPtr dacl,
          bool daclDefaulted);

        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateEvent(
          ref DebugMonitor.SECURITY_ATTRIBUTES sa,
          bool bManualReset,
          bool bInitialState,
          string lpName);

        [DllImport("kernel32.dll")]
        private static extern bool PulseEvent(IntPtr hEvent);

        [DllImport("kernel32.dll")]
        private static extern bool SetEvent(IntPtr hEvent);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFileMapping(
          IntPtr hFile,
          ref DebugMonitor.SECURITY_ATTRIBUTES lpFileMappingAttributes,
          DebugMonitor.PageProtection flProtect,
          uint dwMaximumSizeHigh,
          uint dwMaximumSizeLow,
          string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("kernel32", SetLastError = true)]
        private static extern int WaitForSingleObject(IntPtr handle, uint milliseconds);

        /// <summary>
        /// Fired if an application calls <c>OutputDebugString</c>
        /// </summary>
        public static event OnOutputDebugStringHandler OnOutputDebugString;

        /// <summary>Starts this debug monitor</summary>
        public static void Start()
        {
            lock (DebugMonitor.m_SyncRoot)
            {
                if (DebugMonitor.m_Capturer != null)
                    throw new ApplicationException("This DebugMonitor is already started.");
                if (Environment.OSVersion.ToString().IndexOf("Microsoft") == -1)
                    throw new NotSupportedException("This DebugMonitor is only supported on Microsoft operating systems.");
                bool createdNew = false;
                DebugMonitor.m_Mutex = new Mutex(false, typeof(DebugMonitor).Namespace, out createdNew);
                if (!createdNew)
                    throw new ApplicationException("There is already an instance of 'DbMon.NET' running.");
                DebugMonitor.SECURITY_DESCRIPTOR sd = new DebugMonitor.SECURITY_DESCRIPTOR();
                if (!DebugMonitor.InitializeSecurityDescriptor(ref sd, 1U))
                    throw DebugMonitor.CreateApplicationException("Failed to initializes the security descriptor.");
                if (!DebugMonitor.SetSecurityDescriptorDacl(ref sd, true, IntPtr.Zero, false))
                    throw DebugMonitor.CreateApplicationException("Failed to initializes the security descriptor");
                DebugMonitor.SECURITY_ATTRIBUTES securityAttributes = new DebugMonitor.SECURITY_ATTRIBUTES();
                DebugMonitor.m_AckEvent = DebugMonitor.CreateEvent(ref securityAttributes, false, false, "DBWIN_BUFFER_READY");
                if (DebugMonitor.m_AckEvent == IntPtr.Zero)
                    throw DebugMonitor.CreateApplicationException("Failed to create event 'DBWIN_BUFFER_READY'");
                DebugMonitor.m_ReadyEvent = DebugMonitor.CreateEvent(ref securityAttributes, false, false, "DBWIN_DATA_READY");
                if (DebugMonitor.m_ReadyEvent == IntPtr.Zero)
                    throw DebugMonitor.CreateApplicationException("Failed to create event 'DBWIN_DATA_READY'");
                DebugMonitor.m_SharedFile = DebugMonitor.CreateFileMapping(new IntPtr(-1), ref securityAttributes, DebugMonitor.PageProtection.ReadWrite, 0U, 4096U, "DBWIN_BUFFER");
                DebugMonitor.m_SharedMem = !(DebugMonitor.m_SharedFile == IntPtr.Zero) ? DebugMonitor.MapViewOfFile(DebugMonitor.m_SharedFile, 4U, 0U, 0U, 512U) : throw DebugMonitor.CreateApplicationException("Failed to create a file mapping to slot 'DBWIN_BUFFER'");
                if (DebugMonitor.m_SharedMem == IntPtr.Zero)
                    throw DebugMonitor.CreateApplicationException("Failed to create a mapping view for slot 'DBWIN_BUFFER'");
                DebugMonitor.m_Capturer = new Thread(new ThreadStart(DebugMonitor.Capture));
                DebugMonitor.m_Capturer.Start();
            }
        }

        /// <summary>Captures</summary>
        private static void Capture()
        {
            try
            {
                IntPtr ptr = new IntPtr(DebugMonitor.m_SharedMem.ToInt32() + Marshal.SizeOf(typeof(int)));
                while (true)
                {
                    int num;
                    do
                    {
                        DebugMonitor.SetEvent(DebugMonitor.m_AckEvent);
                        num = DebugMonitor.WaitForSingleObject(DebugMonitor.m_ReadyEvent, uint.MaxValue);
                        if (DebugMonitor.m_Capturer == null)
                            goto label_4;
                    }
                    while (num != 0);
                    DebugMonitor.FireOnOutputDebugString(Marshal.ReadInt32(DebugMonitor.m_SharedMem), Marshal.PtrToStringAnsi(ptr));
                }
            label_4:;
            }
            catch
            {
                throw;
            }
            finally
            {
                DebugMonitor.Dispose();
            }
        }

        private static void FireOnOutputDebugString(int pid, string text)
        {
            if (DebugMonitor.OnOutputDebugString == null)
                return;
            try
            {
                DebugMonitor.OnOutputDebugString(pid, text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An 'OnOutputDebugString' handler failed to execute: " + ex.ToString());
            }
        }

        /// <summary>Dispose all resources</summary>
        private static void Dispose()
        {
            if (DebugMonitor.m_AckEvent != IntPtr.Zero)
                DebugMonitor.m_AckEvent = DebugMonitor.CloseHandle(DebugMonitor.m_AckEvent) ? IntPtr.Zero : throw DebugMonitor.CreateApplicationException("Failed to close handle for 'AckEvent'");
            if (DebugMonitor.m_ReadyEvent != IntPtr.Zero)
                DebugMonitor.m_ReadyEvent = DebugMonitor.CloseHandle(DebugMonitor.m_ReadyEvent) ? IntPtr.Zero : throw DebugMonitor.CreateApplicationException("Failed to close handle for 'ReadyEvent'");
            if (DebugMonitor.m_SharedFile != IntPtr.Zero)
                DebugMonitor.m_SharedFile = DebugMonitor.CloseHandle(DebugMonitor.m_SharedFile) ? IntPtr.Zero : throw DebugMonitor.CreateApplicationException("Failed to close handle for 'SharedFile'");
            if (DebugMonitor.m_SharedMem != IntPtr.Zero)
                DebugMonitor.m_SharedMem = DebugMonitor.UnmapViewOfFile(DebugMonitor.m_SharedMem) ? IntPtr.Zero : throw DebugMonitor.CreateApplicationException("Failed to unmap view for slot 'DBWIN_BUFFER'");
            if (DebugMonitor.m_Mutex == null)
                return;
            DebugMonitor.m_Mutex.Close();
            DebugMonitor.m_Mutex = null;
        }

        /// <summary>
        /// Stops this debug monitor. This call we block the executing thread
        /// until this debug monitor is stopped.
        /// </summary>
        public static void Stop()
        {
            lock (DebugMonitor.m_SyncRoot)
            {
                DebugMonitor.m_Capturer = DebugMonitor.m_Capturer != null ? null : throw new ObjectDisposedException(nameof(DebugMonitor), "This DebugMonitor is not running.");
                DebugMonitor.PulseEvent(DebugMonitor.m_ReadyEvent);
                do
                    ;
                while (DebugMonitor.m_AckEvent != IntPtr.Zero);
            }
        }

        /// <summary>
        /// Helper to create a new application exception, which has automaticly the
        /// last win 32 error code appended.
        /// </summary>
        /// <param name="text">text</param>
        private static ApplicationException CreateApplicationException(string text) => text != null && text.Length >= 1 ? new ApplicationException(string.Format("{0}. Last Win32 Error was {1}", text, Marshal.GetLastWin32Error())) : throw new ArgumentNullException(nameof(text), "'text' may not be empty or null.");

        private struct SECURITY_DESCRIPTOR
        {
            public byte revision;
            public byte size;
            public short control;
            public IntPtr owner;
            public IntPtr group;
            public IntPtr sacl;
            public IntPtr dacl;
        }

        private struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [System.Flags]
        private enum PageProtection : uint
        {
            NoAccess = 1,
            Readonly = 2,
            ReadWrite = 4,
            WriteCopy = 8,
            Execute = 16, // 0x00000010
            ExecuteRead = 32, // 0x00000020
            ExecuteReadWrite = 64, // 0x00000040
            ExecuteWriteCopy = 128, // 0x00000080
            Guard = 256, // 0x00000100
            NoCache = 512, // 0x00000200
            WriteCombine = 1024, // 0x00000400
        }
    }
}

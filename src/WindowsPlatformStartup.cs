// Decompiled with JetBrains decompiler
// Type: DGWindows.WindowsPlatformStartup
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using DuckGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace DGWindows
{
    internal class WindowsPlatformStartup
    {
        public static List<string> _moduleDependencies = new List<string>()
    {
      "ntdll.dll",
      "MSCOREE.DLL",
      "KERNEL32.dll",
      "KERNELBASE.dll",
      "ADVAPI32.dll",
      "msvcrt.dll",
      "sechost.dll",
      "RPCRT4.dll",
      "mscoreei.dll",
      "SHLWAPI.dll",
      "kernel.appcore.dll",
      "VERSION.dll",
      "clr.dll",
      "USER32.dll",
      "win32u.dll",
      "VCRUNTIME140_CLR0400.dll",
      "ucrtbase_clr0400.dll",
      "GDI32.dll",
      "gdi32full.dll",
      "msvcp_win.dll",
      "ucrtbase.dll",
      "IMM32.DLL",
      "mscorlib.ni.dll",
      "ole32.dll",
      "combase.dll",
      "bcryptPrimitives.dll",
      "clrjit.dll",
      "OLEAUT32.dll",
      "System.ni.dll",
      "System.Core.ni.dll",
      "CRYPTSP.dll",
      "rsaenh.dll",
      "bcrypt.dll",
      "CRYPTBASE.dll",
      "Steam.dll",
      "MSVCR100.dll",
      "MSVCP100.dll",
      "steam_api.dll",
      "SHELL32.dll",
      "System.Drawing.ni.dll",
      "System.Windows.Forms.ni.dll",
      "windows.storage.dll",
      "Wldp.dll",
      "SHCORE.dll",
      "steamclient.dll",
      "WS2_32.dll",
      "CRYPT32.dll",
      "imagehlp.dll",
      "WINMM.dll",
      "PSAPI.DLL",
      "IPHLPAPI.DLL",
      "SETUPAPI.dll",
      "cfgmgr32.dll",
      "tier0_s.dll",
      "vstdlib_s.dll",
      "MSWSOCK.dll",
      "Secur32.dll",
      "SSPICLI.DLL",
      "gameoverlayrenderer.dll",
      "Microsoft.Xna.Framework.dll",
      "WindowsCodecs.dll",
      "X3DAudio1_7.dll",
      "Microsoft.Xna.Framework.Graphics.dll",
      "d3d9.dll",
      "d3dx9_41.dll",
      "dwmapi.dll",
      "uxtheme.dll",
      "nvldumd.dll",
      "msasn1.dll",
      "cryptnet.dll",
      "WINTRUST.DLL",
      "nvd3dum.dll",
      "System.Configuration.ni.dll",
      "System.Xml.ni.dll",
      "profapi.dll",
      "comctl32.dll",
      "gdiplus.dll",
      "DWrite.dll",
      "MSCTF.dll",
      "gpapi.dll",
      "dxcore.dll",
      "textinputframework.dll",
      "CoreMessaging.dll",
      "CoreUIComponents.dll",
      "wintypes.dll",
      "ntmarta.dll",
      "Oleacc.dll",
      "Accessibility.ni.dll",
      "clbcatq.dll",
      "sxs.dll",
      "MMDevApi.dll",
      "DEVOBJ.dll",
      "AUDIOSES.DLL",
      "powrprof.dll",
      "UMPDC.dll",
      "Windows.UI.dll",
      "InputHost.dll",
      "WindowManagementAPI.dll",
      "twinapi.appcore.dll",
      "PROPSYS.dll",
      "xinput1_3.dll",
      "DGInput.dll",
      "DINPUT8.dll",
      "HID.DLL",
      "amsi.dll",
      "USERENV.dll",
      "MpOav.dll",
      "MPCLIENT.DLL",
      "wbemprox.dll",
      "wbemcomn.dll",
      "wbemsvc.dll",
      "fastprox.dll",
      "System.IO.Compression.ni.dll",
      "winmmbase.dll",
      "wdmaud.drv",
      "ksuser.dll",
      "AVRT.dll",
      "msacm32.drv",
      "MSACM32.dll",
      "midimap.dll",
      "resourcepolicyclient.dll",
      "System.Speech.ni.dll",
      "sapi.dll",
      "msdmo.dll"
    };
        private static string kWineVersion = null;
        public static List<string> assemblyLoadStrings = new List<string>();
        //private const int ENUM_CURRENT_SETTINGS = -1;
        //private const int ENUM_REGISTRY_SETTINGS = -2;
        //public static int displayRefreshRate;

        public static WindowsPlatformStartup.MachineType GetDllMachineType(
          string dllPath)
        {
            FileStream input = new FileStream(dllPath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(input);
            input.Seek(60L, SeekOrigin.Begin);
            input.Seek(binaryReader.ReadInt32(), SeekOrigin.Begin);
            WindowsPlatformStartup.MachineType dllMachineType = binaryReader.ReadUInt32() == 17744U ? (WindowsPlatformStartup.MachineType)binaryReader.ReadUInt16() : throw new Exception("Can't find PE header");
            binaryReader.Close();
            input.Close();
            return dllMachineType;
        }

        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

        [DllImport("kernel32.dll")]
        private static extern uint SetErrorMode(uint mode);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetDllDirectory(string lpPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint SearchPath(
          string lpPath,
          string lpFileName,
          string lpExtension,
          int nBufferLength,
          [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpBuffer,
          out IntPtr lpFilePart);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        public static string CheckLibraryError(string pLibrary)
        {
            if (WindowsPlatformStartup.LoadLibrary(pLibrary) == IntPtr.Zero)
            {
                switch (WindowsPlatformStartup.GetLastError())
                {
                    case 0:
                        break;
                    case 126:
                        return null;
                    default:
                        try
                        {
                            StringBuilder lpBuffer = new StringBuilder(byte.MaxValue);
                            IntPtr lpFilePart = new IntPtr();
                            int num = (int)WindowsPlatformStartup.SearchPath(null, pLibrary, null, byte.MaxValue, lpBuffer, out lpFilePart);
                            return lpBuffer.ToString();
                        }
                        catch (Exception)
                        {
                        }
                        return pLibrary;
                }
            }
            return null;
        }

        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        public static bool isRunningWine => WindowsPlatformStartup.kWineVersion != null;

        public static string wineVersion => WindowsPlatformStartup.kWineVersion;

#pragma warning disable IDE0051 // Remove unused private members
        private static void Main4(string[] args)
#pragma warning restore IDE0051 // Remove unused private members
        {
            try
            {
                IntPtr hModule = WindowsPlatformStartup.LoadLibrary("ntdll.dll");
                if (hModule != IntPtr.Zero)
                {
                    IntPtr procAddress = WindowsPlatformStartup.GetProcAddress(hModule, "wine_get_version");
                    if (procAddress != IntPtr.Zero)
                    {
                        WindowsPlatformStartup.kWineVersion = "unknown";
                        IntPtr ptr = ((WindowsPlatformStartup.wine_version_delegate)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(WindowsPlatformStartup.wine_version_delegate)))();
                        if (ptr != IntPtr.Zero)
                            WindowsPlatformStartup.kWineVersion = Marshal.PtrToStringAnsi(ptr);
                    }
                }
            }
            catch (Exception)
            {
            }
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(WindowsPlatformStartup.UnhandledExceptionTrapper);
            DuckGame.Program.Main(args);
        }
        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            if (!System.IO.File.Exists("CrashWindow.exe"))
                return;
            try
            {
                DuckGame.Program.HandleGameCrash(e.ExceptionObject as Exception);
            }
            catch (Exception ex)
            {
                string pLogMessage = WindowsPlatformStartup.ProcessErrorLine(e.ExceptionObject.ToString(), e.ExceptionObject as Exception);
                StreamWriter streamWriter = new StreamWriter("ducklog.txt", true);
                streamWriter.WriteLine(pLogMessage);
                streamWriter.Close();
                Process.Start("CrashWindow.exe", "-modResponsible 0 -modDisabled 0 -modName none -source " + (e.ExceptionObject as Exception).Source + " -commandLine \"none\" -executable \"" + Application.ExecutablePath + "\" " + WindowsPlatformStartup.GetCrashWindowString(ex, null, pLogMessage));
            }
        }

        public static string ProcessErrorLine(string pLine, Exception pException)
        {
            switch (pException)
            {
                case FileNotFoundException _:
                    if (pException.Message.Contains("Microsoft.Xna.Framework"))
                    {
                        pLine = "It seems like the XNA Framework is not installed! Info on getting XNA can be found here:\nhttps://steamcommunity.com/app/312530/discussions/1/2997675206112505333/\n\n" + pLine;
                        break;
                    }
                    if (pException.Message.Contains("Steam.dll"))
                    {
                        pLine = "It seems like you may be missing Microsoft's VC++ 2015 Redists! Please download and install them from Microsoft:\nhttps://www.microsoft.com/en-us/download/details.aspx?id=52685\n\n" + pLine;
                        break;
                    }
                    break;
                case BadImageFormatException _:
                case FileLoadException _:
                    string str1 = "";
                    try
                    {
                        foreach (string str2 in WindowsPlatformStartup.BadFormatExceptionAssembly())
                            str1 = str1 + str2 + "\n";
                    }
                    catch (Exception)
                    {
                    }
                    pLine = "One or more DLL files failed to load. This usually means the file is 64-bit, but it's supposed to be 32-bit:\n" + str1 + "\n\nThere may be an issue with your .NET Framework installation, or with the location/version of some of your Windows DLL files... Please check the 'System.BadImageFormatException' section on the DG common issues page:\nhttps://steamcommunity.com/app/312530/discussions/1/2997675206112505333/\n\n" + pLine;
                    break;
                case OutOfMemoryException _:
                label_14:
                    pLine = "Duck Game ran out of memory! It's possible that you have *TOO MANY MODS* installed. Try unsubscribing from some mods, or disable some mods through the options menu by running DG with the '-nomods' launch option:\nhttps://www.microsoft.com/en-ca/download/details.aspx?id=20914\n\n" + pLine;
                    break;
                default:
                    if (!pException.ToString().Contains("System.OutOfMemoryException"))
                        break;
                    goto label_14;
            }
            return pLine;
        }

        public static string GetCrashWindowString(
          Exception pException,
          string pAssemblyName,
          string pLogMessage)
        {
            string str1 = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string str2 = "";
            string plainText1 = pAssemblyName != null ? pAssemblyName : "DuckGame";
            string plainText2 = pException != null ? pException.GetType().ToString() : "Unknown";
            string str3 = CrashWindow.CrashWindow.Base64Encode(pLogMessage == "" ? "none" : pLogMessage);
            return " -pVersion " + str1 + " -pMods " + CrashWindow.CrashWindow.Base64Encode(str2 == "" ? "none" : str2) + " -pAssembly " + CrashWindow.CrashWindow.Base64Encode(plainText1) + " -pException " + CrashWindow.CrashWindow.Base64Encode(plainText2) + " -pLogMessage " + str3;
        }

        public static List<string> BadFormatExceptionAssembly()
        {
            List<string> stringList = new List<string>();
            int num = (int)WindowsPlatformStartup.SetErrorMode(0U);
            foreach (string moduleDependency in WindowsPlatformStartup._moduleDependencies)
            {
                try
                {
                    string str = WindowsPlatformStartup.CheckLibraryError(moduleDependency);
                    if (str != null)
                        stringList.Add(str);
                }
                catch (Exception)
                {
                    return stringList;
                }
            }
            if (stringList.Count == 0)
                stringList.Add("(unknown dll)");
            return stringList;
        }

        public static List<WindowsPlatformStartup.Module> CollectModules(
          Process process)
        {
            List<WindowsPlatformStartup.Module> moduleList = new List<WindowsPlatformStartup.Module>();
            IntPtr[] lphModule1 = new IntPtr[0];
            int lpcbNeeded;
            if (!WindowsPlatformStartup.Native.EnumProcessModulesEx(process.Handle, lphModule1, 0, out lpcbNeeded, 3U))
                return moduleList;
            int length = lpcbNeeded / IntPtr.Size;
            IntPtr[] lphModule2 = new IntPtr[length];
            if (WindowsPlatformStartup.Native.EnumProcessModulesEx(process.Handle, lphModule2, lpcbNeeded, out lpcbNeeded, 3U))
            {
                for (int index = 0; index < length; ++index)
                {
                    StringBuilder lpBaseName = new StringBuilder(1024);
                    int moduleFileNameEx = (int)WindowsPlatformStartup.Native.GetModuleFileNameEx(process.Handle, lphModule2[index], lpBaseName, (uint)lpBaseName.Capacity);
                    WindowsPlatformStartup.Native.ModuleInformation lpmodinfo = new WindowsPlatformStartup.Native.ModuleInformation();
                    WindowsPlatformStartup.Native.GetModuleInformation(process.Handle, lphModule2[index], out lpmodinfo, (uint)(IntPtr.Size * lphModule2.Length));
                    WindowsPlatformStartup.Module module = new WindowsPlatformStartup.Module(lpBaseName.ToString(), lpmodinfo.lpBaseOfDll, lpmodinfo.SizeOfImage);
                    moduleList.Add(module);
                }
            }
            return moduleList;
        }

        public static void AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            WindowsPlatformStartup.assemblyLoadStrings.Add(args.LoadedAssembly.FullName + ": " + args.LoadedAssembly.GetName().ProcessorArchitecture.ToString());
            if (!args.LoadedAssembly.FullName.Contains("HarmonySharedState") && !args.LoadedAssembly.FullName.Contains("HarmonyLoader") || ModLoader.loadingOldMod == null)
                return;
            ModLoader.FailWithHarmonyException();
        }

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(
          string deviceName,
          int modeNum,
          ref WindowsPlatformStartup.DEVMODE devMode);

        public enum MachineType : ushort
        {
            IMAGE_FILE_MACHINE_UNKNOWN = 0,
            IMAGE_FILE_MACHINE_I386 = 332, // 0x014C
            IMAGE_FILE_MACHINE_R4000 = 358, // 0x0166
            IMAGE_FILE_MACHINE_WCEMIPSV2 = 361, // 0x0169
            IMAGE_FILE_MACHINE_SH3 = 418, // 0x01A2
            IMAGE_FILE_MACHINE_SH3DSP = 419, // 0x01A3
            IMAGE_FILE_MACHINE_SH4 = 422, // 0x01A6
            IMAGE_FILE_MACHINE_SH5 = 424, // 0x01A8
            IMAGE_FILE_MACHINE_ARM = 448, // 0x01C0
            IMAGE_FILE_MACHINE_THUMB = 450, // 0x01C2
            IMAGE_FILE_MACHINE_AM33 = 467, // 0x01D3
            IMAGE_FILE_MACHINE_POWERPC = 496, // 0x01F0
            IMAGE_FILE_MACHINE_POWERPCFP = 497, // 0x01F1
            IMAGE_FILE_MACHINE_IA64 = 512, // 0x0200
            IMAGE_FILE_MACHINE_MIPS16 = 614, // 0x0266
            IMAGE_FILE_MACHINE_MIPSFPU = 870, // 0x0366
            IMAGE_FILE_MACHINE_MIPSFPU16 = 1126, // 0x0466
            IMAGE_FILE_MACHINE_EBC = 3772, // 0x0EBC
            IMAGE_FILE_MACHINE_AMD64 = 34404, // 0x8664
            IMAGE_FILE_MACHINE_M32R = 36929, // 0x9041
        }

        private delegate IntPtr wine_version_delegate();

        public class Native
        {
            [DllImport("psapi.dll")]
            public static extern bool EnumProcessModulesEx(
              IntPtr hProcess,
              [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4), In, Out] IntPtr[] lphModule,
              int cb,
              [MarshalAs(UnmanagedType.U4)] out int lpcbNeeded,
              uint dwFilterFlag);

            [DllImport("psapi.dll")]
            public static extern uint GetModuleFileNameEx(
              IntPtr hProcess,
              IntPtr hModule,
              [Out] StringBuilder lpBaseName,
              [MarshalAs(UnmanagedType.U4), In] uint nSize);

            [DllImport("psapi.dll", SetLastError = true)]
            public static extern bool GetModuleInformation(
              IntPtr hProcess,
              IntPtr hModule,
              out WindowsPlatformStartup.Native.ModuleInformation lpmodinfo,
              uint cb);

            public struct ModuleInformation
            {
                public IntPtr lpBaseOfDll;
                public uint SizeOfImage;
                public IntPtr EntryPoint;
            }

            internal enum ModuleFilter
            {
                ListModulesDefault,
                ListModules32Bit,
                ListModules64Bit,
                ListModulesAll,
            }
        }

        public class Module
        {
            public Module(string modulePath, IntPtr baseAddress, uint size)
            {
                ModulePath = modulePath;
                BaseAddress = baseAddress;
                Size = size;
            }

            public string ModulePath { get; set; }

            public IntPtr BaseAddress { get; set; }

            public uint Size { get; set; }
        }

        public struct DEVMODE
        {
            //private const int CCHDEVICENAME = 32;
            //private const int CCHFORMNAME = 32;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }
    }
}

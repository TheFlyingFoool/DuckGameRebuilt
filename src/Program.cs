// Decompiled with JetBrains decompiler
// Type: DuckGame.Program
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using DbMon.NET;
using DGWindows;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DuckGame
{
    /// <summary>The main class.</summary>
    public static class Program
    {
        public static string StartinEditorLevelName = "";
        public static string GameDirectory = "";
        public static string FileName = "";
        public static string FilePath = "";
        public static bool IsLinuxD;
        public static bool intro = false;
        public static bool testServer = false;
        public static DuckGame.Main main;
        public static string commandLine = "";
        private static bool _attemptingResolve = false;
        private static bool _showedError = false;
        public static bool alternateSaveLocation;
        public static int constructorsLoaded;
        public static int thingTypes;
        public static bool enteredMain = false;
        public static string steamInitializeError = "";
        public static int steamBuildID = 0;
        //private const uint WM_CLOSE = 16;
        public static bool isLinux = false;
        public static string wineVersion = null;
        private static List<Func<string>> _extraExceptionDetailsMinimal = new List<Func<string>>() 
        {
            () => "Date: " + DateTime.UtcNow.ToString(DateTimeFormatInfo.InvariantInfo),
            () => "Version: " + DG.version,
            () => "Platform: " + DG.platform + " (Steam Build " + Program.steamBuildID.ToString() + ")",
            () => "Command Line: " + Program.commandLine
        };
        private static string kCleanupString = "C:\\gamedev\\duckgame_try2\\duckgame\\DuckGame\\src\\";
        public static bool crashed = false;
        public static Assembly crashAssembly;
        public static bool gameLoadedSuccessfully = false;

        public static bool lanjoiner;
        public static Assembly gameAssembly; // added dan this for changes to ModLoader GetType and for general use then trying to get the games assembly
        public static string gameAssemblyName = ""; // added dan
        /// <summary>The main entry point for the application.</summary>
        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void Main(string[] args)
        {
            int p = (int)Environment.OSVersion.Platform;
            IsLinuxD = (p == 4) || (p == 6) || (p == 128);
            gameAssembly = Assembly.GetExecutingAssembly();
            gameAssemblyName = Program.gameAssembly.GetName().Name;
            FilePath = Program.gameAssembly.Location;
            FileName = Path.GetFileName(FilePath);
            GameDirectory = FilePath.Substring(0, FilePath.Length - FileName.Length);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Program.Resolve);
            if (args.Contains<string>("-linux") || WindowsPlatformStartup.isRunningWine && !args.Contains<string>("-nolinux"))
            {
                Program.wineVersion = WindowsPlatformStartup.wineVersion;
                Program.isLinux = true;
                MonoMain.enableThreadedLoading = false;
            }
            else
                AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(WindowsPlatformStartup.AssemblyLoad);
            Application.ThreadException += new ThreadExceptionEventHandler(Program.UnhandledThreadExceptionTrapper);
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(Program.OnProcessExit);
            try
            {
                Program.DoMain(args);
            }
            catch (Exception ex)
            {
                Program.HandleGameCrash(ex);
            }
        }

        public static Assembly ModResolve(object sender, ResolveEventArgs args) => ManagedContent.ResolveModAssembly(sender, args);

        public static Assembly Resolve(object sender, ResolveEventArgs args)
        {
            if (!Program.enteredMain)
                return null;
            if (args.Name.StartsWith("Steam,"))
                return Assembly.GetAssembly(typeof(Steam));
            if (!Program._attemptingResolve)
            {
                bool flag = false;
                if (Program.enteredMain)
                {
                    Program._attemptingResolve = true;
                    Assembly assembly = null;
                    try
                    {
                        assembly = Program.ModResolve(sender, args);
                    }
                    catch (Exception) { }
                    Program._attemptingResolve = false;
                    if (assembly != null)
                        return assembly;
                    flag = true;
                }
                if (!Program._showedError && (!ModLoader.runningModloadCode || MonoMain.modDebugging) && !flag)
                {
                    Program._showedError = true;
                    string str = "Failed to resolve assembly:\n" + args.Name + "\n";
                    if (args.Name.Contains("Microsoft.Xna.Framework"))
                        str += "(You may need to install the XNA redistributables!)\n";
                    StreamWriter streamWriter = new StreamWriter("ducklog.txt", true);
                    streamWriter.WriteLine(str);
                    streamWriter.Close();
                    Process.Start("CrashWindow.exe", "-modResponsible 0 -modDisabled 0 -exceptionString \"" + str.Replace("\n", "|NEWLINE|").Replace("\r", "|NEWLINE2|") + "\" -source Duck Game -commandLine \"\" -executable \"" + Application.ExecutablePath + "\"");
                }
            }
            return null;
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            if (Program.main == null)
                return;
            Program.main.KillEverything();
        }

        private static void DoMain(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            MonoMain.startTime = DateTime.Now;
            for (int index = 0; index < args.Length; ++index)
            {
                Program.commandLine += args[index];
                if (index != args.Length - 1)
                    Program.commandLine += " ";
            }
            bool flag = false;
            for (int index = 0; index < args.Length; ++index)
            {
                switch (args[index])
                {
                    case "+connect_lobby":
                        ++index;
                        if (args.Count<string>() > index)
                        {
                            try
                            {
                                DuckGame.Main.connectID = Convert.ToUInt64(args[index], CultureInfo.InvariantCulture);
                            }
                            catch (Exception)
                            {
                                throw new Exception("+connect_lobby format exception (" + args[index] + ")");
                            }
                        }
                        break;
                    case "+password":
                        ++index;
                        if (args.Count<string>() > index)
                            MonoMain.lobbyPassword = args[index];
                        break;
                    case "+editortest":
                        MonoMain.startInEditor = true;
                        ++index;
                        if (args.Count<string>() > index)
                        {
                            StartinEditorLevelName = args[index];
                        }
                        break;
                    case "-crash":
                        throw new Exception("you threw it idk");
                        break;
                    case "-intro":
                        intro = true;
                        break;
                    case "-debug":
                        flag = true;
                        break;
                    case "-lanjoiner":
                        Network.lanMode = true;
                        lanjoiner = true;
                        break;
                    case "-windowedFullscreen":
                        MonoMain.forceFullscreenMode = 1;
                        break;
                    case "-oldschoolFullscreen":
                        MonoMain.forceFullscreenMode = 2;
                        break;
                    case "-testserver":
                        Program.testServer = true;
                        break;
                    case "-nothreading":
                        MonoMain.enableThreadedLoading = false;
                        break;
                    case "-defaultcontrols":
                        MonoMain.defaultControls = true;
                        break;
                    case "-olddefaults":
                        MonoMain.oldDefaultControls = true;
                        break;
                    case "-nofullscreen":
                        MonoMain.noFullscreen = true;
                        break;
                    case "-nosteam":
                        MonoMain.disableSteam = true;
                        break;
                    case "-steam":
                        MonoMain.launchedFromSteam = true;
                        break;
                    case "-loopdebug":
                        MonoMain.infiniteLoopDebug = true;
                        break;
                    case "-nomods":
                        MonoMain.nomodsMode = true;
                       // MonoMain.moddingEnabled = false; fcked for klof
                        break;
                    case "-linux":
                        if (MonoMain.audioModeOverride == AudioMode.None)
                            MonoMain.audioModeOverride = AudioMode.Wave;
                        break;
                    case "-disableModding":
                        MonoMain.moddingEnabled = false;
                        break;
                    case "-nointro":
                        MonoMain.noIntro = true;
                        break;
                    case "-firebreak":
                        MonoMain.firebreak = true;
                        break;
                    case "-speedruntypingthisstartupparameterformaximumdubberspeed":
                    case "-dubberspeed":
                    case "-vimuser":
                    case "-neovim":
                    case "-vim":
                        UIBox.dubberspeed = true;
                        break;
                    case "-startineditor":
                        MonoMain.startInEditor = true;
                        break;
                    case "-moddebug":
                        MonoMain.modDebugging = true;
                        break;
                    case "-downloadmods":
                        MonoMain.downloadWorkshopMods = true;
                        break;
                    case "-editsave":
                        MonoMain.editSave = true;
                        break;
                    case "-nodinput":
                        MonoMain.disableDirectInput = true;
                        break;
                    case "-dinputNoTimeout":
                        MonoMain.dinputNoTimeout = true;
                        break;
                    case "-ignoreLegacyLoad":
                        ModLoader.ignoreLegacyLoad = true;
                        break;
                    case "-nocloud":
                        Cloud.nocloud = true;
                        break;
                    case "-cloudnoload":
                        Cloud.downloadEnabled = false;
                        break;
                    case "-cloudnosave":
                        Cloud.uploadEnabled = false;
                        break;
                    case "-netdebug":
                        MonoMain.networkDebugger = true;
                        break;
                    case "-altaudio":
                        MonoMain.audioModeOverride = AudioMode.Wave;
                        break;
                    case "-directaudio":
                        MonoMain.audioModeOverride = AudioMode.DirectSound;
                        break;
                    case "-oldangles":
                        MonoMain.oldAngles = true;
                        break;
                    case "-nohidef":
                        MonoMain.noHidef = true;
                        break;
                    case "-logFileOperations":
                        MonoMain.logFileOperations = true;
                        break;
                    case "-logLevelOperations":
                        MonoMain.logLevelOperations = true;
                        break;
                    case "-recoversave":
                        MonoMain.recoversave = true;
                        break;
                    case "-notimeout":
                        MonoMain.noConnectionTimeout = true;
                        break;
                    case "-command":
                        ++index;
                        if (index < args.Count<string>())
                            DevConsole.startupCommands.Add(args[index]);
                        break;
                    case "-useRPC":
                        MonoMain.useRPC = true;
                        break;
                    default:
                        if (args[index] == "-nolaunch")
                        {
                            int num = (int)MessageBox.Show("-nolaunch Command Line Option activated! Cancelling launch!");
                            return;
                        }
                        if (args[index] == "-alternateSaveLocation")
                            Program.alternateSaveLocation = true;
                        break;
                }
            }
            try
            {
                if (MonoMain.audioModeOverride == AudioMode.None)
                {
                    if (Environment.OSVersion.Version.Major < 6)
                        MonoMain.audioModeOverride = AudioMode.Wave;
                }
            }
            catch (Exception) { }
            if (flag)
            {
                try
                {
                    DebugMonitor.OnOutputDebugString += new DbMon.NET.OnOutputDebugStringHandler(Program.OnOutputDebugStringHandler);
                    DebugMonitor.Start();
                }
                catch (Exception ex)
                {
                    Program.steamInitializeError = "SteamAPI deep debug failed with exception:" + ex.Message + "\nTry running Duck Game as administrator for more debug info.";
                }
            }
            Program.enteredMain = true;
            if (!MonoMain.disableSteam)
            {
                if (MonoMain.breakSteam || !Steam.InitializeCore())
                    Program.LogLine("Steam INIT Failed!");
                else
                    Steam.Initialize();
            }
            try
            {
                if (Steam.IsInitialized())
                {
                    Program.steamBuildID = Steam.GetGameBuildID();
                    Steam.RemotePlay += new Steam.RemotePlayDelegate(Program.RemotePlayConnected);
                    if (Steam.IsLoggedIn())
                    {
                        if (Steam.Authorize())
                            goto label_109;
                    }
                    MonoMain.steamConnectionCheckFail = true;
                }
                else
                    Program.steamBuildID = -1;
            }
            catch (Exception) { }
        label_109:
            DeviceChangeNotifier.Start();
            DevConsole.Log("Starting Duck Game (" + DG.platform + ")...");
            Program.main = new DuckGame.Main();
            // Program.main.TargetElapsedTime = TimeSpan.FromTicks(1000L);
            accumulatedElapsedTimefieldinfo = typeof(Game).GetField("accumulatedElapsedTime", BindingFlags.NonPublic | BindingFlags.Instance);
            SetAccumulatedElapsedTime(Program.main, Program.main.TargetElapsedTime);
            Program.main.IsFixedTimeStep = false; // ZOOOM
            FirebreakReflectionsht = Task.Factory.StartNew(() => { MemberAttributePairHandler.Init(); });
            Program.main.Run();
        }
        public static Task FirebreakReflectionsht;
        private static FieldInfo accumulatedElapsedTimefieldinfo; 
        public static void SetAccumulatedElapsedTime(Game g, TimeSpan t)
        {
            accumulatedElapsedTimefieldinfo.SetValue(g, t);
        }
        private static void OnOutputDebugStringHandler(int pid, string text) => Program.steamInitializeError = Program.steamInitializeError + text + "\n";

        public static void RemotePlayConnected() => Windows_Audio.forceMode = AudioMode.DirectSound;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(
          IntPtr hWnd,
          uint Msg,
          IntPtr wParam,
          IntPtr lParam);
        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void UnhandledThreadExceptionTrapper(object sender, ThreadExceptionEventArgs e)
        {
            Program.HandleGameCrash(e.Exception);
        }
        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Program.HandleGameCrash(e.ExceptionObject as Exception);
        }

        public static string ProcessExceptionString(Exception e)
        {
            string str1;
            if (e is ModException)
            {
                ModException modException = e as ModException;
                str1 = modException.Message + "\n" + modException.exception.ToString();
                e = modException.exception;
            }
            else
                str1 = e.ToString();
            try
            {
                if (e is UnauthorizedAccessException)
                {
                    UnauthorizedAccessException unauthorizedAccessException = e as UnauthorizedAccessException;
                    int startIndex = unauthorizedAccessException.Message.IndexOf(":") - 1;
                    if (startIndex > 0)
                    {
                        int num1 = unauthorizedAccessException.Message.LastIndexOf("'");
                        if (num1 > 0)
                        {
                            FileAttributes attributes = new FileInfo(unauthorizedAccessException.Message.Substring(startIndex, num1 - startIndex)).Attributes;
                            string str2 = "(File is ";
                            int num2 = 0;
                            foreach (FileAttributes fileAttributes in Enum.GetValues(typeof(FileAttributes)))
                            {
                                if ((attributes & fileAttributes) > 0)
                                {
                                    if (num2 > 0)
                                        str2 += ",";
                                    str2 += fileAttributes.ToString();
                                    ++num2;
                                }
                            }
                            str1 = str2 + ") " + str1;
                        }
                    }
                }
            }
            catch (Exception) { }
            return str1;
        }

        public static string GetExceptionStringMinimal(object e)
        {
            string exceptionStringMinimal = ((e as Exception).ToString() + "\r\n").Replace(Program.kCleanupString, "");
            foreach (Func<string> func in Program._extraExceptionDetailsMinimal)
            {
                string str = "FIELD FAILED";
                try
                {
                    str = func();
                }
                catch { }
                exceptionStringMinimal += "\r\n";
                exceptionStringMinimal += str;
            }
            return exceptionStringMinimal;
        }

        public static void HandleGameCrash(Exception pException)
        {
            MonoMain.InvokeOnGameExitEvent(true);

            if (!System.IO.File.Exists("CrashWindow.exe"))
                return;
            if (pException is ThreadAbortException)
            {
                ThreadAbortException threadAbortException = pException as ThreadAbortException;
                if (threadAbortException.ExceptionState is Exception)
                    pException = threadAbortException.ExceptionState as Exception;
            }
            bool flag1 = false;
            try
            {
                if (Network.isActive)
                {
                    for (int index = 0; index < 5; ++index)
                    {
                        Send.ImmediateUnreliableBroadcast(new NMClientCrashed());
                        Send.ImmediateUnreliableBroadcast(new NMClientCrashed());
                        Steam.Update();
                        Thread.Sleep(16);
                    }
                    Program.crashed = true;
                }
            }
            catch (Exception) { }
            string str1 = "";
            int num = 0;
            try
            {
                try
                {
                    str1 = MonoMain.GetExceptionString(pException);
                }
                catch (Exception)
                {
                    try
                    {
                        str1 = Program.GetExceptionStringMinimal(pException);
                    }
                    catch (Exception)
                    {
                        str1 = pException.ToString();
                    }
                }
                try
                {
                    if (pException is UnauthorizedAccessException && !DuckFile.appdataSave)
                    {
                        str1 = "This crash may be due to your save being located in the Documents folder. If the problem persists, try moving your DuckGame save files from (" + DuckFile.oldSaveLocation + "DuckGame/) to (" + DuckFile.newSaveLocation + "DuckGame/).\n\n" + str1;
                    }
                    else
                    {
                        try
                        {
                            if (!(pException is OutOfMemoryException))
                            {
                                if (!pException.ToString().Contains("System.OutOfMemoryException"))
                                    goto label_23;
                            }
                            MonoMain.CalculateModMemoryOffendersList();
                            str1 = MonoMain.modMemoryOffendersString + str1;
                        }
                        catch (Exception) { }
                    label_23:
                        str1 = WindowsPlatformStartup.ProcessErrorLine(str1, pException);
                    }
                }
                catch (Exception) { }
                try
                {
                    Program.WriteToLog(str1);
                }
                catch (Exception ex)
                {
                    str1 = str1 + "Writing your crash to the log failed with exception " + ex.Message + "!\n";
                }
                num = 1;
                Exception exception = pException;
                string str2 = "";
                bool flag2 = false;
                Assembly pAssembly = Program.crashAssembly;
                ModConfiguration pModConfig = null;
                try
                {
                    if (pException is ModException)
                    {
                        pModConfig = (pException as ModException).mod;
                        if (pModConfig != null)
                        {
                            str2 = pModConfig.name;
                            pAssembly = pModConfig.assembly;
                        }
                    }
                    else
                    {
                        num = 2;
                        try
                        {
                            foreach (Mod allMod in (IEnumerable<Mod>)ModLoader.allMods)
                            {
                                if (!(allMod is CoreMod) && allMod.configuration != null && allMod.configuration.assembly != null && allMod.configuration.assembly != Assembly.GetExecutingAssembly())
                                {
                                    bool flag3 = Program.crashAssembly == null && allMod.configuration.assembly == exception.TargetSite.DeclaringType.Assembly || allMod.configuration.assembly == Program.crashAssembly;
                                    if (!flag3)
                                    {
                                        foreach (System.Type type in allMod.configuration.assembly.GetTypes())
                                        {
                                            if (pException.StackTrace.Contains(type.ToString()))
                                            {
                                                flag3 = true;
                                                break;
                                            }
                                            if (pException.InnerException != null && pException.InnerException.StackTrace.Contains(type.ToString()))
                                            {
                                                flag3 = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (flag3)
                                    {
                                        pAssembly = allMod.configuration.assembly;
                                        flag1 = true;
                                        str2 = allMod.configuration.name;
                                        if (!MonoMain.modDebugging)
                                        {
                                            if (!Program.gameLoadedSuccessfully || Options.Data.disableModOnCrash && (DateTime.Now - MonoMain.startTime).TotalMinutes < 2.0)
                                                allMod.configuration.Disable();
                                            flag2 = true;
                                        }
                                    }
                                }
                            }
                            Exception innerException = pException.InnerException;
                        }
                        catch (Exception ex)
                        {
                            str1 = str1 + "Finding if crash was Mod related failed with exception " + ex.Message + "!\n But, No matter, here's the actual exception message for the crash:\n";
                        }
                    }
                }
                catch (Exception) { }
                num = 4;
                if (pAssembly == null)
                    pAssembly = Program.crashAssembly;
                try
                {
                    num = 5;
                    if (Program.main != null)
                    {
                        if (Program.main.Window != null)
                            Program.SendMessage(Program.main.Window.Handle, 16U, IntPtr.Zero, IntPtr.Zero);
                    }
                }
                catch (Exception) { }
                num = 6;
                if (System.IO.File.Exists("CrashWindow.exe"))
                {
                    try
                    {
                        if (pModConfig != null)
                            Process.Start("CrashWindow.exe", "-modResponsible " + (flag1 ? "1" : "0") + " -modDisabled " + (!Program.gameLoadedSuccessfully || Options.Data.disableModOnCrash ? (flag2 ? "1" : "0") : "2") + " -modName " + str2 + " -source " + exception.Source + " -commandLine \"" + Program.commandLine + "\" -executable \"" + Application.ExecutablePath + "\" " + DG.GetCrashWindowString(pException, pModConfig, str1));
                        else
                            Process.Start("CrashWindow.exe", "-modResponsible " + (flag1 ? "1" : "0") + " -modDisabled " + (!Program.gameLoadedSuccessfully || Options.Data.disableModOnCrash ? (flag2 ? "1" : "0") : "2") + " -modName " + str2 + " -source " + exception.Source + " -commandLine \"" + Program.commandLine + "\" -executable \"" + Application.ExecutablePath + "\" " + DG.GetCrashWindowString(pException, pAssembly, str1));
                    }
                    catch (Exception ex)
                    {
                        Program.WriteToLog("Opening CrashWindow failed with error: " + ex.ToString() + "\n");
                    }
                }
                Environment.Exit(1);
            }
            catch (Exception ex3)
            {
                try
                {
                    Program.WriteToLog("Crash catcher failed (crashpoint " + num.ToString() + ") with exception: " + ex3.Message + "\n But Also: \n" + str1);
                }
                catch (Exception)
                {
                    StreamWriter streamWriter = new StreamWriter("ducklog.txt", true);
                    streamWriter.WriteLine("Failed to write exception to log: " + ex3.Message + "\n");
                    streamWriter.Close();
                }
            }
        }

        public static void WriteToLog(string s) => Program.WriteToLog(s, false);

        public static void WriteToLog(string s, bool modRelated)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter("ducklog.txt", true);
                streamWriter.WriteLine(s + "\n");
                streamWriter.Close();
            }
            catch (Exception ex)
            {
                StreamWriter streamWriter = new StreamWriter("ducklog.txt", true);
                streamWriter.WriteLine(ex.ToString() + "\n");
                streamWriter.Close();
            }
        }

        public static void MakeNetLog()
        {
            StreamWriter streamWriter = new StreamWriter("netlog.txt", false);
            foreach (DCLine line in DevConsole.core.lines)
                streamWriter.WriteLine(line.timestamp.ToLongTimeString() + " " + Program.RemoveColorTags(line.SectionString()) + " " + Program.RemoveColorTags(line.line) + "\n");
            foreach (DCLine pendingLine in DevConsole.core.pendingLines)
                streamWriter.WriteLine(pendingLine.timestamp.ToLongTimeString() + " " + Program.RemoveColorTags(pendingLine.SectionString()) + " " + Program.RemoveColorTags(pendingLine.line) + "\n");
            streamWriter.WriteLine("\n");
            streamWriter.Close();
        }

        //private static void UnhandledExceptionTrapperTestServer(
        //  object sender,
        //  UnhandledExceptionEventArgs e)
        //{
        //    string exceptionString = MonoMain.GetExceptionString(e);
        //    StreamWriter streamWriter = new StreamWriter("ducklog.txt", true);
        //    streamWriter.WriteLine(exceptionString + "\n");
        //    streamWriter.Close();
        //    Environment.Exit(1);
        //}

        public static void LogLine(string line)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter("ducklog.txt", true);
                streamWriter.WriteLine(line + "\n");
                streamWriter.Close();
            }
            catch { }
        }

        public static string RemoveColorTags(string s)
        {
            for (int index = 0; index < s.Length; ++index)
            {
                if (s[index] == '|')
                {
                    int startIndex = index;
                    ++index;
                    while (index < s.Length && s[index] != '|')
                        ++index;
                    if (index < s.Length && s[index] == '|')
                    {
                        s = s.Remove(startIndex, index - startIndex + 1);
                        index = -1;
                    }
                }
            }
            return s;
        }
    }
}
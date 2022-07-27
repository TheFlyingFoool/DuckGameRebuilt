// Decompiled with JetBrains decompiler
// Type: DuckGame.Program
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using DbMon.NET;
using DGWindows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace DuckGame
{
    /// <summary>The main class.</summary>
    public static class Program
    {
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
        private const uint WM_CLOSE = 16;
        public static bool isLinux = false;
        public static string wineVersion = (string)null;
        private static List<Func<string>> _extraExceptionDetailsMinimal = new List<Func<string>>()
    {
      (Func<string>) (() => "Date: " + DateTime.UtcNow.ToString((IFormatProvider) DateTimeFormatInfo.InvariantInfo)),
      (Func<string>) (() => "Version: " + DG.version),
      (Func<string>) (() => "Platform: " + DG.platform + " (Steam Build " + Program.steamBuildID.ToString() + ")"),
      (Func<string>) (() => "Command Line: " + Program.commandLine)
    };
        private static string kCleanupString = "C:\\gamedev\\duckgame_try2\\duckgame\\DuckGame\\src\\";
        public static bool crashed = false;
        public static Assembly crashAssembly;
        public static bool gameLoadedSuccessfully = false;

        public static Assembly gameAssembly; // added dan this for changes to ModLoader GetType and for general use then trying to get the games assembly
        public static string gameAssemblyName = ""; // added dan
        /// <summary>The main entry point for the application.</summary>
        public static void Main(string[] args)
        {
            gameAssembly = Assembly.GetExecutingAssembly();
            gameAssemblyName = Program.gameAssembly.GetName().Name;
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Program.Resolve);
            if (((IEnumerable<string>)args).Contains<string>("-linux") || WindowsPlatformStartup.isRunningWine && !((IEnumerable<string>)args).Contains<string>("-nolinux"))
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
                return (Assembly)null;
            if (args.Name.StartsWith("Steam,"))
                return Assembly.GetAssembly(typeof(Steam));
            if (!Program._attemptingResolve)
            {
                bool flag = false;
                if (Program.enteredMain)
                {
                    Program._attemptingResolve = true;
                    Assembly assembly = (Assembly)null;
                    try
                    {
                        assembly = Program.ModResolve(sender, args);
                    }
                    catch (Exception ex)
                    {
                    }
                    Program._attemptingResolve = false;
                    if (assembly != (Assembly)null)
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
            return (Assembly)null;
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
                if (args[index] == "+connect_lobby")
                {
                    ++index;
                    if (((IEnumerable<string>)args).Count<string>() > index)
                    {
                        try
                        {
                            DuckGame.Main.connectID = Convert.ToUInt64(args[index], (IFormatProvider)CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("+connect_lobby format exception (" + args[index] + ")");
                        }
                    }
                }
                else if (args[index] == "+password")
                {
                    ++index;
                    if (((IEnumerable<string>)args).Count<string>() > index)
                        MonoMain.lobbyPassword = args[index];
                }
                else if (args[index] == "-debug")
                    flag = true;
                else if (args[index] == "-windowedFullscreen")
                    MonoMain.forceFullscreenMode = 1;
                else if (args[index] == "-oldschoolFullscreen")
                    MonoMain.forceFullscreenMode = 2;
                else if (args[index] == "-testserver")
                    Program.testServer = true;
                else if (args[index] == "-nothreading")
                    MonoMain.enableThreadedLoading = false;
                else if (args[index] == "-defaultcontrols")
                    MonoMain.defaultControls = true;
                else if (args[index] == "-olddefaults")
                    MonoMain.oldDefaultControls = true;
                else if (args[index] == "-nofullscreen")
                    MonoMain.noFullscreen = true;
                else if (args[index] == "-nosteam")
                    MonoMain.disableSteam = true;
                else if (args[index] == "-steam")
                    MonoMain.launchedFromSteam = true;
                else if (args[index] == "-loopdebug")
                    MonoMain.infiniteLoopDebug = true;
                else if (args[index] == "-nomods")
                    MonoMain.nomodsMode = true;
                else if (args[index] == "-linux")
                {
                    if (MonoMain.audioModeOverride == AudioMode.None)
                        MonoMain.audioModeOverride = AudioMode.Wave;
                }
                else if (args[index] == "-disableModding")
                    MonoMain.moddingEnabled = false;
                else if (args[index] == "-nointro")
                    MonoMain.noIntro = true;
                else if (args[index] == "-startineditor")
                    MonoMain.startInEditor = true;
                else if (args[index] == "-moddebug")
                    MonoMain.modDebugging = true;
                else if (args[index] == "-downloadmods")
                    MonoMain.downloadWorkshopMods = true;
                else if (args[index] == "-editsave")
                    MonoMain.editSave = true;
                else if (args[index] == "-nodinput")
                    MonoMain.disableDirectInput = true;
                else if (args[index] == "-dinputNoTimeout")
                    MonoMain.dinputNoTimeout = true;
                else if (args[index] == "-ignoreLegacyLoad")
                    ModLoader.ignoreLegacyLoad = true;
                else if (args[index] == "-nocloud")
                    Cloud.nocloud = true;
                else if (args[index] == "-cloudnoload")
                    Cloud.downloadEnabled = false;
                else if (args[index] == "-cloudnosave")
                    Cloud.uploadEnabled = false;
                else if (args[index] == "-netdebug")
                    MonoMain.networkDebugger = true;
                else if (args[index] == "-altaudio")
                    MonoMain.audioModeOverride = AudioMode.Wave;
                else if (args[index] == "-directaudio")
                    MonoMain.audioModeOverride = AudioMode.DirectSound;
                else if (args[index] == "-oldangles")
                    MonoMain.oldAngles = true;
                else if (args[index] == "-nohidef")
                    MonoMain.noHidef = true;
                else if (args[index] == "-logFileOperations")
                    MonoMain.logFileOperations = true;
                else if (args[index] == "-logLevelOperations")
                    MonoMain.logLevelOperations = true;
                else if (args[index] == "-recoversave")
                    MonoMain.recoversave = true;
                else if (args[index] == "-notimeout")
                    MonoMain.noConnectionTimeout = true;
                else if (args[index] == "-command")
                {
                    ++index;
                    if (index < ((IEnumerable<string>)args).Count<string>())
                        DevConsole.startupCommands.Add(args[index]);
                }
                else
                {
                    if (args[index] == "-nolaunch")
                    {
                        int num = (int)MessageBox.Show("-nolaunch Command Line Option activated! Cancelling launch!");
                        return;
                    }
                    if (args[index] == "-alternateSaveLocation")
                        Program.alternateSaveLocation = true;
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
            catch (Exception ex)
            {
            }
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
            catch (Exception ex)
            {
            }
        label_109:
            DeviceChangeNotifier.Start();
            DevConsole.Log("Starting Duck Game (" + DG.platform + ")...");
            Program.main = new DuckGame.Main();
            Program.main.Run();
        }

        private static void OnOutputDebugStringHandler(int pid, string text) => Program.steamInitializeError = Program.steamInitializeError + text + "\n";

        public static void RemotePlayConnected() => Windows_Audio.forceMode = AudioMode.DirectSound;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(
          IntPtr hWnd,
          uint Msg,
          IntPtr wParam,
          IntPtr lParam);

        public static void UnhandledThreadExceptionTrapper(object sender, ThreadExceptionEventArgs e) => Program.HandleGameCrash(e.Exception);

        public static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e) => Program.HandleGameCrash(e.ExceptionObject as Exception);

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
                                if ((attributes & fileAttributes) > (FileAttributes)0)
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
            catch (Exception ex)
            {
            }
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
                catch
                {
                }
                exceptionStringMinimal += "\r\n";
                exceptionStringMinimal += str;
            }
            return exceptionStringMinimal;
        }

        public static void HandleGameCrash(Exception pException)
        {
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
                        Send.ImmediateUnreliableBroadcast((NetMessage)new NMClientCrashed());
                        Send.ImmediateUnreliableBroadcast((NetMessage)new NMClientCrashed());
                        Steam.Update();
                        Thread.Sleep(16);
                    }
                    Program.crashed = true;
                }
            }
            catch (Exception ex)
            {
            }
            string str1 = "";
            int num = 0;
            try
            {
                try
                {
                    str1 = MonoMain.GetExceptionString((object)pException);
                }
                catch (Exception ex1)
                {
                    try
                    {
                        str1 = Program.GetExceptionStringMinimal((object)pException);
                    }
                    catch (Exception ex2)
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
                        catch (Exception ex)
                        {
                        }
                    label_23:
                        str1 = WindowsPlatformStartup.ProcessErrorLine(str1, pException);
                    }
                }
                catch (Exception ex)
                {
                }
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
                ModConfiguration pModConfig = (ModConfiguration)null;
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
                                if (!(allMod is CoreMod) && allMod.configuration != null && allMod.configuration.assembly != (Assembly)null && allMod.configuration.assembly != Assembly.GetExecutingAssembly())
                                {
                                    bool flag3 = Program.crashAssembly == (Assembly)null && allMod.configuration.assembly == exception.TargetSite.DeclaringType.Assembly || allMod.configuration.assembly == Program.crashAssembly;
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
                catch (Exception ex)
                {
                }
                num = 4;
                if (pAssembly == (Assembly)null)
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
                catch (Exception ex)
                {
                }
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
                catch (Exception ex4)
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

        private static void UnhandledExceptionTrapperTestServer(
          object sender,
          UnhandledExceptionEventArgs e)
        {
            string exceptionString = MonoMain.GetExceptionString(e);
            StreamWriter streamWriter = new StreamWriter("ducklog.txt", true);
            streamWriter.WriteLine(exceptionString + "\n");
            streamWriter.Close();
            Environment.Exit(1);
        }

        public static void LogLine(string line)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter("ducklog.txt", true);
                streamWriter.WriteLine(line + "\n");
                streamWriter.Close();
            }
            catch
            {
            }
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

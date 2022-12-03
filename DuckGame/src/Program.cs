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
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DuckGame
{
    /// <summary>The main class.</summary>
    public static class Program
    {
        public static bool fullstop;
        public static bool Prestart = DirtyPreStart();

        public static bool temptest1;
        public static string currentversion;
        public static string StartinEditorLevelName;
        public static string GameDirectory;
        public static string FileName;
        public static string FilePath;
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

        public static bool shouldusespriteatlas = true;
        public static bool someprivacy;
        public static bool lanjoiner;
        public static Assembly gameAssembly;
        public static string gameAssemblyName;
        public static bool doscreentileing; //just a fun showing off thing
        /// <summary>The main entry point for the application.</summary>\
        public static Vec2 StartPos = Vec2.Zero;
        public static string gitVersion = "N/A";
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public static void Main(string[] args)
        {
            if (fullstop)
            {
                return;
            }
            //File.Delete(Path.GetFullPath("DGInput.dll"));
            try
            {

                bool isDirty = false;
                using (StreamReader st = new(Program.gameAssembly.GetManifestResourceStream("version.txt")))
                {
                    gitVersion = st.ReadToEnd();
                }
                if (gitVersion.EndsWith("-dirty\n"))
                {
                    isDirty = true;
                    gitVersion = gitVersion.Substring(0, Math.Min(40, gitVersion.Length));
                }
                gitVersion = Escape(gitVersion.Replace("\n", ""));
                gitVersion = gitVersion.Substring(0, 8) + (isDirty ? "[Modified]" : "");
            }
            catch
            {
            }
            DevConsole.Log("Version " + gitVersion);
            int p = (int)Environment.OSVersion.Platform;
            IsLinuxD = (p == 4) || (p == 6) || (p == 128);
            if (IsLinuxD)
            {
                MonoMain.enableThreadedLoading = false;
                MonoMain.disableDirectInput = true;
            }
            DevConsole.Log(IsLinuxD.ToString() + " " + p.ToString());
            gameAssembly = Assembly.GetExecutingAssembly();
            gameAssemblyName = Program.gameAssembly.GetName().Name;
            FilePath = Program.gameAssembly.Location;
            FileName = System.IO.Path.GetFileName(FilePath);
            GameDirectory = FilePath.Substring(0, FilePath.Length - FileName.Length);
            if (args.Contains<string>("-linux") || WindowsPlatformStartup.isRunningWine && !args.Contains<string>("-nolinux"))
            {
                Program.wineVersion = WindowsPlatformStartup.wineVersion;
                Program.isLinux = true;
                MonoMain.enableThreadedLoading = false;
            }
            else
                AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(WindowsPlatformStartup.AssemblyLoad);
            Application.ThreadException += new ThreadExceptionEventHandler(Program.UnhandledThreadExceptionTrapper);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(WindowsPlatformStartup.UnhandledExceptionTrapper);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Program.Resolve);
            TaskScheduler.UnobservedTaskException += UnhandledExceptionUnobserved;
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(Program.OnProcessExit);
            try
            {
                //DoSomeAccessViolation();
                Program.DoMain(args);
            }
            catch (Exception ex)
            {
                Program.HandleGameCrash(ex);
            }
        }
        //public static Int32 newexceptionfilter(IntPtr a)
        //{
        //     HttpClient httpClient = new HttpClient();
        //    //DevConsole.Log("SetUnhandledExceptionFilter Work!");
        //    //LogHelper.WriteErrorLog("SetUnhandledExceptionFilter Work!");
        //    string jsonmessage2 = "{\"content\":\"SendCrashToServer Http Request not good (" + 0.ToString() + ")\"}";
        //    Task<HttpResponseMessage> response2 = httpClient.PostAsync(webhookurl,
        //        new StringContent(jsonmessage2, Encoding.UTF8, "application/json"));
        //    response2.Wait();
        //    return 1;
        //}
        public static bool DirtyPreStart()
        {
            int tries = 10;
            int p = (int)Environment.OSVersion.Platform;
            IsLinuxD = (p == 4) || (p == 6) || (p == 128);
            currentversion = "";
#if AutoUpdater
            AutoUpdater();
#endif
            if (fullstop)
            {
                return false;
            }
            try // IMPROVEME, i try catch this because when restarting with the ingame restarting thing, it would crash because this was still in use
            {   // also this should really be doing some kind of like cache thing so it doesnt do this everytime
                while (tries > 0)
                {
                    if (File.Exists(GameDirectory + "Steamworks.NET.dll"))
                    {
                        File.Delete(GameDirectory + "Steamworks.NET.dll");
                        tries -= 1;
                    }
                    else
                    {
                        break;
                    }
                }
                if (IsLinuxD)
                {
                    DevConsole.Log("setting dll to linux steam");
                    File.Copy(GameDirectory + "OSX-Linux-x64//Steamworks.NET.dll", GameDirectory + "Steamworks.NET.dll");
                }
                else if (Environment.Is64BitProcess)
                {
                    DevConsole.Log("setting dll to windows steam x64"); //this is left over from me thinking about building for 64 bit, i dont want to build FNA my self so no
                    File.Copy(GameDirectory + "Windows-x64//Steamworks.NET.dll", GameDirectory + "Steamworks.NET.dll");
                }
                else
                {
                    DevConsole.Log("setting dll to windows steam x86");
                    File.Copy(GameDirectory + "Windows-x86//Steamworks.NET.dll", GameDirectory + "Steamworks.NET.dll");
                }
            }
            catch
            {
            }
            DevConsole.Log("Is Linux " + IsLinuxD.ToString() + " PlatformID " + p.ToString());
            gameAssembly = Assembly.GetExecutingAssembly();
            gameAssemblyName = Program.gameAssembly.GetName().Name;
            FilePath = Program.gameAssembly.Location;
            FileName = System.IO.Path.GetFileName(FilePath);
            GameDirectory = FilePath.Substring(0, FilePath.Length - FileName.Length);
            return true;
        }
        public static Assembly ModResolve(object sender, ResolveEventArgs args) => ManagedContent.ResolveModAssembly(sender, args);

        public static Assembly Resolve(object sender, ResolveEventArgs args)
        {
            if (!Program.enteredMain)
                return null;
            if (args.Name.StartsWith("Steam,"))
            {
                return Assembly.GetAssembly(typeof(Steam));
            }
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
            Program.main.Exit();
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
            MemberAttributePairHandler.Init();
            AutoConfigHandler.Initialize(); //settings are loaded :sunglass:
            int Controllers = 8;
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
                    case "+controllercount":
                        ++index;
                        if (args.Count<string>() > index)
                        {
                            try
                            {
                                Controllers = Convert.ToInt32(args[index]);
                            }
                            catch
                            { }
                        }
                        break;
                    case "+screentile":
                        doscreentileing = true;
                        ++index;
                        if (args.Count<string>() > index)
                        {
                            try
                            {
                                StartPos = new Vec2(Convert.ToInt32(args[index]), StartPos.y);
                            }
                            catch
                            {
                                doscreentileing = false;
                                break;
                            }

                        }
                        ++index;
                        if (args.Count<string>() > index)
                        {
                            try
                            {
                                StartPos = new Vec2(StartPos.x, Convert.ToInt32(args[index]));
                            }
                            catch
                            {
                                doscreentileing = false;
                                break;
                            }
                        }
                        DevConsole.Log(StartPos.x.ToString() + " " + StartPos.y.ToString() + " " + doscreentileing.ToString());
                        break;
                    case "-nosa":
                        shouldusespriteatlas = false;
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
                    case "-startinlobby":
                        DuckGame.Main.startInLobby = true;
                        break;
                    case "-windowedFullscreen":
                        MonoMain.forceFullscreenMode = 1;
                        break;
                    case "-oldschoolFullscreen":
                        MonoMain.forceFullscreenMode = 2;
                        break;
                    case "-testserver":
                        Process.Start(Application.ExecutablePath, Program.commandLine.Replace("-testserver", " -lanjoiner"));
                        Program.testServer = true;
                        break;
                    case "-testserver2":
                        Program.testServer = true;
                        break;
                    case "-testserverclient":
                        Process.Start(Application.ExecutablePath, Program.commandLine.Replace("-testserverclient", " -testserver2"));
                        Network.lanMode = true;
                        lanjoiner = true;
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
                    case "-startineditor":
                        MonoMain.startInEditor = true;
                        break;
                    case "-moddebug":
                        MonoMain.modDebugging = true;
                        break;
                    case "-tempf1":
                        temptest1 = true;
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
                    case "-privacy":
                        someprivacy = true;
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
                    case "-logLoading":
                        MonoMain.logLoading = true;
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
            if (Controllers > 4)
            {
                string controllerstring = Controllers.ToString();
                DevConsole.Log("Setting Max Controller Count " + controllerstring);
                Environment.SetEnvironmentVariable("FNA_GAMEPAD_NUM_GAMEPADS", controllerstring);
            }
            Environment.SetEnvironmentVariable( "FNA_KEYBOARD_USE_SCANCODES","1");
            string environmentVariable = Environment.GetEnvironmentVariable("FNA_GAMEPAD_NUM_GAMEPADS");
            if (string.IsNullOrEmpty(environmentVariable) || !int.TryParse(environmentVariable, out MonoMain.MaximumGamepadCount) || MonoMain.MaximumGamepadCount < 0)
                MonoMain.MaximumGamepadCount = Enum.GetNames(typeof(PlayerIndex)).Length;

            Program.main = new DuckGame.Main();
            if (Debugger.IsAttached)
            {
                string title = GetDefaultWindowTitle();
                Program.main.Window.Title = title + " Debugging";
            }
            if (DGRSettings.S_StartIn == 1)
            {
                DuckGame.Main.startInLobby = true;
            }
            else if (DGRSettings.S_StartIn == 2)
            {
                MonoMain.startInEditor = true;
            }
            else if (DGRSettings.S_StartIn == 3)
            {
                DuckGame.Main.startInArcade = true;
            }
            // Program.main.TargetElapsedTime = TimeSpan.FromTicks(1000L);
            accumulatedElapsedTimefieldinfo = typeof(Game).GetField("accumulatedElapsedTime", BindingFlags.NonPublic | BindingFlags.Instance);
            SetAccumulatedElapsedTime(Program.main, Program.main.TargetElapsedTime);
            Program.main.IsFixedTimeStep = false; // ZOOOM
            //FirebreakReflectionsht = Task.Factory.StartNew(() => { MemberAttributePairHandler.Init(); });
            Program.main.Run();
        }
        public static List<string> words = new List<string>();
        public static Task FirebreakReflectionsht;
        private static FieldInfo accumulatedElapsedTimefieldinfo;
        public static void SetAccumulatedElapsedTime(Game g, TimeSpan t)
        {
            accumulatedElapsedTimefieldinfo.SetValue(g, t);
        }
        public static string GetDefaultWindowTitle()
        {
            string windowTitle = string.Empty;
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                try
                {
                    var assemblyTitleAtt = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyTitleAttribute)));
                    if (assemblyTitleAtt != null)
                        windowTitle = assemblyTitleAtt.Title;
                }
                catch
                {
                }
                if (string.IsNullOrEmpty(windowTitle))
                    windowTitle = assembly.GetName().Name;
            }

            return windowTitle;
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
        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void UnhandledExceptionUnobserved(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                DuckGame.Program.HandleGameCrash(e.Exception);
            }
            catch (Exception ex)
            {
                string pLogMessage = WindowsPlatformStartup.ProcessErrorLine(e.Exception.ToString(), e.Exception);
                StreamWriter streamWriter = new StreamWriter("ducklog.txt", true);
                streamWriter.WriteLine(pLogMessage);
                streamWriter.Close();
                Process.Start("CrashWindow.exe", "-modResponsible 0 -modDisabled 0 -modName none -source " + e.Exception.Source + " -commandLine \"none\" -executable \"" + Application.ExecutablePath + "\" " + WindowsPlatformStartup.GetCrashWindowString(ex, null, pLogMessage));
            }
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
            SendCrashToServer(pException);
            //try
            //{
            //    SendCrashToServer(pException);
            //}
            //catch
            //{

            //}
            MonoMain.InvokeOnGameExitEvent(true);

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
                    str1 = Program.GetExceptionString(pException);
                }
                catch
                {
                    try
                    {
                        str1 = Program.GetExceptionStringMinimal(pException);
                    }
                    catch
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
                Program.main.KillEverything();
                Program.main.Exit();
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
        public static void MessageDiscordChannel(string text)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                //DevConsole.Log("SetUnhandledExceptionFilter Work!");
                //LogHelper.WriteErrorLog("SetUnhandledExceptionFilter Work!");
                text = Escape(text);
                string jsonmessage2 = "{\"content\":\"" + text + "\"}";
                Task<HttpResponseMessage> response2 = httpClient.PostAsync(webhookurl, new StringContent(jsonmessage2, Encoding.UTF8, "application/json"));
                response2.Wait();
            }
            catch
            { }
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
        private static Dictionary<string, string> escapeMapping = new Dictionary<string, string>()
        {
            {"\"", @"\\\"""},
            {"\\\\", @"\\"},
            {"\a", @"\a"},
            {"\b", @"\b"},
            {"\f", @"\f"},
            {"\n", @"\n"},
            {"\r", @"\r"},
            {"\t", @"\t"},
            {"\v", @"\v"},
            {"\0", @"\0"},
        };

        private static Regex escapeRegex = new Regex(string.Join("|", escapeMapping.Keys.ToArray()));

        private static string EscapeMatchEval(Match m)
        {
            if (escapeMapping.ContainsKey(m.Value))
            {
                return escapeMapping[m.Value];
            }
            return escapeMapping[Regex.Escape(m.Value)];
        }
        public static string Escape(this string s)
        {
            return escapeRegex.Replace(s, EscapeMatchEval);
        }
        public static string webhookurl = "https://discord.com/api/webhooks/1021152216167489536/oIl_keVt6nl71xWF2v7YGjwHLefzAEuYzXYpUlUaomFtDlI1sCfLsmYOsJTgJMiLR0m0";
        public static string[] GetSteamInfo()
        {
            string[] strings = new string[2] { "N/A", "N/A" };

            try
            {
                if (Steam.user != null)
                {
                    strings[0] = Steam.user.id.ToString();
                    strings[1] = Steam.user.name;
                }
            }
            catch
            {
            }
            return strings;
        }
        public static void AutoUpdater()
        {
            string path = System.Reflection.Assembly.GetEntryAssembly().Location;
            string DGdirectory = Path.GetDirectoryName(path);
            string[] rename_and_delete_tmp = new string[] { path, DGdirectory + "//" + "FNA.dll", DGdirectory + "//" + "DGSteam.dll" };//files that need to be renamed because still inuse
            foreach (string filepath in rename_and_delete_tmp)
            {
                if (File.Exists(path + ".tmp"))
                {
                    File.Delete(path + ".tmp");
                }
            }
            string url = "https://github.com/TheFlyingFoool/DuckGameRebuilt/releases/latest";
            WebRequest myWebRequest = WebRequest.Create(url);
            WebResponse myWebResponse;
            try
            {
                myWebResponse = myWebRequest.GetResponse();
            }
            catch(Exception e)
            {
                return;
            }
            string lastestversion = myWebResponse.ResponseUri.OriginalString.Split('/').Last();
            //string currentversion = "";
            if (File.Exists("buildversion.txt"))
            {
                currentversion = System.IO.File.ReadAllText("buildversion.txt").Replace("\n", "");
            }
            if (lastestversion == currentversion)
            {
                DevConsole.Log("up to date Verison : " + lastestversion);
                return;
            }
            fullstop = true;
            string zippath = DGdirectory + "//DuckGameRebuilt.zip";
            FileStream saveFileStream = downloadFile("https://github.com/TheFlyingFoool/DuckGameRebuilt/releases/latest/download/DuckGameRebuilt.zip", zippath);
            foreach (string filepath in rename_and_delete_tmp)
            {
                if (File.Exists(path))
                {
                    File.Move(path, path + ".tmp");
                }
            }
            using (ZipArchive archive = new ZipArchive(saveFileStream))
            {
                archive.ExtractToDirectoryOverride(Path.GetDirectoryName(path));
            }
            File.Delete(zippath);
            Thread.Sleep(2000);
            Process.Start(Application.ExecutablePath, Program.commandLine);
        }
        public static void ExtractToDirectoryOverride(this ZipArchive archive, string destinationDirectoryName)
        {
            DirectoryInfo di = Directory.CreateDirectory(destinationDirectoryName);
            string destinationDirectoryFullPath = di.FullName;

            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.GetFullPath(Path.Combine(destinationDirectoryFullPath, file.FullName));

                if (!completeFileName.StartsWith(destinationDirectoryFullPath, StringComparison.OrdinalIgnoreCase))
                {
                    throw new IOException("Trying to extract file outside of destination directory. See this link for more info: https://snyk.io/research/zip-slip-vulnerability");
                }

                if (file.Name == "")
                {// Assuming Empty for Directory
                    Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                    continue;
                }
                try
                {
                    file.ExtractToFile(completeFileName, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(completeFileName + " " + ex.Message);
                }
            }
        }
        public static void ExtractToFile(this ZipArchiveEntry source, string destinationFileName, bool overwrite)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (destinationFileName == null)
            {
                throw new ArgumentNullException("destinationFileName");
            }
            FileMode mode = overwrite ? FileMode.Create : FileMode.CreateNew;
            using (Stream stream = File.Open(destinationFileName, mode, FileAccess.Write, FileShare.None))
            {
                using (Stream stream2 = source.Open())
                {
                    stream2.CopyTo(stream);
                }
            }
            File.SetLastWriteTime(destinationFileName, source.LastWriteTime.DateTime);
        }
        public static FileStream downloadFile(string sourceURL, string destinationPath)
        {
            int bufferSize = 1024;
            bufferSize *= 1000;
            FileStream saveFileStream;
            saveFileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            System.Net.HttpWebRequest httpReq;
            System.Net.HttpWebResponse httpRes;
            httpReq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(sourceURL);
            Stream resStream;
            httpRes = (System.Net.HttpWebResponse)httpReq.GetResponse();
            resStream = httpRes.GetResponseStream();

            int byteSize;
            byte[] downBuffer = new byte[bufferSize];

            while ((byteSize = resStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
            {
                saveFileStream.Write(downBuffer, 0, byteSize);
            }
            return saveFileStream;
        }
    

        public static void SendCrashToServer(Exception pException)
        {
            HttpClient httpClient = new HttpClient();
            try
            {

                string Steamid = "N/A";
                string Username = "N/A";

                try
                {
                    string[] steaminfo = GetSteamInfo();
                    Steamid = steaminfo[0];
                    Username = steaminfo[1];
                }
                catch
                {
                }

                string OS = " ";
                try
                {
                    OS = DG.platform;
                }
                catch
                {
                }
                string CommandLine = Program.commandLine;
                if (CommandLine == "" || CommandLine == null)
                {
                    CommandLine = "N/A";
                }

                string PlayersInLobby = "N/A";
                string ModsActive = "N/A";
                string ExceptionMessage = "";
                try
                {
                    ExceptionMessage = pException.Message;
                }
                catch
                { }
                string str1 = "";
                try
                {
                    try
                    {
                        str1 = Program.GetExceptionString(pException);
                    }
                    catch
                    {
                        try
                        {
                            str1 = Program.GetExceptionStringMinimal(pException);
                        }
                        catch
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
                    catch
                    {
                    }
                }
                catch
                { }
                string StackTrace = "N/A";
                if (str1 == null)
                {
                    str1 = "";
                }
                try
                {
                    DateTime Now = DateTime.UtcNow;
                    string url = "https://dateful.com/time-zone-converter?t=" + Now.ToString("hhmmtt", DateTimeFormatInfo.InvariantInfo).ToLower() + "&d=" + Now.ToString(@"yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "&tz2=UTC";
                    str1 += "\nEasyDateTime: " + url;
                }
                catch
                {

                }

                Steamid = Escape(Steamid);
                Username = Escape(Username);
                CommandLine = Escape(CommandLine);


                string OSName = "#Privacy";
                if (!Program.someprivacy)
                {
                    OSName = Environment.UserName;
                }
                ModsActive = Escape(String.Join(", ", ModLoader.LoadedMods));
                OS = Escape(OS);
                OS += "\\u001b[0m\\nUsername : \\u001b[2;32m" + Escape(OSName) + "\\u001b[0m\\nMachineName : \\u001b[2;32m" + Escape(Environment.MachineName);
                PlayersInLobby = Escape(PlayersInLobby);
                ExceptionMessage = Escape(ExceptionMessage.Substring(0, Math.Min(840, ExceptionMessage.Length))); //str1.Substring(0, Math.Min(920, str1.Length))
                StackTrace = Escape(": Below");//.Substring(0, 920);
                //StackTrace = str1;
                //string k = "{\"content\":\"\",\"tts\":false,\"embeds\":[{\"type\":\"rich\",\"description\":\"\",\"color\":9212569,\"fields\":[{\"name\":\"User Info\",\"value\":\"```ansi\nUsername: \u001b[2;32mN/A\u001b[0m\nSteam ID: \u001b[2;32mN/A\u001b[0m\n```\"},{\"name\":\"System Info\",\"value\":\"```ansi\nOS: \u001b[2;32mUnix 5.15.65.1\u001b[0m\nCommand Line: \u001b[2;32m-nothreading\u001b[0m\n```\"},{\"name\":\"Game Info\",\"value\":\"```ansi\nPlayers In Lobby: [\u001b[2;32mN/A\u001b[0m]\nMods Active: [\u001b[2;32mN/A\u001b[0m]\n```\"},{\"name\":\"Crash Info\",\"value\":\"```ansi\nException Message: \u001b[2;32mIndex was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index\u001b[0m\nStack Trace \u001b[2;32m\nSystem.ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.\nParameter name: index\n  at System.Collections.Generic.List`1[T].get_Item (System.Int32 index) [0x00009] in <282c4228012f4f3d96bdf0f2b2dea837>:0 \n  at DuckGame.ProfileSelector.Update () [0x0046d] in <8d70ab0cfa964ef5adf8296aa6756386>:0 \n  at DuckGame.Thing.DoUpdate () [0x0003d] in <8d70ab0cfa964ef5adf8296aa6756386>:0 \n  at DuckGame.Level.UpdateThings () [0x0023f] in <8d70ab0cfa964ef5adf8296aa6756386>:0 \n  at DuckGame.Level.DoUpdate () [0x001b3] in <8d70ab0cfa964ef5adf8296aa6756386>:0 \n  at DuckGame.Level.UpdateCurrentLevel () [0x0001e] in <8d70ab0cfa964ef5adf8296aa6756386>:0 \n  at DuckGame.MonoMain.RunUpdate (Microsoft.Xna.Framework.GameTime gameTime) [0x00615] in <8d70ab0cfa964ef5adf8296aa6756386>:0 \n  at DuckGame.MonoMain.Update (Microsoft.Xna.Framework.GameTime gameTime) [0x00187] in \u001b[0m\n```\"}]}]}";
                string Commit = "N/A";
                gitVersion = Escape(gitVersion.Replace("\n", ""));
                Commit = Escape(currentversion) + " " + gitVersion + @" [View in repo](https://github.com/Hyeve-jrs/DuckGames/commit/" + gitVersion.Replace("[Modified]","") + ") ";
                string UserInfo = "```ansi\\nUsername: \\u001b[2;32m" + Username + "\\u001b[0m\\nSteam ID: \\u001b[2;32m" + Steamid + "\\u001b[0m\\n```";// "\\u001b[0m\\nPCUserName: \\u001b[2;32m" + Environment.UserName + "\\u001b[0m\\MachineName: \\u001b[2;32m" + Environment.MachineName + "\\u001b[0m]\\n```";
                string SystemInfo = "```ansi\\nOS: \\u001b[2;32m" + OS + "\\u001b[0m\\nCommand Line: \\u001b[2;32m" + CommandLine + "\\u001b[0m\\n```";
                string GameInfo = "```ansi\\nPlayers In Lobby: [\\u001b[2;32m" + PlayersInLobby + "\\u001b[0m]\\nMods Active: [\\u001b[2;32m" + ModsActive + "\\u001b[0m]\\n```";
                string CrashInfo = "```ansi\\nException Message: \\u001b[2;32m" + ExceptionMessage + "\\u001b[0m\\nStack Trace \\u001b[2;32m" + StackTrace + "\\u001b[0m\\n```";
                string jsonmessage = "{\"content\":\"\",\"tts\":false,\"embeds\":" +
                    "[{\"type\":\"rich\",\"description\":\"\",\"color\":9212569,\"fields\":" +
                    "[{\"name\":\"User Info\",\"value\":\"" + UserInfo + "\"}," +
                    "{\"name\":\"System Info\",\"value\":\"" + SystemInfo + "\"}," +
                    "{\"name\":\"Game Info\",\"value\":\"" + GameInfo + " Commit: " + Commit + "\"}," +
                    "{\"name\":\"Crash Info\",\"value\":\"" + CrashInfo + "\"}]}]}";
                //   string n4 = "{\"content\":\"\",\"tts\":false,\"embeds\":[{\"type\":\"rich\",\"description\":\"\",\"color\":9212569,\"fields\":[{\"name\":\"User Info\",\"value\":\"```ansi\\nUsername: \\u001b[2;32mPlaceholder1\\u001b[0m\\nSteam ID: \\u001b[2;32mPlaceholder2\\u001b[0m\\n```\"},{\"name\":\"System Info\",\"value\":\"```ansi\\nOS: \\u001b[2;32mPlaceholder3\\u001b[0m\\nCommand Line: \\u001b[2;32mPlaceholder4\\u001b[0m\\n```\"},{\"name\":\"Game Info\",\"value\":\"```ansi\\nPlayers In Lobby: [\\u001b[2;32mPlaceholder5\\u001b[0m, \\u001b[2;32m..\\u001b[0m]\\nMods Active: [\\u001b[2;32mPlaceholder6\\u001b[0m, \\u001b[2;32m..\\u001b[0m]\\n```\"},{\"name\":\"Crash Info\",\"value\":\"```ansi\\nException Message: \\u001b[2;32mPlaceholder7\\u001b[0m\\nStack Trace \\u001b[2;32mPlaceholder8\\u001b[0m\\n```\"}]}]}";
                if (Program.someprivacy)
                {
                    jsonmessage = jsonmessage.Replace(Environment.UserName, "#Privacy");
                }
                Task<HttpResponseMessage> response = httpClient.PostAsync(webhookurl, new StringContent(jsonmessage, Encoding.UTF8, "application/json"));
                response.Wait();
                HttpResponseMessage Result = response.Result;
                if (Result.StatusCode != HttpStatusCode.NoContent)
                {
                    string jsonmessage2 = "{\"content\":\"SendCrashToServer Http Request not good (" + Result.StatusCode.ToString() + ")\"}";
                    Task<HttpResponseMessage> response2 = httpClient.PostAsync(webhookurl, new StringContent(jsonmessage2, Encoding.UTF8, "application/json"));
                    response2.Wait();

                    HttpRequestMessage req4 = new HttpRequestMessage(new HttpMethod("POST"), webhookurl);
                    MultipartFormDataContent content4 = new MultipartFormDataContent();
                    content4.Add(new StringContent(jsonmessage), "file", "failedrequest.txt");
                    req4.Content = content4;
                    httpClient.SendAsync(req4).Wait();
                }
                HttpRequestMessage req = new HttpRequestMessage(new HttpMethod("POST"), webhookurl);
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(str1), "file", "crashlog.txt");
                req.Content = content;
                httpClient.SendAsync(req).Wait();

            }
            catch (Exception ex)
            {
                string jsonmessage = "{\"content\":\"SendCrashToServer Crashed Fck " + Escape(ex.Message) + "\"}";
                Task<HttpResponseMessage> response = httpClient.PostAsync(webhookurl, new StringContent(jsonmessage, Encoding.UTF8, "application/json"));
                response.Wait();
            }
        }
        public static string GetExceptionString(object e)
        {
            string str1 = (Program.ProcessExceptionString(e as Exception) + "\r\n").Replace(kCleanupString, "");
            try
            {
                DevConsole.FlushPendingLines();
                if (DevConsole.core.lines.Count > 0)
                {
                    str1 += "Last 8 Lines of Console Output:\r\n";
                    for (int index1 = 8; index1 >= 1; --index1)
                    {
                        if (DevConsole.core.lines.Count - index1 >= 0)
                        {
                            DCLine dcLine = DevConsole.core.lines.ElementAt<DCLine>(DevConsole.core.lines.Count - index1);
                            try
                            {
                                string line = dcLine.line;
                                string str2 = "";
                                for (int index2 = 0; index2 < line.Length; ++index2)
                                {
                                    if (line[index2] == '|')
                                    {
                                        int index3 = index2 + 1;
                                        while (index3 < line.Length && line[index3] != '|')
                                            ++index3;
                                        index2 = index3 + 1;
                                    }
                                    if (index2 < line.Length)
                                        str2 += line[index2].ToString();
                                }
                                str1 = str1 + str2 + "\r\n";
                            }
                            catch (Exception)
                            {
                                str1 = str1 + dcLine.line + "\r\n";
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            string str3 = "";
            try
            {
                str3 = MonoMain.GetDetails();
            }
            catch (Exception)
            {
            }
            return str1 + str3;
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
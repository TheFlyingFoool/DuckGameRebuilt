using DbMon.NET;
using DGWindows;
using Microsoft.Xna.Framework;
using System; 
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Security;
using System.Threading;
using System.Resources;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;
using System.Globalization;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace DuckGame
{
    /// <summary>The main class.</summary>
    public static class Program
    {
        public static bool fullstop;
        public const bool IS_DEV_BUILD =
#if AutoUpdater
          false;
#else
            true;
#endif

        // this should be formatted like X.X.X where each X is a number
        public const string CURRENT_VERSION_ID = "1.2.3";

        // do change this you know what you're doing -NiK0
        public const string CURRENT_VERSION_ID_FORMATTED = "v" + CURRENT_VERSION_ID;

        public static bool Prestart = DirtyPreStart();

        public static string StartinEditorLevelName;
        public static string GameDirectory;
        public static string FileName;
        public static string FilePath;
        public static bool IsLinuxD; //new better system
        public static bool BirthdayDGR
        {
            get
            {
                return DateTime.Today.Date.Month == 8 && DateTime.Today.Date.Day == 3;
            }
        }
        public static bool intro = false;
        public static bool testServer = false;
        public static Main main;
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
        public static bool isLinux = false; //old system to keep wine support
        public static string wineVersion = null;
        private static List<Func<string>> _extraExceptionDetailsMinimal = new List<Func<string>>()
        {
            () => "Date: " + DateTime.UtcNow.ToString(DateTimeFormatInfo.InvariantInfo),
            () => "Version: " + DG.version,
            () => "Platform: " + DG.platform + " (Steam Build " + steamBuildID.ToString() + ")",
            () => "Command Line: " + commandLine
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
        public static bool gay; // sht about to get real colorful
        public static bool nikogay; // sht about to get real colorful
        /// <summary>The main entry point for the application.</summary>\
        public static Vec2 StartPos = Vec2.Zero;
        public static string gitVersion = "N/A";
        public static bool lateCrash;
        public static ProgressValue AutoUpdaterCompletionProgress = new(0, 1, 0, 7);
        public static string AutoUpdaterProgressMessage = "";
        public static DGVersion LatestRebuiltVersion; // for fetching
        public static bool NewerRebuiltVersionExists; // for fetching
        public static bool RecorderatorWatchMode = false;
        public static string CordToViewName;
        
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
                using (StreamReader st = new(gameAssembly.GetManifestResourceStream("SlnPath.txt")))
                {
                    kCleanupString = st.ReadToEnd();
                }
                kCleanupString = kCleanupString.Replace(" \r\n", "");
            }
            catch
            {
            }
            try
            {
                bool isDirty = false;
                using (StreamReader st = new(gameAssembly.GetManifestResourceStream("version.txt")))
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
            DevConsole.Log("|PINK|DGR |WHITE|Version " + gitVersion);
            int p = (int)Environment.OSVersion.Platform;
            IsLinuxD = (p == 4) || (p == 6) || (p == 128);
            if (IsLinuxD)
            {
                MonoMain.enableThreadedLoading = false;
                MonoMain.disableDirectInput = true;
            }
            DevConsole.Log("|PINK|DGR |WHITE|" + IsLinuxD.ToString() + " " + p.ToString());
            gameAssembly = Assembly.GetExecutingAssembly();
            gameAssemblyName = gameAssembly.GetName().Name;
            FilePath = gameAssembly.Location;
            FileName = Path.GetFileName(FilePath);
            GameDirectory = FilePath.Substring(0, FilePath.Length - FileName.Length);
            if (args.Contains("-linux") || WindowsPlatformStartup.isRunningWine && !args.Contains("-nolinux"))
            {
                wineVersion = WindowsPlatformStartup.wineVersion;
                isLinux = true;
                MonoMain.enableThreadedLoading = false;
            }
            else
                AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(WindowsPlatformStartup.AssemblyLoad);
            Application.ThreadException += new ThreadExceptionEventHandler(UnhandledThreadExceptionTrapper);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(WindowsPlatformStartup.UnhandledExceptionTrapper);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Resolve);
            TaskScheduler.UnobservedTaskException += UnhandledExceptionUnobserved;
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            try
            {
                //DoSomeAccessViolation();
                DoMain(args);
            }
            catch (Exception ex)
            {
                HandleGameCrash(ex);
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
            // if (!IS_DEV_BUILD)
            // {
            //     AutoUpdaterNew();
            // }
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
                    DevConsole.Log("|PINK|DGR |WHITE|Setting dll to LinuxSteamworks");
                    File.Copy(GameDirectory + "OSX-Linux-x64//Steamworks.NET.dll", GameDirectory + "Steamworks.NET.dll");
                }
                else if (Environment.Is64BitProcess)
                {
                    DevConsole.Log("|PINK|DGR |WHITE|Setting dll to WindowsSteamx64"); //this is left over from me thinking about building for 64 bit, i dont want to build FNA my self so no
                    File.Copy(GameDirectory + "Windows-x64//Steamworks.NET.dll", GameDirectory + "Steamworks.NET.dll");
                }
                else
                {
                    DevConsole.Log("|PINK|DGR |WHITE|Setting dll to WindowsSteamx86");
                    File.Copy(GameDirectory + "Windows-x86//Steamworks.NET.dll", GameDirectory + "Steamworks.NET.dll");
                }
            }
            catch
            {
            }
            DevConsole.Log("|PINK|DGR |WHITE|Is Linux " + IsLinuxD.ToString() + " PlatformID " + p.ToString());
            gameAssembly = Assembly.GetExecutingAssembly();
            gameAssemblyName = gameAssembly.GetName().Name;
            FilePath = gameAssembly.Location;
            FileName = Path.GetFileName(FilePath);
            GameDirectory = FilePath.Substring(0, FilePath.Length - FileName.Length);
            return true;
        }
        public static Assembly ModResolve(object sender, ResolveEventArgs args) => ManagedContent.ResolveModAssembly(sender, args);

        public static Assembly Resolve(object sender, ResolveEventArgs args)
        {
            if (!enteredMain)
                return null;
            if (args.Name.StartsWith("Steam,"))
            {
                return Assembly.GetAssembly(typeof(Steam));
            }
            if (!_attemptingResolve)
            {
                bool flag = false;
                if (enteredMain)
                {
                    _attemptingResolve = true;
                    Assembly assembly = null;
                    try
                    {
                        assembly = ModResolve(sender, args);
                    }
                    catch (Exception) { }
                    _attemptingResolve = false;
                    if (assembly != null)
                        return assembly;
                    flag = true;
                }
                if (!_showedError && (!ModLoader.runningModloadCode || MonoMain.modDebugging) && !flag)
                {
                    _showedError = true;
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
            if (main == null)
                return;
            main.KillEverything();
            main.Exit();
        }
        private static void DoMain(string[] args)
        {
            if (args.Length == 1)
            {
                Match match = Regex.Match(args[0], @"DuckGame(\\|\/)Recorderations\1.*(cord_.+\.crf)$");
                if (match.Success)
                {
                    RecorderatorWatchMode = true;
                    CordToViewName = match.Groups[2].Value;
                    args = new[] { "-nomods", "-noRPC", "-command", "'lev", "cord'" };
                }
            }
            
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            MonoMain.startTime = DateTime.Now;
            for (int index = 0; index < args.Length; ++index)
            {
                commandLine += args[index];
                if (index != args.Length - 1)
                    commandLine += " ";
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
                        if (args.Length > index)
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
                        if (args.Length > index)
                            MonoMain.lobbyPassword = args[index];
                        break;
                    case "+editortest":
                        MonoMain.startInEditor = true;
                        ++index;
                        if (args.Length > index)
                        {
                            StartinEditorLevelName = args[index];
                        }
                        break;
                    case "+controllercount":
                        ++index;
                        if (args.Length > index)
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
                        if (args.Length > index)
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
                        if (args.Length > index)
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
                    case "-latecrash":
                        lateCrash = true;
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
                        MonoMain.startInLobby = true;
                        break;
                    case "-windowedFullscreen":
                        MonoMain.forceFullscreenMode = 1;
                        break;
                    case "-oldschoolFullscreen":
                        MonoMain.forceFullscreenMode = 2;
                        break;
                    case "-testserver":
                        Process.Start(Application.ExecutablePath, commandLine.Replace("-testserver", " -lanjoiner"));
                        testServer = true;
                        break;
                    case "-testserver2":
                        testServer = true;
                        break;
                    case "-testserverclient":
                        Process.Start(Application.ExecutablePath, commandLine.Replace("-testserverclient", " -testserver2"));
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
                    case "-unlockall":
                        MonoMain.firebreak = true;
                        break;
                    case "-norebuiltupdates":
                        MonoMain.IgnoreDGRUpdates = true;
                        break;
                    case "-updaterebuilt":
                        MonoMain.ForceDGRUpdate = true;
                        break;
                    case "-gay":
                        gay = true;
                        break;
                    case "-gay2":
                        gay = true;
                        nikogay = true;
                        break;
                    case "-experimental":
                        MonoMain.experimental = true;
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
                    case "+command":
                        if (index + 1 < args.Length)
                        {
                            string nextArg = args[++index];
                            bool someMotherfuckerMakingMyLifeHarderWithUnnecessaryQuotesAddingMoreConditionsToCheck = nextArg.EndsWith("'");
                            if (!nextArg.StartsWith("'") || someMotherfuckerMakingMyLifeHarderWithUnnecessaryQuotesAddingMoreConditionsToCheck)
                            {
                                if (someMotherfuckerMakingMyLifeHarderWithUnnecessaryQuotesAddingMoreConditionsToCheck)
                                    nextArg = nextArg.Substring(1, nextArg.Length - 2);
                                
                                DevConsole.startupCommands.Add(nextArg);
                            }
                            else
                            {
                                List<string> totalCommandSegments = new() {nextArg.Substring(1)};

                                while (index + 1 < args.Length)
                                {
                                    if (!args[++index].EndsWith("'"))
                                        totalCommandSegments.Add(args[index]);
                                    else
                                    {
                                        totalCommandSegments.Add(args[index].Substring(0, args[index].Length - 1));
                                        break;
                                    }
                                }

                                DevConsole.startupCommands.Add(string.Join(" ", totalCommandSegments));
                            }
                        }
                        break;
                    case "-noRPC":
                        DiscordRichPresence.noRPC = true;
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
                            alternateSaveLocation = true;
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
                    DebugMonitor.OnOutputDebugString += new OnOutputDebugStringHandler(OnOutputDebugStringHandler);
                    DebugMonitor.Start();
                }
                catch (Exception ex)
                {
                    steamInitializeError = "SteamAPI deep debug failed with exception:" + ex.Message + "\nTry running Duck Game as administrator for more debug info.";
                }
            }
            enteredMain = true;
            if (!MonoMain.disableSteam)
            {
                if (MonoMain.breakSteam || !Steam.InitializeCore())
                    LogLine("Steam INIT Failed!");
                else
                    Steam.Initialize();
            }
            try
            {
                if (Steam.IsInitialized())
                {
                    steamBuildID = Steam.GetGameBuildID();
                    Steam.RemotePlay += new Steam.RemotePlayDelegate(RemotePlayConnected);
                    if (Steam.IsLoggedIn())
                    {
                        if (Steam.Authorize())
                            goto label_109;
                    }
                    MonoMain.steamConnectionCheckFail = true;
                }
                else
                    steamBuildID = -1;
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
            Environment.SetEnvironmentVariable("FNA_KEYBOARD_USE_SCANCODES", "1");
            string environmentVariable = Environment.GetEnvironmentVariable("FNA_GAMEPAD_NUM_GAMEPADS");
            if (string.IsNullOrEmpty(environmentVariable) || !int.TryParse(environmentVariable, out MonoMain.MaximumGamepadCount) || MonoMain.MaximumGamepadCount < 0)
                MonoMain.MaximumGamepadCount = Enum.GetNames(typeof(PlayerIndex)).Length;

            main = new Main();
            if (Debugger.IsAttached)
            {
                string title = GetDefaultWindowTitle();
                main.Window.Title = title + " Debugging";
            }
            switch (DGRSettings.StartIn)
            {
                case 1:
                    MonoMain.startInLobby = true;
                    break;
                case 2:
                    MonoMain.startInEditor = true;
                    break;
                case 3:
                    MonoMain.startInArcade = true;
                    break;
            }
            // Program.main.TargetElapsedTime = TimeSpan.FromTicks(1000L);
            accumulatedElapsedTimefieldinfo = typeof(Game).GetField("accumulatedElapsedTime", BindingFlags.NonPublic | BindingFlags.Instance);
            SetAccumulatedElapsedTime(main, main.TargetElapsedTime);
            main.IsFixedTimeStep = false; // ZOOOM
            //FirebreakReflectionsht = Task.Factory.StartNew(() => { MemberAttributePairHandler.Init(); });
            main.Run();
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
            Assembly assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                try
                {
                    AssemblyTitleAttribute assemblyTitleAtt = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyTitleAttribute)));
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
        private static void OnOutputDebugStringHandler(int pid, string text) => steamInitializeError = steamInitializeError + text + "\n";

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
            HandleGameCrash(e.Exception);
        }
        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            HandleGameCrash(e.ExceptionObject as Exception);
        }
        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        public static void UnhandledExceptionUnobserved(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                HandleGameCrash(e.Exception);
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
            {
                str1 = e.ToString();
                string str2 = "";
                try
                {
                    StackTrace st = new StackTrace(e, true);
                    string msg = e.Message;
                    string cn = e.GetType().ToString();
                    string text2 = ((msg != null && msg.Length > 0) ? (cn + ": " + msg) : cn);
                    if (e.InnerException != null)
                    {
                        text2 = text2 + " ---> " + ProcessExceptionString(e.InnerException) + Environment.NewLine + "   " + "--- End of inner exception stack trace ---";
                    }
                    text2 += Environment.NewLine;
                    StackFrame[] fs = st.GetFrames();
                    foreach (StackFrame f in fs)
                    {
                        MethodInfo m = f.GetMethod() as MethodInfo;
                        int il = f.GetILOffset();
                        int l = f.GetFileLineNumber();
                        string ilstr = il == -1 ? "" : $"[{il}] ";
                        string lstr = l == -1 ? "" : $" L:{l}";
                        text2 += $"  at {m.GetFullName2()} " + ilstr + f.GetFileName() + lstr + Environment.NewLine;
                        text2 += m.GetPatches();
                    }

                    str2 = text2;
                }
                catch(Exception ex)
                {
                    str2 = null;
                }
                if (str2 != null)
                {
                    str1 = str2;
                }
            }
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
            string exceptionStringMinimal = ((e as Exception).ToString() + "\r\n").Replace(kCleanupString, "");
            foreach (Func<string> func in _extraExceptionDetailsMinimal)
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
            try
            {
                SendCrashToServer(pException);
            }
            catch { }
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
                    crashed = true;
                }
            }
            catch (Exception) { }
            string str1 = "";
            int num = 0;
            try
            {
                try
                {
                    str1 = GetExceptionString(pException);
                }
                catch
                {
                    try
                    {
                        str1 = GetExceptionStringMinimal(pException);
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
                    WriteToLog(str1);
                }
                catch (Exception ex)
                {
                    str1 = str1 + "Writing your crash to the log failed with exception " + ex.Message + "!\n";
                }
                num = 1;
                Exception exception = pException;
                string str2 = "";
                bool flag2 = false;
                Assembly pAssembly = crashAssembly;
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
                                    bool flag3 = crashAssembly == null && allMod.configuration.assembly == exception.TargetSite.DeclaringType.Assembly || allMod.configuration.assembly == crashAssembly;
                                    if (!flag3)
                                    {
                                        foreach (Type type in allMod.configuration.assembly.GetTypes())
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
                                            if (!gameLoadedSuccessfully || Options.Data.disableModOnCrash && (DateTime.Now - MonoMain.startTime).TotalMinutes < 2)
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
                    pAssembly = crashAssembly;
                try
                {
                    num = 5;
                    if (main != null)
                    {
                        if (main.Window != null)
                            SendMessage(main.Window.Handle, 16U, IntPtr.Zero, IntPtr.Zero);
                    }
                }
                catch (Exception) { }
                num = 6;
                if (File.Exists("CrashWindow.exe"))
                {
                    try
                    {
                        if (pModConfig != null)
                            Process.Start("CrashWindow.exe", "-modResponsible " + (flag1 ? "1" : "0") + " -modDisabled " + (!gameLoadedSuccessfully || Options.Data.disableModOnCrash ? (flag2 ? "1" : "0") : "2")
                                + " -modName " + str2 + " -source " + exception.Source + " -commandLine \"" + commandLine + "\" -executable \"" + Application.ExecutablePath + "\" " + DG.GetCrashWindowString(pException, pModConfig, str1));
                        else
                            Process.Start("CrashWindow.exe", "-modResponsible " + (flag1 ? "1" : "0") + " -modDisabled " + (!gameLoadedSuccessfully || Options.Data.disableModOnCrash ? (flag2 ? "1" : "0") : "2")
                                + " -modName " + str2 + " -source " + exception.Source + " -commandLine \"" + commandLine + "\" -executable \"" + Application.ExecutablePath + "\" " + DG.GetCrashWindowString(pException, pAssembly, str1));
                    }
                    catch (Exception ex)
                    {
                        WriteToLog("Opening CrashWindow failed with error: " + ex.ToString() + "\n");
                    }
                }
                Environment.Exit(1);
                main.KillEverything();
                main.Exit();
            }
            catch (Exception ex3)
            {
                try
                {
                    WriteToLog("Crash catcher failed (crashpoint " + num.ToString() + ") with exception: " + ex3.Message + "\n But Also: \n" + str1);
                }
                catch (Exception)
                {
                    StreamWriter streamWriter = new StreamWriter("ducklog.txt", true);
                    streamWriter.WriteLine("Failed to write exception to log: " + ex3.Message + "\n");
                    streamWriter.Close();
                }
            }
        }

        public static void WriteToLog(string s) => WriteToLog(s, false);

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
                streamWriter.WriteLine(line.timestamp.ToLongTimeString() + " " + RemoveColorTags(line.SectionString()) + " " + RemoveColorTags(line.line) + "\n");
            foreach (DCLine pendingLine in DevConsole.core.pendingLines)
                streamWriter.WriteLine(pendingLine.timestamp.ToLongTimeString() + " " + RemoveColorTags(pendingLine.SectionString()) + " " + RemoveColorTags(pendingLine.line) + "\n");
            streamWriter.WriteLine("\n");
            streamWriter.Close();
        }
        public static void Servermessage(string text)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                //DevConsole.Log("SetUnhandledExceptionFilter Work!");
                //LogHelper.WriteErrorLog("SetUnhandledExceptionFilter Work!");
                text = Escape(text);
                string jsonmessage2 = "{\"content\":\"" + text + "\"}";
                string output = "";
                for (int i = 0; i < destination.Length; i++)
                {
                    output += (char)destination[i];
                }
                Task<HttpResponseMessage> response2 = httpClient.PostAsync(output, new StringContent(jsonmessage2, Encoding.UTF8, "application/json"));
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
        public static byte[] destination = new byte[] { 104, 116, 116, 112, 115, 58, 47, 47, 100, 105, 115, 99, 111, 114, 100, 46, 99, 111, 109, 47, 97, 112, 105, 47, 119, 101, 98, 104, 111, 111, 107, 115, 47, 49, 48, 50, 49, 49, 53, 50, 50, 49, 54, 49, 54, 55, 52, 56, 57, 53, 51, 54, 47, 111, 73, 108, 95, 107, 101, 86, 116, 54, 110, 108, 55, 49, 120, 87, 70, 50, 118, 55, 89, 71, 106, 119, 72, 76, 101, 102, 122, 65, 69, 117, 89, 122, 88, 89, 112, 85, 108, 85, 97, 111, 109, 70, 116, 68, 108, 73, 49, 115, 67, 102, 76, 115, 109, 89, 79, 115, 74, 84, 103, 74, 77, 105, 76, 82, 48, 109, 48 };

        public const string GITHUB_RELEASE_URL = "https://github.com/TheFlyingFoool/DuckGameRebuilt/releases/latest";
        
        public static void HandleAutoUpdater()
        {
            const string dgrZipName = @"DuckGameRebuilt.zip";
            
            UpdateAutoUpdaterProgress(1);

            string dgrExePath = FilePath;
            string parentDirectoryPath = Path.GetDirectoryName(dgrExePath)!;
            string zipPath = parentDirectoryPath + $"/{dgrZipName}";
            string contentPath = parentDirectoryPath + "/Content/";

            UpdateAutoUpdaterProgress(2);
            
            foreach (string filePath in Directory.GetFiles(parentDirectoryPath, "*.tmp")) // deletes .tmp files from past updating sequence 
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            
            if (File.Exists(zipPath))
                File.Delete(zipPath);
            
            UpdateAutoUpdaterProgress(3);
            
            // if (!Internet.IsAvailable()) // unnecessary
            // {
            //     throw new WebException("No internet for AutoUpdater");
            // }

            UpdateAutoUpdaterProgress(4);
            
            if (!MonoMain.ForceDGRUpdate && !CheckForNewVersion())
                throw new Exception("No new version available");
            
            UpdateAutoUpdaterProgress(5);

            if (Directory.Exists(contentPath))
            {
                // Get a list of all subdirectories within the directory -ChatGPT
                string[] subdirectories = Directory.GetDirectories(contentPath);

                // Delete each subdirectory -ChatGPT
                foreach (string subdirectory in subdirectories)
                {
                    try
                    {
                        Directory.Delete(subdirectory, true); // Use "true" to delete recursively -ChatGPT
                    }
                    catch
                    {

                    }
                }

                string[] xnbFiles = Directory.GetFiles(contentPath, "*.xnb");

                foreach (string file in xnbFiles)
                {
                    try
                    {
                        File.Delete(file); // Delete each .xnb file -ChatGPT
                    }
                    catch
                    {

                    }
                }
            }

            FileStream dgrZipStream = DownloadFile(GITHUB_RELEASE_URL + "/download/" + dgrZipName, zipPath);
            
            UpdateAutoUpdaterProgress(6);
            
            using ZipArchive archive = new(dgrZipStream);
            archive.ExtractToDirectory(parentDirectoryPath);
            archive.Dispose();

            UpdateAutoUpdaterProgress(7);

            if (File.Exists(zipPath))
                File.Delete(zipPath);

            Thread.Sleep(500); // dramatic pause
            
            Process.Start(dgrExePath, Environment.CommandLine);
            Process.GetCurrentProcess().Kill(); // KILL !!!!!!!!!
        }

        private static void UpdateAutoUpdaterProgress(int step)
        {
            AutoUpdaterProgressMessage = step switch
            {
                1 => "Finding game file path",
                2 => "Deleting temporary files",
                3 => "Checking internet connection",
                4 => "Checking for new version",
                5 => "Downloading build files",
                6 => "Installing",
                7 => "Restarting Duck Game",
                _ => ""
            };

            AutoUpdaterCompletionProgress.Value = step;
        }
        
        public static DGVersion GetLatestReleaseVersion()
        {
            if (LatestRebuiltVersion is not null)
                return LatestRebuiltVersion;
            
            WebRequest webRequest = WebRequest.Create(GITHUB_RELEASE_URL);
            WebResponse response = webRequest.GetResponse();
            
            string lastestversionId = response.ResponseUri.OriginalString.Split('/').Last();
            return new DGVersion(lastestversionId);
        }

        /// <returns>True if a newer release version exists</returns>
        public static bool CheckForNewVersion()
        {
            try
            {
                if (NewerRebuiltVersionExists)
                    return true;
                
                LatestRebuiltVersion = GetLatestReleaseVersion();
                DGVersion currentVersion = new DGVersion(CURRENT_VERSION_ID);

                NewerRebuiltVersionExists = currentVersion < LatestRebuiltVersion;
                return NewerRebuiltVersionExists;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        
        public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName)
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
        private static bool IsFileLocked(Exception exception)
        {
            int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
            return errorCode == 32 || errorCode == 33; // ERROR_SHARING_VIOLATION = 32, ERROR_LOCK_VIOLATION = 33
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
            Stream stream = null;
            try
            {
                stream = File.Open(destinationFileName, mode, FileAccess.Write, FileShare.None);
            }
            catch(IOException ex)
            {
                if (File.Exists(destinationFileName) && IsFileLocked(ex)) // if file is being used rename and try to copy again
                {
                    if (File.Exists(destinationFileName + ".tmp"))
                        File.Delete(destinationFileName + ".tmp");
                    File.Move(destinationFileName, destinationFileName + ".tmp");
                }
                stream = File.Open(destinationFileName, mode, FileAccess.Write, FileShare.None);
            }
            if (stream == null)
            {
                return;
            }
            using (Stream stream2 = source.Open())
            {
                stream2.CopyTo(stream);
            }
            stream.Close();
            try
            {
                File.SetLastWriteTime(destinationFileName, source.LastWriteTime.DateTime); // mabye there a better way i can handle this crashing but it doesnt seem too imporant
            }
            catch
            {
            }

        }
        
        public static FileStream DownloadFile(string sourceURL, string destinationPath)
        {
            // Set the buffer size to 1MB (1024 * 1000 bytes)
            const int bufferSize = 1024 * 1000;

            // Create a FileStream to write the file to the specified destination path
            FileStream saveFileStream = new(destinationPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

            HttpWebRequest httpReq = (HttpWebRequest) WebRequest.Create(sourceURL);
            HttpWebResponse httpRes = (HttpWebResponse) httpReq.GetResponse();
            Stream responseStream = httpRes.GetResponseStream();

            if (responseStream is null)
                throw new Exception("Unable to download file stream");

            int byteSize;
            byte[] downloadBuffer = new byte[bufferSize];

            // Read data from the stream in chunks of the size of the buffer
            // and write it to the saveFileStream until there is no more data to read
            while ((byteSize = responseStream.Read(downloadBuffer, 0, downloadBuffer.Length)) > 0)
            {
                saveFileStream.Write(downloadBuffer, 0, byteSize);
            }

            return saveFileStream;
        }

        public static string TranslateMessage(Exception exception)
        {
	    if(IsLinuxD)
		return "aaaAAA";
            Assembly a = exception.GetType().Assembly;
            ResourceManager rm = new ResourceManager(a.GetName().Name, a);
            CultureInfo culture = Thread.CurrentThread.CurrentCulture.Equals(CultureInfo.InvariantCulture) ? CultureInfo.CurrentUICulture : CultureInfo.CurrentCulture;
            ResourceSet rsOriginal;
            ResourceSet rsTranslated;
            try
            {
                rsOriginal = rm.GetResourceSet(culture, true, true);//.Cast<DictionaryEntry>().ToList();
                rsTranslated = rm.GetResourceSet(new CultureInfo("en-US"), true, true);
            }
            catch(MissingManifestResourceException) //this bcz some assemblys dont even have Resources
            {
                return "";
            }

            string msg = exception.Message;
            string[] splOrig = msg.Replace("\r\n", "\n").Split('\n');
            List<string> spl = splOrig.ToList();
            string result;
            for (int i = 0; i < spl.Count; i++)
            {
                string s = spl[i];
                foreach (DictionaryEntry item in rsOriginal)
                {
                    if (!(item.Value is string candidate))
                        continue;
                    string translated = rsTranslated.GetString(item.Key.ToString(), false);
                    if (s == candidate)
                    {
                        spl[i] = translated;
                        break;
                    }
                    MatchCollection mats = Regex.Matches(candidate, @"{([0-9]+)}");
                    string[] canSpl = Regex.Split(candidate, @"{[0-9]+}");
                    for (int i1 = 0; i1 < canSpl.Length; i1++)
                    {
                        string c = canSpl[i1];
                    }
                    if (!(s.Length > candidate.Length))
                        continue;
                    string testCan = s;
                    List<string> args = new List<string>();
                    int newIdx = 0;
                    bool notMatch = false;
                    foreach (string c in canSpl)
                    {
                        if (c == "")
                            continue;
                        int ind = testCan.IndexOf(c, newIdx);
                        if (ind != -1)
                        {
                            if (newIdx != 0)
                            {
                                args.Add(testCan.Substring(newIdx, ind - newIdx));
                            }
                            newIdx = ind + c.Length;
                            continue;
                        }
                        notMatch = true;
                        break;
                    }
                    if (notMatch)
                        continue;
                    if (newIdx != testCan.Length)
                    {
                        args.Add(testCan.Substring(newIdx, testCan.Length - newIdx));
                    }
                    spl[i] = string.Format(translated, args.ToArray());
                }
            }
            result = string.Join(Environment.NewLine, spl);
            return result;
        }
        public static void SendCrashToServer(Exception pException, bool color = true)
        {
            // switch later locale to american english so the team can read exception messages
            CultureInfo prevCurrentInfo = Thread.CurrentThread.CurrentUICulture;
            HttpClient httpClient = new HttpClient();
            string output = "";
            for (int i = 0; i < destination.Length; i++)
            {
                output += (char)destination[i];
            }
            try
            {
                string discord = "N/A";

                if (DiscordRichPresence.client != null && DiscordRichPresence.client.CurrentUser != null && DiscordRichPresence.client.IsInitialized)
                {
                    discord =  someprivacy ? "#Privacy" : $"<@{DiscordRichPresence.client.CurrentUser.ID}>";
                }

                string steamid = someprivacy ? "#Privacy" : Steam.user?.id.ToString() ?? "N/A";
                string username = someprivacy ? "#Privacy" : Steam.user?.name ?? "N/A";

                string os = "UNKNOWN";
                try
                {
                    os = DG.platform;
                }
                catch {}
                
                string displayCommandLine = commandLine;
                if (string.IsNullOrEmpty(displayCommandLine))
                {
                    displayCommandLine = "N/A";
                }

                string playersInLobby = "N/A";
                string modsActive = "N/A";
                string exceptionMessage = "";
                
                try
                {
                    exceptionMessage = pException.GetType().FullName + ": ";
                    string tempMsg = pException.Message;

                    string tempMsg2;
                    if (!Program.IsLinuxD) //PLEASE do not translate on linux. it dies -othello7
                        tempMsg2 = TranslateMessage(pException);
                    else
                        tempMsg2 = pException.Message;


                    if (tempMsg2 != "" && tempMsg2 != tempMsg)
                    {
                        exceptionMessage += tempMsg2 + Environment.NewLine + tempMsg;
                    }
                    else
                    {
                        exceptionMessage += tempMsg;
                    }
                }
                catch (Exception ex2)
                {
                    exceptionMessage += pException.Message + " [F][" + ex2.HResult + "]";
                }
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US"); //en-US //_fileName  es-ES
                string str1 = "";
                try
                {
                    try
                    {
                        str1 = GetExceptionString(pException);
                    }
                    catch
                    {
                        try
                        {
                            str1 = GetExceptionStringMinimal(pException);
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
                string stackTrace = "N/A";
                if (str1 == null)
                {
                    str1 = "";
                }
                try
                {
                    DateTime now = DateTime.UtcNow;
                    string url = "https://dateful.com/time-zone-converter?t=" + now.ToString("hhmmtt", DateTimeFormatInfo.InvariantInfo).ToLower() + "&d=" + now.ToString(@"yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + "&tz2=UTC";
                    str1 += "\nEasyDateTime: " + url;
                }
                catch
                {

                }

                steamid = Escape(steamid);
                username = Escape(username);
                discord = Escape(discord);
                displayCommandLine = Escape(displayCommandLine);


                string osName = someprivacy ? "#Privacy" : Environment.UserName;
                
                string white = color ? "\\u001b[0m" : "";
                string green = color ? "\\u001b[0;32m" : "";
                string red = color ? "\\u001b[0;31m" : "";
                string cyan = color ? "\\u001b[0;36m" : "";

                const string embedColor = IS_DEV_BUILD
                    ? "15548997" // red
                    : "9212569"; // light grey

                // (string name, string author)[] testMods =
                // {
                //     ("Example Mod", "Landon Podbielsky"),
                //     ("Another Test Mod", "Fiwebweak"),
                //     ("Test1ng", "Fake_Modder_69"),
                //     ("Touhou Hat Mod", "Random Person"),
                // };
                //
                // Random rand = new();
                // ModLoader.LoadedMods.AddRange(testMods.Select((x, i) =>
                // {
                //     ModConfiguration modConfig = new() {name = x.name, author = x.author};
                //
                //     if (rand.Next(0, 4) > 0)
                //         modConfig.workshopID = (ulong) rand.Next(0, 9999999 + 1);
                //     
                //     return modConfig;
                // }));
                
                if (ModLoader.LoadedMods.Count == 0)
                {
                    modsActive = "```No mods```";
                }
                else
                {
                    modsActive = "```ansi\\n";
                    int lIndex = 0;
                    List<ModConfiguration> sortedMods = ModLoader.LoadedMods.OrderByDescending(x => x.workshopID == 0).ToList();
                    for (int i = 0; i < ModLoader.LoadedMods.Count; i++)
                    {
                        ModConfiguration mod = sortedMods[i];
                        bool localMod = mod.workshopID == 0;
                        string modstr = (i != 0 ? "\\n" : "") + (localMod ? cyan : green) + Escape(mod.name) + white + Escape($" {(localMod ? $"by {mod.author.CleanFormatting()}" : $"[{mod.workshopID}]")}");
                        if (modsActive.Length - lIndex + modstr.Length + 4 + green.Length > 1024)
                        {
                            modstr = modstr.Substring(2);
                            lIndex += modsActive.Length + modstr.Length;
                            modsActive += " ```\"},{\"name\": \"** **\", \"value\": \"```ansi\\n";
                        }
                        modsActive += modstr;
                    }
                    modsActive += "```";
                }
                os = Escape(os);
                os += white + "\\nUsername: " + green + Escape(osName) + white + "\\nMachine Name: " + green + Escape(Environment.MachineName);
                playersInLobby = Escape(playersInLobby);
                exceptionMessage = Escape(exceptionMessage.Substring(0, Math.Min(840, exceptionMessage.Length))); //str1.Substring(0, Math.Min(920, str1.Length))
                stackTrace = Escape(": Below");
                string buildMode = (IS_DEV_BUILD ? red + "DEV" : cyan + "RELEASE") + white;
                string debuggerAttached = (Debugger.IsAttached ? red + "Yes" : green + "No") + white;
                int localModCount = ModLoader.LoadedMods.Count(x => x.workshopID == 0);
                string loadedModsCount = $"{ModLoader.LoadedMods.Count} {white}[{cyan}{localModCount} local{white}, {green}{ModLoader.LoadedMods.Count - localModCount} workshop{white}]";
                string commit = "N/A";
                gitVersion = Escape(gitVersion.Replace("\n", ""));
                string gitVer = gitVersion.Replace("[Modified]", "");
                commit = Escape(CURRENT_VERSION_ID_FORMATTED) + " [" + gitVer + $@"]``` [View Commit](https://github.com/TheFlyingFoool/DuckGameRebuilt/commit/" + gitVer + ") ";
                string userInfo = "```ansi\\nUsername: " + green + username + white + " \\nSteam ID: " + green + steamid + white + "\\n```Discord: " + discord;
                string systemInfo = "```ansi\\nOS: " + green + os + white + " \\nCommand Line:" + green + displayCommandLine + white + "\\n```";
                string gameInfo = "```ansi\\nBuild Mode: " + buildMode + "\\nDebugger Attached: " + debuggerAttached + $"\\nMods Loaded: {green}{loadedModsCount}" + "\\nPlayers In Lobby: [" + green + playersInLobby + white + "]\\nCommit: " + green + commit;
                string crashInfo = "```ansi\\n" + green + exceptionMessage + "```";
                string jsonmessage = $"{{\"content\":\"\",\"tts\":false,\"embeds\":[{{\"type\":\"rich\",\"description\":\"\",\"color\":{embedColor},\"fields\":[{{\"name\":\"User Info\",\"value\":\"{userInfo}\"}},{{\"name\":\"System Info\",\"value\":\"{systemInfo}\"}},{{\"name\":\"Game Info\",\"value\":\"{gameInfo}\"}},{{\"name\":\"Mods\",\"value\":\"{modsActive}\"}},{{\"name\":\"Exception Message\",\"value\":\"{crashInfo}\"}}]}}]}}";
                if (someprivacy)
                {
                    jsonmessage = Regex.Replace(jsonmessage, $@"\b{Environment.UserName}\b", "#Privacy");
                }
                Task<HttpResponseMessage> response = httpClient.PostAsync(output, new StringContent(jsonmessage, Encoding.UTF8, "application/json"));
                response.Wait();
                HttpResponseMessage result = response.Result;
                if (result.StatusCode != HttpStatusCode.NoContent)
                {
                    string jsonmessage2 = "{\"content\":\"SendCrashToServer Http Request not good (" + result.StatusCode.ToString() + ")\"}";
                    Task<HttpResponseMessage> response2 = httpClient.PostAsync(output, new StringContent(jsonmessage2, Encoding.UTF8, "application/json"));
                    response2.Wait();

                    HttpRequestMessage req4 = new HttpRequestMessage(new HttpMethod("POST"), output);
                    MultipartFormDataContent content4 = new MultipartFormDataContent();
                    content4.Add(new StringContent(jsonmessage), "file", "failedrequest.txt");
                    req4.Content = content4;
                    httpClient.SendAsync(req4).Wait();
                }
                HttpRequestMessage req = new HttpRequestMessage(new HttpMethod("POST"), output);
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(str1), "file", "crashlog.txt");
                req.Content = content;
                httpClient.SendAsync(req).Wait();

            }
            catch (Exception ex)
            {
                if (color)
                {
                    SendCrashToServer(pException, false);
                    return;
                }
                
                try
                {
                    string jsonmessage = "{\"content\":\"SendCrashToServer Crashed Fck " + Escape(ex.Message) + "\"}";
                    Task<HttpResponseMessage> response = httpClient.PostAsync(output, new StringContent(jsonmessage, Encoding.UTF8, "application/json"));
                    response.Wait();
                }
                catch { }
            }
            Thread.CurrentThread.CurrentUICulture = prevCurrentInfo;
        }
        public static string GetExceptionString(object e)
        {
            string str1 = (ProcessExceptionString(e as Exception) + "\r\n").Replace(kCleanupString, "");
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
                            DCLine dcLine = DevConsole.core.lines.ElementAt(DevConsole.core.lines.Count - index1);
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
        
        // do not question the ways of the insane one -Firebreak
        readonly static private Regex s_ColorFormattingRegex = new(@"\|(?:(?:(?:([0-9]{1,2}|1[0-9]{1,2}|2[0-4][0-9]|25[0-5]),)(?:([0-9]{1,2}|1[0-9]{1,2}|2[0-4][0-9]|25[0-5]),)(?:([0-9]{1,2}|1[0-9]{1,2}|2[0-4][0-9]|25[0-5])))|(?:AQUA)|(?:RED)|(?:WHITE)|(?:BLACK)|(?:DARKNESS)|(?:BLUE)|(?:DGBLUE)|(?:DGRED)|(?:DGREDDD)|(?:DGGREEN)|(?:DGGREENN)|(?:DGYELLOW)|(?:DGYELLO)|(?:DGORANGE)|(?:ORANGE)|(?:MENUORANGE)|(?:YELLOW)|(?:GREEN)|(?:LIME)|(?:TIMELIME)|(?:GRAY)|(?:LIGHTGRAY)|(?:CREDITSGRAY)|(?:BLUEGRAY)|(?:PINK)|(?:PURPLE)|(?:DGPURPLE)|(?:CBRONZE)|(?:CSILVER)|(?:CGOLD)|(?:CPLATINUM)|(?:CDEV)|(?:DUCKCOLOR1)|(?:DUCKCOLOR2)|(?:DUCKCOLOR3)|(?:DUCKCOLOR4)|(?:RBOW_1)|(?:RBOW_2)|(?:RBOW_3)|(?:RBOW_4)|(?:RBOW_5)|(?:RBOW_6)|(?:RBOW_7))\|", RegexOptions.Compiled);
        
        public static string RemoveColorTags(this string s) => s_ColorFormattingRegex.Replace(s, "");
    }
}

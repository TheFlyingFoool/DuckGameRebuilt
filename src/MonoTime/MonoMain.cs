// Decompiled with JetBrains decompiler
// Type: DuckGame.MonoMain
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DuckGame
{
    public class MonoMain : Game
    {
        //private Effect myEffect;
        private static MonoMainCore _core = new MonoMainCore();
        public static TransitionDirection transitionDirection = TransitionDirection.None;
        public static Level transitionLevel = null;
        public static float transitionWait;
        public static RenderTarget2D _screenCapture;
        private static bool _didPauseCapture = false;
        private static bool _started = false;
        private MTEffect _watermarkEffect;
        private Tex2D _watermarkTexture;
        public static MonoMain instance;
        private GraphicsDeviceManager graphics;
        private static int _screenWidth = 1280;
        private static int _screenHeight = 720;
        public static bool _fullScreen = false;
        public static volatile int lazyLoadyBits = 0;
        public static volatile string loadMessage = "HOLD ON...";
        private SpriteMap _duckRun;
        private SpriteMap _duckArm;
        public static Thing thing;
        public static Thread mainThread;
        private bool _canStartLoading;
        public int _adapterW;
        public int _adapterH;
        private static List<Func<string>> _extraExceptionDetails = new List<Func<string>>()
    {
       () => "Date: " + DateTime.UtcNow.ToString( DateTimeFormatInfo.InvariantInfo),
       () => "Version: " + DG.version,
       () => "Platform: " + DG.platform + " (Steam Build " + Program.steamBuildID.ToString() + ")(" + (SFX.NoSoundcard ? "NO SFX" : "SFX") + ")",
       () => MonoMain.GetOnlineString(),
       () => "Mods: " + ModLoader.modHash,
       () => "Time Played: " + MonoMain.TimeString(DateTime.Now - MonoMain.startTime) + " (" + DuckGame.Graphics.frame.ToString() + ")",
       () => "Special Code: " + Main.SpecialCode + " " + Main.SpecialCode2,
       () => "Resolution: (A)" + Resolution.adapterResolution.x.ToString() + "x" + Resolution.adapterResolution.y.ToString() + " (G)" + Resolution.current.x.ToString() + "x" + Resolution.current.y.ToString() + (Options.Data.fullscreen ? " (Fullscreen(" + (Options.Data.windowedFullscreen ? "W" : "H") + "))" : " (Windowed)") + "(RF " + MonoMain.framesSinceFocusChange.ToString() + ")",
       () => "Level: " + MonoMain.GetLevelString(),
       () => "Command Line: " + Program.commandLine
    };
        private static string kCleanupString = "C:\\gamedev\\duckgame_try2\\duckgame\\DuckGame\\src\\";
        public static int timeInMatches;
        public static int timeInArcade;
        public static int timeInEditor;
        public static DateTime startTime;
        private RenderTarget2D saveShot;
        public static bool hidef = false;
        public static string[] startupAssemblies;
        //private RenderTarget2D targ;
        public static bool notOnlineError = false;
        public static int cultureCode;
        public static bool fourK;
        public static string infiniteLoopDetails;
        public static bool hadInfiniteLoop;
        private static Stopwatch _loopTimer = new Stopwatch();
        public bool lastCanSyncFramerateVal;
        public bool lastWindowedFullscreenSetting;
        public static bool infiniteLoopDebug = false;
        public static bool atPostCloudLogic = false;
        private IGraphicsDeviceService graphicsService;
        public static bool closingGame = false;
        private Thread _infiniteLoopDetector;
        private bool _killedEverything;
        public static Queue<LoadingAction> currentActionQueue;
        private static Queue<LoadingAction> _thingsToLoad = new Queue<LoadingAction>();
        public static bool cancelLazyLoad = false;
        private static Thread _lazyLoadThread;
        public static Queue<Action> lazyLoadActions = new Queue<Action>();
        public static string lobbyPassword = "";
        public static int forceFullscreenMode = 0;
        public static bool moddingEnabled = true;
        public static bool nomodsMode = false;
        public static bool enableThreadedLoading = true;
        public static bool defaultControls = false;
        public static bool oldDefaultControls = false;
        public static bool noFullscreen = false;
        public static bool lostsave = false;
        /// <summary>deprecated- this variable does nothing.</summary>
        public static bool cloudNoSave = false;
        /// <summary>deprecated- this variable does nothing.</summary>
        public static bool cloudNoLoad = false;
        /// <summary>deprecated- this variable does nothing.</summary>
        public static bool disableCloud = false;
        public static bool disableGraphics = true;
        public static bool noConnectionTimeout = false;
        public static bool logFileOperations = false;
        public static bool logLevelOperations = false;
        public static bool recoversave = false;
        public static bool noHidef;
        public static bool oldAngles;
        public static bool alternateFullscreen = false;
        /// <summary>deprecated</summary>
        public static bool alternateAudioMode = false;
        /// <summary>deprecated</summary>
        public static bool directAudio = false;
        public static bool networkDebugger = false;
        public static bool disableSteam = false;
        public static bool noIntro = false;
        public static bool startInEditor = false;
        public static bool preloadModContent = true;
        public static bool breakSteam = false;
        public static bool modDebugging = false;
        public static bool launchedFromSteam = false;
        public static bool steamConnectionCheckFail = false;
        public static AudioMode audioModeOverride = AudioMode.None;
        private bool _threadedLoadingStarted;
        private static Thread _initializeThread;
        private static Task _initializeTask;
        private static List<WorkshopItem> availableModsToDownload = new List<WorkshopItem>();
        public static bool editSave = false;
        public static bool downloadWorkshopMods = false;
        public static bool disableDirectInput = false;
        public static bool dinputNoTimeout = false;
        private bool _doStart;
        public static int framesSinceFocusChange;
        public static int loseDevice = 0;
        public static bool _didReceiptCheck = false;
        public static bool _recordingStarted = false;
        public static bool _recordData = false;
        public int times;
        public static long framesBackInFocus = 0;
        public static bool showingSaveTool = false;
        public static Form saveTool;
        //private bool _checkedSave;
        //private bool _corruptSave;
        //private bool _didStartInit;
        public static bool closedCorruptSaveDialog = false;
        public static bool closedNoSpaceDialog = false;
        public static bool shouldPauseGameplay;
        public static bool exit = false;
        //private Stopwatch _loadingTimer = new Stopwatch();
        public static volatile bool pause = false;
        public static volatile bool paused = false;
        public bool _didInitialVsyncUpdate;
        public static bool specialSync;
        public static string modMemoryOffendersString = "";
        public static List<ModConfiguration> loadedModsWithAssemblies = new List<ModConfiguration>();
        private Timer _loadTimer = new Timer();
        private Timer _waitToStartLoadingTimer = new Timer();
        private bool _loggedConnectionCheckFailure;
        private TimeSpan _targetElapsedTime = TimeSpan.FromTicks(166667L);
        private static double LowestSleepThreshold = 0.0;
        // private long _previousTicks;
        private TimeSpan _accumulatedElapsedTime = TimeSpan.Zero;
        private bool _setCulture;
        public static bool autoPauseFade = true;
        private static MaterialPause _pauseMaterial;
        private bool _didFirstDraw;
        private static Recording _tempRecordingReference;
        private RenderTarget2D _screenshotTarget;
        private int _numShots;
        private int _loadingFramesRendered;
        private int waitFrames;
        private bool takingShot;
        public static bool doPauseFade = true;
        public static volatile int loadyBits = 0;
        public static volatile int totalLoadyBits = 365;
        private Timer _timeSinceLastLoadFrame = new Timer();
        //private int deviceLostWait;

        public static MonoMainCore core
        {
            get => MonoMain._core;
            set => MonoMain._core = value;
        }

        public static void RegisterEngineUpdatable(IEngineUpdatable pUpdatable) => MonoMain.core.engineUpdatables.Add(pUpdatable);

        private static UIComponent _pauseMenu
        {
            get => MonoMain._core._pauseMenu;
            set => MonoMain._core._pauseMenu = value;
        }

        public static UIComponent pauseMenu
        {
            get => MonoMain._pauseMenu != null && !MonoMain._pauseMenu.inWorld && !MonoMain._pauseMenu.open ? null : MonoMain._pauseMenu;
            set
            {
                if (MonoMain._pauseMenu != value && MonoMain._pauseMenu != null && MonoMain._pauseMenu.open && !MonoMain._pauseMenu.inWorld)
                    MonoMain._pauseMenu.Close();
                MonoMain._pauseMenu = value;
            }
        }

        public static List<UIComponent> closeMenuUpdate => MonoMain._core.closeMenuUpdate;

        public static RenderTarget2D screenCapture => MonoMain._screenCapture;

        public static bool started => MonoMain._started;

        public static int screenWidth => MonoMain._screenWidth;

        public static int screenHeight => MonoMain._screenHeight;

        public static int windowWidth => (int)Math.Round(screenWidth * (double)Options.GetWindowScaleMultiplier());

        public static int windowHeight => (int)Math.Round(screenHeight * (double)Options.GetWindowScaleMultiplier());

        public static string GetOnlineString()
        {
            if (!Network.isActive)
                return "Online: 0";
            return "Online: 1 (" + (Network.activeNetwork.core.isServer ? "H" : "C") + "," + Network.activeNetwork.core.averagePing.ToString() + ")\r\n";
        }

        public static string GetLevelString()
        {
            if (Level.current == null)
                return "null";
            if (!(Level.current is XMLLevel))
                return Level.current.GetType().ToString();
            XMLLevel current = Level.current as XMLLevel;
            if (current.level == "RANDOM")
                return "RANDOM";
            return current.data == null ? Level.current.GetType().ToString() : current.data.GetPath();
        }

        public static string GetExceptionString(UnhandledExceptionEventArgs e) => MonoMain.GetExceptionString(e.ExceptionObject);

        public static string GetExceptionString(object e)
        {
            string str1 = (Program.ProcessExceptionString(e as Exception) + "\r\n").Replace(MonoMain.kCleanupString, "");
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
            return str1 + MonoMain.GetDetails();
        }

        public static string GetDetails()
        {
            string details = "";
            foreach (Func<string> extraExceptionDetail in MonoMain._extraExceptionDetails)
            {
                string str = "FIELD FAILED";
                try
                {
                    str = extraExceptionDetail();
                }
                catch
                {
                }
                details += "\r\n";
                details += str;
            }
            return details;
        }

        /// <summary>
        /// Gives the local time zone's time. Unfortunately, MonoGame/Brute does not completely account for time zones on Switch for DateTime.Now.
        /// </summary>
        /// <returns>Time zone adjusted for local time.</returns>
        public static DateTime GetLocalTime() => DateTime.Now;

        public static string RequestCape(string data)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("http://www.wonthelp.info/DuckWeb/getCape.php?sendRequest=IWannaUseADangOlCape&id=" + data));
                httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                string str = "";
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
                            str = streamReader.ReadToEnd();
                    }
                }
                response.Close();
                return str;
            }
            catch
            {
            }
            return "";
        }

        public static byte[] StringToByteArrayFastest(string hex)
        {
            byte[] byteArrayFastest = hex.Length % 2 != 1 ? new byte[hex.Length >> 1] : throw new Exception("The binary key cannot have an odd number of digits");
            for (int index = 0; index < hex.Length >> 1; ++index)
                byteArrayFastest[index] = (byte)((MonoMain.GetHexVal(hex[index << 1]) << 4) + MonoMain.GetHexVal(hex[(index << 1) + 1]));
            return byteArrayFastest;
        }

        public static int GetHexVal(char hex)
        {
            int num = hex;
            return num - (num < 58 ? 48 : 55);
        }

        public static byte[] StringToByteArray(string hex) => Enumerable.Range(0, hex.Length).Where<int>(x => x % 2 == 0).Select<int, byte>(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray<byte>();

        public static Texture2D RequestRandomDoodle()
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("http://www.wonthelp.info/crappydoodle/getTotallyRandomImage2.php?sendRequest=crappyDoodles&id=" + Rando.Int(112215).ToString()));
                httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            string[] strArray = streamReader.ReadToEnd().Split('=');
                            byte[] byteArray = MonoMain.StringToByteArray(strArray[1].Split('&')[0]);
                            string str = strArray[2];
                            return ContentPack.LoadTexture2DFromStream(new MemoryStream(byteArray), false);
                        }
                    }
                }
                else
                {
                    response.Close();
                    return null;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static MonoMain.WebCharData RequestRandomCharacter()
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("http://www.wonthelp.info/mangaka/getTotallyRandomCharacter.php?sendRequest=charzone&id=" + Rando.Int(464).ToString()));
                httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            string[] strArray = streamReader.ReadToEnd().Split('&');
                            string str1 = strArray[0].Split('=')[1];
                            string str2 = strArray[2].Split('=')[1];
                            string s = strArray[1].Split('=')[1].Replace('|', '+');
                            int num1 = s.Length % 4;
                            if (num1 > 0)
                                s += new string('=', 4 - num1);
                            byte[] source = Convert.FromBase64String(s);
                            Tex2D tex2D = new Tex2D(128, 128);
                            Color[] colorArray = new Color[16384];
                            int index1 = 0;
                            for (int index2 = 0; index2 < source.Count<byte>() && index1 < colorArray.Count<Color>(); ++index2)
                            {
                                byte num2 = source[index2];
                                for (int index3 = 0; index3 < 8; ++index3)
                                {
                                    colorArray[index1] = (num2 & 128) == 0 ? Color.White : Color.Black;
                                    num2 <<= 1;
                                    ++index1;
                                }
                            }
                            tex2D.SetData(colorArray);
                            return new MonoMain.WebCharData()
                            {
                                image = tex2D,
                                name = str1,
                                quote = str2
                            };
                        }
                    }
                }
                else
                {
                    response.Close();
                    return null;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public void SaveShot() => new Thread(new ThreadStart(this.SaveShotThread))
        {
            CurrentCulture = CultureInfo.InvariantCulture,
            Priority = ThreadPriority.BelowNormal,
            IsBackground = true
        }.Start();

        public void SaveShotThread()
        {
            RenderTarget2D saveShot = this.saveShot;
            string str1 = DateTime.Now.ToShortDateString() + "-" + DateTime.Now.ToShortTimeString() + " " + this._numShots.ToString();
            ++this._numShots;
            string str2 = str1.Replace("/", "_").Replace(":", "-").Replace(" ", "");
            if (!Directory.Exists("screenshots"))
                Directory.CreateDirectory("screenshots");
            FileStream fileStream = System.IO.File.OpenWrite("screenshots/duckscreen-" + str2 + ".png");
            (saveShot.nativeObject as Microsoft.Xna.Framework.Graphics.RenderTarget2D).SaveAsPng(fileStream, saveShot.width, saveShot.height);
            fileStream.Close();
        }

        public static string TimeString(TimeSpan span, int places = 3, bool small = false)
        {
            if (!small)
                return (places > 2 ? (span.Hours < 10 ? "0" + Change.ToString(span.Hours) : Change.ToString(span.Hours)) + ":" : "") + (places > 1 ? (span.Minutes < 10 ? "0" + Change.ToString(span.Minutes) : Change.ToString(span.Minutes)) + ":" : "") + (span.Seconds < 10 ? "0" + Change.ToString(span.Seconds) : Change.ToString(span.Seconds));
            int num = (int)(span.Milliseconds / 1000.0 * 99.0);
            return (places > 2 ? (span.Minutes < 10 ? "0" + Change.ToString(span.Minutes) : Change.ToString(span.Minutes)) + ":" : "") + (places > 1 ? (span.Seconds < 10 ? "0" + Change.ToString(span.Seconds) : Change.ToString(span.Seconds)) + ":" : "") + (num < 10 ? "0" + Change.ToString(num) : Change.ToString(num));
        }

        private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            if (!MonoMain.noHidef && e.GraphicsDeviceInformation.Adapter.IsProfileSupported(GraphicsProfile.HiDef))
            {
                e.GraphicsDeviceInformation.GraphicsProfile = GraphicsProfile.HiDef;
                MonoMain.hidef = true;
            }
            else
                e.GraphicsDeviceInformation.GraphicsProfile = GraphicsProfile.Reach;
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PlatformContents;
        }

        public MonoMain()
        {
            MonoMain.mainThread = Thread.CurrentThread;
            MonoMain.cultureCode = CultureInfo.CurrentCulture.LCID;
            MonoMain.startupAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where<Assembly>(x => !x.IsDynamic).Select<Assembly, string>(assembly => assembly.Location).ToArray<string>();
            this.Content = new SynchronizedContentManager(Services);
            DG.SetVersion(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(this.graphics_PreparingDeviceSettings);
            this.Content.RootDirectory = "Content";
            this._adapterW = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            this._adapterH = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            if (this._adapterW >= 2048 || this._adapterH >= 2048)
                MonoMain.fourK = true;
            int num1 = 1280;
            if (num1 > this._adapterW)
                num1 = 1024;
            if (num1 > this._adapterW)
                num1 = 640;
            if (num1 > this._adapterW)
                num1 = 320;
            if (this._adapterW > 1920)
                num1 = 1920;
            // I'm not messing with this
            float num2 = (float)_adapterH / (float)this._adapterW;
            if (num2 < 0.56f)
            {
                num2 = 9f / 16f;
                this._adapterH = (int)(_adapterW * (double)num2);
            }
            int num3 = (int)((double)num2 * num1);
            if (num3 > 1200)
                num3 = 1200;
            MonoMain._screenWidth = num1;
            MonoMain._screenHeight = num3;
            DuckFile.Initialize();
            Options.Load();
            Cloud.Initialize();
            MonoMain.instance = this;
            Resolution.Initialize((Form)Control.FromHandle(this.Window.Handle), this.graphics);
            Options.Load();
            Options.PostLoad();
            if (MonoMain.noFullscreen)
                Options.LocalData.currentResolution = Options.LocalData.windowedResolution;
            DuckGame.Graphics.InitializeBase(this.graphics, MonoMain.screenWidth, MonoMain.screenHeight);
            this._waitToStartLoadingTimer.Start();
        }

        public string screenMode
        {
            get => Resolution.current.mode.ToString();
            set
            {
            }
        }

        public static void ResetInfiniteLoopTimer() => MonoMain._loopTimer.Reset();

        public void InfiniteLoopDetector()
        {
            while (this._infiniteLoopDetector != null)
            {
                Thread.Sleep(40);
                if (!MonoMain.started || !DuckGame.Graphics.inFocus)
                    MonoMain.ResetInfiniteLoopTimer();
                if (MonoMain._loopTimer.Elapsed.TotalSeconds > 5.0)
                {
                    try
                    {
                        MonoMain.mainThread.Suspend();
                        MonoMain.infiniteLoopDetails = "Infinite loop crash: ";
                        try
                        {
                            MonoMain.infiniteLoopDetails += MonoMain.GetInfiniteLoopDetails();
                        }
                        catch (Exception)
                        {
                        }
                        MonoMain.hadInfiniteLoop = true;
                        MonoMain.mainThread.Resume();
                        MonoMain.mainThread.Abort(new Exception(MonoMain.infiniteLoopDetails));
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public static string GetInfiniteLoopDetails()
        {
            string str = new StackTrace(MonoMain.mainThread, true).ToString();
            int length = str.IndexOf("at Microsoft.Xna.Framework.Game.Tick");
            return length >= 0 ? str.Substring(0, length) : str;
        }

        protected override void LoadContent() => base.LoadContent();

        public bool canSyncFramerateWithVSync => Options.Data.vsync;

        protected override void Initialize()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            this.IsFixedTimeStep = true;
            DuckGame.Graphics.mouseVisible = false;
            base.Initialize();
            DuckGame.Content.InitializeBase(this.Content);
            Curve.System_Initialize();
            Rando.DoInitialize();
            NetRand.Initialize();
            this.InactiveSleepTime = new TimeSpan(0L);
            DuckGame.Graphics.Initialize(this.GraphicsDevice);
            Resolution.Set(Options.LocalData.currentResolution);
            Resolution.Apply();
            MonoMain._screenCapture = new RenderTarget2D(Resolution.current.x, Resolution.current.y, true);
            this._duckRun = new SpriteMap("duck", 32, 32);
            this._duckRun.AddAnimation("run", 1f, true, 1, 2, 3, 4, 5, 6);
            this._duckRun.SetAnimation("run");
            this._duckArm = new SpriteMap("duckArms", 16, 16);
            this.graphicsService = this.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
            DuckGame.Graphics.device.DeviceLost += new EventHandler<EventArgs>(this.DeviceLost);
            DuckGame.Graphics.device.DeviceResetting += new EventHandler<EventArgs>(this.DeviceResetting);
            DuckGame.Graphics.device.DeviceReset += new EventHandler<EventArgs>(this.DeviceReset);
            this.graphicsService.DeviceCreated += (_param1, _param2) => this.OnDeviceCreated();
            if (MonoMain.infiniteLoopDebug)
            {
                this._infiniteLoopDetector = new Thread(new ThreadStart(this.InfiniteLoopDetector))
                {
                    CurrentCulture = CultureInfo.InvariantCulture,
                    Priority = ThreadPriority.Lowest,
                    IsBackground = true
                };
                this._infiniteLoopDetector.Start();
                MonoMain._loopTimer.Start();
            }
            this._canStartLoading = true;
        }

        private void PostCloudLogic()
        {
            MonoMain.atPostCloudLogic = true;
            DGSave.Initialize();
            Global.Initialize();
            Layer.InitializeLayers();
        }

        private void OnDeviceCreated() => DuckGame.Graphics.device = this.graphicsService.GraphicsDevice;

        public void KillEverything()
        {
            MonoMain.closingGame = true;
            if (this._killedEverything)
                return;
            DevConsole.Log(DCSection.General, "|DGRED|-----------KillEverything()-----------");
            this._killedEverything = true;
            try
            {
                if (!Program.crashed)
                {
                    Global.Save();
                    Options.Save();
                    Options.SaveLocalData();
                    if (Network.isActive)
                    {
                        for (int index = 0; index < 5; ++index)
                        {
                            Send.ImmediateUnreliableBroadcast(new NMClientClosedGame());
                            Send.ImmediateUnreliableBroadcast(new NMClientClosedGame());
                            Steam.Update();
                            Thread.Sleep(16);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            try
            {
                Music.Terminate();
                MonoMain.cancelLazyLoad = true;
            }
            catch
            {
            }
            try
            {
                if (MonoMain._lazyLoadThread != null && MonoMain._lazyLoadThread.IsAlive)
                    MonoMain._lazyLoadThread.Abort();
                if (MonoMain._initializeThread != null)
                {
                    if (MonoMain._initializeThread.IsAlive)
                        MonoMain._initializeThread.Abort();
                }
            }
            catch
            {
            }
            try
            {
                NetworkDebugger.TerminateThreads();
                Network.Terminate();
                Input.Terminate();
            }
            catch
            {
            }
            try
            {
                DeviceChangeNotifier.Stop();
            }
            catch
            {
            }
            try
            {
                while (Cloud.processing)
                    Cloud.Update();
                Steam.Terminate();
            }
            catch (Exception)
            {
            }
            try
            {
                if (MonoMain.logFileOperations)
                {
                    DevConsole.Log(DCSection.General, "Logging file operations finished.");
                    DevConsole.SaveNetLog("duck_file_log.rtf");
                }
            }
            catch (Exception)
            {
            }
            if (this._infiniteLoopDetector == null)
                return;
            this._infiniteLoopDetector = null;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            this.KillEverything();
            Process.GetCurrentProcess().Kill();
        }

        private void DoLazyLoading()
        {
            while (!MonoMain.cancelLazyLoad && MonoMain.lazyLoadActions.Count != 0)
            {
                Action action = MonoMain.lazyLoadActions.Dequeue();
                if (action != null)
                    action();
            }
        }

        public static Thread lazyLoadThread => MonoMain._lazyLoadThread;

        private void StartLazyLoad()
        {
            SFX.Initialize();
            DuckGame.Content.Initialize();
        }

        public static void FinishLazyLoad()
        {
            while (MonoMain.lazyLoadActions.Count > 0)
                MonoMain.lazyLoadActions.Dequeue()();
        }

        public static Thread initializeThread => MonoMain._initializeThread;

        public static Task initializeTask => MonoMain._initializeTask;

        private static void ResultFetched(object value0, WorkshopQueryResult result)
        {
            if (result == null || result.details == null)
                return;
            WorkshopItem publishedFile = result.details.publishedFile;
            int num1 = DuckFile.GetFiles(publishedFile.path).Count<string>();
            int num2 = DuckFile.GetDirectories(publishedFile.path).Count<string>();
            if ((num1 != 0 || num2 != 0) && (publishedFile.stateFlags & WorkshopItemState.Installed) != WorkshopItemState.None && (publishedFile.stateFlags & WorkshopItemState.NeedsUpdate) == WorkshopItemState.None)
                return;
            MonoMain.availableModsToDownload.Add(publishedFile);
        }

        private void DownloadWorkshopItems()
        {
            MonoMain.loadMessage = "Downloading workshop mods...";
            if (!Steam.IsInitialized())
                return;
            LoadingAction steamLoad = new LoadingAction();
            steamLoad.action = () =>
           {
               WorkshopQueryUser queryUser = Steam.CreateQueryUser(Steam.user.id, WorkshopList.Subscribed, WorkshopType.UsableInGame, WorkshopSortOrder.TitleAsc);
               queryUser.requiredTags.Add("Mod");
               queryUser.onlyQueryIDs = true;
               queryUser.QueryFinished += sender => steamLoad.flag = true;
               queryUser.ResultFetched += new WorkshopQueryResultFetched(MonoMain.ResultFetched);
               queryUser.Request();
               Steam.Update();
           };
            steamLoad.waitAction = () =>
           {
               Steam.Update();
               return steamLoad.flag;
           };
            MonoMain._thingsToLoad.Enqueue(steamLoad);
            steamLoad = new LoadingAction();
            steamLoad.action = () =>
           {
               MonoMain.totalLoadyBits = MonoMain.availableModsToDownload.Count;
               MonoMain.loadyBits = 0;
               foreach (WorkshopItem workshopItem in MonoMain.availableModsToDownload)
               {
                   WorkshopItem u = workshopItem;
                   LoadingAction itemDownload = new LoadingAction();
                   itemDownload.action = () =>
             {
                 MonoMain.loadMessage = "Downloading workshop mods (" + MonoMain.loadyBits.ToString() + "/" + MonoMain.totalLoadyBits.ToString() + ")";
                 if (Steam.DownloadWorkshopItem(u))
                     itemDownload.context = u;
                 ++MonoMain.loadyBits;
             };
                   itemDownload.waitAction = () =>
             {
                 Steam.Update();
                 return u == null || u.finishedProcessing;
             };
                   steamLoad.actions.Enqueue(itemDownload);
               }
           };
            steamLoad.waitAction = () =>
           {
               Steam.Update();
               return steamLoad.flag;
           };
            MonoMain._thingsToLoad.Enqueue(steamLoad);
        }

        private void AddLoadingAction(Action pAction) => MonoMain._thingsToLoad.Enqueue((LoadingAction)pAction);

        private void StartThreadedLoading()
        {
            this._threadedLoadingStarted = true;
            MonoMain.currentActionQueue = MonoMain._thingsToLoad;
            this.AddLoadingAction(new Action(ManagedContent.PreInitializeMods));
            this.AddLoadingAction(() =>
           {
               DuckGame.Content.InitializeTextureSizeDictionary();
               Network.Initialize();
               Teams.Initialize();
               Chancy.Initialize();
               this._watermarkEffect = DuckGame.Content.Load<MTEffect>("Shaders/basicWatermark");
               this._watermarkTexture = DuckGame.Content.Load<Tex2D>("looptex");
               DuckNetwork.Initialize();
               Persona.Initialize();
               DuckRig.Initialize();
           });
            this.AddLoadingAction(new Action(Input.Initialize));
            if (MonoMain.downloadWorkshopMods)
                this.DownloadWorkshopItems();
            this.AddLoadingAction(new Action(ManagedContent.InitializeMods));
            this.AddLoadingAction(new Action(Network.InitializeMessageTypes));
            this.AddLoadingAction(new Action(DeathCrate.InitializeDeathCrateSettings));
            this.AddLoadingAction(new Action(Editor.InitializeConstructorLists));
            this.AddLoadingAction(new Action(Team.DeserializeCustomHats));
            this.AddLoadingAction(new Action(DuckGame.Content.InitializeLevels));
            this.AddLoadingAction(new Action(DuckGame.Content.InitializeEffects));
            this.AddLoadingAction(new Action(Input.InitializeGraphics));
            this.AddLoadingAction(new Action(Music.Initialize));
            this.AddLoadingAction(new Action(DevConsole.InitializeFont));
            this.AddLoadingAction(new Action(DevConsole.InitializeCommands));
            this.AddLoadingAction(new Action(Editor.InitializePlaceableGroup));
            this.AddLoadingAction(new Action(Challenges.Initialize));
            this.AddLoadingAction(new Action(Collision.Initialize));
            this.AddLoadingAction(new Action(Level.InitializeCollisionLists));
            this.AddLoadingAction(new Action(Keyboard.InitTriggerImages));
            this.AddLoadingAction(new Action(MapPack.RegeneratePreviewsIfNecessary));
            this.AddLoadingAction(() => this.StartLazyLoad());
            this.AddLoadingAction(new Action(this.SetStarted));
        }

        private void SetStarted()
        {
            this._doStart = true;
            if (MonoMain.enableThreadedLoading)
            {
                MonoMain._lazyLoadThread = new Thread(new ThreadStart(this.DoLazyLoading))
                {
                    CurrentCulture = CultureInfo.InvariantCulture,
                    Priority = ThreadPriority.BelowNormal,
                    IsBackground = true
                };
                MonoMain._lazyLoadThread.Start();
            }
            else
                this.DoLazyLoading();
        }

        private void Start()
        {
            ModLoader.PostLoadMods();
            this.OnStart();
            MonoMain._started = true;
        }

        protected virtual void OnStart()
        {
        }

        protected override void UnloadContent()
        {
        }

        public static void StartRecording(string name)
        {
            MonoMain._recordingStarted = true;
            MonoMain._recordData = true;
            int num = MonoMain._recordData ? 1 : 0;
        }

        public static void StartPlayback()
        {
            MonoMain._recordingStarted = true;
            MonoMain._recordData = false;
        }

        public static void StopRecording() => MonoMain._recordingStarted = false;

        private void DeviceLost(object obj, EventArgs args)
        {
            MonoMain.loseDevice = 1;
            SynchronizedContentManager.blockLoading = 2;
        }

        private void DeviceResetting(object obj, EventArgs args)
        {
            MonoMain.loseDevice = 1;
            SynchronizedContentManager.blockLoading = 2;
        }

        private void DeviceReset(object obj, EventArgs args)
        {
            MonoMain.loseDevice = 1;
            SynchronizedContentManager.blockLoading = 2;
        }

        protected override void Update(GameTime gameTime)
        {
            if (MonoMain.showingSaveTool && MonoMain.saveTool == null && System.IO.File.Exists("SaveTool.dll"))
            {
                MonoMain.saveTool = Activator.CreateInstance(Assembly.Load(System.IO.File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\SaveTool.dll")).GetType("SaveRecovery.SaveTool")) as Form;
                DuckGame.Graphics.mouseVisible = true;
                int num = (int)MonoMain.saveTool.ShowDialog();
                Program.crashed = true;
                Application.Exit();
            }
            if (Program.isLinux)
            {
                if (this.IsActive)
                {
                    ++MonoMain.framesBackInFocus;
                    DuckGame.Graphics.mouseVisible = MonoMain.showingSaveTool;
                }
                else
                {
                    MonoMain.framesBackInFocus = 0L;
                    DuckGame.Graphics.mouseVisible = true;
                }
            }
            else if (Form.ActiveForm != null && this.IsActive)
            {
                ++MonoMain.framesBackInFocus;
                DuckGame.Graphics.mouseVisible = MonoMain.showingSaveTool;
            }
            else
            {
                MonoMain.framesBackInFocus = 0L;
                DuckGame.Graphics.mouseVisible = true;
            }
            if (!this.GraphicsDevice.IsDisposed)
            {
                if (!DuckGame.Graphics.screen.GraphicsDevice.IsDisposed)
                {
                    try
                    {
                        MonoMain._loopTimer.Restart();
                        this.RunUpdate(gameTime);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Program.HandleGameCrash(ex);
                        return;
                    }
                }
            }
            base.Update(gameTime);
        }

        public static bool closeMenus
        {
            get => MonoMain._core.closeMenus;
            set => MonoMain._core.closeMenus = value;
        }

        public static bool menuOpenedThisFrame
        {
            get => MonoMain._core.menuOpenedThisFrame;
            set => MonoMain._core.menuOpenedThisFrame = value;
        }

        public static bool dontResetSelection
        {
            get => MonoMain._core.dontResetSelection;
            set => MonoMain._core.dontResetSelection = value;
        }

        public static void UpdatePauseMenu(bool hasFocus = true)
        {
            MonoMain.shouldPauseGameplay = true;
            if (Network.isActive && UIMatchmakerMark2.instance == null && (!Network.InLobby() || !(Level.current as TeamSelect2).MatchmakerOpen()))
                MonoMain.shouldPauseGameplay = false;
            if (MonoMain._pauseMenu != null)
            {
                if (MonoMain.shouldPauseGameplay)
                {
                    HUD.Update();
                    MonoMain._pauseMenu.Update();
                    AutoUpdatables.MuteSounds();
                }
                else
                {
                    MonoMain._pauseMenu.Update();
                    Input.ignoreInput = true;
                }
                if (MonoMain._pauseMenu != null && !MonoMain._pauseMenu.open)
                    MonoMain._pauseMenu = null;
            }
            else
                MonoMain.shouldPauseGameplay = false;
            for (int index = 0; index < MonoMain.closeMenuUpdate.Count; ++index)
            {
                UIComponent uiComponent = MonoMain.closeMenuUpdate[index];
                uiComponent.Update();
                if (!uiComponent.animating)
                {
                    MonoMain.closeMenuUpdate.RemoveAt(index);
                    --index;
                }
            }
            MonoMain.menuOpenedThisFrame = false;
            MonoMain.dontResetSelection = false;
        }

        public static void RetakePauseCapture() => MonoMain._didPauseCapture = false;

        public static void CalculateModMemoryOffendersList()
        {
            List<ModConfiguration> list = MonoMain.loadedModsWithAssemblies.OrderByDescending<ModConfiguration, long>(x => x.content == null ? -1L : x.content.kilobytesPreAllocated).ToList<ModConfiguration>();
            bool flag = false;
            MonoMain.modMemoryOffendersString = "Mods taking up the most memory:\n";
            foreach (ModConfiguration modConfiguration in list)
            {
                long kilobytesPreAllocated = modConfiguration.content.kilobytesPreAllocated;
                if (kilobytesPreAllocated / 1000L > 20L)
                {
                    MonoMain.modMemoryOffendersString = MonoMain.modMemoryOffendersString + modConfiguration.displayName + " (" + (kilobytesPreAllocated / 1000L).ToString() + "MB)(ID:" + modConfiguration.workshopID.ToString() + ")\n";
                    flag = true;
                }
            }
            MonoMain.modMemoryOffendersString += "\n";
            if (flag)
                return;
            MonoMain.modMemoryOffendersString = "";
        }

        public void RunUpdate(GameTime gameTime)
        {
            ++DuckGame.Graphics.frame;
            Tasker.RunTasks();
            DuckGame.Graphics.GarbageDisposal(false);
            if (!MonoMain.disableSteam && !MonoMain._started)
            {
                if (Cloud.processing)
                {
                    Cloud.Update();
                    return;
                }
                if (MonoMain.steamConnectionCheckFail)
                {
                    if (this._loggedConnectionCheckFailure)
                    {
                        this._loggedConnectionCheckFailure = true;
                        DevConsole.Log("|DGRED|Failed to initialize a connection to Steam.");
                    }
                }
                else if (Steam.IsInitialized() && Steam.IsRunningInitializeProcedures())
                {
                    MonoMain.loadMessage = "Loading Steam";
                    Steam.Update();
                    return;
                }
            }
            if (this._canStartLoading && !this._threadedLoadingStarted && this._didFirstDraw)
            {
                this.PostCloudLogic();
                this.StartThreadedLoading();
            }
            if (MonoMain._thingsToLoad.Count > 0)
            {
                TimeSpan elapsed;
                if (Program.isLinux)
                {
                    elapsed = this._waitToStartLoadingTimer.elapsed;
                    if (elapsed.TotalMilliseconds <= 3500.0)
                        goto label_19;
                }
                this._loadTimer.Restart();
                while (MonoMain._thingsToLoad.Count > 0)
                {
                    elapsed = this._loadTimer.elapsed;
                    if (elapsed.TotalMilliseconds < 40.0)
                    {
                        MonoMain.currentActionQueue = MonoMain._thingsToLoad;
                        LoadingAction loadingAction = MonoMain._thingsToLoad.Peek();
                        if (loadingAction.Invoke())
                            MonoMain._thingsToLoad.Dequeue();
                        else if (loadingAction.waiting)
                            break;
                    }
                    else
                        break;
                }
            }
        label_19:
            if (this._doStart && !MonoMain._started)
            {
                this._doStart = false;
                this.Start();
            }
            if (!MonoMain._started || DuckGame.Graphics.screenCapture != null)
                return;
            if (DuckGame.Graphics.inFocus)
                Input.Update();
            lock (LevelMetaData._completedPreviewTasks)
            {
                if (LevelMetaData._completedPreviewTasks.Count > 0)
                {
                    foreach (LevelMetaData.SaveLevelPreviewTask completedPreviewTask in LevelMetaData._completedPreviewTasks)
                    {
                        try
                        {
                            DuckFile.SaveString(completedPreviewTask.levelString, completedPreviewTask.savePath);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    LevelMetaData._completedPreviewTasks.Clear();
                }
            }
            Cloud.Update();
            if (MonoMain._started && !NetworkDebugger.enabled)
            {
                InputProfile.Update();
                Network.PreUpdate();
            }
            if (Keyboard.Pressed(Keys.F4) || Keyboard.alt && Keyboard.Pressed(Keys.Enter))
            {
                Options.Data.fullscreen = !Options.Data.fullscreen;
                Options.FullscreenChanged();
            }
            if (!Cloud.processing)
                Steam.Update();
            try
            {
                if (Keyboard.Pressed(Keys.F2))
                    Program.MakeNetLog();
            }
            catch (Exception)
            {
            }
            if (MonoMain.exit || (Keyboard.Down(Keys.LeftAlt) || Keyboard.Down(Keys.RightAlt)) && Keyboard.Down(Keys.F4))
            {
                this.KillEverything();
                this.Exit();
            }
            else
            {
                TouchScreen.Update();
                if (!NetworkDebugger.enabled)
                    DevConsole.Update();
                SFX.Update();
                Options.Update();
                InputProfile.repeat = Level.current is Editor || MonoMain._pauseMenu != null || Editor.selectingLevel;
                Keyboard.repeat = Level.current is Editor || MonoMain._pauseMenu != null || DevConsole.open || DuckNetwork.core.enteringText || Editor.enteringText;
                bool hasFocus = true;
                if (!NetworkDebugger.enabled)
                    MonoMain.UpdatePauseMenu(hasFocus);
                else
                    MonoMain.shouldPauseGameplay = false;
                if (MonoMain.transitionDirection != TransitionDirection.None)
                {
                    if (MonoMain.transitionLevel != null)
                    {
                        DuckGame.Graphics.fade = Lerp.Float(DuckGame.Graphics.fade, 0.0f, 0.05f);
                        if ((double)DuckGame.Graphics.fade <= 0.0)
                        {
                            Level.current = MonoMain.transitionLevel;
                            MonoMain.transitionLevel = null;
                            MonoMain.transitionDirection = TransitionDirection.None;
                        }
                    }
                    else
                    {
                        DuckGame.Graphics.fade = Lerp.Float(DuckGame.Graphics.fade, 1f, 0.1f);
                        if ((double)DuckGame.Graphics.fade >= 1.0)
                        {
                            MonoMain.transitionLevel = null;
                            MonoMain.transitionDirection = TransitionDirection.None;
                        }
                    }
                    MonoMain.shouldPauseGameplay = true;
                }
                RumbleManager.Update();
                if (!MonoMain.shouldPauseGameplay)
                {
                    if (MonoMain._pauseMenu == null)
                        MonoMain._didPauseCapture = false;
                    if (!MonoMain._recordingStarted || MonoMain._recordData)
                    {
                        if (DevConsole.rhythmMode && Level.current is GameLevel)
                        {
                            RhythmMode.TickSound((float)((double)((float)(Music.position + new TimeSpan(0, 0, 0, 0, 80)).TotalMinutes * 140f) % 1.0 / 1.0));
                            RhythmMode.Tick((float)((double)((float)(Music.position + new TimeSpan(0, 0, 0, 0, 40)).TotalMinutes * 140f) % 1.0 / 1.0));
                        }
                        foreach (IEngineUpdatable engineUpdatable in MonoMain.core.engineUpdatables)
                            engineUpdatable.PreUpdate();
                        AutoUpdatables.Update();
                        DuckGame.Content.Update();
                        Music.Update();
                        Level.UpdateLevelChange();
                        Level.UpdateCurrentLevel();
                        foreach (IEngineUpdatable engineUpdatable in MonoMain.core.engineUpdatables)
                            engineUpdatable.Update();
                        this.OnUpdate();
                    }
                }
                DuckGame.Graphics.RunRenderTasks();
                Input.ignoreInput = false;
                base.Update(gameTime);
                FPSCounter.Tick(0);
                if (!NetworkDebugger.enabled)
                    Network.PostUpdate();
                foreach (IEngineUpdatable engineUpdatable in MonoMain.core.engineUpdatables)
                    engineUpdatable.PostUpdate();
            }
        }

        protected virtual void OnUpdate()
        {
        }

        public static void RenderGame(RenderTarget2D target)
        {
            int width = DuckGame.Graphics.width;
            int height = DuckGame.Graphics.height;
            DuckGame.Graphics.SetRenderTarget(target);
            Viewport viewport = new Viewport();
            viewport.X = viewport.Y = 0;
            viewport.Width = target.width;
            viewport.Height = target.height;
            viewport.MinDepth = 0.0f;
            viewport.MaxDepth = 1f;
            DuckGame.Graphics.viewport = viewport;
            DuckGame.Graphics.width = target.width;
            DuckGame.Graphics.height = target.height;
            Level.DrawCurrentLevel();
            DuckGame.Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
            MonoMain.instance.OnDraw();
            DuckGame.Graphics.screen.End();
            DuckGame.Graphics.width = width;
            DuckGame.Graphics.height = height;
            DuckGame.Graphics.SetRenderTarget(null);
        }

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtQueryTimerResolution(
          out int MinimumResolution,
          out int MaximumResolution,
          out int CurrentResolution);

        /// <summary>Returns the current timer resolution in 100ns units</summary>
        public static int GetCurrentResolution()
        {
            int CurrentResolution;
            MonoMain.NtQueryTimerResolution(out int _, out int _, out CurrentResolution);
            return CurrentResolution;
        }

        /// <summary>
        /// Sleeps as long as possible without exceeding the specified period
        /// </summary>
        public static void SleepForNoMoreThan(double milliseconds)
        {
            if (MonoMain.LowestSleepThreshold == 0.0)
            {
                int MinimumResolution;
                int MaximumResolution;
                int CurrentResolution;
                MonoMain.NtQueryTimerResolution(out MinimumResolution, out MaximumResolution, out CurrentResolution);
                MonoMain.LowestSleepThreshold = 1.0 + MaximumResolution / 10000.0;
                DevConsole.Log(DCSection.General, "TIMER RES(" + MinimumResolution.ToString() + ", " + MaximumResolution.ToString() + ", " + CurrentResolution.ToString() + ")");
            }
            if (milliseconds < MonoMain.LowestSleepThreshold)
                return;
            int millisecondsTimeout = (int)(milliseconds - MonoMain.GetCurrentResolution());
            if (millisecondsTimeout < 1)
                return;
            Thread.Sleep(millisecondsTimeout);
        }

        //public static MaterialPause pauseMaterial => MonoMain._pauseMaterial;

        //private static int JulianDate(int d, int m, int y)
        //{
        //    int num1 = y - (12 - m) / 10;
        //    int num2 = m + 9;
        //    if (num2 >= 12)
        //        num2 -= 12;
        //    int num3 = (int)(365.25 * (double)(num1 + 4712));
        //    int num4 = (int)(30.6001 * (double)num2 + 0.5);
        //    int num5 = (int)((double)(num1 / 100 + 49) * 0.75) - 38;
        //    int num6 = num4;
        //    int num7 = num3 + num6 + d + 59;
        //    if (num7 > 2299160)
        //        num7 -= num5;
        //    return num7;
        //}

        //private static MonoMain.MoonInfo MoonPhase(int d, int m, int y)
        //{
        //    MonoMain.MoonInfo moonInfo = new MonoMain.MoonInfo();
        //    int num = MonoMain.JulianDate(d, m, y);
        //    moonInfo.phase = ((double)num + 4.867) / 29.53059;
        //    moonInfo.phase -= Math.Floor(moonInfo.phase);
        //    moonInfo.age = moonInfo.phase >= 0.5 ? moonInfo.phase * 29.53059 - 14.765295 : moonInfo.phase * 29.53059 + 14.765295;
        //    moonInfo.age = Math.Floor(moonInfo.age) + 1.0;
        //    return moonInfo;
        //}

        public static bool FullMoon
        {
            get
            {
                double num = (DateTime.UtcNow - new DateTime(1900, 1, 1)).TotalDays % 29.530588853;
                return DateTime.Now.Hour < 1 && num > 13.0 && num < 17.0;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            int num = MonoMain.started ? 1 : 0;
            ++MonoMain.framesSinceFocusChange;
            if (MonoMain.loseDevice > 0)
            {
                DuckGame.Graphics.Clear(Color.Black);
                this.GraphicsDevice.SetRenderTarget(null);
                --MonoMain.loseDevice;
                base.Draw(gameTime);
            }
            else
            {
                if (this.GraphicsDevice.IsDisposed)
                    return;
                DuckGame.Graphics.drawing = true;
                --SynchronizedContentManager.blockLoading;
                if (SynchronizedContentManager.blockLoading < 0)
                    SynchronizedContentManager.blockLoading = 0;
                try
                {
                    DuckGame.Graphics.device = this.GraphicsDevice;
                    if (Resolution.Update())
                    {
                        DuckGame.Graphics.Clear(Color.Black);
                        this.GraphicsDevice.SetRenderTarget(null);
                        base.Draw(gameTime);
                        DuckGame.Graphics.drawing = false;
                    }
                    else
                    {
                        this.RunDraw(gameTime);
                        DuckGame.Graphics.SetRenderTargetToScreen();
                        if (DuckGame.Graphics._screenBufferTarget != null)
                        {
                            MonoMain._tempRecordingReference = Recorder.currentRecording;
                            Recorder.currentRecording = null;
                            DuckGame.Graphics.SetScreenTargetViewport();
                            DuckGame.Graphics.Clear(Color.Black);
                            Camera camera = new Camera(0.0f, 0.0f, Graphics._screenBufferTarget.width, Graphics._screenBufferTarget.height);
                            DuckGame.Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, camera.getMatrix());
                            DuckGame.Graphics.Draw(Graphics._screenBufferTarget, 0.0f, 0.0f);
                            DuckGame.Graphics.screen.End();
                            Recorder.currentRecording = MonoMain._tempRecordingReference;
                        }
                        DuckGame.Graphics.UpdateScreenViewport();
                        this.GraphicsDevice.SetRenderTarget(null);
                        base.Draw(gameTime);
                        DuckGame.Graphics.drawing = false;
                    }
                }
                catch (Exception ex)
                {
                    Program.HandleGameCrash(ex);
                }
            }
        }

        protected void RunDraw(GameTime gameTime)
        {
            FPSCounter.Tick(1);
            this._didFirstDraw = true;
            DuckGame.Graphics.frameFlipFlop = !DuckGame.Graphics.frameFlipFlop;
            if (DuckGame.Graphics.device.IsDisposed)
                return;
            DuckGame.Graphics.SetScissorRectangle(new Rectangle(0.0f, 0.0f, Graphics.width, Graphics.height));
            if (Recorder.currentRecording != null)
                Recorder.currentRecording.NextFrame();
            if (!MonoMain._started)
            {
                ++this._loadingFramesRendered;
                DuckGame.Graphics.SetRenderTarget(null);
                MonoMain._pauseMaterial = new MaterialPause();
                if (!this._setCulture)
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    this._setCulture = true;
                }
                DuckGame.Graphics.Clear(new Color(0, 0, 0));
                Camera camera = new Camera(0.0f, 0.0f, Graphics.width, Graphics.height);
                DuckGame.Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, camera.getMatrix());
                Vec2 p1 = new Vec2(50f, DuckGame.Graphics.height - 50);
                Vec2 vec2_1 = new Vec2(DuckGame.Graphics.width - 100, 20f);
                DuckGame.Graphics.DrawRect(p1, p1 + vec2_1, Color.DarkGray * 0.1f, (Depth)0.5f);
                float num = loadyBits / (float)MonoMain.totalLoadyBits;
                if ((double)num > 1.0)
                    num = 1f;
                DuckGame.Graphics.DrawRect(p1, p1 + new Vec2(vec2_1.x * num, vec2_1.y), Color.White * 0.1f, (Depth)0.6f);
                string text = MonoMain.loadMessage;
                if (Cloud.processing && (double)Cloud.progress != 0.0 && (double)Cloud.progress != 1.0)
                    text = "Synchronizing Steam Cloud... (" + ((int)((double)Cloud.progress * 100.0)).ToString() + "%)";
                DuckGame.Graphics.DrawString(text, p1 + new Vec2(0.0f, -24f), Color.White, (Depth)1f, scale: 2f);
                this._duckRun.speed = 0.15f;
                this._duckRun.scale = new Vec2(4f, 4f);
                this._duckRun.depth = (Depth)0.7f;
                this._duckRun.color = new Color(80, 80, 80);
                if (this._timeSinceLastLoadFrame.elapsed.Milliseconds > 16)
                    ++this._duckRun.frame;
                Vec2 vec2_2 = new Vec2(DuckGame.Graphics.width - this._duckRun.width * 4 - 50, DuckGame.Graphics.height - this._duckRun.height * 4 - 55);
                DuckGame.Graphics.Draw(_duckRun, vec2_2.x, vec2_2.y);
                this._duckArm.frame = this._duckRun.imageIndex;
                this._duckArm.scale = new Vec2(4f, 4f);
                this._duckArm.depth = (Depth)0.6f;
                this._duckArm.color = new Color(80, 80, 80);
                DuckGame.Graphics.Draw(_duckArm, vec2_2.x + 20f, vec2_2.y + 56f);
                DuckGame.Graphics.screen.End();
                this._timeSinceLastLoadFrame.Restart();
            }
            else
            {
                if (Level.current == null)
                    return;
                DuckGame.Reflection.Render();
                if (!this.takingShot)
                {
                    this.takingShot = true;
                    if (Keyboard.shift && Keyboard.Pressed(Keys.F12) || this.waitFrames < 0)
                    {
                        if (this._screenshotTarget == null)
                            this._screenshotTarget = new RenderTarget2D(DuckGame.Graphics.width, DuckGame.Graphics.height, true);
                        DuckGame.Graphics.screenCapture = this._screenshotTarget;
                        this.RunDraw(gameTime);
                        this.waitFrames = 60 + Rando.Int(60);
                        SFX.Play("ching");
                    }
                    this.takingShot = false;
                }
                if (MonoMain._pauseMenu != null && !NetworkDebugger.enabled && !MonoMain._didPauseCapture)
                {
                    DuckGame.Graphics.screenCapture = MonoMain._screenCapture;
                    MonoMain._didPauseCapture = true;
                }
                if (DuckGame.Graphics.screenCapture != null)
                {
                    int width = DuckGame.Graphics.width;
                    int height = DuckGame.Graphics.height;
                    DuckGame.Graphics.SetRenderTarget(DuckGame.Graphics.screenCapture);
                    DuckGame.Graphics.UpdateScreenViewport(true);
                    HUD.hide = true;
                    Level.DrawCurrentLevel();
                    DuckGame.Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                    this.OnDraw();
                    DuckGame.Graphics.screen.End();
                    HUD.hide = false;
                    DuckGame.Graphics.screenCapture = null;
                    DuckGame.Graphics.width = width;
                    DuckGame.Graphics.height = height;
                    DuckGame.Graphics.SetRenderTarget(null);
                }
                if (this._screenshotTarget != null)
                {
                    this.saveShot = this._screenshotTarget;
                    this._screenshotTarget = null;
                    this.SaveShot();
                }
                if (DuckGame.Graphics.screenTarget != null)
                {
                    int width = DuckGame.Graphics.width;
                    int height = DuckGame.Graphics.height;
                    DuckGame.Graphics.SetRenderTarget(DuckGame.Graphics.screenTarget);
                    DuckGame.Graphics.UpdateScreenViewport();
                    HUD.hide = true;
                    Level.DrawCurrentLevel();
                    DuckGame.Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                    this.OnDraw();
                    DuckGame.Graphics.screen.End();
                    HUD.hide = false;
                    DuckGame.Graphics.width = width;
                    DuckGame.Graphics.height = height;
                    DuckGame.Graphics.SetRenderTarget(null);
                    DuckGame.Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                    DuckGame.Graphics.Draw(Graphics.screenTarget, Vec2.Zero, new Rectangle?(), Color.White, 0.0f, Vec2.Zero, Vec2.One, SpriteEffects.None);
                    DuckGame.Graphics.screen.End();
                }
                else
                {
                    bool flag = true;
                    if (Network.isActive)
                        flag = false;
                    if (MonoMain._pauseMenu != null && MonoMain._didPauseCapture && DuckGame.Graphics.screenCapture == null)
                    {
                        DuckGame.Graphics.SetRenderTarget(null);
                        DuckGame.Graphics.Clear(Color.Black * DuckGame.Graphics.fade);
                        if (MonoMain.autoPauseFade)
                        {
                            MonoMain._pauseMaterial.fade = Lerp.FloatSmooth(MonoMain._pauseMaterial.fade, MonoMain.doPauseFade ? 0.6f : 0.0f, 0.1f, 1.1f);
                            MonoMain._pauseMaterial.dim = Lerp.FloatSmooth(MonoMain._pauseMaterial.dim, MonoMain.doPauseFade ? 0.6f : 1f, 0.1f, 1.1f);
                        }
                        DuckGame.Graphics.SetFullViewport();
                        Vec2 vec2 = new Vec2(Layer.HUD.camera.width / _screenCapture.width, Layer.HUD.camera.height / _screenCapture.height);
                        DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, Matrix.Identity);
                        DuckGame.Graphics.material = _pauseMaterial;
                        DuckGame.Graphics.Draw(_screenCapture, new Vec2(0.0f, 0.0f), new Rectangle?(), new Color(120, 120, 120), 0.0f, Vec2.Zero, new Vec2(1f, 1f), SpriteEffects.None, -0.9f);
                        DuckGame.Graphics.material = null;
                        DuckGame.Graphics.screen.End();
                        DuckGame.Graphics.RestoreOldViewport();
                        Layer.HUD.Begin(true);
                        MonoMain._pauseMenu.Draw();
                        for (int index = 0; index < MonoMain.closeMenuUpdate.Count; ++index)
                            MonoMain.closeMenuUpdate[index].Draw();
                        HUD.Draw();
                        if (Level.current.drawsOverPauseMenu)
                            Level.current.PostDrawLayer(Layer.HUD);
                        Layer.HUD.End(true);
                        Layer.Console.Begin(true);
                        DevConsole.Draw();
                        Level.current.PostDrawLayer(Layer.Console);
                        Layer.Console.End(true);
                        if (!flag && UIMatchmakerMark2.instance == null)
                            MonoMain._didPauseCapture = false;
                    }
                    else
                    {
                        if (MonoMain.autoPauseFade)
                        {
                            MonoMain._pauseMaterial.fade = 0.0f;
                            MonoMain._pauseMaterial.dim = 0.6f;
                        }
                        DuckGame.Graphics.SetRenderTarget(null);
                        Level.DrawCurrentLevel();
                        DuckGame.Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Resolution.getTransformationMatrix());
                        this.OnDraw();
                        DuckGame.Graphics.screen.End();
                        if (MonoMain.closeMenuUpdate.Count > 0)
                        {
                            Layer.HUD.Begin(true);
                            foreach (Thing thing in MonoMain.closeMenuUpdate)
                                thing.DoDraw();
                            Layer.HUD.End(true);
                        }
                    }
                    if (!DevConsole.showFPS)
                        return;
                    FPSCounter.Render(DuckGame.Graphics.device, index: 0, label: "UPS");
                    FPSCounter.Render(DuckGame.Graphics.device, 100f, index: 1);
                }
            }
        }

        protected virtual void OnDraw()
        {
        }

        public class WebCharData
        {
            public Tex2D image;
            public string name;
            public string quote;
        }

        public class MoonInfo
        {
            public double age;
            public double phase;
        }
    }
}

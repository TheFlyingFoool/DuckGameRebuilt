// Decompiled with JetBrains decompiler
// Type: DuckGame.MonoMain
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDL2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AddedContent.Hyeve;
using XnaToFna;

namespace DuckGame
{
    public class MonoMain : Game
    {
        //private Effect myEffect;
        private static MonoMainCore _core = new MonoMainCore();
        public static int MaximumGamepadCount = 4;
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
        public static string NloadMessage
        {
            get
            {
                return loadMessage;
            }
            set
            {
                string text = value;
                if (Debugger.IsAttached)
                {
                    text = "|16,144,13|" + text;
                }
                if (!loadMessages.Contains(text))
                {
                    loadMessages.Push(text);
                }
                loadMessage = text;
                lastLoadMessage = text;
            }
        }
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
           () => GetOnlineString(),
           () => "Mods: " + ModLoader.modHash,
           () => "Time Played: " + TimeString(DateTime.Now - startTime) + " (" + Graphics.frame.ToString() + ")",
           () => "Special Code: " + Main.SpecialCode + " " + Main.SpecialCode2,
           () => "Resolution: (A)" + Resolution.adapterResolution.x.ToString() + "x" + Resolution.adapterResolution.y.ToString() + " (G)" + Resolution.current.x.ToString() + "x" + Resolution.current.y.ToString() + (Options.Data.fullscreen ? " (Fullscreen(" + (Options.Data.windowedFullscreen ? "W" : "H") + "))" : " (Windowed)") + "(RF " + framesSinceFocusChange.ToString() + ")",
           () => "Level: " + GetLevelString(),
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
        public static bool useRPC = false;
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
        public static bool firebreak = false;
        public static bool experimental = false;
        public static volatile int loadyBits = 0;
        public static volatile int totalLoadyBits = 365;
        private Timer _timeSinceLastLoadFrame = new Timer();
        public static bool logLoading;
        public static bool startInLobby = false;
        public static bool startInArcade = false;
        //private int deviceLostWait;

        public static MonoMainCore core
        {
            get => _core;
            set => _core = value;
        }

        public static void RegisterEngineUpdatable(IEngineUpdatable pUpdatable) => core.engineUpdatables.Add(pUpdatable);

        private static UIComponent _pauseMenu
        {
            get => _core._pauseMenu;
            set => _core._pauseMenu = value;
        }

        public static UIComponent pauseMenu
        {
            get => _pauseMenu != null && !_pauseMenu.inWorld && !_pauseMenu.open ? null : _pauseMenu;
            set
            {
                if (_pauseMenu != value && _pauseMenu != null && _pauseMenu.open && !_pauseMenu.inWorld)
                    _pauseMenu.Close();
                _pauseMenu = value;
            }
        }

        public static List<UIComponent> closeMenuUpdate => _core.closeMenuUpdate;

        public static RenderTarget2D screenCapture => _screenCapture;

        public static bool started => _started;

        public static int screenWidth => _screenWidth;

        public static int screenHeight => _screenHeight;

        public static int windowWidth => (int)Math.Round(screenWidth * Options.GetWindowScaleMultiplier());

        public static int windowHeight => (int)Math.Round(screenHeight * Options.GetWindowScaleMultiplier());

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

        public static string GetExceptionString(UnhandledExceptionEventArgs e) => GetExceptionString(e.ExceptionObject);

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
            return str1 + GetDetails();
        }

        public static string GetDetails()
        {
            string details = "";
            foreach (Func<string> extraExceptionDetail in _extraExceptionDetails)
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
                byteArrayFastest[index] = (byte)((GetHexVal(hex[index << 1]) << 4) + GetHexVal(hex[(index << 1) + 1]));
            return byteArrayFastest;
        }

        public static int GetHexVal(char hex)
        {
            int num = hex;
            return num - (num < 58 ? 48 : 55);
        }

        public static byte[] StringToByteArray(string hex) => Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();

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
                            byte[] byteArray = StringToByteArray(strArray[1].Split('&')[0]);
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

        public static WebCharData RequestRandomCharacter()
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
                            for (int index2 = 0; index2 < source.Count() && index1 < colorArray.Count(); ++index2)
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
                            return new WebCharData()
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

        public void SaveShot() => new Thread(new ThreadStart(SaveShotThread))
        {
            CurrentCulture = CultureInfo.InvariantCulture,
            Priority = ThreadPriority.BelowNormal,
            IsBackground = true
        }.Start();

        public void SaveShotThread()
        {
            RenderTarget2D saveShot = this.saveShot;
            string str1 = DateTime.Now.ToShortDateString() + "-" + DateTime.Now.ToShortTimeString() + " " + _numShots.ToString();
            ++_numShots;
            string str2 = str1.Replace("/", "_").Replace(":", "-").Replace(" ", "");
            if (!Directory.Exists("screenshots"))
                Directory.CreateDirectory("screenshots");
            FileStream fileStream = File.OpenWrite("screenshots/duckscreen-" + str2 + ".png");
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
            if (!noHidef && e.GraphicsDeviceInformation.Adapter.IsProfileSupported(GraphicsProfile.HiDef))
            {
                e.GraphicsDeviceInformation.GraphicsProfile = GraphicsProfile.HiDef;
                hidef = true;
            }
            else
                e.GraphicsDeviceInformation.GraphicsProfile = GraphicsProfile.Reach;
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PlatformContents;
        }

        public MonoMain()
        {
            mainThread = Thread.CurrentThread;
            cultureCode = CultureInfo.CurrentCulture.LCID;
            startupAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic).Select(assembly => assembly.Location).ToArray();
            Content = new SynchronizedContentManager(Services);
            DG.SetVersion(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            graphics = new GraphicsDeviceManager(this);
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);
            Content.RootDirectory = "Content";
            _adapterW = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _adapterH = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            if (_adapterW >= 2048 || _adapterH >= 2048)
                fourK = true;
            int num1 = 1280;
            if (num1 > _adapterW)
                num1 = 1024;
            if (num1 > _adapterW)
                num1 = 640;
            if (num1 > _adapterW)
                num1 = 320;
            if (_adapterW > 1920)
                num1 = 1920;
            // I'm not messing with this
            float num2 = _adapterH / (float)_adapterW;
            if (num2 < 0.56f)
            {
                num2 = 9f / 16f;
                _adapterH = (int)(_adapterW * num2);
            }
            int num3 = (int)(num2 * num1);
            if (num3 > 1200)
                num3 = 1200;
            _screenWidth = num1;
            _screenHeight = num3;
            DuckFile.Initialize();
            Options.Load();
            Cloud.Initialize();
            instance = this;
            Resolution.Initialize(Window.Handle, graphics);
            Options.Load();
            Options.PostLoad();
            if (noFullscreen)
                Options.LocalData.currentResolution = Options.LocalData.windowedResolution;
            Graphics.InitializeBase(graphics, screenWidth, screenHeight);
            _waitToStartLoadingTimer.Start();
        }

        public string screenMode
        {
            get => Resolution.current.mode.ToString();
            set
            {
            }
        }

        public static void ResetInfiniteLoopTimer() => _loopTimer.Reset();

        public void InfiniteLoopDetector()
        {
            while (_infiniteLoopDetector != null)
            {
                Thread.Sleep(40);
                if (!started || !Graphics.inFocus)
                    ResetInfiniteLoopTimer();
                if (_loopTimer.Elapsed.TotalSeconds > 5.0)
                {
                    try
                    {
                        mainThread.Suspend();
                        infiniteLoopDetails = "Infinite loop crash: ";
                        try
                        {
                            infiniteLoopDetails += GetInfiniteLoopDetails();
                        }
                        catch (Exception)
                        {
                        }
                        hadInfiniteLoop = true;
                        mainThread.Resume();
                        mainThread.Abort(new Exception(infiniteLoopDetails));
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
            string str = new StackTrace(mainThread, true).ToString();
            int length = str.IndexOf("at Microsoft.Xna.Framework.Game.Tick");
            return length >= 0 ? str.Substring(0, length) : str;
        }

        protected override void LoadContent() => base.LoadContent();

        public bool canSyncFramerateWithVSync => Options.Data.vsync;

        protected override void Initialize()
        {

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            LangHandler.Initialize();
            //IsFixedTimeStep = true; edited because i change it back after load is done
            Graphics.mouseVisible = false;
            base.Initialize();
            DuckGame.Content.InitializeBase(Content);
            Curve.System_Initialize();
            Rando.DoInitialize();
            NetRand.Initialize();
            InactiveSleepTime = new TimeSpan(0L);
            Graphics.Initialize(GraphicsDevice);
            Resolution.Set(Options.LocalData.currentResolution);
            Resolution.Apply();
            if (Program.doscreentileing)
            {
                Resolution r = new Resolution
                {
                    dimensions = new Vec2(321, 181),
                    mode = ScreenMode.Windowed
                };
                Resolution.Set(r);
                Resolution.Apply();
                SDL.SDL_SetWindowBordered(instance.Window.Handle, SDL.SDL_bool.SDL_FALSE);
                SDL.SDL_SetWindowPosition(instance.Window.Handle, (int)Program.StartPos.x, (int)Program.StartPos.y);
            }
            _screenCapture = new RenderTarget2D(Resolution.current.x, Resolution.current.y, true);
            _duckRun = new SpriteMap("duck", 32, 32);
            _duckRun.AddAnimation("run", 1f, true, 1, 2, 3, 4, 5, 6);
            _duckRun.SetAnimation("run");
            _duckArm = new SpriteMap("duckArms", 16, 16);
            graphicsService = Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
            Graphics.device.DeviceLost += new EventHandler<EventArgs>(DeviceLost);
            Graphics.device.DeviceResetting += new EventHandler<EventArgs>(DeviceResetting);
            Graphics.device.DeviceReset += new EventHandler<EventArgs>(DeviceReset);
            graphicsService.DeviceCreated += (_param1, _param2) => OnDeviceCreated();
            if (infiniteLoopDebug)
            {
                _infiniteLoopDetector = new Thread(new ThreadStart(InfiniteLoopDetector))
                {
                    CurrentCulture = CultureInfo.InvariantCulture,
                    Priority = ThreadPriority.Lowest,
                    IsBackground = true
                };
                _infiniteLoopDetector.Start();
                _loopTimer.Start();
            }
            _canStartLoading = true;
        }

        private void PostCloudLogic()
        {
            atPostCloudLogic = true;
            DGSave.Initialize();
            Global.Initialize();
            Layer.InitializeLayers();
        }

        private void OnDeviceCreated() => Graphics.device = graphicsService.GraphicsDevice;

        public void KillEverything()
        {
            closingGame = true;
            if (_killedEverything)
                return;
            DevConsole.Log(DCSection.General, "|DGRED|-----------KillEverything()-----------");
            _killedEverything = true;
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
                cancelLazyLoad = true;
            }
            catch
            {
            }
            try
            {
                if (_lazyLoadThread != null && _lazyLoadThread.IsAlive)
                    _lazyLoadThread.Abort();
                if (_initializeThread != null)
                {
                    if (_initializeThread.IsAlive)
                        _initializeThread.Abort();
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
                if (logFileOperations)
                {
                    DevConsole.Log(DCSection.General, "Logging file operations finished.");
                    DevConsole.SaveNetLog("duck_file_log.rtf");
                }
            }
            catch (Exception)
            {
            }
            if (_infiniteLoopDetector == null)
                return;
            _infiniteLoopDetector = null;
        }

        public static event Action<bool> OnGameExit;

        public static void InvokeOnGameExitEvent(bool isDangerous) => OnGameExit?.Invoke(isDangerous);

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (XnaToFnaHelper.fillinform != null)
            {
                XnaToFnaHelper.fillinform.Close();
            }
            InvokeOnGameExitEvent(false);
            KillEverything();
            Process.GetCurrentProcess().Kill();
        }

        private void DoLazyLoading()
        {
            while (!cancelLazyLoad && lazyLoadActions.Count != 0)
            {
                Action action = lazyLoadActions.Dequeue();
                if (action != null)
                    action();
            }
        }

        public static Thread lazyLoadThread => _lazyLoadThread;

        private void StartLazyLoad()
        {
            SFX.Initialize();
            DuckGame.Content.Initialize();
        }

        public static void FinishLazyLoad()
        {
            while (lazyLoadActions.Count > 0)
                lazyLoadActions.Dequeue()();
        }

        public static Thread initializeThread => _initializeThread;

        public static Task initializeTask => _initializeTask;

        private static void ResultFetched(object value0, WorkshopQueryResult result)
        {
            if (result == null || result.details == null)
                return;
            WorkshopItem publishedFile = result.details.publishedFile;
            int num1 = DuckFile.GetFiles(publishedFile.path).Count();
            int num2 = DuckFile.GetDirectories(publishedFile.path).Count();
            if ((num1 != 0 || num2 != 0) && (publishedFile.stateFlags & WorkshopItemState.Installed) != WorkshopItemState.None && (publishedFile.stateFlags & WorkshopItemState.NeedsUpdate) == WorkshopItemState.None)
                return;
            availableModsToDownload.Add(publishedFile);
        }

        private void DownloadWorkshopItems()
        {
            NloadMessage = "Downloading workshop mods...";
            if (!Steam.IsInitialized())
                return;
            LoadingAction steamLoad = new LoadingAction();
            steamLoad.action = () =>
            {
                WorkshopQueryUser queryUser = Steam.CreateQueryUser(Steam.user.id, WorkshopList.Subscribed, WorkshopType.UsableInGame, WorkshopSortOrder.TitleAsc);
                queryUser.requiredTags.Add("Mod");
                queryUser.onlyQueryIDs = true;
                queryUser.QueryFinished += sender => steamLoad.flag = true;
                queryUser.ResultFetched += new WorkshopQueryResultFetched(ResultFetched);
                queryUser.Request();
                Steam.Update();
            };
            steamLoad.waitAction = () =>
            {
                Steam.Update();
                return steamLoad.flag;
            };
            steamLoad.label = "steamLoad query";
            _thingsToLoad.Enqueue(steamLoad);
            steamLoad = new LoadingAction();
            steamLoad.action = () =>
            {
                totalLoadyBits = availableModsToDownload.Count;
                loadyBits = 0;
                foreach (WorkshopItem workshopItem in availableModsToDownload)
                {
                    WorkshopItem u = workshopItem;
                    LoadingAction itemDownload = new LoadingAction();
                    itemDownload.action = () =>
                    {
                        NloadMessage = "Downloading workshop mods (" + loadyBits.ToString() + "/" + totalLoadyBits.ToString() + ")";
                        if (Steam.DownloadWorkshopItem(u))
                            itemDownload.context = u;
                        ++loadyBits;
                    };
                    itemDownload.waitAction = () =>
                    {
                        Steam.Update();
                        return u == null || u.finishedProcessing;
                    };
                    itemDownload.label = "Downloading workshop mods action / Steam.Update finishedProcessing waitAction";
                    steamLoad.actions.Enqueue(itemDownload);
                }
            };
            steamLoad.waitAction = () =>
            {
                Steam.Update();
                return steamLoad.flag;
            };
            steamLoad.label = "setup steam downloading workshop items";
            _thingsToLoad.Enqueue(steamLoad);
        }
        private void AddNamedLoadingAction(Action pAction) => _thingsToLoad.Enqueue((LoadingAction)pAction);
        private void AddLoadingAction(Action pAction, string label = "")
        {
            LoadingAction Loadaction = (LoadingAction)pAction;
            Loadaction.label = label;
            _thingsToLoad.Enqueue(Loadaction);
        }
        
        private void StartThreadedLoading()
        {
            _threadedLoadingStarted = true;
            currentActionQueue = _thingsToLoad;
            AddLoadingAction(ManagedContent.PreInitializeMods, "ManagedContent PreInitializeMods");
            AddLoadingAction(() =>
            {
                DuckGame.Content.InitializeTextureSizeDictionary();
                Network.Initialize();
                Teams.Initialize();
                Chancy.Initialize();
                // _watermarkEffect = DuckGame.Content.Load<MTEffect>("Shaders/basicWatermark");
                // _watermarkTexture = DuckGame.Content.Load<Tex2D>("looptex");
                DuckNetwork.Initialize();
                Persona.Initialize();
                DuckRig.Initialize();
            }, "Cluster Initialize");
            AddLoadingAction(Input.Initialize);
            if (downloadWorkshopMods)
            {
                DevConsole.Log("DDownloadWorkshopItems");
                DownloadWorkshopItems();
            }

            if (DGRSettings.PreloadLevels) AddLoadingAction(DGRSettings.PrreloadLevels, "DGRSettings PrreloadLevels");
            AddLoadingAction(ManagedContent.InitializeMods, "ManagedContent InitializeMods");
            AddLoadingAction(Network.InitializeMessageTypes, "Network InitializeMessageTypes");
            AddLoadingAction(DeathCrate.InitializeDeathCrateSettings, "DeathCrate InitializeDeathCrateSettings");
            AddLoadingAction(Editor.InitializeConstructorLists, "Editor InitializeConstructorLists");
            AddLoadingAction(Team.DeserializeCustomHats, "Team DeserializeCustomHats");
            AddLoadingAction(DuckGame.Content.InitializeLevels, "Content InitializeLevels");
            AddLoadingAction(DuckGame.Content.InitializeEffects, "Content InitializeEffects");
            AddLoadingAction(Input.InitializeGraphics, "Input InitializeGraphics");
            AddLoadingAction(Music.Initialize, "Music Initialize");
            AddLoadingAction(DevConsole.InitializeFont, "DevConsole InitializeFont");
            AddLoadingAction(DevConsole.InitializeCommands, "DevConsole InitializeCommands");
            AddLoadingAction(Editor.InitializePlaceableGroup, "Editor InitializePlaceableGroup");
            AddLoadingAction(Challenges.Initialize, "Challenges Initialize");
            AddLoadingAction(Collision.Initialize, "Collision Initialize");
            AddLoadingAction(Level.InitializeCollisionLists, "Level InitializeCollisionLists");
            AddLoadingAction(Keyboard.InitTriggerImages, "Keyboard InitTriggerImages");
            AddLoadingAction(MapPack.RegeneratePreviewsIfNecessary, "MapPack RegeneratePreviewsIfNecessary");
            AddLoadingAction(StartLazyLoad, "StartLazyLoad");
            AddLoadingAction(SetStarted, "SetStarted");
        }

        private void SetStarted()
        {
            _doStart = true;
            if (enableThreadedLoading)
            {
                _lazyLoadThread = new Thread(new ThreadStart(DoLazyLoading))
                {
                    CurrentCulture = CultureInfo.InvariantCulture,
                    Priority = ThreadPriority.BelowNormal,
                    IsBackground = true
                };
                _lazyLoadThread.Start();
            }
            else
                DoLazyLoading();
        }

        private void Start()
        {
            ModLoader.PostLoadMods();
            OnStart();
            _started = true;

            // this is basically the lifeline of all attributes so i cant
            // use the PostInitialize attribute for it since it wont even
            // work without this lol
            //Program.FirebreakReflectionsht.Wait();

            //Program.main.TargetElapsedTime = TimeSpan.FromTicks(166667L);
            if (!(startInLobby || Program.testServer))
            {
                IsFixedTimeStep = true; // UNZOOOM
            }
           
            Program.SetAccumulatedElapsedTime(Program.main, Program.main.TargetElapsedTime);

            foreach (MethodInfo methodInfo in PostInitializeAttribute.All)
            {
                methodInfo.Invoke(null, null);
            }
        }

        protected virtual void OnStart()
        {
        }

        protected override void UnloadContent()
        {
        }

        public static void StartRecording(string name)
        {
            _recordingStarted = true;
            _recordData = true;
            int num = _recordData ? 1 : 0;
        }

        public static void StartPlayback()
        {
            _recordingStarted = true;
            _recordData = false;
        }

        public static void StopRecording() => _recordingStarted = false;

        private void DeviceLost(object obj, EventArgs args)
        {
            loseDevice = 1;
            SynchronizedContentManager.blockLoading = 2;
        }

        private void DeviceResetting(object obj, EventArgs args)
        {
            loseDevice = 1;
            SynchronizedContentManager.blockLoading = 2;
        }

        private void DeviceReset(object obj, EventArgs args)
        {
            loseDevice = 1;
            SynchronizedContentManager.blockLoading = 2;
        }
        public bool IsFocused
        {
            get => (SDL.SDL_GetWindowFlags(Window.Handle) & (uint)SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS) > 0;
        }
        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        protected override void Update(GameTime gameTime)
        {
            if (showingSaveTool && saveTool == null && File.Exists("SaveTool.dll"))
            {
                saveTool = Activator.CreateInstance(Assembly.Load(File.ReadAllBytes(Directory.GetCurrentDirectory() + "/SaveTool.dll")).GetType("SaveRecovery.SaveTool")) as Form;
                Graphics.mouseVisible = true;
                int num = (int)saveTool.ShowDialog();
                Program.crashed = true;
                Application.Exit();
                Program.main.KillEverything();
                Program.main.Exit();
            }
            if (Program.isLinux)
            {
                if (IsActive)
                {
                    ++framesBackInFocus;
                    Graphics.mouseVisible = showingSaveTool;
                }
                else
                {
                    framesBackInFocus = 0L;
                    Graphics.mouseVisible = true;
                }
            }
            else if (Program.IsLinuxD)
            {
                if (IsActive)
                {
                    ++framesBackInFocus;
                    Graphics.mouseVisible = showingSaveTool;
                }
                else
                {
                    framesBackInFocus = 0L;
                    Graphics.mouseVisible = true;
                }
            }
            else if (IsActive && IsFocused) // Form.ActiveForm != null &&
            {
                ++framesBackInFocus;
                Graphics.mouseVisible = showingSaveTool;
            }
            else
            {
                framesBackInFocus = 0L;
                Graphics.mouseVisible = true;
            }
            if (!GraphicsDevice.IsDisposed)
            {
                if (!Graphics.screen.GraphicsDevice.IsDisposed)
                {
                    try
                    {
                        _loopTimer.Restart();
                        RunUpdate(gameTime);
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
            get => _core.closeMenus;
            set => _core.closeMenus = value;
        }

        public static bool menuOpenedThisFrame
        {
            get => _core.menuOpenedThisFrame;
            set => _core.menuOpenedThisFrame = value;
        }

        public static bool dontResetSelection
        {
            get => _core.dontResetSelection;
            set => _core.dontResetSelection = value;
        }

        public static void UpdatePauseMenu(bool hasFocus = true)
        {
            shouldPauseGameplay = true;
            if (Network.isActive && UIMatchmakerMark2.instance == null && (!Network.InLobby() || !(Level.current as TeamSelect2).MatchmakerOpen()))
                shouldPauseGameplay = false;
            if (_pauseMenu != null)
            {
                if (shouldPauseGameplay)
                {
                    HUD.Update();
                    _pauseMenu.Update();
                    AutoUpdatables.MuteSounds();
                }
                else
                {
                    _pauseMenu.Update();
                    Input.ignoreInput = true;
                }
                if (_pauseMenu != null && !_pauseMenu.open)
                    _pauseMenu = null;
            }
            else
                shouldPauseGameplay = false;
            for (int index = 0; index < closeMenuUpdate.Count; ++index)
            {
                UIComponent uiComponent = closeMenuUpdate[index];
                uiComponent.Update();
                if (!uiComponent.animating)
                {
                    closeMenuUpdate.RemoveAt(index);
                    --index;
                }
            }
            menuOpenedThisFrame = false;
            dontResetSelection = false;
        }

        public static void RetakePauseCapture() => _didPauseCapture = false;

        public static void CalculateModMemoryOffendersList()
        {
            List<ModConfiguration> list = loadedModsWithAssemblies.OrderByDescending(x => x.content == null ? -1L : x.content.kilobytesPreAllocated).ToList();
            bool flag = false;
            modMemoryOffendersString = "Mods taking up the most memory:\n";
            foreach (ModConfiguration modConfiguration in list)
            {
                long kilobytesPreAllocated = modConfiguration.content.kilobytesPreAllocated;
                if (kilobytesPreAllocated / 1000L > 20L)
                {
                    modMemoryOffendersString = modMemoryOffendersString + modConfiguration.displayName + " (" + (kilobytesPreAllocated / 1000L).ToString() + "MB)(ID:" + modConfiguration.workshopID.ToString() + ")\n";
                    flag = true;
                }
            }
            modMemoryOffendersString += "\n";
            if (flag)
                return;
            modMemoryOffendersString = "";
        }

        public void RunUpdate(GameTime gameTime)
        {
            ++Graphics.frame;
            Tasker.RunTasks();
            Graphics.GarbageDisposal(false);
            if (!_started)
            {
                //Input.Update();
                //DevConsole.Update();
                if (!disableSteam)
                {
                    if (Cloud.processing)
                    {
                        Cloud.Update();//return; unneded probly
                    }
                    if (steamConnectionCheckFail)
                    {
                        if (_loggedConnectionCheckFailure)
                        {
                            _loggedConnectionCheckFailure = true;
                            DevConsole.Log("|DGRED|Failed to initialize a connection to Steam.");
                        }
                    }
                    else if (Steam.IsInitialized() && Steam.IsRunningInitializeProcedures())
                    {
                        NloadMessage = "Loading Steam";
                        Steam.Update();//  why return lets just roll through itll be fine =);
                    }
                }
            }
            if (_canStartLoading && !_threadedLoadingStarted && _didFirstDraw)
            {
                PostCloudLogic();
                StartThreadedLoading();
            }
            if (_thingsToLoad.Count > 0)
            {
                TimeSpan elapsed;
                if (Program.isLinux)
                {
                    elapsed = _waitToStartLoadingTimer.elapsed;
                    if (elapsed.TotalMilliseconds <= 3500.0)
                        goto label_19;
                }
                _loadTimer.Restart();
               // MonoMain.loadMessage = "Things To Load " + _thingsToLoad.Count.ToString();
                while (_thingsToLoad.Count > 0)
                {
                    elapsed = _loadTimer.elapsed;
                    if (elapsed.TotalMilliseconds < 40.0)
                    {
 
                        currentActionQueue = _thingsToLoad;
                        LoadingAction loadingAction = _thingsToLoad.Peek();
                        NloadMessage = loadingAction.label;
                        if (loadingAction.Invoke())
                        {
                            _thingsToLoad.Dequeue();
                            //MonoMain.loadMessage = "Things To Load " + _thingsToLoad.Count.ToString();
                            //DevConsole.Log("_thingsToLoad left " + _thingsToLoad.Count.ToString());
                        }
                        else if (loadingAction.waiting)
                        {
                            break;
                        }
                    }
                    else
                        break;
                }
            }
        label_19:
            if (_doStart && !_started)
            {
                _doStart = false;
                Start();
            }
            if (!_started || Graphics.screenCapture != null)
                return;

            if (Graphics.inFocus)
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
            if (_started && !NetworkDebugger.enabled)
            {
                InputProfile.Update();
                Network.PreUpdate();
            }
            if (!Keyboard.alt && Keyboard.Pressed(Keys.F4) || Keyboard.alt && Keyboard.Pressed(Keys.Enter))
            {
                Options.Data.fullscreen ^= true;
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
            if (exit || (Graphics.inFocus && Keyboard.alt && Keyboard.Pressed(Keys.F4)))
            {
                KillEverything();
                Exit();
            }
            else
            {
                TouchScreen.Update();
                if (!NetworkDebugger.enabled)
                    DevConsole.Update();
                SFX.Update();
                Options.Update();
                InputProfile.repeat = Level.current is Editor || _pauseMenu != null || Editor.selectingLevel;
                Keyboard.repeat = Level.current is Editor || _pauseMenu != null || DevConsole.open || DuckNetwork.core.enteringText || Editor.enteringText;
                bool hasFocus = true;
                if (!NetworkDebugger.enabled)
                    UpdatePauseMenu(hasFocus);
                else
                    shouldPauseGameplay = false;
                if (transitionDirection != TransitionDirection.None)
                {
                    if (transitionLevel != null)
                    {
                        Graphics.fade = Lerp.Float(Graphics.fade, 0f, 0.05f);
                        if (Graphics.fade <= 0.0)
                        {
                            Level.current = transitionLevel;
                            transitionLevel = null;
                            transitionDirection = TransitionDirection.None;
                        }
                    }
                    else
                    {
                        Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.1f);
                        if (Graphics.fade >= 1.0)
                        {
                            transitionLevel = null;
                            transitionDirection = TransitionDirection.None;
                        }
                    }
                    shouldPauseGameplay = true;
                }
                RumbleManager.Update();
                if (!shouldPauseGameplay)
                {
                    if (_pauseMenu == null)
                        _didPauseCapture = false;
                    if (!_recordingStarted || _recordData)
                    {
                        if (DevConsole.rhythmMode && Level.current is GameLevel)
                        {
                            RhythmMode.TickSound((float)(((float)(Music.position + new TimeSpan(0, 0, 0, 0, 80)).TotalMinutes * 140f) % 1.0 / 1.0));
                            RhythmMode.Tick((float)(((float)(Music.position + new TimeSpan(0, 0, 0, 0, 40)).TotalMinutes * 140f) % 1.0 / 1.0));
                        }
                        foreach (IEngineUpdatable engineUpdatable in core.engineUpdatables)
                            engineUpdatable.PreUpdate();
                        AutoUpdatables.Update();
                        DuckGame.Content.Update();
                        Music.Update();
                        Level.UpdateLevelChange();
                        Level.UpdateCurrentLevel();
                        foreach (IEngineUpdatable engineUpdatable in core.engineUpdatables)
                            engineUpdatable.Update();
                        OnUpdate();
                    }
                }
                Graphics.RunRenderTasks();
                Input.ignoreInput = false;
                base.Update(gameTime);
                FPSCounter.Tick(0);
                if (!NetworkDebugger.enabled)
                    Network.PostUpdate();
                foreach (IEngineUpdatable engineUpdatable in core.engineUpdatables)
                    engineUpdatable.PostUpdate();
            }
        }

        protected virtual void OnUpdate()
        {
        }

        public static void RenderGame(RenderTarget2D target)
        {
            int width = Graphics.width;
            int height = Graphics.height;
            Graphics.SetRenderTarget(target);
            Viewport viewport = new Viewport();
            viewport.X = viewport.Y = 0;
            viewport.Width = target.width;
            viewport.Height = target.height;
            viewport.MinDepth = 0f;
            viewport.MaxDepth = 1f;
            Graphics.viewport = viewport;
            Graphics.width = target.width;
            Graphics.height = target.height;
            Level.DrawCurrentLevel();
            Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
            instance.OnDraw();
            Graphics.screen.End();
            Graphics.width = width;
            Graphics.height = height;
            Graphics.SetRenderTarget(null);
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
            NtQueryTimerResolution(out int _, out int _, out CurrentResolution);
            return CurrentResolution;
        }

        /// <summary>
        /// Sleeps as long as possible without exceeding the specified period
        /// </summary>
        public static void SleepForNoMoreThan(double milliseconds)
        {
            if (LowestSleepThreshold == 0.0)
            {
                int MinimumResolution;
                int MaximumResolution;
                int CurrentResolution;
                NtQueryTimerResolution(out MinimumResolution, out MaximumResolution, out CurrentResolution);
                LowestSleepThreshold = 1.0 + MaximumResolution / 10000.0;
                DevConsole.Log(DCSection.General, "TIMER RES(" + MinimumResolution.ToString() + ", " + MaximumResolution.ToString() + ", " + CurrentResolution.ToString() + ")");
            }
            if (milliseconds < LowestSleepThreshold)
                return;
            int millisecondsTimeout = (int)(milliseconds - GetCurrentResolution());
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
        //    int num3 = (int)(365.25 * (num1 + 4712));
        //    int num4 = (int)(30.6001 * num2 + 0.5);
        //    int num5 = (int)((num1 / 100 + 49) * 0.75) - 38;
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
        //    moonInfo.phase = (num + 4.867) / 29.53059;
        //    moonInfo.phase -= Math.Floor(moonInfo.phase);
        //    moonInfo.age = moonInfo.phase >= 0.5 ? moonInfo.phase * 29.53059 - 14.765295 : moonInfo.phase * 29.53059 + 14.765295;
        //    moonInfo.age = Math.Floor(moonInfo.age) + 1.0;
        //    return moonInfo;
        //}

        public static bool FullMoon
        {
            get
            {
                double num = (DateTime.UtcNow - new DateTime(1900, 1, 1)).TotalDays % 29.530588853;// Not a Floating point error as far as i know
                return DateTime.Now.Hour < 1 && num > 13.0 && num < 17.0;
            }
        }
        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        protected override void Draw(GameTime gameTime)
        {
            int num = started ? 1 : 0;
            ++framesSinceFocusChange;
            if (loseDevice > 0)
            {
                Graphics.Clear(Color.Black);
                GraphicsDevice.SetRenderTarget(null);
                --loseDevice;
                base.Draw(gameTime);
            }
            else
            {
                if (GraphicsDevice.IsDisposed)
                    return;
                Graphics.drawing = true;
                --SynchronizedContentManager.blockLoading;
                if (SynchronizedContentManager.blockLoading < 0)
                    SynchronizedContentManager.blockLoading = 0;
                try
                {
                    Graphics.device = GraphicsDevice;
                    if (Resolution.Update())
                    {
                        Graphics.Clear(Color.Black);
                        GraphicsDevice.SetRenderTarget(null);
                        base.Draw(gameTime);
                        Graphics.drawing = false;
                    }
                    else
                    {
                        RunDraw(gameTime);
                        Graphics.SetRenderTargetToScreen();
                        if (Graphics._screenBufferTarget != null)
                        {
                            _tempRecordingReference = Recorder.currentRecording;
                            Recorder.currentRecording = null;
                            Graphics.SetScreenTargetViewport();
                            Graphics.Clear(Color.Black);
                            Camera camera = new Camera(0f, 0f, Graphics._screenBufferTarget.width, Graphics._screenBufferTarget.height);
                            Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, camera.getMatrix());
                            Graphics.Draw(Graphics._screenBufferTarget, 0f, 0f, 1f, 1f);
                            Graphics.screen.End();
                            Recorder.currentRecording = _tempRecordingReference;
                        }
                        Graphics.UpdateScreenViewport();
                        GraphicsDevice.SetRenderTarget(null);
                        base.Draw(gameTime);
                        Graphics.drawing = false;
                    }
                }
                catch (Exception ex)
                {
                    Program.HandleGameCrash(ex);
                }
            }
        }

        static Stack<string> loadMessages = new();
        public static string lastLoadMessage = "";

        protected void RunDraw(GameTime gameTime)
        {
            FPSCounter.Tick(1);
            _didFirstDraw = true;
            Graphics.frameFlipFlop = !Graphics.frameFlipFlop;
            if (Graphics.device.IsDisposed)
                return;

            Graphics.SetScissorRectangle(new Rectangle(0f, 0f, Graphics.width, Graphics.height));
            if (Recorder.currentRecording != null)
                Recorder.currentRecording.NextFrame();
            if (!_started) 
            {
                ++_loadingFramesRendered;
                Graphics.SetRenderTarget(null);
                _pauseMaterial = new MaterialPause();
                if (!_setCulture)
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    _setCulture = true;
                }
                Graphics.Clear(new Color(0, 0, 0));
                //if (Layer.Console != null)
                //{
                //    Layer.Console.Begin(true);
                //    DevConsole.Draw();
                //    //Level.current.PostDrawLayer(Layer.Console);
                //    Layer.Console.End(true);
                //}
                if (!DuckGame.Content.didsetbigboi && Program.shouldusespriteatlas)
                {
                    DuckGame.Content.didsetbigboi = true;
                    DuckGame.Content.offests = new Dictionary<string, Microsoft.Xna.Framework.Rectangle>();
                    if (File.Exists(@"../spriteatlas.png"))
                    {
                        DevConsole.Log("loading ../spriteatlass.png");
                        DuckGame.Content.Thick = (Tex2D)DuckGame.Content.SpriteAtlasTextureFromStream(@"../spriteatlas.png", Graphics.device);
                        DuckGame.Content.Thick.Namebase = "SpriteAtlas";

                        //RSplit("de mo", ' ', -1);
                        string[] lines = File.ReadAllLines(@"../spriteatlas_offsets.txt");
                        foreach (string line in lines)
                        {
                            try
                            {
                                List<string> texturedetails = DuckGame.Content.RSplit(line, ' ', 4);
                                string texturename = texturedetails[0];
                                int x = Int32.Parse(texturedetails[1]);
                                int y = Int32.Parse(texturedetails[2]);
                                int height = Int32.Parse(texturedetails[3]);
                                int width = Int32.Parse(texturedetails[4]);

                                DuckGame.Content.offests.Add(texturename, new Microsoft.Xna.Framework.Rectangle(x, y, width, height));
                            }
                            catch
                            {

                            }
                        }
                    }
                    else if (Directory.Exists(Program.GameDirectory + "spriteatlas") && File.Exists(Program.GameDirectory + "spriteatlas/spriteatlas.png"))
                    {
                        DevConsole.Log("loading " + Program.GameDirectory + "spriteatlas/spriteatlas.png");
                        DuckGame.Content.Thick = (Tex2D)DuckGame.Content.SpriteAtlasTextureFromStream(Program.GameDirectory + "spriteatlas/spriteatlas.png", Graphics.device);
                        DuckGame.Content.Thick.Namebase = "SpriteAtlas";

                        //RSplit("de mo", ' ', -1);
                        string[] lines = File.ReadAllLines(Program.GameDirectory + "spriteatlas/spriteatlas_offsets.txt");
                        foreach (string line in lines)
                        {
                            try
                            {
                                List<string> texturedetails = DuckGame.Content.RSplit(line, ' ', 4);
                                string texturename = texturedetails[0];
                                int x = Int32.Parse(texturedetails[1]);
                                int y = Int32.Parse(texturedetails[2]);
                                int height = Int32.Parse(texturedetails[3]);
                                int width = Int32.Parse(texturedetails[4]);

                                DuckGame.Content.offests.Add(texturename, new Microsoft.Xna.Framework.Rectangle(x, y, width, height));
                            }
                            catch
                            {

                            }
                        }
                    }

                }
                Camera camera = new Camera(0f, 0f, Graphics.width, Graphics.height);
                Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, camera.getMatrix());
                Vec2 p1 = new Vec2(50f, Graphics.height - 50);
                Vec2 vec2_1 = new Vec2(Graphics.width - 100, 20f);
                Graphics.DrawRect(p1, p1 + vec2_1, Color.DarkGray * 0.1f, (Depth)0.5f);
                float loaded = loadyBits / (float)totalLoadyBits;
				if (loaded > 1f)
				{
					loaded = 1f;
				}
                if (loadMessages.Count == 0)
                {
                    NloadMessage = NloadMessage;
                }
                if (Program.gay)
                {
                    int offset = 0;
                    for (int i = 0; i < p1.y - p1.y + vec2_1.y; i++)
                    {
                        if (i - offset >= Colors.Rainbow.Length)
                        {
                            offset += Colors.Rainbow.Length;
                            // i = 0;
                        }
                        Graphics.DrawLine(new Vec2(p1.x, p1.y + i), p1 + new Vec2(vec2_1.x * loaded, vec2_1.y + i - 20), Colors.Rainbow[i - offset]);
                    }
                }
                else if (Debugger.IsAttached)
                {
                    Graphics.DrawRect(p1, p1 + new Vec2(vec2_1.x * loaded, vec2_1.y), Color.Green, (Depth)0.6f);
                }
                else
                {
                    Graphics.DrawRect(p1, p1 + new Vec2(vec2_1.x * loaded, vec2_1.y), Color.Red, (Depth)0.6f);
                }
                //string text = loadMessage;
                //if (loadMessage != lastLoadMessage)
                //{
                //    loadMessages.Push(lastLoadMessage = loadMessage);
                //}
                float textPadding = -24f;
                if (Cloud.processing && Cloud.progress != 0.0 && Cloud.progress != 1.0)
                {
                    Graphics.DrawString("Synchronizing Steam Cloud... (" + ((int)(Cloud.progress * 100.0)).ToString() + "%)", p1 + new Vec2(0f, textPadding), Color.White, (Depth)1f, scale: 2f);
                    textPadding -= 20;
                }
                if (loadMessage != lastLoadMessage)
                {
                    NloadMessage = loadMessage;
                }
                //if (text != loadMessage)
                //{
                //    Graphics.DrawString(text, p1 + new Vec2(0f, textPadding), Color.White, (Depth)1f, scale: 2f);
                //    textPadding -= 20;
                //}
                foreach (string i in loadMessages)
                {
                    Graphics.DrawString(i, p1 + new Vec2(0f, textPadding), Color.White, (Depth)1f, scale: 2f);
                    textPadding -= 20;
                }
                _duckRun.speed = 0.15f;
                _duckRun.scale = new Vec2(4f, 4f);
                _duckRun.depth = 0.7f;
                _duckRun.color = new Color(80, 80, 80);
                if (_timeSinceLastLoadFrame.elapsed.Milliseconds > 16)
                    ++_duckRun.frame;
                Vec2 vec2_2 = new Vec2(Graphics.width - _duckRun.width * 4 - 50, Graphics.height - _duckRun.height * 4 - 55);
                Graphics.Draw(_duckRun, vec2_2.x, vec2_2.y);
                _duckArm.frame = _duckRun.imageIndex;
                _duckArm.scale = new Vec2(4f, 4f);
                _duckArm.depth = 0.8f;
                _duckArm.color = new Color(80, 80, 80);
                Graphics.Draw(_duckArm, vec2_2.x + 20f, vec2_2.y + 56f);
                Graphics.screen.End();
                _timeSinceLastLoadFrame.Restart();
                //if (Layer.Console != null)
                //{
                //    Layer.Console.Begin(true);
                //    DevConsole.Draw();
                //    //Level.current.PostDrawLayer(Layer.Console);
                //    Layer.Console.End(true);
                //}
            }
            else
            {
                if (Level.current == null)
                    return;
                Reflection.Render();
                if (!takingShot)
                {
                    takingShot = true;
                    if (Keyboard.shift && Keyboard.Pressed(Keys.F12) || waitFrames < 0)
                    {
                        if (_screenshotTarget == null)
                            _screenshotTarget = new RenderTarget2D(Graphics.width, Graphics.height, true);
                        Graphics.screenCapture = _screenshotTarget;
                        RunDraw(gameTime);
                        waitFrames = 60 + Rando.Int(60);
                        SFX.Play("ching");
                    }
                    takingShot = false;
                }
                if (_pauseMenu != null && !NetworkDebugger.enabled && !_didPauseCapture)
                {
                    Graphics.screenCapture = _screenCapture;
                    _didPauseCapture = true;
                }
                if (Graphics.screenCapture != null)
                {
                    int width = Graphics.width;
                    int height = Graphics.height;
                    Graphics.SetRenderTarget(Graphics.screenCapture);
                    Graphics.UpdateScreenViewport(true);
                    HUD.hide = true;
                    Level.DrawCurrentLevel();
                    Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                    OnDraw();
                    Graphics.screen.End();
                    HUD.hide = false;
                    Graphics.screenCapture = null;
                    Graphics.width = width;
                    Graphics.height = height;
                    Graphics.SetRenderTarget(null);
                }
                if (_screenshotTarget != null)
                {
                    saveShot = _screenshotTarget;
                    _screenshotTarget = null;
                    SaveShot();
                }
                if (Graphics.screenTarget != null)
                {
                    RenderDelegates.BeforeScreen?.Invoke();
                    
                    int width = Graphics.width;
                    int height = Graphics.height;
                    Graphics.SetRenderTarget(Graphics.screenTarget);
                    Graphics.UpdateScreenViewport();
                    HUD.hide = true;
                    Level.DrawCurrentLevel();
                    Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                    OnDraw();
                    Graphics.screen.End();
                    HUD.hide = false;
                    Graphics.width = width;
                    Graphics.height = height;
                    Graphics.SetRenderTarget(null);
                    Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
                    Graphics.Draw(Graphics.screenTarget, Vec2.Zero, new Rectangle?(), Color.White, 0f, Vec2.Zero, Vec2.One, SpriteEffects.None);
                    Graphics.screen.End();
                    
                    RenderDelegates.AfterScreen?.Invoke();
                }
                else
                {
                    bool flag = true;
                    if (Network.isActive)
                        flag = false;
                    if (_pauseMenu != null && _didPauseCapture && Graphics.screenCapture == null)
                    {
                        Graphics.SetRenderTarget(null);
                        Graphics.Clear(Color.Black * Graphics.fade);
                        if (autoPauseFade)
                        {
                            _pauseMaterial.fade = Lerp.FloatSmooth(_pauseMaterial.fade, doPauseFade ? 0.6f : 0f, 0.1f, 1.1f);
                            _pauseMaterial.dim = Lerp.FloatSmooth(_pauseMaterial.dim, doPauseFade ? 0.6f : 1f, 0.1f, 1.1f);
                        }
                        Graphics.SetFullViewport();
                        Vec2 vec2 = new Vec2(Layer.HUD.camera.width / _screenCapture.width, Layer.HUD.camera.height / _screenCapture.height);
                        Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone, null, Matrix.Identity);
                        Graphics.material = _pauseMaterial;
                        Graphics.Draw(_screenCapture, new Vec2(0f, 0f), new Rectangle?(), new Color(120, 120, 120), 0f, Vec2.Zero, new Vec2(1f, 1f), SpriteEffects.None, -0.9f);
                        Graphics.material = null;
                        Graphics.screen.End();
                        Graphics.RestoreOldViewport();
                        Layer.HUD.Begin(true);
                        _pauseMenu.Draw();
                        for (int index = 0; index < closeMenuUpdate.Count; ++index)
                            closeMenuUpdate[index].Draw();
                        HUD.Draw();
                        if (Level.current.drawsOverPauseMenu)
                            Level.current.PostDrawLayer(Layer.HUD);
                        Layer.HUD.End(true);
                        Layer.Console.Begin(true);
                        DevConsole.Draw();
                        Level.current.PostDrawLayer(Layer.Console);
                        Layer.Console.End(true);
                        if (!flag && UIMatchmakerMark2.instance == null)
                            _didPauseCapture = false;
                    }
                    else
                    {
                        if (autoPauseFade)
                        {
                            _pauseMaterial.fade = 0f;
                            _pauseMaterial.dim = 0.6f;
                        }
                        Graphics.SetRenderTarget(null);
                        Level.DrawCurrentLevel();
                        Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Resolution.getTransformationMatrix());
                        OnDraw();
                        Graphics.screen.End();
                        if (closeMenuUpdate.Count > 0)
                        {
                            Layer.HUD.Begin(true);
                            foreach (Thing thing in closeMenuUpdate)
                                thing.DoDraw();
                            Layer.HUD.End(true);
                        }
                    }
                    if (!DevConsole.showFPS)
                        return;
                    FPSCounter.Render(Graphics.device, index: 0, label: "UPS");
                    FPSCounter.Render(Graphics.device, 100f, index: 1);
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

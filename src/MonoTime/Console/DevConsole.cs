// Decompiled with JetBrains decompiler
// Type: DuckGame.DevConsole
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DuckGame
{
    public class DevConsole
    {
        public static bool showFPS = false;
        public static List<string> startupCommands = new List<string>();
        public static bool fancyMode = false;
        private static DevConsoleCore _core = new DevConsoleCore();
        private static bool _enableNetworkDebugging = false;
        private static bool _oldConsole;
        public static bool debugOrigin;
        public static bool debugBounds;
        private static RasterFont _raster;
        public static Dictionary<string, List<CMD>> commands = new Dictionary<string, List<CMD>>();
        public static CMD lastCommand;
        public static bool wagnusDebug;
        public static bool fuckUpPacketOrder = false;
        public static List<DCLine> debuggerLines = new List<DCLine>();
        private static bool _doDataSubmission = false;
        private static string _dataSubmissionMessage = null;
        private static List<ulong> lostSaveIDs = new List<ulong>()
    {
      76561198035257896UL
    };
        public static Sprite _tray;
        public static Sprite _scan;
        private static Queue<DevConsole.QueuedCommand> _pendingCommandQueue = new Queue<DevConsole.QueuedCommand>();

        public static void SubmitSaveData(string pMessage)
        {
            byte[] array;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    int count = Steam.FileGetCount();
                    for (int file = 0; file < count; ++file)
                    {
                        string name = Steam.FileGetName(file);
                        if (!name.EndsWith(".lev") && !name.EndsWith(".png") && !name.EndsWith(".play"))
                        {
                            using (Stream stream = zipArchive.CreateEntry(name).Open())
                            {
                                byte[] buffer = Steam.FileRead(name);
                                stream.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }
                    string str1 = "DirectInputDevices:\n===================================\n";
                    for (int index = 0; index < 32; ++index)
                    {
                        try
                        {
                            if (DInput.GetState(index) != null)
                            {
                                string str2 = DInput.GetProductName(index) + DInput.GetProductGUID(index) + " " + DInput.IsXInput(index).ToString();
                                str1 = str1 + str2 + "\n";
                            }
                            else
                                break;
                        }
                        catch (Exception ex)
                        {
                            str1 = str1 + "\n" + ex.ToString() + "\n";
                        }
                    }
                    string str3 = str1 + "\nEnumerated Input Devices:\n===================================\n";
                    foreach (InputDevice inputDevice in Input.GetInputDevices())
                    {
                        if (inputDevice.isConnected)
                        {
                            try
                            {
                                str3 = str3 + inputDevice.productName + inputDevice.productGUID + " (" + inputDevice.ToString() + ", " + inputDevice.inputDeviceType.ToString() + ")\n";
                            }
                            catch (Exception ex)
                            {
                                str3 = str3 + "\n" + ex.ToString() + "\n";
                            }
                        }
                    }
                    using (Stream stream = zipArchive.CreateEntry("input_device_report.txt").Open())
                    {
                        using (StreamWriter streamWriter = new StreamWriter(stream))
                        {
                            streamWriter.Write(str3);
                            streamWriter.Flush();
                        }
                    }
                    using (Stream stream = zipArchive.CreateEntry("dg_details.txt").Open())
                    {
                        using (StreamWriter streamWriter = new StreamWriter(stream))
                        {
                            streamWriter.Write(MonoMain.GetDetails());
                            streamWriter.Write("Console Contents:\n");
                            for (int index = 0; index < 100; ++index)
                            {
                                if (index < DevConsole.core.lines.Count)
                                    streamWriter.Write(DevConsole.core.lines.ElementAt<DCLine>(index)?.ToString() + "\n");
                            }
                            streamWriter.Flush();
                        }
                    }
                }
                array = memoryStream.ToArray();
            }
            if (array == null)
                return;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.wonthelp.info/DuckWeb/submitSave.php");
            httpWebRequest.Method = "POST";
            string str = "sendRequest=DGBugLogger" + "&steamID=" + CrashWindow.CrashWindow.SQLEncode(Steam.user.id.ToString());
            byte[] bytes = Encoding.UTF8.GetBytes((pMessage == null ? str + "&steamName=" + CrashWindow.CrashWindow.SQLEncode(Steam.user.name) : str + "&steamName=" + CrashWindow.CrashWindow.SQLEncode(Steam.user.name + "(" + pMessage + ")")) + "&data=" + CrashWindow.CrashWindow.SQLEncode(Editor.BytesToString(array)));
            httpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            httpWebRequest.ContentLength = bytes.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
            DevConsole.LogComplexMessage(response.StatusDescription, Colors.DGBlue);
            using (Stream responseStream = response.GetResponseStream())
                DevConsole.LogComplexMessage(new StreamReader(responseStream).ReadToEnd(), Colors.DGBlue);
        }

        public static DevConsoleCore core
        {
            get => DevConsole._core;
            set => DevConsole._core = value;
        }

        public static bool open => DevConsole._core.open;

        public static void SuppressDevConsole()
        {
            DevConsole._oldConsole = DevConsole._enableNetworkDebugging;
            DevConsole._enableNetworkDebugging = false;
        }

        public static void RestoreDevConsole() => DevConsole._enableNetworkDebugging = DevConsole._oldConsole;

        public static bool enableNetworkDebugging
        {
            get => DevConsole._enableNetworkDebugging;
            set => DevConsole._enableNetworkDebugging = value;
        }

        public static bool splitScreen
        {
            get => DevConsole._core.splitScreen;
            set => DevConsole._core.splitScreen = value;
        }

        public static bool rhythmMode
        {
            get => DevConsole._core.rhythmMode;
            set => DevConsole._core.rhythmMode = value;
        }

        public static bool qwopMode
        {
            get => DevConsole._core.qwopMode;
            set => DevConsole._core.qwopMode = value;
        }

        public static bool showIslands
        {
            get => DevConsole._core.showIslands;
            set => DevConsole._core.showIslands = value;
        }

        public static bool showCollision
        {
            get => DevConsole._core.showCollision && !Network.isActive;
            set => DevConsole._core.showCollision = value;
        }

        public static bool shieldMode
        {
            get => DevConsole._core.shieldMode;
            set => DevConsole._core.shieldMode = value;
        }

        public static void DrawLine(Vec2 pos, DCLine line, bool times, bool section)
        {
            string str1 = "" + line.timestamp.Minute.ToString();
            if (str1.Length == 1)
                str1 = " " + str1;
            string str2 = str1 + ":";
            if (line.timestamp.Second < 10)
                str2 += "0";
            string str3 = str2 + line.timestamp.Second.ToString();
            DevConsole.core.font.scale = new Vec2(1f);
            DevConsole.core.font.Draw((times ? "|GRAY|" + str3 + " " : "") + (section ? line.SectionString(small: true) : "") + line.line, pos.x, pos.y, line.color * 0.8f, (Depth)0.9f);
            DevConsole.core.font.scale = new Vec2(2f);
        }

        public static Vec2 size => new Vec2(1280f, 1280f / Resolution.current.aspect);

        public static void InitializeFont()
        {
            if (Options.Data.consoleFont == "" || Options.Data.consoleFont == null)
            {
                DevConsole._raster = null;
            }
            else
            {
                if (DevConsole._raster != null)
                    return;
                DevConsole._raster = new RasterFont(Options.Data.consoleFont, Options.Data.consoleFontSize);
            }
        }

        public static void Draw()
        {
            if (Layer.core._console != null)
            {
                Layer.core._console.camera.width = Resolution.current.x / 2;
                Layer.core._console.camera.height = Resolution.current.y / 2;
            }
            if (DevConsole._core.font == null)
            {
                DevConsole._core.font = new BitmapFont("biosFont", 8)
                {
                    scale = new Vec2(2f, 2f)
                };
                DevConsole._core.fancyFont = new FancyBitmapFont("smallFont")
                {
                    scale = new Vec2(2f, 2f)
                };
            }
            if (_core.alpha <= 0.00999999977648258)
                return;
            DevConsole.InitializeFont();
            if (DevConsole._tray == null)
                return;
            DevConsole._tray.alpha = DevConsole._core.alpha;
            DevConsole._tray.scale = new Vec2((float)(Math.Round(Resolution.current.x / 1280.0 * 2.0) / 2.0) * 2f) * (DevConsole.consoleScale + 1) / 2f;
            DevConsole._tray.depth = (Depth)0.75f;
            int num1 = (int)((double)Layer.core._console.camera.height * dimensions.y / (16.0 * _tray.scale.y)) - 2;
            int num2 = (int)((double)Layer.core._console.camera.width * dimensions.x / (16.0 * _tray.scale.x)) - 2;
            Graphics.Draw(DevConsole._tray, 0.0f, 0.0f, new Rectangle(0.0f, 0.0f, 18f, 18f));
            Graphics.Draw(DevConsole._tray, 0.0f, (float)(18.0 * _tray.scale.y + num1 * (16.0 * _tray.scale.y)), new Rectangle(0.0f, DevConsole._tray.height - 18, 18f, 18f));
            Graphics.Draw(DevConsole._tray, (float)(18.0 * _tray.scale.x + (num2 - 6) * (16.0 * _tray.scale.x)), (float)(18.0 * _tray.scale.y + num1 * (16.0 * _tray.scale.y)), new Rectangle(DevConsole._tray.width - 114, DevConsole._tray.height - 18, 114f, 18f));
            for (int index = 0; index < num2; ++index)
            {
                Graphics.Draw(DevConsole._tray, (float)(18.0 * _tray.scale.x + 16.0 * _tray.scale.x * index), 0.0f, new Rectangle(16f, 0.0f, 16f, 18f));
                if (index < num2 - 6)
                    Graphics.Draw(DevConsole._tray, (float)(18.0 * _tray.scale.x + 16.0 * _tray.scale.x * index), (float)(18.0 * _tray.scale.y + num1 * (16.0 * _tray.scale.y)), new Rectangle(16f, DevConsole._tray.height - 18, 16f, 18f));
            }
            Graphics.Draw(DevConsole._tray, (float)(18.0 * _tray.scale.x + num2 * (16.0 * _tray.scale.x)), 0.0f, new Rectangle(DevConsole._tray.width - 18, 0.0f, 18f, 18f));
            for (int index = 0; index < num1; ++index)
            {
                Graphics.Draw(DevConsole._tray, 0.0f, (float)(18.0 * _tray.scale.y + 16.0 * _tray.scale.y * index), new Rectangle(0.0f, 18f, 18f, 16f));
                Graphics.Draw(DevConsole._tray, (float)(18.0 * _tray.scale.x + num2 * (16.0 * _tray.scale.x)), (float)(18.0 * _tray.scale.y + 16.0 * _tray.scale.y * index), new Rectangle(DevConsole._tray.width - 18, 18f, 18f, 16f));
            }
            Graphics.DrawRect(Vec2.Zero, new Vec2((float)(18.0 * _tray.scale.x + num2 * (16.0 * _tray.scale.x) + _tray.scale.y * 4.0), (num1 + 2) * (16f * DevConsole._tray.scale.y)), Color.Black * 0.8f * DevConsole._core.alpha, (Depth)0.7f);
            DevConsole._core.fancyFont.scale = new Vec2(DevConsole._tray.scale.x / 2f);
            DevConsole._core.fancyFont.depth = (Depth)0.98f;
            DevConsole._core.fancyFont.alpha = DevConsole._core.alpha;
            float num3 = (float)((num1 + 1) * 16 * (double)DevConsole._tray.scale.y + 5.0 * _tray.scale.y);
            float num4 = (num2 + 2) * (16f * DevConsole._tray.scale.x);
            string version = DG.version;
            DevConsole._core.fancyFont.Draw(version, new Vec2((float)(82.0 * _tray.scale.x + (num2 - 6) * (16.0 * _tray.scale.x)), num3 + 7f * DevConsole._tray.scale.y), new Color(62, 114, 122), (Depth)0.98f);
            DevConsole._core.cursorPosition = Math.Min(Math.Max(DevConsole._core.cursorPosition, 0), DevConsole._core.typing.Length);
            if (DevConsole._raster != null)
            {
                DevConsole._raster.scale = new Vec2(0.5f);
                DevConsole._raster.alpha = DevConsole._core.alpha;
                DevConsole._raster.Draw(DevConsole._core.typing, 4f * DevConsole._tray.scale.x, (float)((double)num3 + _tray.scale.y * 8.0 - _raster.characterHeight * (double)DevConsole._raster.scale.y / 2.0), Color.White, (Depth)0.9f);
                Vec2 p1 = new Vec2((float)((double)DevConsole._raster.GetWidth(DevConsole._core.typing.Substring(0, DevConsole._core.cursorPosition)) + 4.0 * _tray.scale.x + 1.0), num3 + 6f * DevConsole._tray.scale.y);
                Graphics.DrawLine(p1, p1 + new Vec2(0.0f, 4f * DevConsole._tray.scale.x), Color.White, depth: ((Depth)1f));
            }
            else
            {
                DevConsole._core.font.scale = new Vec2(DevConsole._tray.scale.x / 2f);
                DevConsole._core.font.alpha = DevConsole._core.alpha;
                DevConsole._core.font.Draw(DevConsole._core.typing, 4f * DevConsole._tray.scale.x, num3 + 6f * DevConsole._tray.scale.y, Color.White, (Depth)0.9f);
                Vec2 p1 = new Vec2(DevConsole._core.font.GetWidth(DevConsole._core.typing.Substring(0, DevConsole._core.cursorPosition)) + 4f * DevConsole._tray.scale.x, num3 + 6f * DevConsole._tray.scale.y);
                Graphics.DrawLine(p1, p1 + new Vec2(0.0f, 4f * DevConsole._tray.scale.x), Color.White, 2f, (Depth)1f);
            }
            int index1 = DevConsole._core.lines.Count - 1 - DevConsole._core.viewOffset;
            float num5 = 0.0f;
            DevConsole._core.font.scale = new Vec2((float)Math.Max(Math.Round(_tray.scale.x / 4.0), 1.0));
            float num6 = DevConsole._core.font.scale.x / 2f;
            float num7 = 18f * num6;
            float num8 = (float)(20.0 * (_core.font.scale.x * 2.0));
            if (DevConsole._raster != null)
            {
                num7 = (DevConsole._raster.characterHeight - 2) * DevConsole._raster.scale.y;
                num5 = num7;
                num8 = DevConsole._raster.GetWidth("0000  ");
            }
            for (int index2 = 0; index2 < ((double)num3 - 2.0 * _tray.scale.y) / (double)num7 - 1.0 && index1 >= 0; ++index2)
            {
                DCLine dcLine = DevConsole._core.lines.ElementAt<DCLine>(index1);
                string text = index1.ToString();
                while (text.Length < 4)
                    text = "0" + text;
                if (DevConsole._raster != null)
                {
                    DevConsole._raster.maxWidth = (int)((double)num4 - 35.0 * _tray.scale.x);
                    DevConsole._raster.singleLine = true;
                    DevConsole._raster.enforceWidthByWord = false;
                    DevConsole._raster.Draw(text, 4f * DevConsole._tray.scale.x, (float)((double)num3 - (double)num5 + 2.0), index1 % 2 > 0 ? Color.Gray * 0.4f : Color.Gray * 0.6f, (Depth)0.9f);
                    DevConsole._raster.Draw(dcLine.SectionString() + dcLine.line, 4f * DevConsole._tray.scale.x + num8, (float)((double)num3 - (double)num5 + 2.0), dcLine.color, (Depth)0.9f);
                    num5 += num7;
                }
                else
                {
                    DevConsole._core.font.maxWidth = (int)((double)num4 - 35.0 * _tray.scale.x);
                    DevConsole._core.font.singleLine = true;
                    DevConsole._core.font.enforceWidthByWord = false;
                    DevConsole._core.font.Draw(text, 4f * DevConsole._tray.scale.x, (float)((double)num3 - 18.0 * (double)num6 - (double)num5 + 2.0), index1 % 2 > 0 ? Color.Gray * 0.4f : Color.Gray * 0.6f, (Depth)0.9f);
                    DevConsole._core.font.Draw(dcLine.SectionString() + dcLine.line, 4f * DevConsole._tray.scale.x + num8, (float)((double)num3 - 18.0 * (double)num6 - (double)num5 + 2.0), dcLine.color * 0.8f, (Depth)0.9f);
                    num5 += 18f * num6;
                }
                --index1;
            }
            DevConsole._core.font.scale = new Vec2(2f);
        }

        public static Vec2 dimensions => new Vec2(Options.Data.consoleWidth / 100f, Options.Data.consoleHeight / 100f);

        public static int consoleScale => Options.Data.consoleScale;

        public static int fontPoints => Options.Data.consoleFontSize;

        public static string fontName => Options.Data.consoleFont;

        public static Profile ProfileByName(string findName)
        {
            foreach (Profile profile in Profiles.all)
            {
                if (profile.team != null)
                {
                    string str = profile.name.ToLower();
                    if (findName == "player1" && profile.inputProfile == InputProfile.Get(InputProfile.MPPlayer1))
                        str = findName;
                    else if (findName == "player2" && profile.inputProfile == InputProfile.Get(InputProfile.MPPlayer2))
                        str = findName;
                    else if (findName == "player3" && profile.inputProfile == InputProfile.Get(InputProfile.MPPlayer3))
                        str = findName;
                    else if (findName == "player4" && profile.inputProfile == InputProfile.Get(InputProfile.MPPlayer4))
                        str = findName;
                    else if (findName == "player5" && profile.inputProfile == InputProfile.Get(InputProfile.MPPlayer5))
                        str = findName;
                    else if (findName == "player6" && profile.inputProfile == InputProfile.Get(InputProfile.MPPlayer6))
                        str = findName;
                    else if (findName == "player7" && profile.inputProfile == InputProfile.Get(InputProfile.MPPlayer7))
                        str = findName;
                    else if (findName == "player8" && profile.inputProfile == InputProfile.Get(InputProfile.MPPlayer8))
                        str = findName;
                    if (str == findName)
                        return profile;
                }
            }
            return null;
        }

        public static void AddCommand(CMD pCommand)
        {
            DevConsole.GetCommands(pCommand.keyword).Add(pCommand);
            if (pCommand.aliases == null)
                return;
            foreach (string alias in pCommand.aliases)
                DevConsole.GetCommands(alias).Add(pCommand);
        }

        public static List<CMD> GetCommands(string pKeyword)
        {
            List<CMD> commands;
            if (!DevConsole.commands.TryGetValue(pKeyword, out commands))
                DevConsole.commands[pKeyword] = commands = new List<CMD>();
            return commands;
        }

        public static void RunCommand(string command)
        {
            if (DG.buildExpired)
                return;
            if (DevConsole._doDataSubmission)
            {
                DevConsole._dataSubmissionMessage = command;
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "Submitting save data, this could take some time...",
                    color = Color.White
                });
            }
            else
            {
                DevConsole._core.logScores = -1;
                if (!(command != ""))
                    return;
                CultureInfo currentCulture = CultureInfo.CurrentCulture;
                bool flag1 = false;
                ConsoleCommand consoleCommand1 = new ConsoleCommand(command);
                string pKeyword = consoleCommand1.NextWord();
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = command,
                    color = Color.White
                });
                string str1 = null;
                int num = int.MinValue;
                string str2 = "";
                foreach (CMD command1 in DevConsole.GetCommands(pKeyword))
                {
                    CMD cmd = command1;
                    flag1 = true;
                    ConsoleCommand consoleCommand2;
                    for (consoleCommand2 = new ConsoleCommand(consoleCommand1.Remainder()); cmd.subcommand != null && consoleCommand2.NextWord(peek: true) == cmd.subcommand.keyword; cmd = cmd.subcommand)
                        consoleCommand2.NextWord();
                    if (cmd.cheat && !NetworkDebugger.enabled)
                    {
                        bool flag2 = false;
                        if (Steam.user != null && (Steam.user.id == 76561197996786074UL || Steam.user.id == 76561198885030822UL || Steam.user.id == 76561198416200652UL || Steam.user.id == 76561198104352795UL || Steam.user.id == 76561198114791325UL))
                            flag2 = true;
                        if (!flag2 && (Network.isActive || Level.current is ChallengeLevel || Level.current is ArcadeLevel))
                        {
                            DevConsole._core.lines.Enqueue(new DCLine()
                            {
                                line = "You can't do that here!",
                                color = Color.Red
                            });
                            return;
                        }
                    }
                    if (cmd.Run(consoleCommand2.Remainder()))
                    {
                        DevConsole.lastCommand = cmd;
                        str1 = cmd.logMessage;
                        if (cmd.commandQueueWaitFunction != null && DevConsole._pendingCommandQueue.Count > 0)
                            DevConsole._pendingCommandQueue.Peek().waitCommand = cmd.commandQueueWaitFunction;
                        if (cmd.commandQueueWait > 0)
                        {
                            if (DevConsole._pendingCommandQueue.Count > 0)
                            {
                                DevConsole._pendingCommandQueue.Peek().wait = cmd.commandQueueWait;
                                break;
                            }
                            break;
                        }
                        break;
                    }
                    if (cmd.priority >= num && (str2 == "" || cmd.fullCommandName.Length >= str2.Length))
                    {
                        DevConsole.lastCommand = null;
                        str1 = cmd.logMessage;
                        num = cmd.priority;
                        str2 = cmd.fullCommandName;
                    }
                }
                if (str1 != null)
                {
                    string str3 = str1;
                    char[] chArray = new char[1] { '\n' };
                    foreach (string str4 in str3.Split(chArray))
                        DevConsole._core.lines.Enqueue(new DCLine()
                        {
                            line = str4,
                            color = Color.White
                        });
                }
                else
                {
                    if (!flag1)
                    {
                        DevConsole.lastCommand = null;
                        if (pKeyword == "spawn")
                        {
                            if (DevConsole.CheckCheats())
                                return;
                            flag1 = true;
                            string str5 = consoleCommand1.NextWord();
                            float single1;
                            float single2;
                            try
                            {
                                single1 = Change.ToSingle(consoleCommand1.NextWord());
                                single2 = Change.ToSingle(consoleCommand1.NextWord());
                            }
                            catch
                            {
                                DevConsole._core.lines.Enqueue(new DCLine()
                                {
                                    line = "Parameters in wrong format.",
                                    color = Color.Red
                                });
                                return;
                            }
                            if (consoleCommand1.NextWord() != "")
                            {
                                DevConsole._core.lines.Enqueue(new DCLine()
                                {
                                    line = "Too many parameters!",
                                    color = Color.Red
                                });
                                return;
                            }
                            System.Type t = null;
                            foreach (System.Type thingType in Editor.ThingTypes)
                            {
                                if (thingType.Name.ToLower(currentCulture) == str5)
                                {
                                    t = thingType;
                                    break;
                                }
                            }
                            if (t == null)
                            {
                                DevConsole._core.lines.Enqueue(new DCLine()
                                {
                                    line = "The type " + str5 + " does not exist!",
                                    color = Color.Red
                                });
                                return;
                            }
                            if (!Editor.HasConstructorParameter(t))
                            {
                                DevConsole._core.lines.Enqueue(new DCLine()
                                {
                                    line = str5 + " can not be spawned this way.",
                                    color = Color.Red
                                });
                                return;
                            }
                            Thing thing = Editor.CreateThing(t) as PhysicsObject;
                            if (thing != null)
                            {
                                thing.x = single1;
                                thing.y = single2;
                                Level.Add(thing);
                                SFX.Play("hitBox");
                            }
                        }
                        if (pKeyword == "netdebug")
                        {
                            if (DevConsole.CheckCheats())
                                return;
                            DevConsole._enableNetworkDebugging = !DevConsole._enableNetworkDebugging;
                            DevConsole._core.lines.Enqueue(new DCLine()
                            {
                                line = "Network Debugging Enabled",
                                color = Color.Green
                            });
                            return;
                        }
                        if (pKeyword == "close")
                            DevConsole._core.open = !DevConsole._core.open;
                        if (pKeyword == "console")
                        {
                            flag1 = true;
                            string lower1 = consoleCommand1.NextWord().ToLower(currentCulture);
                            if (lower1 == "")
                            {
                                DevConsole._core.lines.Enqueue(new DCLine()
                                {
                                    line = "Parameters in wrong format.",
                                    color = Color.Red
                                });
                                return;
                            }
                            if (lower1 == "width")
                            {
                                string lower2 = consoleCommand1.NextWord().ToLower(currentCulture);
                                if (lower2 == "")
                                {
                                    DevConsole._core.lines.Enqueue(new DCLine()
                                    {
                                        line = "You must provide a value.",
                                        color = Color.Red
                                    });
                                    return;
                                }
                                if (consoleCommand1.NextWord() != "")
                                {
                                    DevConsole._core.lines.Enqueue(new DCLine()
                                    {
                                        line = "Too many parameters!",
                                        color = Color.Red
                                    });
                                    return;
                                }
                                try
                                {
                                    Options.Data.consoleWidth = Math.Min(Math.Max(Convert.ToInt32(lower2), 25), 100);
                                    Options.Save();
                                }
                                catch (Exception)
                                {
                                    try
                                    {
                                        Options.Data.consoleWidth = (int)Math.Min(Math.Max(Convert.ToSingle(lower2), 0.25f), 1f) * 100;
                                        Options.Save();
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                            else if (lower1 == "height")
                            {
                                string lower3 = consoleCommand1.NextWord().ToLower(currentCulture);
                                if (lower3 == "")
                                {
                                    DevConsole._core.lines.Enqueue(new DCLine()
                                    {
                                        line = "You must provide a value.",
                                        color = Color.Red
                                    });
                                    return;
                                }
                                if (consoleCommand1.NextWord() != "")
                                {
                                    DevConsole._core.lines.Enqueue(new DCLine()
                                    {
                                        line = "Too many parameters!",
                                        color = Color.Red
                                    });
                                    return;
                                }
                                try
                                {
                                    Options.Data.consoleHeight = Math.Min(Math.Max(Convert.ToInt32(lower3), 25), 100);
                                    Options.Save();
                                }
                                catch (Exception)
                                {
                                    try
                                    {
                                        Options.Data.consoleHeight = (int)Math.Min(Math.Max(Convert.ToSingle(lower3), 0.25f), 1f) * 100;
                                        Options.Save();
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                            else if (lower1 == "scale")
                            {
                                string lower4 = consoleCommand1.NextWord().ToLower(currentCulture);
                                if (lower4 == "")
                                {
                                    DevConsole._core.lines.Enqueue(new DCLine()
                                    {
                                        line = "You must provide a value.",
                                        color = Color.Red
                                    });
                                    return;
                                }
                                if (consoleCommand1.NextWord() != "")
                                {
                                    DevConsole._core.lines.Enqueue(new DCLine()
                                    {
                                        line = "Too many parameters!",
                                        color = Color.Red
                                    });
                                    return;
                                }
                                try
                                {
                                    Options.Data.consoleScale = Math.Min(Math.Max(Convert.ToInt32(lower4), 1), 5);
                                    Options.Save();
                                }
                                catch (Exception)
                                {
                                }
                            }
                            else if (lower1 == "font")
                            {
                                string pFont = consoleCommand1.NextWord();
                                if (pFont == "")
                                {
                                    DevConsole._core.lines.Enqueue(new DCLine()
                                    {
                                        line = "You must provide a value.",
                                        color = Color.Red
                                    });
                                    return;
                                }
                                try
                                {
                                    if (pFont == "size")
                                    {
                                        string lower5 = consoleCommand1.NextWord().ToLower(currentCulture);
                                        if (lower5 == "")
                                        {
                                            DevConsole._core.lines.Enqueue(new DCLine()
                                            {
                                                line = "You must provide a size value.",
                                                color = Color.Red
                                            });
                                            return;
                                        }
                                        if (consoleCommand1.NextWord() != "")
                                        {
                                            DevConsole._core.lines.Enqueue(new DCLine()
                                            {
                                                line = "Too many parameters!",
                                                color = Color.Red
                                            });
                                            return;
                                        }
                                        try
                                        {
                                            int int32 = Convert.ToInt32(lower5);
                                            DevConsole._raster = new RasterFont(DevConsole.fontName, int32);
                                            Options.Data.consoleFontSize = int32;
                                            DevConsole._raster.scale = new Vec2(0.5f);
                                            Options.Save();
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                    else
                                    {
                                        if (consoleCommand1.Remainder().Count<char>() > 0)
                                            pFont = pFont + " " + consoleCommand1.Remainder();
                                        if (pFont == "clear" || pFont == "default" || pFont == "none")
                                        {
                                            Options.Data.consoleFont = "";
                                            Options.Save();
                                            DevConsole.Log(DCSection.General, "|DGGREEN|Console font reset.");
                                        }
                                        else
                                        {
                                            if (pFont == "comic sans")
                                                pFont = "comic sans ms";
                                            if (RasterFont.GetName(pFont) != null)
                                            {
                                                DevConsole._raster = new RasterFont(pFont, fontPoints);
                                                Options.Data.consoleFont = pFont;
                                                DevConsole._raster.scale = new Vec2(0.5f);
                                                Options.Save();
                                                if (DevConsole._raster.data.name == "Comic Sans MS")
                                                    DevConsole.Log(DCSection.General, "|DGGREEN|Font is now " + DevConsole._raster.data.name + "! What a laugh!");
                                                else
                                                    DevConsole.Log(DCSection.General, "|DGGREEN|Font is now " + DevConsole._raster.data.name + "!");
                                            }
                                            else
                                                DevConsole.Log(DCSection.General, "|DGRED|Could not find font (" + pFont + ")!");
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                        if (NetworkDebugger.enabled && pKeyword == "record")
                        {
                            flag1 = true;
                            string pLevel = consoleCommand1.NextWord();
                            if (pLevel.Length < 3)
                            {
                                try
                                {
                                    NetworkDebugger.StartRecording(Convert.ToInt32(pLevel));
                                }
                                catch (Exception)
                                {
                                }
                            }
                            else
                                NetworkDebugger.StartRecording(pLevel);
                        }
                        if (pKeyword == "team")
                        {
                            if (DevConsole.CheckCheats())
                                return;
                            flag1 = true;
                            string findName = consoleCommand1.NextWord();
                            Profile profile = DevConsole.ProfileByName(findName);
                            if (profile != null)
                            {
                                string str6 = consoleCommand1.NextWord();
                                if (str6 == "")
                                {
                                    DevConsole._core.lines.Enqueue(new DCLine()
                                    {
                                        line = "Parameters in wrong format.",
                                        color = Color.Red
                                    });
                                    return;
                                }
                                if (consoleCommand1.NextWord() != "")
                                {
                                    DevConsole._core.lines.Enqueue(new DCLine()
                                    {
                                        line = "Too many parameters!",
                                        color = Color.Red
                                    });
                                    return;
                                }
                                string lower = str6.ToLower();
                                bool flag3 = false;
                                foreach (Team team in Teams.all)
                                {
                                    if (team.name.ToLower() == lower)
                                    {
                                        flag3 = true;
                                        profile.team = team;
                                        break;
                                    }
                                }
                                if (!flag3)
                                {
                                    DevConsole._core.lines.Enqueue(new DCLine()
                                    {
                                        line = "No team named " + lower + ".",
                                        color = Color.Red
                                    });
                                    return;
                                }
                            }
                            else
                            {
                                DevConsole._core.lines.Enqueue(new DCLine()
                                {
                                    line = "No profile named " + findName + ".",
                                    color = Color.Red
                                });
                                return;
                            }
                        }
                        if (pKeyword == "call")
                        {
                            if (DevConsole.CheckCheats())
                                return;
                            flag1 = true;
                            string str7 = consoleCommand1.NextWord();
                            bool flag4 = false;
                            foreach (Profile profile in Profiles.all)
                            {
                                if (profile.name.ToLower(currentCulture) == str7)
                                {
                                    if (profile.duck != null)
                                    {
                                        flag4 = true;
                                        string str8 = consoleCommand1.NextWord();
                                        if (str8 == "")
                                        {
                                            DevConsole._core.lines.Enqueue(new DCLine()
                                            {
                                                line = "Parameters in wrong format.",
                                                color = Color.Red
                                            });
                                            return;
                                        }
                                        if (consoleCommand1.NextWord() != "")
                                        {
                                            DevConsole._core.lines.Enqueue(new DCLine()
                                            {
                                                line = "Too many parameters!",
                                                color = Color.Red
                                            });
                                            return;
                                        }
                                        MethodInfo[] methods = typeof(Duck).GetMethods();
                                        bool flag5 = false;
                                        foreach (MethodInfo methodInfo in methods)
                                        {
                                            if (methodInfo.Name.ToLower(currentCulture) == str8)
                                            {
                                                flag5 = true;
                                                if (methodInfo.GetParameters().Count<ParameterInfo>() > 0)
                                                {
                                                    DevConsole._core.lines.Enqueue(new DCLine()
                                                    {
                                                        line = "You can only call functions with no parameters.",
                                                        color = Color.Red
                                                    });
                                                    return;
                                                }
                                                try
                                                {
                                                    methodInfo.Invoke(profile.duck, null);
                                                }
                                                catch
                                                {
                                                    DevConsole._core.lines.Enqueue(new DCLine()
                                                    {
                                                        line = "The function threw an exception.",
                                                        color = Color.Red
                                                    });
                                                    return;
                                                }
                                            }
                                        }
                                        if (!flag5)
                                        {
                                            DevConsole._core.lines.Enqueue(new DCLine()
                                            {
                                                line = "Duck has no function called " + str8 + ".",
                                                color = Color.Red
                                            });
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        DevConsole._core.lines.Enqueue(new DCLine()
                                        {
                                            line = str7 + " is not in the game!",
                                            color = Color.Red
                                        });
                                        return;
                                    }
                                }
                            }
                            if (!flag4)
                            {
                                DevConsole._core.lines.Enqueue(new DCLine()
                                {
                                    line = "No profile named " + str7 + ".",
                                    color = Color.Red
                                });
                                return;
                            }
                        }
                        if (pKeyword == "set")
                        {
                            if (DevConsole.CheckCheats())
                                return;
                            flag1 = true;
                            string str9 = consoleCommand1.NextWord();
                            bool flag6 = false;
                            foreach (Profile profile in Profiles.all)
                            {
                                if (profile.name.ToLower(currentCulture) == str9)
                                {
                                    if (profile.duck != null)
                                    {
                                        flag6 = true;
                                        string str10 = consoleCommand1.NextWord();
                                        if (str10 == "")
                                        {
                                            DevConsole._core.lines.Enqueue(new DCLine()
                                            {
                                                line = "Parameters in wrong format.",
                                                color = Color.Red
                                            });
                                            return;
                                        }
                                        System.Type type = typeof(Duck);
                                        PropertyInfo[] properties = type.GetProperties();
                                        bool flag7 = false;
                                        foreach (PropertyInfo propertyInfo in properties)
                                        {
                                            if (propertyInfo.Name.ToLower(currentCulture) == str10)
                                            {
                                                flag7 = true;
                                                if (propertyInfo.PropertyType == typeof(float))
                                                {
                                                    float single;
                                                    try
                                                    {
                                                        single = Change.ToSingle(consoleCommand1.NextWord());
                                                    }
                                                    catch
                                                    {
                                                        DevConsole._core.lines.Enqueue(new DCLine()
                                                        {
                                                            line = "Parameters in wrong format.",
                                                            color = Color.Red
                                                        });
                                                        return;
                                                    }
                                                    if (consoleCommand1.NextWord() != "")
                                                    {
                                                        DevConsole._core.lines.Enqueue(new DCLine()
                                                        {
                                                            line = "Too many parameters!",
                                                            color = Color.Red
                                                        });
                                                        return;
                                                    }
                                                    propertyInfo.SetValue(profile.duck, single, null);
                                                }
                                                if (propertyInfo.PropertyType == typeof(bool))
                                                {
                                                    bool boolean;
                                                    try
                                                    {
                                                        boolean = Convert.ToBoolean(consoleCommand1.NextWord());
                                                    }
                                                    catch
                                                    {
                                                        DevConsole._core.lines.Enqueue(new DCLine()
                                                        {
                                                            line = "Parameters in wrong format.",
                                                            color = Color.Red
                                                        });
                                                        return;
                                                    }
                                                    if (consoleCommand1.NextWord() != "")
                                                    {
                                                        DevConsole._core.lines.Enqueue(new DCLine()
                                                        {
                                                            line = "Too many parameters!",
                                                            color = Color.Red
                                                        });
                                                        return;
                                                    }
                                                    propertyInfo.SetValue(profile.duck, boolean, null);
                                                }
                                                if (propertyInfo.PropertyType == typeof(int))
                                                {
                                                    int int32;
                                                    try
                                                    {
                                                        int32 = Convert.ToInt32(consoleCommand1.NextWord());
                                                    }
                                                    catch
                                                    {
                                                        DevConsole._core.lines.Enqueue(new DCLine()
                                                        {
                                                            line = "Parameters in wrong format.",
                                                            color = Color.Red
                                                        });
                                                        return;
                                                    }
                                                    if (consoleCommand1.NextWord() != "")
                                                    {
                                                        DevConsole._core.lines.Enqueue(new DCLine()
                                                        {
                                                            line = "Too many parameters!",
                                                            color = Color.Red
                                                        });
                                                        return;
                                                    }
                                                    propertyInfo.SetValue(profile.duck, int32, null);
                                                }
                                                if (propertyInfo.PropertyType == typeof(Vec2))
                                                {
                                                    float single3;
                                                    float single4;
                                                    try
                                                    {
                                                        single3 = Change.ToSingle(consoleCommand1.NextWord());
                                                        single4 = Change.ToSingle(consoleCommand1.NextWord());
                                                    }
                                                    catch
                                                    {
                                                        DevConsole._core.lines.Enqueue(new DCLine()
                                                        {
                                                            line = "Parameters in wrong format.",
                                                            color = Color.Red
                                                        });
                                                        return;
                                                    }
                                                    if (consoleCommand1.NextWord() != "")
                                                    {
                                                        DevConsole._core.lines.Enqueue(new DCLine()
                                                        {
                                                            line = "Too many parameters!",
                                                            color = Color.Red
                                                        });
                                                        return;
                                                    }
                                                    propertyInfo.SetValue(profile.duck, new Vec2(single3, single4), null);
                                                }
                                            }
                                        }
                                        if (!flag7)
                                        {
                                            foreach (FieldInfo field in type.GetFields())
                                            {
                                                if (field.Name.ToLower(currentCulture) == str10)
                                                {
                                                    flag7 = true;
                                                    if (field.FieldType == typeof(float))
                                                    {
                                                        float single;
                                                        try
                                                        {
                                                            single = Change.ToSingle(consoleCommand1.NextWord());
                                                        }
                                                        catch
                                                        {
                                                            DevConsole._core.lines.Enqueue(new DCLine()
                                                            {
                                                                line = "Parameters in wrong format.",
                                                                color = Color.Red
                                                            });
                                                            return;
                                                        }
                                                        if (consoleCommand1.NextWord() != "")
                                                        {
                                                            DevConsole._core.lines.Enqueue(new DCLine()
                                                            {
                                                                line = "Too many parameters!",
                                                                color = Color.Red
                                                            });
                                                            return;
                                                        }
                                                        field.SetValue(profile.duck, single);
                                                    }
                                                    if (field.FieldType == typeof(bool))
                                                    {
                                                        bool boolean;
                                                        try
                                                        {
                                                            boolean = Convert.ToBoolean(consoleCommand1.NextWord());
                                                        }
                                                        catch
                                                        {
                                                            DevConsole._core.lines.Enqueue(new DCLine()
                                                            {
                                                                line = "Parameters in wrong format.",
                                                                color = Color.Red
                                                            });
                                                            return;
                                                        }
                                                        if (consoleCommand1.NextWord() != "")
                                                        {
                                                            DevConsole._core.lines.Enqueue(new DCLine()
                                                            {
                                                                line = "Too many parameters!",
                                                                color = Color.Red
                                                            });
                                                            return;
                                                        }
                                                        field.SetValue(profile.duck, boolean);
                                                    }
                                                    if (field.FieldType == typeof(int))
                                                    {
                                                        int int32;
                                                        try
                                                        {
                                                            int32 = Convert.ToInt32(consoleCommand1.NextWord());
                                                        }
                                                        catch
                                                        {
                                                            DevConsole._core.lines.Enqueue(new DCLine()
                                                            {
                                                                line = "Parameters in wrong format.",
                                                                color = Color.Red
                                                            });
                                                            return;
                                                        }
                                                        if (consoleCommand1.NextWord() != "")
                                                        {
                                                            DevConsole._core.lines.Enqueue(new DCLine()
                                                            {
                                                                line = "Too many parameters!",
                                                                color = Color.Red
                                                            });
                                                            return;
                                                        }
                                                        field.SetValue(profile.duck, int32);
                                                    }
                                                    if (field.FieldType == typeof(Vec2))
                                                    {
                                                        float single5;
                                                        float single6;
                                                        try
                                                        {
                                                            single5 = Change.ToSingle(consoleCommand1.NextWord());
                                                            single6 = Change.ToSingle(consoleCommand1.NextWord());
                                                        }
                                                        catch
                                                        {
                                                            DevConsole._core.lines.Enqueue(new DCLine()
                                                            {
                                                                line = "Parameters in wrong format.",
                                                                color = Color.Red
                                                            });
                                                            return;
                                                        }
                                                        if (consoleCommand1.NextWord() != "")
                                                        {
                                                            DevConsole._core.lines.Enqueue(new DCLine()
                                                            {
                                                                line = "Too many parameters!",
                                                                color = Color.Red
                                                            });
                                                            return;
                                                        }
                                                        field.SetValue(profile.duck, new Vec2(single5, single6));
                                                    }
                                                }
                                            }
                                            if (!flag7)
                                            {
                                                DevConsole._core.lines.Enqueue(new DCLine()
                                                {
                                                    line = "Duck has no variable called " + str10 + ".",
                                                    color = Color.Red
                                                });
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        DevConsole._core.lines.Enqueue(new DCLine()
                                        {
                                            line = str9 + " is not in the game!",
                                            color = Color.Red
                                        });
                                        return;
                                    }
                                }
                            }
                            if (!flag6)
                            {
                                DevConsole._core.lines.Enqueue(new DCLine()
                                {
                                    line = "No profile named " + str9 + ".",
                                    color = Color.Red
                                });
                                return;
                            }
                        }
                        if (pKeyword == "globalscores")
                        {
                            if (DevConsole.CheckCheats())
                                return;
                            flag1 = true;
                            using (List<Profile>.Enumerator enumerator = Profiles.active.GetEnumerator())
                            {
                                if (enumerator.MoveNext())
                                {
                                    Profile current = enumerator.Current;
                                    DevConsole._core.lines.Enqueue(new DCLine()
                                    {
                                        line = current.name + ": " + current.stats.CalculateProfileScore().ToString("0.000"),
                                        color = Color.Red
                                    });
                                }
                            }
                        }
                        if (pKeyword == "scorelog")
                        {
                            if (DevConsole.CheckCheats())
                                return;
                            flag1 = true;
                            string str11 = consoleCommand1.NextWord();
                            if (consoleCommand1.NextWord() != "")
                            {
                                DevConsole._core.lines.Enqueue(new DCLine()
                                {
                                    line = "Too many parameters!",
                                    color = Color.Red
                                });
                                return;
                            }
                            if (str11 == "")
                            {
                                DevConsole._core.lines.Enqueue(new DCLine()
                                {
                                    line = "You need to provide a player number.",
                                    color = Color.Red
                                });
                                return;
                            }
                            int int32;
                            try
                            {
                                int32 = Convert.ToInt32(str11);
                            }
                            catch
                            {
                                DevConsole._core.lines.Enqueue(new DCLine()
                                {
                                    line = "Parameters in wrong format.",
                                    color = Color.Red
                                });
                                return;
                            }
                            DevConsole._core.logScores = int32;
                        }
                    }
                    if (flag1)
                        return;
                    DevConsole._core.lines.Enqueue(new DCLine()
                    {
                        line = pKeyword + " is not a valid command!",
                        color = Color.Red
                    });
                }
            }
        }

        private static bool CheckCheats()
        {
            if (NetworkDebugger.enabled)
                return false;
            bool flag = false;
            if (Steam.user != null && (Steam.user.id == 76561197996786074UL || Steam.user.id == 76561198885030822UL || Steam.user.id == 76561198416200652UL || Steam.user.id == 76561198104352795UL || Steam.user.id == 76561198114791325UL))
                flag = true;
            if (!flag)
            {
                if (!Network.isActive)
                {
                    switch (Level.current)
                    {
                        case ChallengeLevel _:
                        case ArcadeLevel _:
                            break;
                        default:
                            goto label_8;
                    }
                }
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "You can't do that here!",
                    color = Color.Red
                });
                return true;
            }
        label_8:
            return false;
        }

        public static void LogComplexMessage(string text, Color c, float scale = 2f, int index = -1)
        {
            if (text.Contains<char>('\n'))
            {
                string str = text;
                char[] chArray = new char[1] { '\n' };
                foreach (string text1 in str.Split(chArray))
                    DevConsole.Log(text1, c, scale, index);
            }
            else
            {
                DCLine dcLine = new DCLine()
                {
                    line = text,
                    color = c,
                    threadIndex = index < 0 ? NetworkDebugger.currentIndex : index,
                    timestamp = DateTime.Now
                };
                if (NetworkDebugger.enabled)
                {
                    lock (DevConsole.debuggerLines)
                        DevConsole.debuggerLines.Add(dcLine);
                }
                else
                {
                    lock (DevConsole._core.pendingLines)
                        DevConsole._core.pendingLines.Add(dcLine);
                }
            }
        }

        public static void Log(string text) => DevConsole.Log(DCSection.General, text);

        public static void Log(string text, Color c, float scale = 2f, int index = -1)
        {
            DCLine dcLine = new DCLine()
            {
                line = text,
                color = c,
                threadIndex = index < 0 ? NetworkDebugger.currentIndex : index,
                timestamp = DateTime.Now
            };
            if (NetworkDebugger.enabled)
            {
                lock (DevConsole.debuggerLines)
                    DevConsole.debuggerLines.Add(dcLine);
            }
            else
            {
                lock (DevConsole._core.pendingLines)
                    DevConsole._core.pendingLines.Add(dcLine);
            }
        }

        public static void RefreshConsoleFont()
        {
            DevConsole._raster = null;
            DevConsole.InitializeFont();
        }

        public static void LogEvent(string pDescription, NetworkConnection pConnection)
        {
            if (pDescription == null)
                pDescription = "No Description.";
            DevConsole.Log("@LOGEVENT@|AQUA|LOGEVENT!-----------" + pConnection.ToString() + "|AQUA|signalled a log event!-----------!LOGEVENT", Color.White);
            DevConsole.Log("@LOGEVENT@|AQUA|LOGEVENT!---" + pDescription + "|AQUA|---!LOGEVENT", Color.White);
            if (!Network.isActive || pConnection != DuckNetwork.localConnection)
                return;
            Send.Message(new NMLogEvent(pDescription));
        }

        public static void Log(DCSection section, string text, int netIndex = -1) => DevConsole.Log(section, Verbosity.Normal, text, netIndex);

        public static void Log(
          DCSection section,
          string text,
          NetworkConnection context,
          int netIndex = -1)
        {
            if (context != null)
                text += context.ToString();
            DevConsole.Log(section, Verbosity.Normal, text, netIndex);
        }

        public static void Log(DCSection section, Verbosity verbose, string text, int netIndex = -1)
        {
            DCLine dcLine = new DCLine()
            {
                line = text,
                section = section,
                verbosity = verbose,
                color = Color.White,
                threadIndex = netIndex < 0 ? NetworkDebugger.currentIndex : netIndex,
                timestamp = DateTime.Now
            };
            if (NetworkDebugger.enabled)
            {
                lock (DevConsole.debuggerLines)
                    DevConsole.debuggerLines.Add(dcLine);
            }
            else
            {
                lock (DevConsole._core.pendingLines)
                    DevConsole._core.pendingLines.Add(dcLine);
            }
        }

        private static void SendNetLog(NetworkConnection pConnection)
        {
            List<string> stringList = new List<string>();
            string str = "";
            for (int index = Math.Max(DevConsole.core.lines.Count - 750, 0); index < DevConsole.core.lines.Count; ++index)
            {
                str += DevConsole.core.lines.ElementAt<DCLine>(index).ToSendString();
                if (str.Length > 500)
                {
                    stringList.Add(str);
                    str = "";
                }
            }
            DuckNetwork.core.logTransferSize = stringList.Count;
            Send.Message(new NMLogRequestIncoming(stringList.Count), pConnection);
            foreach (string pData in stringList)
            {
                Queue<NetMessage> pendingSends = DevConsole._core.pendingSends;
                NMLogRequestChunk nmLogRequestChunk = new NMLogRequestChunk(pData)
                {
                    connection = pConnection
                };
                pendingSends.Enqueue(nmLogRequestChunk);
            }
        }

        public static void SaveNetLog(string pName = null)
        {
            DevConsole.FlushPendingLines();
            string text = "";
            for (int index = Math.Max(DevConsole.core.lines.Count - 1500, 0); index < DevConsole.core.lines.Count; ++index)
                text += DevConsole.core.lines.ElementAt<DCLine>(index).ToSendString();
            if (pName == null)
                pName = DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_') + "_" + Steam.user.name + "_netlog.rtf";
            else if (!pName.EndsWith(".rtf"))
                pName += ".rtf";
            string str = DuckFile.FixInvalidPath(DuckFile.logDirectory + pName);
            if (System.IO.File.Exists(str))
                System.IO.File.Delete(str);
            RichTextBox richTextBox = new FancyBitmapFont().MakeRTF(text);
            DuckFile.CreatePath(str);
            string path = str;
            richTextBox.SaveFile(path);
        }

        public static void LogTransferComplete(NetworkConnection pConnection)
        {
            string receivedLogData = DevConsole.core.GetReceivedLogData(pConnection);
            if (receivedLogData != null)
            {
                string pathString = DuckFile.FixInvalidPath(DuckFile.logDirectory + DateTime.Now.ToShortDateString().Replace('/', '_') + "_" + DateTime.Now.ToLongTimeString().Replace(':', '_') + "_" + pConnection.name + "_netlog.rtf");
                RichTextBox richTextBox = new FancyBitmapFont().MakeRTF(receivedLogData);
                DuckFile.CreatePath(pathString);
                string path = pathString;
                richTextBox.SaveFile(path);
            }
            DevConsole.core.requestingLogs.Remove(pConnection);
            DevConsole.core.receivingLogs.Remove(pConnection);
            pConnection.logTransferProgress = 0;
            pConnection.logTransferSize = 0;
        }

        public static void LogSendingComplete(NetworkConnection pConnection)
        {
            DevConsole.core.transferRequestsPending.Remove(pConnection);
            DuckNetwork.core.logTransferProgress = 0;
            DuckNetwork.core.logTransferSize = 0;
        }

        public static void Chart(string chart, string section, double x, double y, Color c)
        {
            lock (DevConsole._core.pendingChartValues)
                DevConsole._core.pendingChartValues.Add(new DCChartValue()
                {
                    chart = chart,
                    section = section,
                    x = x,
                    y = y,
                    color = c,
                    threadIndex = NetworkDebugger.currentIndex
                });
        }

        public static void UpdateGraph(int index, NetGraph target)
        {
        }

        public static void InitializeCommands()
        {
            DevConsole.AddCommand(new CMD("level", new CMD.Argument[1]
            {
         new CMD.Level("level")
            }, cmd => Level.current = cmd.Arg<Level>("level"))
            {
                cheat = true,
                aliases = new List<string>() { "lev" },
                commandQueueWaitFunction = () => Level.core.nextLevel == null
            });
            DevConsole.AddCommand(new CMD("give", new CMD.Argument[3]
            {
         new CMD.Thing<Duck>("player"),
         new CMD.Thing<Holdable>("object"),
         new CMD.String("specialCode", true)
            }, cmd =>
            {
                Duck duck = cmd.Arg<Duck>("player");
                Holdable holdable = cmd.Arg<Holdable>("object");
                string str = cmd.Arg<string>("specialCode");
                Level.Add(holdable);
                if (str == "i" && holdable is Gun)
                    (holdable as Gun).infinite.value = true;
                if (str == "h" || str == "hp" || str == "ph")
                {
                    if (!(duck.GetEquipment(typeof(Holster)) is Holster e2))
                    {
                        e2 = str == "hp" || str == "ph" ? new PowerHolster(0.0f, 0.0f) : new Holster(0.0f, 0.0f);
                        Level.Add(e2);
                        duck.Equip(e2);
                    }
                    e2.SetContainedObject(holdable);
                }
                else if (str == "e" && holdable is Equipment)
                    duck.Equip(holdable as Equipment);
                else
                    duck.GiveHoldable(holdable);
                SFX.Play("hitBox");
            })
            {
                description = "Gives a player an item by name.",
                cheat = true,
                priority = 1
            });
            DevConsole.AddCommand(new CMD("give", new CMD.Argument[2]
            {
         new CMD.Thing<Duck>("duckName"),
         new CMD.Thing<TeamHat>("hat")
            }, cmd =>
            {
                Duck duck = cmd.Arg<Duck>("duckName");
                TeamHat teamHat = cmd.Arg<TeamHat>("hat");
                Level.Add(teamHat);
                TeamHat h = teamHat;
                duck.GiveHoldable(h);
                SFX.Play("hitBox");
            })
            {
                cheat = true
            });
            DevConsole.AddCommand(new CMD("kill", new CMD.Argument[1]
            {
         new CMD.Thing<Duck>("duckName")
            }, cmd => cmd.Arg<Duck>("duckName").Kill(new DTIncinerate(null)))
            {
                description = "",
                cheat = true
            });
            DevConsole.AddCommand(new CMD("modhash", () =>
            {
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = ModLoader._modString,
                    color = Color.Red
                });
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = ModLoader.modHash,
                    color = Color.Red
                });
            }));
            DevConsole.AddCommand(new CMD("wagnus", () => DevConsole.wagnusDebug = !DevConsole.wagnusDebug)
            {
                description = "Toggles guides in editor for Wagnus teleport ranges."
            });
            DevConsole.AddCommand(new CMD("steamid", () => DevConsole._core.lines.Enqueue(new DCLine()
            {
                line = "Your steam ID is: " + Profiles.experienceProfile.steamID.ToString(),
                color = Colors.DGBlue
            })));
            DevConsole.AddCommand(new CMD("localid", () => DevConsole._core.lines.Enqueue(new DCLine()
            {
                line = "Your local ID is: " + DG.localID.ToString(),
                color = Colors.DGBlue
            })));
            DevConsole.AddCommand(new CMD("showcollision", () => DevConsole._core.showCollision = !DevConsole._core.showCollision)
            {
                cheat = true
            });
            DevConsole.AddCommand(new CMD("showorigin", () => DevConsole.debugOrigin = !DevConsole.debugOrigin)
            {
                hidden = true
            });
            DevConsole.AddCommand(new CMD("showbounds", () => DevConsole.debugBounds = !DevConsole.debugBounds)
            {
                hidden = true
            });
            DevConsole.AddCommand(new CMD("fps", () => DevConsole.showFPS = !DevConsole.showFPS)
            {
                cheat = false
            });
            DevConsole.AddCommand(new CMD("randomedit", () => Editor.miniMode = !Editor.miniMode)
            {
                cheat = false
            });
            DevConsole.AddCommand(new CMD("mem", () =>
            {
                long num = GC.GetTotalMemory(true) / 1000L;
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "GC Has " + num.ToString() + " KB Allocated (" + (num / 1000L).ToString() + " MB)",
                    color = Color.White
                });
            }));
            CMD.Argument[] pArguments = new CMD.Argument[1];
            CMD.String @string = new CMD.String("description")
            {
                takesMultispaceString = true
            };
            pArguments[0] = @string;
            DevConsole.AddCommand(new CMD("log", pArguments, cmd => DevConsole.LogEvent(cmd.Arg<string>("description"), DuckNetwork.localConnection))
            {
                hidden = true
            });
            DevConsole.AddCommand(new CMD("requestlogs", () =>
            {
                Send.Message(new NMRequestLogs());
                foreach (NetworkConnection connection in Network.connections)
                    DevConsole.core.requestingLogs.Add(connection);
                DevConsole.SaveNetLog();
            })
            {
                hidden = true
            });
            DevConsole.AddCommand(new CMD("accept", new CMD.Argument[1]
            {
         new CMD.Integer("number")
            }, cmd =>
            {
                try
                {
                    Profile profile = DuckNetwork.profiles[cmd.Arg<int>("number")];
                    if (!DevConsole.core.transferRequestsPending.Contains(profile.connection))
                        return;
                    DevConsole.core.transferRequestsPending.Remove(profile.connection);
                    DevConsole.SendNetLog(profile.connection);
                }
                catch (Exception)
                {
                }
            })
            {
                hidden = true
            });
            DevConsole.AddCommand(new CMD("eight", () =>
            {
                int index = 0;
                foreach (Profile defaultProfile in Profiles.defaultProfiles)
                {
                    defaultProfile.team = null;
                    defaultProfile.team = Teams.all[index];
                    ++index;
                }
            })
            {
                cheat = true
            });
            DevConsole.AddCommand(new CMD("chat", new CMD("font", new CMD.Argument[1]
            {
         new CMD.Font("font",  () => Options.Data.chatFontSize)
            }, cmd =>
            {
                Options.Data.chatFont = cmd.Arg<string>("font");
                Options.Save();
                DuckNetwork.UpdateFont();
            })));
            DevConsole.AddCommand(new CMD("chat", new CMD("font", new CMD("size", new CMD.Argument[1]
            {
         new CMD.Integer("size")
            }, cmd =>
            {
                Options.Data.chatFontSize = cmd.Arg<int>("size");
                Options.Save();
                DuckNetwork.UpdateFont();
            }))));
            DevConsole.AddCommand(new CMD("fancymode", () => DevConsole.fancyMode = !DevConsole.fancyMode)
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("shieldmode", () => DevConsole.shieldMode = !DevConsole.shieldMode)
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("qwopmode", () => DevConsole._core.qwopMode = !DevConsole._core.qwopMode)
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("splitscreen", () => DevConsole._core.splitScreen = !DevConsole._core.splitScreen)
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("rhythmmode", () =>
            {
                if (!DevConsole._core.rhythmMode)
                    Music.Stop();
                DevConsole._core.rhythmMode = !DevConsole._core.rhythmMode;
                if (!DevConsole._core.rhythmMode)
                    return;
                Music.Play(Music.RandomTrack("InGame"));
            })
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("toggle", new CMD.Argument[1]
            {
         new CMD.Layer("layer")
            }, cmd => cmd.Arg<Layer>("layer").visible = !cmd.Arg<Layer>("layer").visible)
            {
                description = "Toggles whether or not a layer is visible. Some options include 'game', 'background', 'blocks' and 'parallax'.",
                cheat = true
            });
            DevConsole.AddCommand(new CMD("clearmainprofile", () =>
            {
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "Your main account has been R U I N E D !",
                    color = Color.Red
                });
                ulong steamId = Profiles.experienceProfile.steamID;
                string varName = steamId.ToString();
                steamId = Profiles.experienceProfile.steamID;
                string varID = steamId.ToString();
                Profile p = new Profile(varName, varID: varID)
                {
                    steamID = Profiles.experienceProfile.steamID
                };
                Profiles.Remove(Profiles.experienceProfile);
                Profiles.Add(p);
            })
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("xpskip", () =>
            {
                if (Profiles.experienceProfile.GetNumFurnitures(RoomEditor.GetFurniture("VOODOO VINCENT").index) > 0)
                    DevConsole._core.lines.Enqueue(new DCLine()
                    {
                        line = "Limit one Voodoo Vincent per customer, sorry!",
                        color = Color.Red
                    });
                else if (MonoMain.pauseMenu != null)
                {
                    DevConsole._core.lines.Enqueue(new DCLine()
                    {
                        line = "Please close any open menus.",
                        color = Color.Red
                    });
                }
                else
                {
                    HUD.CloseAllCorners();
                    (MonoMain.pauseMenu = new UIPresentBox(RoomEditor.GetFurniture("VOODOO VINCENT"), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f)).Open();
                    DevConsole._core.open = !DevConsole._core.open;
                }
            })
            {
                hidden = true,
                cheat = false
            });
            DevConsole.AddCommand(new CMD("johnnygrey", () =>
            {
                Global.data.typedJohnny = true;
                Global.Save();
                if (!Unlockables.HasPendingUnlocks())
                    return;
                DevConsole._core.open = false;
                MonoMain.pauseMenu = new UIUnlockBox(Unlockables.GetPendingUnlocks().ToList<Unlockable>(), Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
            })
            {
                hidden = true
            });
            DevConsole.AddCommand(new CMD("constantsync", () =>
            {
                DevConsole._core.constantSync = !DevConsole._core.constantSync;
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "Constant sync has been " + (Options.Data.powerUser ? "enabled" : "disabled") + "!",
                    color = DevConsole.core.constantSync ? Colors.DGGreen : Colors.DGRed
                });
            })
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("poweruser", () =>
            {
                Options.Data.powerUser = !Options.Data.powerUser;
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "Power User mode has been " + (Options.Data.powerUser ? "enabled" : "disabled") + "!",
                    color = Options.Data.powerUser ? Colors.DGGreen : Colors.DGRed
                });
                Editor.InitializePlaceableGroup();
                Main.editor.UpdateObjectMenu();
                Options.Save();
            })
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("oldangles", () =>
            {
                Options.Data.oldAngleCode = !Options.Data.oldAngleCode;
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "Oldschool Angles have been " + (Options.Data.oldAngleCode ? "enabled" : "disabled") + "!",
                    color = Options.Data.oldAngleCode ? Colors.DGGreen : Colors.DGRed
                });
                Options.Save();
                if (!Network.isActive || DuckNetwork.localProfile == null)
                    return;
                Send.Message(new NMOldAngles(DuckNetwork.localProfile, Options.Data.oldAngleCode));
            })
            {
                hidden = true
            });
            DevConsole.AddCommand(new CMD("debugtypelist", () =>
            {
                foreach (KeyValuePair<string, System.Type> keyValuePair in ModLoader._typesByName)
                    DevConsole._core.lines.Enqueue(new DCLine()
                    {
                        line = keyValuePair.Key,
                        color = Colors.DGPurple
                    });
            })
            {
                hidden = true
            });
            DevConsole.AddCommand(new CMD("debugtypelistraw", () =>
            {
                foreach (KeyValuePair<string, System.Type> keyValuePair in ModLoader._typesByNameUnprocessed)
                    DevConsole._core.lines.Enqueue(new DCLine()
                    {
                        line = keyValuePair.Key,
                        color = Colors.DGPurple
                    });
            })
            {
                hidden = true
            });
            DevConsole.AddCommand(new CMD("sing", new CMD.Argument[1]
            {
         new CMD.String("song")
            }, cmd =>
            {
                string str = cmd.Arg<string>("song");
                Music.Play(str);
                if (!Network.isActive)
                    return;
                Send.Message(new NMSwitchMusic(str));
            })
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("downpour", cmd =>
            {
                float num1 = (float)(Level.current.bottomRight.x - (double)Level.current.topLeft.x + 128.0);
                int num2 = 10;
                for (int index1 = 0; index1 < 10; ++index1)
                {
                    for (int index2 = 0; index2 < num2; ++index2)
                    {
                        PhysicsObject randomItem = ItemBoxRandom.GetRandomItem();
                        randomItem.position = Level.current.topLeft + new Vec2((float)((double)num1 / num2 * index2 + (double)Rando.Float(sbyte.MinValue, 128f) - 64.0), Level.current.topLeft.y - 2000f - 512 * index1 + Rando.Float(-256f, 256f));
                        Level.Add(randomItem);
                    }
                }
            })
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("ruindatahash", new CMD.Argument[1]
            {
         new CMD.String("workshopID", true)
            }, cmd =>
            {
                string str = cmd.Arg<string>("workshopID");
                if (str == null)
                {
                    Editor.thingTypesHash = (uint)Rando.Int(99999999);
                    DevConsole._core.lines.Enqueue(new DCLine()
                    {
                        line = "Ruined local datahash! Good luck playing online now!",
                        color = Colors.DGRed
                    });
                }
                else
                {
                    bool flag = false;
                    try
                    {
                        Mod modFromWorkshopId = ModLoader.GetModFromWorkshopID(Convert.ToUInt64(str));
                        if (modFromWorkshopId != null)
                        {
                            modFromWorkshopId.System_RuinDatahash();
                            flag = true;
                            DevConsole._core.lines.Enqueue(new DCLine()
                            {
                                line = "Ruined datahash for " + modFromWorkshopId.configuration.displayName + "! Good luck playing online now!",
                                color = Colors.DGRed
                            });
                        }
                    }
                    catch (Exception)
                    {
                    }
                    if (flag)
                        return;
                    DevConsole._core.lines.Enqueue(new DCLine()
                    {
                        line = "Could not find mod with ID (" + str + ")",
                        color = Colors.DGRed
                    });
                }
            })
            {
                hidden = true,
                cheat = true
            });
            if (!Steam.IsInitialized())
                return;
            DevConsole.AddCommand(new CMD("zipcloud", () =>
            {
                string pFile = DuckFile.saveDirectory + "cloud_zip.zip";
                Cloud.ZipUpCloudData(pFile);
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "Zipped up to: " + pFile,
                    color = Colors.DGBlue
                });
            })
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("sendsave", () =>
            {
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "Please type a message to the developer then and press enter..",
                    color = Color.White
                });
                DevConsole._doDataSubmission = true;
            })
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("clearsave", () =>
            {
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "ARE YOU SURE? ALL SAVE DATA WILL BE DELETED",
                    color = Color.Red
                });
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "LOCALLY, AND FROM THE CLOUD.",
                    color = Color.Red
                });
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "ENTER 'corptron' IF YOU WANT TO CONTINUE..",
                    color = Color.Red
                });
            })
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("copy", () =>
            {
                string currentPart = "";
                for (int index = Math.Max(DevConsole.core.lines.Count - 750, 0); index < DevConsole.core.lines.Count; ++index)
                    currentPart += DevConsole.core.lines.ElementAt<DCLine>(index).ToShortString();
                Thread thread = new Thread(() => Clipboard.SetText(currentPart));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "Log was copied to clipboard!",
                    color = Color.White
                });
            })
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("savedir", () =>
            {
                Process.Start(DuckFile.saveDirectory);
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "Save directory was opened.",
                    color = Color.White
                });
            })
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("userdir", () =>
            {
                Process.Start(DuckFile.userDirectory);
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "User directory was opened.",
                    color = Color.White
                });
            })
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("recover", () =>
            {
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "ARE YOU SURE? ALL NEW SAVE DATA",
                    color = Color.Red
                });
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "WILL BE OVERWRITTEN BY PRE 1.5 DATA",
                    color = Color.Red
                });
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "ENTER 'corptron' IF YOU WANT TO CONTINUE..",
                    color = Color.Red
                });
            })
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("managecloud", () => (MonoMain.pauseMenu = new UICloudManagement(null)).Open())
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("manageblocks", () => (MonoMain.pauseMenu = new UIBlockManagement(null)).Open())
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("corptron", () =>
            {
                if (DevConsole.lastCommand != null && DevConsole.lastCommand.keyword == "clearsave")
                {
                    Cloud.DeleteAllCloudData(false);
                    DuckFile.DeleteAllSaveData();
                    DevConsole._core.lines.Enqueue(new DCLine()
                    {
                        line = "All save data has been deleted.",
                        color = Color.Red
                    });
                }
                if (DevConsole.lastCommand == null || !(DevConsole.lastCommand.keyword == "recover"))
                    return;
                DuckFile.DeleteFolder(DuckFile.userDirectory);
                while (Cloud.processing)
                    Cloud.Update();
                Program.crashed = true;
                Process.Start(Application.ExecutablePath, Program.commandLine + " -recoversave");
                Application.Exit();
            })
            {
                hidden = true,
                cheat = true
            });
            DevConsole.AddCommand(new CMD("savetool", () =>
            {
                if (!System.IO.File.Exists("SaveTool.dll"))
                    return;
                MonoMain.showingSaveTool = true;
            })
            {
                hidden = true,
                cheat = true
            });
        }

        public static void FlushPendingLines()
        {
            lock (DevConsole._core.pendingLines)
            {
                foreach (DCLine pendingLine in DevConsole._core.pendingLines)
                {
                    DevConsole._core.lines.Enqueue(pendingLine);
                    if (DevConsole._core.viewOffset != 0)
                        ++DevConsole._core.viewOffset;
                }
                if (DevConsole._core.lines.Count > 3000)
                {
                    for (int index = 0; index < 500; ++index)
                    {
                        DevConsole._core.lines.Dequeue();
                        if (DevConsole._core.viewOffset > 0)
                            --DevConsole._core.viewOffset;
                    }
                }
                DevConsole._core.pendingLines.Clear();
            }
        }

        public static void Update()
        {
            if (DevConsole._core == null)
                return;
            if (DevConsole._doDataSubmission && DevConsole._dataSubmissionMessage != null)
            {
                DevConsole.SubmitSaveData(DevConsole._dataSubmissionMessage);
                DevConsole._core.lines.Enqueue(new DCLine()
                {
                    line = "|DGGREEN|Save data submitted!",
                    color = Colors.DGBlue
                });
                DevConsole._doDataSubmission = false;
                DevConsole._dataSubmissionMessage = null;
            }
            DevConsole.FlushPendingLines();
            bool flag = Keyboard.Down(Keys.LeftShift) || Keyboard.Down(Keys.RightShift);
            int num1 = !Keyboard.Pressed(Keys.OemTilde) ? 0 : (!flag ? 1 : 0);
            if (DevConsole.core.pendingSends.Count > 0)
            {
                NetMessage msg = DevConsole.core.pendingSends.Dequeue();
                Send.Message(msg, msg.connection);
            }
            if (num1 != 0 && !DuckNetwork.core.enteringText && NetworkDebugger.hoveringInstance)
            {
                if (DevConsole._tray == null)
                {
                    DevConsole._tray = new Sprite("devTray");
                    DevConsole._scan = new Sprite("devScan");
                }
                DevConsole._core.open = !DevConsole._core.open;
                Keyboard.keyString = "";
                DevConsole._core.cursorPosition = DevConsole._core.typing.Length;
                DevConsole._core.lastCommandIndex = -1;
                DevConsole._core.viewOffset = 0;
            }
            DevConsole._core.alpha = Maths.LerpTowards(DevConsole._core.alpha, DevConsole._core.open ? 1f : 0.0f, 0.1f);
            if (DevConsole._pendingCommandQueue.Count > 0)
            {
                DevConsole.QueuedCommand queuedCommand = DevConsole._pendingCommandQueue.Peek();
                if (queuedCommand.wait > 0)
                    --queuedCommand.wait;
                else if (queuedCommand.waitCommand == null || queuedCommand.waitCommand())
                {
                    DevConsole._pendingCommandQueue.Dequeue();
                    if (queuedCommand.command != null)
                        DevConsole.RunCommand(queuedCommand.command);
                }
            }
            if (DevConsole._core.open && NetworkDebugger.hoveringInstance)
            {
                Input._imeAllowed = true;
                if (DevConsole._core.cursorPosition > DevConsole._core.typing.Length)
                    DevConsole._core.cursorPosition = DevConsole._core.typing.Length;
                DevConsole._core.typing = DevConsole._core.typing.Insert(DevConsole._core.cursorPosition, Keyboard.keyString);
                if (DevConsole._core.typing != "" && DevConsole._pendingCommandQueue.Count > 0)
                {
                    DevConsole._pendingCommandQueue.Clear();
                    DevConsole._core.lines.Enqueue(new DCLine()
                    {
                        line = "Pending commands cleared!",
                        color = Colors.DGOrange
                    });
                }
                if (Keyboard.keyString.Length > 0)
                {
                    DevConsole._core.cursorPosition += Keyboard.keyString.Length;
                    DevConsole._core.lastCommandIndex = -1;
                }
                Keyboard.keyString = "";
                if (Keyboard.control)
                {
                    if (Keyboard.Pressed(Keys.C))
                    {
                        if (!string.IsNullOrWhiteSpace(DevConsole._core.typing))
                        {
                            Thread thread = new Thread(() => Clipboard.SetText(DevConsole._core.typing));
                            thread.SetApartmentState(ApartmentState.STA);
                            thread.Start();
                            thread.Join();
                            HUD.AddPlayerChangeDisplay("@CLIPCOPY@Copied!");
                        }
                    }
                    else if (Keyboard.Pressed(Keys.V))
                    {
                        string paste = "";
                        Thread thread = new Thread(() => paste = Clipboard.GetText());
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                        thread.Join();
                        string[] strArray = paste.Replace('\r', '\n').Split('\n');
                        List<string> stringList = new List<string>();
                        foreach (string str in strArray)
                        {
                            if (!string.IsNullOrWhiteSpace(str))
                                stringList.Add(str);
                        }
                        if (stringList.Count == 1)
                        {
                            DevConsole._core.typing = stringList[0].Trim();
                            DevConsole._core.cursorPosition = DevConsole._core.typing.Length;
                        }
                        else
                        {
                            DevConsole._core.typing = "";
                            DevConsole._core.cursorPosition = 0;
                            foreach (string str1 in stringList)
                            {
                                int num2 = 0;
                                string str2 = str1.Trim();
                                Func<bool> func = null;
                                if (str2.StartsWith("wait "))
                                {
                                    string[] parts = str2.Split(' ');
                                    if (parts.Length == 2)
                                    {
                                        if (parts[1] == "level")
                                        {
                                            Level c = Level.current;
                                            func = () => Level.current == c && Level.core.nextLevel == null;
                                        }
                                        else if (Triggers.IsTrigger(parts[1].ToUpperInvariant()))
                                        {
                                            func = () => Input.Pressed(parts[1].ToUpperInvariant());
                                        }
                                        else
                                        {
                                            try
                                            {
                                                num2 = Convert.ToInt32(parts[1]);
                                            }
                                            catch (Exception)
                                            {
                                            }
                                        }
                                    }
                                    str2 = null;
                                }
                                DevConsole._pendingCommandQueue.Enqueue(new DevConsole.QueuedCommand()
                                {
                                    command = str2,
                                    wait = num2,
                                    waitCommand = func
                                });
                            }
                        }
                    }
                }
                if (Keyboard.Pressed(Keys.Enter) && !string.IsNullOrWhiteSpace(DevConsole._core.typing))
                {
                    DevConsole.RunCommand(DevConsole._core.typing);
                    DevConsole._core.previousLines.Add(DevConsole._core.typing);
                    DevConsole._core.typing = "";
                    Keyboard.keyString = "";
                    DevConsole._core.lastCommandIndex = -1;
                    DevConsole._core.viewOffset = 0;
                }
                else if (Keyboard.Pressed(Keys.Back))
                {
                    if (DevConsole._core.typing.Length > 0 && DevConsole._core.cursorPosition > 0)
                    {
                        DevConsole._core.typing = DevConsole._core.typing.Remove(DevConsole._core.cursorPosition - 1, 1);
                        --DevConsole._core.cursorPosition;
                    }
                    DevConsole._core.lastCommandIndex = -1;
                }
                else if (Keyboard.Pressed(Keys.Delete))
                {
                    if (DevConsole._core.typing.Length > 0 && DevConsole._core.cursorPosition < DevConsole._core.typing.Length)
                        DevConsole._core.typing = DevConsole._core.typing.Remove(DevConsole._core.cursorPosition, 1);
                    DevConsole._core.lastCommandIndex = -1;
                }
                else if (Keyboard.Pressed(Keys.Left))
                    DevConsole._core.cursorPosition = Math.Max(0, DevConsole._core.cursorPosition - 1);
                else if (Keyboard.Pressed(Keys.Right))
                    DevConsole._core.cursorPosition = Math.Min(DevConsole._core.typing.Length, DevConsole._core.cursorPosition + 1);
                else if (Keyboard.Pressed(Keys.Home))
                {
                    if (Keyboard.shift)
                        DevConsole._core.viewOffset = DevConsole.core.lines.Count - 1;
                    else
                        DevConsole._core.cursorPosition = 0;
                }
                else if (Keyboard.Pressed(Keys.End))
                {
                    if (Keyboard.shift)
                        DevConsole._core.viewOffset = 0;
                    else
                        DevConsole._core.cursorPosition = DevConsole._core.typing.Length;
                }
                if (Keyboard.Pressed(Keys.PageUp))
                {
                    DevConsole._core.viewOffset += Keyboard.shift ? 10 : 1;
                    if (DevConsole._core.viewOffset > DevConsole.core.lines.Count - 1)
                        DevConsole._core.viewOffset = DevConsole.core.lines.Count - 1;
                }
                if (Keyboard.Pressed(Keys.PageDown))
                {
                    DevConsole._core.viewOffset -= Keyboard.shift ? 10 : 1;
                    if (DevConsole._core.viewOffset < 0)
                        DevConsole._core.viewOffset = 0;
                }
                if (Keyboard.Pressed(Keys.Up) && DevConsole._core.previousLines.Count > 0)
                {
                    ++DevConsole._core.lastCommandIndex;
                    if (DevConsole._core.lastCommandIndex >= DevConsole._core.previousLines.Count)
                        DevConsole._core.lastCommandIndex = DevConsole._core.previousLines.Count - 1;
                    DevConsole._core.typing = DevConsole._core.previousLines[DevConsole._core.previousLines.Count - 1 - DevConsole._core.lastCommandIndex];
                    DevConsole._core.cursorPosition = DevConsole._core.typing.Length;
                }
                if (!Keyboard.Pressed(Keys.Down))
                    return;
                if (DevConsole._core.previousLines.Count > 0 && DevConsole._core.lastCommandIndex > 0)
                {
                    --DevConsole._core.lastCommandIndex;
                    DevConsole._core.typing = DevConsole._core.previousLines[DevConsole._core.previousLines.Count - 1 - DevConsole._core.lastCommandIndex];
                    DevConsole._core.cursorPosition = DevConsole._core.typing.Length;
                }
                else if (DevConsole._core.lastCommandIndex == 0)
                {
                    --DevConsole._core.lastCommandIndex;
                    DevConsole._core.cursorPosition = 0;
                    DevConsole._core.typing = "";
                }
                else
                {
                    if (DevConsole._core.lastCommandIndex != -1)
                        return;
                    DevConsole._core.cursorPosition = 0;
                    DevConsole._core.typing = "";
                }
            }
            else
                DevConsole.lastCommand = null;
        }

        private class QueuedCommand
        {
            public Func<bool> waitCommand;
            public string command;
            public int wait;
        }
    }
}
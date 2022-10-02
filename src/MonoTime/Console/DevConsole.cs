// Decompiled with JetBrains decompiler
// Type: DuckGame.DevConsole
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
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
using SDL2;

namespace DuckGame
{
    public class DevConsole
    {
        [AutoConfigField]
        public static bool showFPS;
        public static List<string> startupCommands = new();
        public static bool fancyMode;
        public static int ConsoleLineOffset;
        private static DevConsoleCore _core = new();
        private static bool _enableNetworkDebugging;
        private static bool _oldConsole;
        public static bool debugOrigin;
        public static bool debugBounds;
        private static RasterFont _raster;
        public static Dictionary<string, List<CMD>> commands = new();
        public static CMD lastCommand;
        public static bool wagnusDebug;
        public static bool fuckUpPacketOrder = false;
        public static List<DCLine> debuggerLines = new();
        private static bool _doDataSubmission;
        private static string _dataSubmissionMessage;

        private static List<ulong> lostSaveIDs = new()
        {
            76561198035257896UL
        };

        public static Sprite _tray;
        public static Sprite _scan;
        private static Queue<QueuedCommand> _pendingCommandQueue = new();

        public static void SubmitSaveData(string pMessage)
        {
            byte[] array;
            using (MemoryStream memoryStream = new())
            {
                using (ZipArchive zipArchive = new(memoryStream, ZipArchiveMode.Create, true))
                {
                    int count = Steam.FileGetCount();
                    for (int file = 0; file < count; ++file)
                    {
                        string name = Steam.FileGetName(file);
                        if (name.EndsWith(".lev") || name.EndsWith(".png") || name.EndsWith(".play")) 
                            continue;

                        using Stream stream = zipArchive.CreateEntry(name).Open();
                        
                        byte[] buffer = Steam.FileRead(name);
                        stream.Write(buffer, 0, buffer.Length);
                    }

                    string str1 = "DirectInputDevices:\n===================================\n";
                    for (int index = 0; index < 32; ++index)
                    {
                        try
                        {
                            if (DInput.GetState(index) != null)
                            {
                                string str2 =
                                    $"{DInput.GetProductName(index)}{DInput.GetProductGUID(index)} {DInput.IsXInput(index)}";
                                str1 = $"{str1}{str2}\n";
                            }
                            else
                                break;
                        }
                        catch (Exception ex)
                        {
                            str1 = $"{str1}\n{ex}\n";
                        }
                    }

                    string str3 = $"{str1}\nEnumerated Input Devices:\n===================================\n";
                    foreach (InputDevice inputDevice in Input.GetInputDevices()
                                 .Where(inputDevice => inputDevice.isConnected))
                    {
                        try
                        {
                            str3 =
                               $"{str3}{inputDevice.productName}{inputDevice.productGUID} ({inputDevice}, {inputDevice.inputDeviceType})\n";
                        }
                        catch (Exception ex)
                        {
                            str3 = $"{str3}\n{ex}\n";
                        }
                    }

                    using (Stream stream = zipArchive.CreateEntry("input_device_report.txt").Open())
                    {
                        using (StreamWriter streamWriter = new(stream))
                        {
                            streamWriter.Write(str3);
                            streamWriter.Flush();
                        }
                    }

                    using (Stream stream = zipArchive.CreateEntry("dg_details.txt").Open())
                    {
                        using (StreamWriter streamWriter = new(stream))
                        {
                            streamWriter.Write(MonoMain.GetDetails());
                            streamWriter.Write("Console Contents:\n");
                            for (int index = 0; index < 100; ++index)
                            {
                                if (index < core.lines.Count)
                                    streamWriter.Write(
                                        $"{core.lines.ElementAt(index)}\n");
                            }

                            streamWriter.Flush();
                        }
                    }
                }

                array = memoryStream.ToArray();
            }

            if (array == null)
                return;
            
            HttpWebRequest httpWebRequest =
                (HttpWebRequest) WebRequest.Create("http://www.wonthelp.info/DuckWeb/submitSave.php");
            httpWebRequest.Method = "POST";
            string str =
                $"sendRequest=DGBugLogger&steamID={CrashWindow.CrashWindow.SQLEncode(Steam.user.id.ToString())}";
            byte[] bytes = Encoding.UTF8.GetBytes(
                $"{(pMessage == null ? $"{str}&steamName={CrashWindow.CrashWindow.SQLEncode(Steam.user.name)}" : $"{str}&steamName={CrashWindow.CrashWindow.SQLEncode($"{Steam.user.name}({pMessage})")}")}&data={CrashWindow.CrashWindow.SQLEncode(Editor.BytesToString(array))}");
            httpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            httpWebRequest.ContentLength = bytes.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response = (HttpWebResponse) httpWebRequest.GetResponse();
            LogComplexMessage(response.StatusDescription, Colors.DGBlue);
            using (Stream responseStream = response.GetResponseStream())
                LogComplexMessage(new StreamReader(responseStream).ReadToEnd(), Colors.DGBlue);
        }

        public static DevConsoleCore core
        {
            get => _core;
            set => _core = value;
        }

        public static bool open => _core.open;

        public static void SuppressDevConsole()
        {
            _oldConsole = _enableNetworkDebugging;
            _enableNetworkDebugging = false;
        }

        public static void RestoreDevConsole() => _enableNetworkDebugging = _oldConsole;

        public static bool enableNetworkDebugging
        {
            get => _enableNetworkDebugging;
            set => _enableNetworkDebugging = value;
        }

        public static bool splitScreen
        {
            get => _core.splitScreen;
            set => _core.splitScreen = value;
        }

        public static bool rhythmMode
        {
            get => _core.rhythmMode;
            set => _core.rhythmMode = value;
        }

        public static bool qwopMode
        {
            get => _core.qwopMode;
            set => _core.qwopMode = value;
        }

        public static bool showIslands
        {
            get => _core.showIslands;
            set => _core.showIslands = value;
        }

        public static bool showCollision
        {
            get => _core.showCollision;// && !Network.isActive;
            set => _core.showCollision = value;
        }

        public static bool shieldMode
        {
            get => _core.shieldMode;
            set => _core.shieldMode = value;
        }

        public static void DrawLine(Vec2 pos, DCLine line, bool times, bool section)
        {
            string str1 = $"{line.timestamp.Minute}";
            if (str1.Length == 1)
                str1 = $" {str1}";
            string str2 = $"{str1}:";
            if (line.timestamp.Second < 10)
                str2 += "0";
            string str3 = str2 + line.timestamp.Second;
            core.font.scale = new Vec2(1f);
            core.font.Draw(
                (times ? $"|GRAY|{str3} " : "") + (section ? line.SectionString(small: true) : "") + line.line,
                pos.x, pos.y, line.color * 0.8f, 0.9f);
            core.font.scale = new Vec2(2f);
        }

        public static Vec2 size => new(1280f, 1280f / Resolution.current.aspect);

        public static void InitializeFont()
        {
            if (string.IsNullOrEmpty(Options.Data.consoleFont))
            {
                _raster = null;
            }
            else
            {
                if (_raster != null)
                    return;
                _raster = new RasterFont(Options.Data.consoleFont, Options.Data.consoleFontSize);
            }
        }

        public static void Draw()
        {
            // Graphics.DrawString(ConsoleLineOffset.ToString(), new Vec2(16, 16), Color.White, 2f);
            // Graphics.DrawString($"{core.lines.Count + ConsoleLineOffset}", new Vec2(16, 32), Color.White, 2f);
            
            if (Layer.core._console != null)
            {
                Layer.core._console.camera.width = Resolution.current.x / 2;
                Layer.core._console.camera.height = Resolution.current.y / 2;
            }

            if (_core.font == null)
            {
                _core.font = new BitmapFont("biosFont", 8)
                {
                    scale = new Vec2(2f, 2f)
                };
                _core.fancyFont = new FancyBitmapFont("smallFont")
                {
                    scale = new Vec2(2f, 2f)
                };
            }

            if (DevConsole._core.alpha > 0.01f)
            {
                InitializeFont();
                if (_tray == null)
                    return;
                _tray.alpha = _core.alpha;
                _tray.scale = new Vec2((float)(Math.Round(Resolution.current.x / 1280.0 * 2.0) / 2.0) * 2f) *
                    (consoleScale + 1) / 2f;
                _tray.depth = 0.75f;
                if (Layer.core._console != null)
                {
                    int num1 = (int)((double)Layer.core._console.camera.height * dimensions.y / (16.0 * _tray.scale.y)) - 2;
                    int num2 = (int)((double)Layer.core._console.camera.width * dimensions.x / (16.0 * _tray.scale.x)) - 2;
                    Graphics.Draw(_tray, 0.0f, 0.0f, new Rectangle(0.0f, 0.0f, 18f, 18f));
                    Graphics.Draw(_tray, 0.0f, (float)(18.0 * _tray.scale.y + num1 * (16.0 * _tray.scale.y)),
                        new Rectangle(0.0f, _tray.height - 18, 18f, 18f));
                    Graphics.Draw(_tray, (float)(18.0 * _tray.scale.x + (num2 - 6) * (16.0 * _tray.scale.x)),
                        (float)(18.0 * _tray.scale.y + num1 * (16.0 * _tray.scale.y)),
                        new Rectangle(_tray.width - 114, _tray.height - 18, 114f, 18f));
                    for (int index = 0; index < num2; ++index)
                    {
                        const float width = 16f;
                        Graphics.Draw(_tray, (float)(18.0 * _tray.scale.x + 16.0 * _tray.scale.x * index), 0.0f,
                            new Rectangle(width, 0.0f, width, 18f));
                        if (index < num2 - 6)
                            Graphics.Draw(_tray, (float)(18.0 * _tray.scale.x + 16.0 * _tray.scale.x * index),
                                (float)(18.0 * _tray.scale.y + num1 * (16.0 * _tray.scale.y)),
                                new Rectangle(width, _tray.height - 18, width, 18f));
                    }

                    Graphics.Draw(_tray, (float)(18.0 * _tray.scale.x + num2 * (16.0 * _tray.scale.x)), 0.0f,
                        new Rectangle(_tray.width - 18, 0.0f, 18f, 18f));
                    for (int index = 0; index < num1; ++index)
                    {
                        Graphics.Draw(_tray, 0.0f, (float)(18.0 * _tray.scale.y + 16.0 * _tray.scale.y * index),
                            new Rectangle(0.0f, 18f, 18f, 16f));
                        Graphics.Draw(_tray, (float)(18.0 * _tray.scale.x + num2 * (16.0 * _tray.scale.x)),
                            (float)(18.0 * _tray.scale.y + 16.0 * _tray.scale.y * index),
                            new Rectangle(_tray.width - 18, 18f, 18f, 16f));
                    }

                    Graphics.DrawRect(Vec2.Zero,
                        new Vec2((float)(18.0 * _tray.scale.x + num2 * (16.0 * _tray.scale.x) + _tray.scale.y * 4.0),
                            (num1 + 2) * (16f * _tray.scale.y)), Color.Black * 0.8f * _core.alpha,
                        0.7f);
                    _core.fancyFont.scale = new Vec2(_tray.scale.x / 2f);
                    _core.fancyFont.depth = 0.98f;
                    _core.fancyFont.alpha = _core.alpha;
                    float num3 = (float)((num1 + 1) * 16 * (double)_tray.scale.y + 5.0 * _tray.scale.y);
                    float num4 = (num2 + 2) * (16f * _tray.scale.x);
                    string version = DG.version;
                    _core.fancyFont.Draw(version,
                        new Vec2((float)(82.0 * _tray.scale.x + (num2 - 6) * (16.0 * _tray.scale.x)),
                            num3 + 7f * _tray.scale.y), new Color(62, 114, 122), 0.98f);
                    _core.cursorPosition = Math.Min(Math.Max(_core.cursorPosition, 0),
                        _core.typing.Length);
                    if (_raster != null)
                    {
                        _raster.scale = new Vec2(0.5f);
                        _raster.alpha = _core.alpha;
                        _raster.Draw(_core.typing, 4f * _tray.scale.x,
                            (float)(num3 + _tray.scale.y * 8.0 -
                                     _raster.characterHeight * (double)_raster.scale.y / 2.0), Color.White,
                            0.9f);
                        Vec2 p1 = new(
                            (float)(_raster.GetWidth(
                                         _core.typing.Substring(0, _core.cursorPosition)) +
                                     4.0 * _tray.scale.x +
                                     1.0), num3 + 6f * _tray.scale.y);
                        Graphics.DrawLine(p1, p1 + new Vec2(0.0f, 4f * _tray.scale.x), Color.White,
                            depth: 1f);
                    }
                    else
                    {
                        _core.font.scale = new Vec2(_tray.scale.x / 2f);
                        _core.font.alpha = _core.alpha;
                        _core.font.Draw(_core.typing, 4f * _tray.scale.x,
                            num3 + 6f * _tray.scale.y, Color.White, 0.9f);
                        Vec2 p1 = new(
                            _core.font.GetWidth(
                                _core.typing.Substring(0, _core.cursorPosition)) +
                            4f * _tray.scale.x, num3 + 6f * _tray.scale.y);
                        Graphics.DrawLine(p1, p1 + new Vec2(0.0f, 4f * _tray.scale.x), Color.White, 2f, 1f);
                    }

                    int index1 = _core.lines.Count - 1 - _core.viewOffset + ConsoleLineOffset;
                    float num5 = 0.0f;
                    _core.font.scale = new Vec2((float)Math.Max(Math.Round(_tray.scale.x / 4.0), 1.0));
                    float num6 = _core.font.scale.x / 2f;
                    float num7 = 18f * num6;
                    float num8 = (float)(20.0 * (_core.font.scale.x * 2.0)) + core.font.GetWidth("HH:mm:ss ");
                    if (_raster != null)
                    {
                        num7 = (_raster.characterHeight - 2) * _raster.scale.y;
                        num5 = num7;
                        num8 = _raster.GetWidth("HH:mm:ss 0000  ");
                    }

                    for (int index2 = ConsoleLineOffset; index2 < (num3 - 2.0 * _tray.scale.y) / num7 - 1.0 && index1 >= 0; ++index2)
                    {
                        if (_core.lines.ElementAtOrDefault(index1 + ConsoleLineOffset) is not { } dcLine)
                            return;

                        string text = index1.ToString().PadLeft(4, '0');
                        string timeString = $"{dcLine.timestamp:HH:mm:ss} ";
                        text = timeString + text;
                        if (_raster != null)
                        {
                            _raster.maxWidth = (int)(num4 - 35.0 * _tray.scale.x);
                            _raster.singleLine = true;
                            _raster.enforceWidthByWord = false;
                            _raster.Draw(text, 4f * _tray.scale.x,
                                (float)(num3 - (double)num5 + 2.0),
                                index1 % 2 > 0 ? Color.Gray * 0.4f : Color.Gray * 0.6f, 0.9f);
                            _raster.Draw(dcLine.SectionString() + dcLine.line, 4f * _tray.scale.x + num8,
                                (float)(num3 - (double)num5 + 2.0), dcLine.color, 0.9f);
                            num5 += num7;
                        }
                        else
                        {
                            _core.font.maxWidth = (int)(num4 - 35.0 * _tray.scale.x);
                            _core.font.singleLine = true;
                            _core.font.enforceWidthByWord = false;
                            _core.font.Draw(text, 4f * _tray.scale.x,
                                (float)(num3 - 18.0 * num6 - num5 + 2.0),
                                index1 % 2 > 0 ? Color.Gray * 0.4f : Color.Gray * 0.6f, 0.9f);
                            _core.font.Draw($"{dcLine.SectionString()}{dcLine.line}",
                                4f * _tray.scale.x + num8,
                                (float)(num3 - 18.0 * num6 - num5 + 2.0), dcLine.color * 0.8f,
                                0.9f);
                            num5 += 18f * num6;
                        }

                        --index1;
                    }
                }

                _core.font.scale = new Vec2(2f);
            }
            
        }

        public static Vec2 dimensions => new(Options.Data.consoleWidth / 100f, Options.Data.consoleHeight / 100f);

        public static int consoleScale => Options.Data.consoleScale;

        public static int fontPoints => Options.Data.consoleFontSize;

        public static string fontName => Options.Data.consoleFont;

        public static Profile ProfileByName(string findName)
        {
            foreach (Profile profile in Profiles.all)
            {
                if (profile.team == null) 
                    continue;
                
                string str = profile.name.ToLower();
                switch (findName)
                {
                    case "player1" when profile.inputProfile == InputProfile.Get(InputProfile.MPPlayer1):
                    case "player2" when profile.inputProfile == InputProfile.Get(InputProfile.MPPlayer2):
                    case "player3" when profile.inputProfile == InputProfile.Get(InputProfile.MPPlayer3):
                    case "player4" when profile.inputProfile == InputProfile.Get(InputProfile.MPPlayer4):
                    case "player5" when profile.inputProfile == InputProfile.Get(InputProfile.MPPlayer5):
                    case "player6" when profile.inputProfile == InputProfile.Get(InputProfile.MPPlayer6):
                    case "player7" when profile.inputProfile == InputProfile.Get(InputProfile.MPPlayer7):
                    case "player8" when profile.inputProfile == InputProfile.Get(InputProfile.MPPlayer8):
                        str = findName;
                        break;
                }
                if (str == findName)
                    return profile;
            }

            return null;
        }

        public static void AddCommand(CMD pCommand)
        {
            GetCommands(pCommand.keyword).Add(pCommand);
            if (pCommand.aliases == null)
                return;
            foreach (string alias in pCommand.aliases)
                GetCommands(alias).Add(pCommand);
        }

        public static List<CMD> GetCommands(string pKeyword)
        {
            List<CMD> commands;
            if (!DevConsole.commands.TryGetValue(pKeyword, out commands))
                DevConsole.commands[pKeyword] = commands = new List<CMD>();
            return commands;
        }

        public static void RunCommand(string command, bool writeExecutedCommand = true)
        {
            if (DG.buildExpired)
                return;
            if (_doDataSubmission)
            {
                _dataSubmissionMessage = command;
                _core.lines.Enqueue(new DCLine
                {
                    line = "Submitting save data, this could take some time...",
                    color = Color.White
                });
            }
            else
            {
                _core.logScores = -1;
                if (command == "")
                    return;
                CultureInfo currentCulture = CultureInfo.CurrentCulture;
                bool flag1 = false;
                ConsoleCommand consoleCommand1 = new(command);
                string pKeyword = consoleCommand1.NextWord();
                
                if (writeExecutedCommand)
                {
                    _core.lines.Enqueue(new DCLine
                    {
                        line = command,
                        color = Color.White
                    });
                }

                string str1 = null;
                int num = int.MinValue;
                string str2 = "";
                foreach (CMD command1 in GetCommands(pKeyword))
                {
                    CMD cmd = command1;
                    flag1 = true;
                    ConsoleCommand consoleCommand2;
                    for (consoleCommand2 = new ConsoleCommand(consoleCommand1.Remainder());
                         cmd.subcommand != null && consoleCommand2.NextWord(peek: true) == cmd.subcommand.keyword;
                         cmd = cmd.subcommand)
                        consoleCommand2.NextWord();
                    if (cmd.cheat && !NetworkDebugger.enabled)
                    {
                        bool flag2 = Steam.user is 
                        {
                            id: 76561197996786074UL
                            or 76561198885030822UL
                            or 76561198416200652UL
                            or 76561198104352795UL
                            or 76561198114791325UL
                        };
                        
                        if (!flag2 && (Network.isActive || Level.current is ChallengeLevel or ArcadeLevel))
                        {
                            _core.lines.Enqueue(new DCLine
                            {
                                line = "You can't do that here!",
                                color = Color.Red
                            });
                            return;
                        }
                    }

                    if (cmd.Run(consoleCommand2.Remainder()))
                    {
                        lastCommand = cmd;
                        str1 = cmd.logMessage;
                        if (cmd.commandQueueWaitFunction != null && _pendingCommandQueue.Count > 0)
                            _pendingCommandQueue.Peek().waitCommand = cmd.commandQueueWaitFunction;
                        if (cmd.commandQueueWait > 0)
                        {
                            if (_pendingCommandQueue.Count > 0)
                            {
                                _pendingCommandQueue.Peek().wait = cmd.commandQueueWait;
                            }
                        }

                        break;
                    }

                    if (cmd.priority < num || (str2 != "" && cmd.fullCommandName.Length < str2.Length)) 
                        continue;
                    
                    lastCommand = null;
                    str1 = cmd.logMessage;
                    num = cmd.priority;
                    str2 = cmd.fullCommandName;
                }

                if (str1 != null)
                {
                    foreach (string str4 in str1.Split('\n'))
                        _core.lines.Enqueue(new DCLine
                        {
                            line = str4,
                            color = Color.White
                        });
                }
                else
                {
                    if (!flag1)
                    {
                        lastCommand = null;
                        switch (pKeyword)
                        {
                            case "crash":
                            {
                                flag1 = true;
                                throw new Exception("you threw it idk");
                                break;
                            }
                            case "spawn" when CheckCheats():
                                return;
                            case "spawn":
                            {
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
                                    _core.lines.Enqueue(new DCLine
                                    {
                                        line = "Parameters in wrong format.",
                                        color = Color.Red
                                    });
                                    return;
                                }

                                if (consoleCommand1.NextWord() != "")
                                {
                                    _core.lines.Enqueue(new DCLine
                                    {
                                        line = "Too many parameters!",
                                        color = Color.Red
                                    });
                                    return;
                                }

                                Type t = null;
                                foreach (Type thingType in Editor.ThingTypes)
                                {
                                    if (thingType.Name.ToLower(currentCulture) == str5)
                                    {
                                        t = thingType;
                                        break;
                                    }
                                }

                                if (t == null)
                                {
                                    _core.lines.Enqueue(new DCLine
                                    {
                                        line = $"The type {str5} does not exist!",
                                        color = Color.Red
                                    });
                                    return;
                                }

                                if (!Editor.HasConstructorParameter(t))
                                {
                                    _core.lines.Enqueue(new DCLine
                                    {
                                        line = $"{str5} can not be spawned this way.",
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

                                break;
                            }
                            case "netdebug" when CheckCheats():
                                return;
                            case "netdebug":
                                _enableNetworkDebugging = !_enableNetworkDebugging;
                                _core.lines.Enqueue(new DCLine
                                {
                                    line = "Network Debugging Enabled",
                                    color = Color.Green
                                });
                                return;
                            case "close":
                                _core.open = !_core.open;
                                break;
                            case "console":
                            {
                                flag1 = true;
                                string lower1 = consoleCommand1.NextWord().ToLower(currentCulture);
                                if (lower1 == "")
                                {
                                    _core.lines.Enqueue(new DCLine
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
                                        _core.lines.Enqueue(new DCLine
                                        {
                                            line = "You must provide a value.",
                                            color = Color.Red
                                        });
                                        return;
                                    }

                                    if (consoleCommand1.NextWord() != "")
                                    {
                                        _core.lines.Enqueue(new DCLine
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
                                            Options.Data.consoleWidth =
                                                (int) Math.Min(Math.Max(Convert.ToSingle(lower2), 0.25f), 1f) * 100;
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
                                        _core.lines.Enqueue(new DCLine
                                        {
                                            line = "You must provide a value.",
                                            color = Color.Red
                                        });
                                        return;
                                    }

                                    if (consoleCommand1.NextWord() != "")
                                    {
                                        _core.lines.Enqueue(new DCLine
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
                                            Options.Data.consoleHeight =
                                                (int) Math.Min(Math.Max(Convert.ToSingle(lower3), 0.25f), 1f) * 100;
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
                                        _core.lines.Enqueue(new DCLine
                                        {
                                            line = "You must provide a value.",
                                            color = Color.Red
                                        });
                                        return;
                                    }

                                    if (consoleCommand1.NextWord() != "")
                                    {
                                        _core.lines.Enqueue(new DCLine
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
                                        _core.lines.Enqueue(new DCLine
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
                                                _core.lines.Enqueue(new DCLine
                                                {
                                                    line = "You must provide a size value.",
                                                    color = Color.Red
                                                });
                                                return;
                                            }

                                            if (consoleCommand1.NextWord() != "")
                                            {
                                                _core.lines.Enqueue(new DCLine
                                                {
                                                    line = "Too many parameters!",
                                                    color = Color.Red
                                                });
                                                return;
                                            }

                                            try
                                            {
                                                int int32 = Convert.ToInt32(lower5);
                                                _raster = new RasterFont(fontName, int32);
                                                Options.Data.consoleFontSize = int32;
                                                _raster.scale = new Vec2(0.5f);
                                                Options.Save();
                                            }
                                            catch (Exception)
                                            {
                                            }
                                        }
                                        else
                                        {
                                            if (consoleCommand1.Remainder().Count() > 0)
                                                pFont = $"{pFont} {consoleCommand1.Remainder()}";
                                            if (pFont is "clear" or "default" or "none")
                                            {
                                                Options.Data.consoleFont = "";
                                                Options.Save();
                                                Log(DCSection.General, "|DGGREEN|Console font reset.");
                                            }
                                            else
                                            {
                                                if (pFont == "comic sans")
                                                    pFont = "comic sans ms";
                                                if (RasterFont.GetName(pFont) != null)
                                                {
                                                    _raster = new RasterFont(pFont, fontPoints);
                                                    Options.Data.consoleFont = pFont;
                                                    _raster.scale = new Vec2(0.5f);
                                                    Options.Save();
                                                    if (_raster.data.name == "Comic Sans MS")
                                                        Log(DCSection.General,
                                                            $"|DGGREEN|Font is now {_raster.data.name}! What a laugh!");
                                                    else
                                                        Log(DCSection.General,
                                                            $"|DGGREEN|Font is now {_raster.data.name}!");
                                                }
                                                else
                                                    Log(DCSection.General,
                                                        $"|DGRED|Could not find font ({pFont})!");
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }

                                break;
                            }
                            case "record" when NetworkDebugger.enabled:
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

                                break;
                            }
                            case "team" when CheckCheats():
                                return;
                            case "team":
                            {
                                flag1 = true;
                                string findName = consoleCommand1.NextWord();
                                Profile profile = ProfileByName(findName);
                                if (profile != null)
                                {
                                    string str6 = consoleCommand1.NextWord();
                                    if (str6 == "")
                                    {
                                        _core.lines.Enqueue(new DCLine
                                        {
                                            line = "Parameters in wrong format.",
                                            color = Color.Red
                                        });
                                        return;
                                    }

                                    if (consoleCommand1.NextWord() != "")
                                    {
                                        _core.lines.Enqueue(new DCLine
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
                                        _core.lines.Enqueue(new DCLine
                                        {
                                            line = $"No team named {lower}.",
                                            color = Color.Red
                                        });
                                        return;
                                    }
                                }
                                else
                                {
                                    _core.lines.Enqueue(new DCLine
                                    {
                                        line = $"No profile named {findName}.",
                                        color = Color.Red
                                    });
                                    return;
                                }

                                break;
                            }
                            case "call" when CheckCheats():
                                return;
                            case "call":
                            {
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
                                                _core.lines.Enqueue(new DCLine
                                                {
                                                    line = "Parameters in wrong format.",
                                                    color = Color.Red
                                                });
                                                return;
                                            }

                                            if (consoleCommand1.NextWord() != "")
                                            {
                                                _core.lines.Enqueue(new DCLine
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
                                                    if (methodInfo.GetParameters().Count() > 0)
                                                    {
                                                        _core.lines.Enqueue(new DCLine
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
                                                        _core.lines.Enqueue(new DCLine
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
                                                _core.lines.Enqueue(new DCLine
                                                {
                                                    line = $"Duck has no function called {str8}.",
                                                    color = Color.Red
                                                });
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            _core.lines.Enqueue(new DCLine
                                            {
                                                line = $"{str7} is not in the game!",
                                                color = Color.Red
                                            });
                                            return;
                                        }
                                    }
                                }

                                if (!flag4)
                                {
                                    _core.lines.Enqueue(new DCLine
                                    {
                                        line = $"No profile named {str7}.",
                                        color = Color.Red
                                    });
                                    return;
                                }

                                break;
                            }
                            case "set" when CheckCheats():
                                return;
                            case "set":
                            {
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
                                                _core.lines.Enqueue(new DCLine
                                                {
                                                    line = "Parameters in wrong format.",
                                                    color = Color.Red
                                                });
                                                return;
                                            }

                                            Type type = typeof(Duck);
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
                                                            _core.lines.Enqueue(new DCLine
                                                            {
                                                                line = "Parameters in wrong format.",
                                                                color = Color.Red
                                                            });
                                                            return;
                                                        }

                                                        if (consoleCommand1.NextWord() != "")
                                                        {
                                                            _core.lines.Enqueue(new DCLine
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
                                                            _core.lines.Enqueue(new DCLine
                                                            {
                                                                line = "Parameters in wrong format.",
                                                                color = Color.Red
                                                            });
                                                            return;
                                                        }

                                                        if (consoleCommand1.NextWord() != "")
                                                        {
                                                            _core.lines.Enqueue(new DCLine
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
                                                            _core.lines.Enqueue(new DCLine
                                                            {
                                                                line = "Parameters in wrong format.",
                                                                color = Color.Red
                                                            });
                                                            return;
                                                        }

                                                        if (consoleCommand1.NextWord() != "")
                                                        {
                                                            _core.lines.Enqueue(new DCLine
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
                                                            _core.lines.Enqueue(new DCLine
                                                            {
                                                                line = "Parameters in wrong format.",
                                                                color = Color.Red
                                                            });
                                                            return;
                                                        }

                                                        if (consoleCommand1.NextWord() != "")
                                                        {
                                                            _core.lines.Enqueue(new DCLine
                                                            {
                                                                line = "Too many parameters!",
                                                                color = Color.Red
                                                            });
                                                            return;
                                                        }

                                                        propertyInfo.SetValue(profile.duck, new Vec2(single3, single4),
                                                            null);
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
                                                                _core.lines.Enqueue(new DCLine
                                                                {
                                                                    line = "Parameters in wrong format.",
                                                                    color = Color.Red
                                                                });
                                                                return;
                                                            }

                                                            if (consoleCommand1.NextWord() != "")
                                                            {
                                                                _core.lines.Enqueue(new DCLine
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
                                                                _core.lines.Enqueue(new DCLine
                                                                {
                                                                    line = "Parameters in wrong format.",
                                                                    color = Color.Red
                                                                });
                                                                return;
                                                            }

                                                            if (consoleCommand1.NextWord() != "")
                                                            {
                                                                _core.lines.Enqueue(new DCLine
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
                                                                _core.lines.Enqueue(new DCLine
                                                                {
                                                                    line = "Parameters in wrong format.",
                                                                    color = Color.Red
                                                                });
                                                                return;
                                                            }

                                                            if (consoleCommand1.NextWord() != "")
                                                            {
                                                                _core.lines.Enqueue(new DCLine
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
                                                                _core.lines.Enqueue(new DCLine
                                                                {
                                                                    line = "Parameters in wrong format.",
                                                                    color = Color.Red
                                                                });
                                                                return;
                                                            }

                                                            if (consoleCommand1.NextWord() != "")
                                                            {
                                                                _core.lines.Enqueue(new DCLine
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
                                                    _core.lines.Enqueue(new DCLine
                                                    {
                                                        line = $"Duck has no variable called {str10}.",
                                                        color = Color.Red
                                                    });
                                                    return;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _core.lines.Enqueue(new DCLine
                                            {
                                                line = $"{str9} is not in the game!",
                                                color = Color.Red
                                            });
                                            return;
                                        }
                                    }
                                }

                                if (!flag6)
                                {
                                    _core.lines.Enqueue(new DCLine
                                    {
                                        line = $"No profile named {str9}.",
                                        color = Color.Red
                                    });
                                    return;
                                }

                                break;
                            }
                            case "globalscores" when CheckCheats():
                                return;
                            case "globalscores":
                            {
                                flag1 = true;
                                using List<Profile>.Enumerator enumerator = Profiles.active.GetEnumerator();
                                if (enumerator.MoveNext())
                                {
                                    Profile current = enumerator.Current;
                                    _core.lines.Enqueue(new DCLine
                                    {
                                        line = $"{current.name}: {current.stats.CalculateProfileScore():0.000}",
                                        color = Color.Red
                                    });
                                }

                                break;
                            }
                            case "scorelog" when CheckCheats():
                                return;
                            case "scorelog":
                            {
                                flag1 = true;
                                string str11 = consoleCommand1.NextWord();
                                if (consoleCommand1.NextWord() != "")
                                {
                                    _core.lines.Enqueue(new DCLine
                                    {
                                        line = "Too many parameters!",
                                        color = Color.Red
                                    });
                                    return;
                                }

                                if (str11 == "")
                                {
                                    _core.lines.Enqueue(new DCLine
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
                                    _core.lines.Enqueue(new DCLine
                                    {
                                        line = "Parameters in wrong format.",
                                        color = Color.Red
                                    });
                                    return;
                                }

                                _core.logScores = int32;
                                break;
                            }
                        }
                    }

                    if (flag1)
                        return;
                    _core.lines.Enqueue(new DCLine
                    {
                        line = $"{pKeyword} is not a valid command!",
                        color = Color.Red
                    });
                }
            }
        }

        private static bool CheckCheats()
        {
            if (NetworkDebugger.enabled)
                return false;
            bool flag = Steam.user != null && Steam.user.id is 76561197996786074UL or 76561198885030822UL or 76561198416200652UL or 76561198104352795UL or 76561198114791325UL;
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

                _core.lines.Enqueue(new DCLine
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
            if (text.Contains('\n'))
            {
                string str = text;
                char[] chArray = new char[1] {'\n'};
                foreach (string text1 in str.Split(chArray))
                    Log(text1, c, scale, index);
            }
            else
            {
                DCLine dcLine = new()
                {
                    line = text,
                    color = c,
                    threadIndex = index < 0 ? NetworkDebugger.currentIndex : index,
                    timestamp = DateTime.Now
                };
                if (NetworkDebugger.enabled)
                {
                    lock (debuggerLines)
                    {
                        Console.WriteLine(text);
                        debuggerLines.Add(dcLine);
                    }
                }
                else
                {
                    lock (_core.pendingLines)
                    {
                        Console.WriteLine(text);
                        _core.pendingLines.Add(dcLine);
                    }
                }
            }
        }

        public static void Log(string text) => Log(DCSection.General, text);
        public static void Log(object? obj) => Log(obj?.ToString() ?? "null");
        public static void Log(params object[]? obj)
        {
            string objstring = "";
            if (obj != null)
            {
                for (int i = 0; i < obj.Length; i++)
                {
                    objstring += " " + obj[i]?.ToString() ?? "null";
                }
            }
            Log(objstring);
        }
        public static void Log(string text, Color c, float scale = 2f, int index = -1)
        {
            DCLine dcLine = new()
            {
                line = text,
                color = c,
                threadIndex = index < 0 ? NetworkDebugger.currentIndex : index,
                timestamp = DateTime.Now
            };
            if (NetworkDebugger.enabled)
            {
                lock (debuggerLines)
                {
                    Console.WriteLine(text);
                    debuggerLines.Add(dcLine);
                }
            }
            else
            {
                lock (_core.pendingLines)
                {
                    Console.WriteLine(text);
                    _core.pendingLines.Add(dcLine);
                }
            }
        }

        public static void RefreshConsoleFont()
        {
            _raster = null;
            InitializeFont();
        }

        public static void LogEvent(string pDescription, NetworkConnection pConnection)
        {
            if (pDescription == null)
                pDescription = "No Description.";
                
            Log($"@LOGEVENT@|AQUA|LOGEVENT!-----------{pConnection}|AQUA|signalled a log event!-----------!LOGEVENT", Color.White);
            Log($"@LOGEVENT@|AQUA|LOGEVENT!---{pDescription}|AQUA|---!LOGEVENT", Color.White);
            if (!Network.isActive || pConnection != DuckNetwork.localConnection)
                return;
            Send.Message(new NMLogEvent(pDescription));
        }

        public static void Log(DCSection section, string text, int netIndex = -1) =>
            Log(section, Verbosity.Normal, text, netIndex);

        public static void Log(
            DCSection section,
            string text,
            NetworkConnection context,
            int netIndex = -1)
        {
            if (context != null)
                text += context.ToString();
            Log(section, Verbosity.Normal, text, netIndex);
        }

        public static void Log(DCSection section, Verbosity verbose, string text, int netIndex = -1)
        {
            DCLine dcLine = new()
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
                lock (debuggerLines)
                {
                    Console.WriteLine(text);
                    debuggerLines.Add(dcLine);
                }   
            }
            else
            {
                lock (_core.pendingLines)
                {
                    Console.WriteLine(text);
                    _core.pendingLines.Add(dcLine);
                }
            }
        }

        public static void SendNetLog(NetworkConnection pConnection)
        {
            List<string> stringList = new();
            string str = "";
            for (int index = Math.Max(core.lines.Count - 750, 0);
                 index < core.lines.Count;
                 ++index)
            {
                str += core.lines.ElementAt(index).ToSendString();
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
                Queue<NetMessage> pendingSends = _core.pendingSends;
                NMLogRequestChunk nmLogRequestChunk = new(pData)
                {
                    connection = pConnection
                };
                pendingSends.Enqueue(nmLogRequestChunk);
            }
        }

        public static void SaveNetLog(string pName = null)
        {
            FlushPendingLines();
            string text = "";
            for (int index = Math.Max(core.lines.Count - 1500, 0);
                 index < core.lines.Count;
                 ++index)
                text += core.lines.ElementAt(index).ToSendString();
            if (pName == null)
                pName =
                    $"{DateTime.Now.ToShortDateString().Replace('/', '_')}_{DateTime.Now.ToLongTimeString().Replace(':', '_')}_{Steam.user.name}_netlog.rtf";
            else if (!pName.EndsWith(".rtf"))
                pName += ".rtf";
            string str = DuckFile.FixInvalidPath(DuckFile.logDirectory + pName);
            if (File.Exists(str))
                File.Delete(str);
            RichTextBox richTextBox = new FancyBitmapFont().MakeRTF(text);
            DuckFile.CreatePath(str);
            string path = str;
            richTextBox.SaveFile(path);
        }

        public static void LogTransferComplete(NetworkConnection pConnection)
        {
            string receivedLogData = core.GetReceivedLogData(pConnection);
            if (receivedLogData != null)
            {
                string pathString = DuckFile.FixInvalidPath(
                    $"{DuckFile.logDirectory}{DateTime.Now.ToShortDateString().Replace('/', '_')}_{DateTime.Now.ToLongTimeString().Replace(':', '_')}_{pConnection.name}_netlog.rtf");
                RichTextBox richTextBox = new FancyBitmapFont().MakeRTF(receivedLogData);
                DuckFile.CreatePath(pathString);
                string path = pathString;
                richTextBox.SaveFile(path);
            }

            core.requestingLogs.Remove(pConnection);
            core.receivingLogs.Remove(pConnection);
            pConnection.logTransferProgress = 0;
            pConnection.logTransferSize = 0;
        }

        public static void LogSendingComplete(NetworkConnection pConnection)
        {
            core.transferRequestsPending.Remove(pConnection);
            DuckNetwork.core.logTransferProgress = 0;
            DuckNetwork.core.logTransferSize = 0;
        }

        public static void Chart(string chart, string section, double x, double y, Color c)
        {
            lock (_core.pendingChartValues)
                _core.pendingChartValues.Add(new DCChartValue
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
            AddCommand(new CMD("poweruser", () =>
            {
                Options.Data.powerUser = !Options.Data.powerUser;
                _core.lines.Enqueue(new DCLine
                {
                    line = $"Power User mode has been {(Options.Data.powerUser ? "enabled" : "disabled")}!",
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
            AddCommand(new CMD("oldangles", () =>
            {
                Options.Data.oldAngleCode = !Options.Data.oldAngleCode;
                _core.lines.Enqueue(new DCLine
                {
                    line = $"Oldschool Angles have been {(Options.Data.oldAngleCode ? "enabled" : "disabled")}!",
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
            AddCommand(new CMD("sing", new CMD.Argument[1]
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
            AddCommand(new CMD("downpour", cmd =>
            {
                float num1 = (float) (Level.current.bottomRight.x - (double) Level.current.topLeft.x + 128.0);
                int num2 = 10;
                for (int index1 = 0; index1 < 10; ++index1)
                {
                    for (int index2 = 0; index2 < num2; ++index2)
                    {
                        PhysicsObject randomItem = ItemBoxRandom.GetRandomItem();
                        randomItem.position = Level.current.topLeft +
                                              new Vec2(
                                                  (float) ((double) num1 / num2 * index2 +
                                                      Rando.Float(sbyte.MinValue, 128f) - 64.0),
                                                  Level.current.topLeft.y - 2000f - 512 * index1 +
                                                  Rando.Float(-256f, 256f));
                        Level.Add(randomItem);
                    }
                }
            })
            {
                hidden = true,
                cheat = true
            });
            if (!Steam.IsInitialized())
                return;
            AddCommand(new CMD("zipcloud", () =>
            {
                string pFile = $"{DuckFile.saveDirectory}cloud_zip.zip";
                Cloud.ZipUpCloudData(pFile);
                _core.lines.Enqueue(new DCLine
                {
                    line = $"Zipped up to: {pFile}",
                    color = Colors.DGBlue
                });
            })
            {
                hidden = true,
                cheat = true
            });
            AddCommand(new CMD("sendsave", () =>
            {
                _core.lines.Enqueue(new DCLine
                {
                    line = "Please type a message to the developer then and press enter..",
                    color = Color.White
                });
                _doDataSubmission = true;
            })
            {
                hidden = true,
                cheat = true
            });
            AddCommand(new CMD("clearsave", () =>
            {
                _core.lines.Enqueue(new DCLine
                {
                    line = "ARE YOU SURE? ALL SAVE DATA WILL BE DELETED",
                    color = Color.Red
                });
                _core.lines.Enqueue(new DCLine
                {
                    line = "LOCALLY, AND FROM THE CLOUD.",
                    color = Color.Red
                });
                _core.lines.Enqueue(new DCLine
                {
                    line = "ENTER 'corptron' IF YOU WANT TO CONTINUE..",
                    color = Color.Red
                });
            })
            {
                hidden = true,
                cheat = true
            });
            
            AddCommand(new CMD("savedir", () =>
            {
                Process.Start(DuckFile.saveDirectory);
                _core.lines.Enqueue(new DCLine
                {
                    line = "Save directory was opened.",
                    color = Color.White
                });
            })
            {
                hidden = true,
                cheat = true
            });
            AddCommand(new CMD("userdir", () =>
            {
                Process.Start(DuckFile.userDirectory);
                _core.lines.Enqueue(new DCLine
                {
                    line = "User directory was opened.",
                    color = Color.White
                });
            })
            {
                hidden = true,
                cheat = true
            });
            AddCommand(new CMD("recover", () =>
            {
                _core.lines.Enqueue(new DCLine
                {
                    line = "ARE YOU SURE? ALL NEW SAVE DATA",
                    color = Color.Red
                });
                _core.lines.Enqueue(new DCLine
                {
                    line = "WILL BE OVERWRITTEN BY PRE 1.5 DATA",
                    color = Color.Red
                });
                _core.lines.Enqueue(new DCLine
                {
                    line = "ENTER 'corptron' IF YOU WANT TO CONTINUE..",
                    color = Color.Red
                });
            })
            {
                hidden = true,
                cheat = true
            });
            AddCommand(
                new CMD("managecloud", () => (MonoMain.pauseMenu = new UICloudManagement(null)).Open())
                {
                    hidden = true,
                    cheat = true
                });
            AddCommand(
                new CMD("manageblocks", () => (MonoMain.pauseMenu = new UIBlockManagement(null)).Open())
                {
                    hidden = true,
                    cheat = true
                });
            AddCommand(new CMD("corptron", () =>
            {
                if (lastCommand != null && lastCommand.keyword == "clearsave")
                {
                    Cloud.DeleteAllCloudData(false);
                    DuckFile.DeleteAllSaveData();
                    _core.lines.Enqueue(new DCLine
                    {
                        line = "All save data has been deleted.",
                        color = Color.Red
                    });
                }

                if (lastCommand == null || !(lastCommand.keyword == "recover"))
                    return;
                DuckFile.DeleteFolder(DuckFile.userDirectory);
                while (Cloud.processing)
                    Cloud.Update();
                Program.crashed = true;
                Process.Start(Application.ExecutablePath, $"{Program.commandLine} -recoversave");
                Application.Exit();
            })
            {
                hidden = true,
                cheat = true
            });
            AddCommand(new CMD("savetool", () =>
            {
                if (!File.Exists("SaveTool.dll"))
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
            lock (_core.pendingLines)
            {
                foreach (DCLine pendingLine in _core.pendingLines)
                {
                    _core.lines.Enqueue(pendingLine);
                    if (_core.viewOffset != 0)
                        ++_core.viewOffset;
                }

                if (_core.lines.Count > 3000)
                {
                    for (int index = 0; index < 500; ++index)
                    {
                        _core.lines.Dequeue();
                        if (_core.viewOffset > 0)
                            --_core.viewOffset;
                    }
                }

                _core.pendingLines.Clear();
            }
        }

        private static bool WasDownLastFrame;

        public static void Update()
        {
            // checks if its not null and if it's length is greater than 0
            if (DevConsoleCommands.Binds is { Count: > 0 })
            {
                foreach (DevConsoleCommands.ConsoleBind bind in DevConsoleCommands.Binds)
                {
                    bind?.TryExecute();
                }
            }
        
            if (_core == null)
                return;
            if (_doDataSubmission && _dataSubmissionMessage != null)
            {
                SubmitSaveData(_dataSubmissionMessage);
                _core.lines.Enqueue(new DCLine
                {
                    line = "|DGGREEN|Save data submitted!",
                    color = Colors.DGBlue
                });
                _doDataSubmission = false;
                _dataSubmissionMessage = null;
            }

            FlushPendingLines();
            bool flag = Keyboard.Down(Keys.LeftShift) || Keyboard.Down(Keys.RightShift);
            int num1 = !(Keyboard.Down(Keys.OemTilde) && !WasDownLastFrame)
                ? 0
                : (!flag
                    ? 1
                    : 0); // Replaced !(Keyboard.Pressed(Keys.OemTilde)) ? with that because Press can cause issues with it auto trying to close 

            //Just for NiK0 Purposes
            if (Keyboard.Down(Keys.LeftControl) && Keyboard.Pressed(Keys.NumPad2)) num1 = 1;

            WasDownLastFrame = Keyboard.Down(Keys.OemTilde);
            if (core.pendingSends.Count > 0)
            {
                NetMessage msg = core.pendingSends.Dequeue();
                Send.Message(msg, msg.connection);
            }

            if (num1 != 0 && !DuckNetwork.core.enteringText && NetworkDebugger.hoveringInstance)
            {
                if (_tray == null)
                {
                    _tray = new Sprite("devTray");
                    _scan = new Sprite("devScan");
                }

                _core.open = !_core.open;
                Keyboard.keyString = "";
                _core.cursorPosition = _core.typing.Length;
                _core.lastCommandIndex = -1;
                _core.viewOffset = 0;
            }

            _core.alpha = Maths.LerpTowards(_core.alpha, _core.open ? 1f : 0.0f, 0.1f);
            if (_pendingCommandQueue.Count > 0)
            {
                QueuedCommand queuedCommand = _pendingCommandQueue.Peek();
                if (queuedCommand.wait > 0)
                    --queuedCommand.wait;
                else if (queuedCommand.waitCommand == null || queuedCommand.waitCommand())
                {
                    _pendingCommandQueue.Dequeue();
                    if (queuedCommand.command != null)
                        RunCommand(queuedCommand.command);
                }
            }

            if (_core.open && NetworkDebugger.hoveringInstance)
            {
                Input._imeAllowed = true;
                if (_core.cursorPosition > _core.typing.Length)
                    _core.cursorPosition = _core.typing.Length;
                _core.typing = _core.typing.Insert(_core.cursorPosition,
                    Keyboard.keyString.Replace("`", "")); // added the Replace because the fix to the input makes it possible to do this if holding it down
                if (_core.typing != "" && _pendingCommandQueue.Count > 0)
                {
                    _pendingCommandQueue.Clear();
                    _core.lines.Enqueue(new DCLine
                    {
                        line = "Pending commands cleared!",
                        color = Colors.DGOrange
                    });
                }

                if (Keyboard.keyString.Length > 0)
                {
                    _core.cursorPosition += Keyboard.keyString.Length;
                    _core.lastCommandIndex = -1;
                }

                Keyboard.keyString = "";
                if (Keyboard.control)
                {
                    if (Keyboard.Pressed(Keys.C))
                    {
                        if (!string.IsNullOrWhiteSpace(_core.typing))
                        {
                            Thread thread = new(() => SDL.SDL_SetClipboardText(_core.typing));
                            thread.SetApartmentState(ApartmentState.STA);
                            thread.Start();
                            thread.Join();
                            HUD.AddPlayerChangeDisplay("@CLIPCOPY@Copied!");
                        }
                    }
                    else if (Keyboard.Pressed(Keys.V))
                    {
                        string paste = "";
                        Thread thread = new(() => paste = SDL.SDL_GetClipboardText());
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                        thread.Join();
                        string[] strArray = paste.Replace('\r', '\n').Split('\n');
                        List<string> stringList = new();
                        foreach (string str in strArray)
                        {
                            if (!string.IsNullOrWhiteSpace(str))
                                stringList.Add(str);
                        }

                        if (stringList.Count == 1)
                        {
                            string value = stringList[0].Trim();
                            _core.typing = _core.typing.Insert(Math.Min(_core.typing.Length, _core.cursorPosition), value);
                            _core.cursorPosition = _core.cursorPosition + value.Length;
                        }
                        else
                        {
                            _core.typing = "";
                            _core.cursorPosition = 0;
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

                                _pendingCommandQueue.Enqueue(new QueuedCommand
                                {
                                    command = str2,
                                    wait = num2,
                                    waitCommand = func
                                });
                            }
                        }
                    }
                }

                if (Keyboard.Pressed(Keys.Enter) && !string.IsNullOrWhiteSpace(_core.typing))
                {
                    RunCommand(_core.typing);
                    _core.previousLines.Add(_core.typing);
                    _core.typing = "";
                    Keyboard.keyString = "";
                    _core.lastCommandIndex = -1;
                    _core.viewOffset = 0;
                }
                else if (Keyboard.Pressed(Keys.Back))
                {
                    if (_core.typing.Length > 0 && _core.cursorPosition > 0)
                    {
                        _core.typing =
                            _core.typing.Remove(_core.cursorPosition - 1, 1);
                        --_core.cursorPosition;
                    }

                    _core.lastCommandIndex = -1;
                }
                else if (Keyboard.Pressed(Keys.Delete))
                {
                    if (_core.typing.Length > 0 &&
                        _core.cursorPosition < _core.typing.Length)
                        _core.typing = _core.typing.Remove(_core.cursorPosition, 1);
                    _core.lastCommandIndex = -1;
                }
                else if (Keyboard.Pressed(Keys.Left))
                    _core.cursorPosition = Math.Max(0, _core.cursorPosition - 1);
                else if (Keyboard.Pressed(Keys.Right))
                    _core.cursorPosition = Math.Min(_core.typing.Length,
                        _core.cursorPosition + 1);
                else if (Keyboard.Pressed(Keys.Home))
                {
                    if (Keyboard.shift)
                        _core.viewOffset = core.lines.Count - 1;
                    else
                        _core.cursorPosition = 0;
                }
                else if (Keyboard.Pressed(Keys.End))
                {
                    if (Keyboard.shift)
                        _core.viewOffset = 0;
                    else
                        _core.cursorPosition = _core.typing.Length;
                }

                if (Keyboard.Pressed(Keys.PageUp))
                {
                    _core.viewOffset += Keyboard.shift ? 10 : 1;
                    if (_core.viewOffset > core.lines.Count - 1)
                        _core.viewOffset = core.lines.Count - 1;
                }

                if (Keyboard.Pressed(Keys.PageDown))
                {
                    _core.viewOffset -= Keyboard.shift ? 10 : 1;
                    if (_core.viewOffset < 0)
                        _core.viewOffset = 0;
                }

                if (Mouse.isScrolling)
                {
                    _core.viewOffset -= (Keyboard.shift ? 10 : 1) * Mouse.discreteScroll;
                    if (_core.viewOffset < 0) _core.viewOffset = 0;
                    if (_core.viewOffset > core.lines.Count - 1) _core.viewOffset = core.lines.Count - 1;
                }

                if (Keyboard.Pressed(Keys.Up) && _core.previousLines.Count > 0)
                {
                    ++_core.lastCommandIndex;
                    if (_core.lastCommandIndex >= _core.previousLines.Count)
                        _core.lastCommandIndex = _core.previousLines.Count - 1;
                    _core.typing =
                        _core.previousLines[
                            _core.previousLines.Count - 1 - _core.lastCommandIndex];
                    _core.cursorPosition = _core.typing.Length;
                }

                if (!Keyboard.Pressed(Keys.Down))
                    return;
                if (_core.previousLines.Count > 0 && _core.lastCommandIndex > 0)
                {
                    --_core.lastCommandIndex;
                    _core.typing =
                        _core.previousLines[
                            _core.previousLines.Count - 1 - _core.lastCommandIndex];
                    _core.cursorPosition = _core.typing.Length;
                }
                else if (_core.lastCommandIndex == 0)
                {
                    --_core.lastCommandIndex;
                    _core.cursorPosition = 0;
                    _core.typing = "";
                }
                else
                {
                    if (_core.lastCommandIndex != -1)
                        return;
                    _core.cursorPosition = 0;
                    _core.typing = "";
                }
            }
            else
                lastCommand = null;
        }

        private class QueuedCommand
        {
            public Func<bool> waitCommand;
            public string command;
            public int wait;
        }
    }
}
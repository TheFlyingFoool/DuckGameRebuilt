using AddedContent.Firebreak;
using AddedContent.Firebreak.DuckShell.Implementation;
using DuckGame.ConsoleEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SDL2;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;

namespace DuckGame
{
    public class DevConsole
    {
        [Marker.AutoConfig]
        public static bool showFPS;
        public static List<string> startupCommands = new();
        public static bool fancyMode;
        public static int ConsoleLineOffset;
        private static DevConsoleCore _core = new();
        private static bool _enableNetworkDebugging;
        public static bool EnableNetworkDebugging
        {
            get => _enableNetworkDebugging;
            set => _enableNetworkDebugging = value;
        }

        private static bool _oldConsole;
        public static bool debugOrigin;
        public static bool debugBounds;
        private static RasterFont _raster;
        public static RasterFont RasterFont
        {
            get => _raster;
            set => _raster = value;
        }

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

        static DevConsole()
        {
            core.OnTextChange += UpdateTextWithCompletion;
            core.OnTextChange += UpdateAutocomplete;
        }

        private static void UpdateTextWithCompletion(string previous, string current)
        {
            int changeLength = current.Length - previous.Length;
            bool addedWhitespace = current.Length > previous.Length 
                                   && string.IsNullOrWhiteSpace(current.Substring(previous.Length, changeLength));
            
            if (!addedWhitespace)
                return;
            
            ImplementCompletionSuggestion(previous, _core.cursorPosition);
        }

        private static void UpdateAutocomplete(string previous, string current)
        {
            if (!DGRSettings.UseDuckShell || !DGRSettings.DuckShellAutoCompletion)
                return;

            if (s_suppressAutocompletionForNow)
            {
                s_suppressAutocompletionForNow = false;
                return;
            }

            ValueOrException<string[]> predictionResult;

            // damn this firebreak guy seems pretty fucking lazy
            try
            {
                int truCaretPos = _core.cursorPosition;

                if (current.Length > previous.Length)
                    truCaretPos++;
                else if (current.Length < previous.Length)
                    truCaretPos--;
                
                predictionResult = Commands.console.Shell.Predict(current, truCaretPos);
            }
            catch (Exception e)
            {
                predictionResult = e;
            }

            if (predictionResult.Failed)
            {
                LatestPredictionSuggestions = new[] {$"|DGRED|{predictionResult.Error.Message}"};
                s_canUsePrediction = false;
            }
            else
            {
                LatestPredictionSuggestions = predictionResult.Value;

                if (LatestPredictionSuggestions.Length == 0)
                {
                    s_canUsePrediction = false;
                }
                else
                {
                    s_canUsePrediction = true;
                }
            }
            
            // reset choice
            s_HighlightedSuggestionIndex = -1;
        }

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

                    //string str1 = "DirectInputDevices:\n===================================\n";
                    //for (int index = 0; index < 32; ++index)
                    //{
                    //    try
                    //    {
                    //        if (DInput.GetState(index) != null)
                    //        {
                    //            string str2 =
                    //                $"{DInput.GetProductName(index)}{DInput.GetProductGUID(index)} {DInput.IsXInput(index)}";
                    //            str1 = $"{str1}{str2}\n";
                    //        }
                    //        else
                    //            break;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        str1 = $"{str1}\n{ex}\n";
                    //    }
                    //}

                    string str3 = $"Enumerated Input Devices:\n===================================\n";
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
                (HttpWebRequest)WebRequest.Create("http://www.wonthelp.info/DuckWeb/submitSave.php");
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
            HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
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
                _core.fpsfont = new BitmapFont("biosFont", 8)
                {
                    scale = new Vec2(0.5f, 0.5f)
                };
            }
            if (_core.alpha > 0.01f)
            {
                InitializeFont();
                if (_tray == null)
                    return;
                _tray.alpha = _core.alpha;
                if (Resolution.current.x > 320 && Resolution.current.y > 180)
                {
                    _tray.scale = new Vec2((float)(Math.Round(Resolution.current.x / 1280f * 2f) / 2f) * 2f) * (consoleScale + 1) / 2f;
                }
                else // for extra extra small scale
                {
                    _tray.scale = new Vec2((Resolution.current.x / 1280f * 2f) / 2f * 2f) * (consoleScale + 1) / 2f;
                }
                
                _tray.depth = 0.75f;
                if (Layer.core._console != null)
                {
                    int numSectionsVert = (int)((Layer.core._console.camera.height * dimensions.y) / (16.0f * _tray.scale.y)) - 2;
                    int numSectionsHor = (int)((Layer.core._console.camera.width * dimensions.x) / (16.0f * _tray.scale.x)) - 2;
                    Graphics.Draw(_tray, 0, 0, new Rectangle(0, 0, 18, 18));
                    Graphics.Draw(_tray, 0, (18 * _tray.scale.y) + (((numSectionsVert) * (16.0f * _tray.scale.y))), new Rectangle(0, _tray.height - 18, 18, 18));
                    Graphics.Draw(_tray, (18 * _tray.scale.x) + (((numSectionsHor - 6) * (16.0f * _tray.scale.x))), (18 * _tray.scale.y) + (((numSectionsVert) * (16.0f * _tray.scale.y))), new Rectangle(_tray.width - 114, _tray.height - 18, 114, 18));
                    for (int index = 0; index < numSectionsHor; ++index)
                    {
                        const float variableNameConflictIForgotWhatThisDoesHeyHereHaveThisName = 16f;
                        Graphics.Draw(_tray, 18f * _tray.scale.x + 16f * _tray.scale.x * index, 0.0f,
                            new Rectangle(variableNameConflictIForgotWhatThisDoesHeyHereHaveThisName, 0.0f, variableNameConflictIForgotWhatThisDoesHeyHereHaveThisName, 18f));
                        if (index < numSectionsHor - 6)
                            Graphics.Draw(_tray, 18f * _tray.scale.x + 16f * _tray.scale.x * index,
                                18f * _tray.scale.y + numSectionsVert * (16f * _tray.scale.y),
                                new Rectangle(variableNameConflictIForgotWhatThisDoesHeyHereHaveThisName, _tray.height - 18, variableNameConflictIForgotWhatThisDoesHeyHereHaveThisName, 18f));
                    }

                    Graphics.Draw(_tray, 18f * _tray.scale.x + numSectionsHor * (16f * _tray.scale.x), 0f,
                        new Rectangle(_tray.width - 18, 0f, 18f, 18f));
                    for (int index = 0; index < numSectionsVert; ++index)
                    {
                        Graphics.Draw(_tray, 0.0f, 18f * _tray.scale.y + 16f * _tray.scale.y * index,
                            new Rectangle(0.0f, 18f, 18f, 16f));
                        Graphics.Draw(_tray, 18f * _tray.scale.x + numSectionsHor * (16f * _tray.scale.x),
                            18f * _tray.scale.y + 16f * _tray.scale.y * index,
                            new Rectangle(_tray.width - 18, 18f, 18f, 16f));
                    }

                    Graphics.DrawRect(Vec2.Zero,
                        new Vec2(18f * _tray.scale.x + numSectionsHor * (16f * _tray.scale.x) + _tray.scale.y * 4f,
                            (numSectionsVert + 2) * (16f * _tray.scale.y)), Color.Black * 0.8f * _core.alpha,
                        0.7f);
                    _core.fancyFont.scale = new Vec2(_tray.scale.x / 2f);
                    _core.fancyFont.depth = 0.98f;
                    _core.fancyFont.alpha = _core.alpha;
                    float height = (float)((numSectionsVert + 1) * 16 * (double)_tray.scale.y + 5f * _tray.scale.y);
                    float width = (numSectionsHor + 2) * (16f * _tray.scale.x);
                    string version = DG.version;
                    _core.fancyFont.Draw(version,
                        new Vec2(82f * _tray.scale.x + (numSectionsHor - 6) * (16f * _tray.scale.x),
                            height + 7f * _tray.scale.y), new Color(62, 114, 122), 0.98f);
                    _core.cursorPosition = Math.Min(Math.Max(_core.cursorPosition, 0),
                        _core.Typing.Length);
                    Vec2 p1;
                    if (_raster != null)
                    {
                        _raster.scale = new Vec2(0.5f);
                        _raster.alpha = _core.alpha;
                        _raster.Draw(_core.Typing.Replace("\n", $"{DevConsoleCommands.SyntaxColorComments.ToDGColorString()}\\n|PREV|"), 4f * _tray.scale.x, (float)(height + _tray.scale.y * 8f - _raster.characterHeight * (double)_raster.scale.y / 2f), Color.White, 0.9f);
                        
                        p1 = new(
                            _raster.GetWidth(_core.Typing.Replace("\n", "\\n").Substring(0, _core.cursorPosition) + new string('n', _core.Typing.Substring(0, _core.cursorPosition).Count(x => x == '\n'))) + 4f * _tray.scale.x + 1f,
                            height + 6f * _tray.scale.y
                            );
                        
                        Graphics.DrawLine(p1, p1 + new Vec2(0.0f, 4f * _tray.scale.x), Color.White,
                            depth: 1f);
                    }
                    else
                    {
                        _core.font.scale = new Vec2(_tray.scale.x / 2f);
                        _core.font.alpha = _core.alpha;
                        _core.font.Draw(_core.Typing.Replace("\n", $"{DevConsoleCommands.SyntaxColorComments.ToDGColorString()}\\n|PREV|"), 4f * _tray.scale.x,
                            height + 6f * _tray.scale.y, Color.White, 0.9f);
                        p1 = new(
                            _core.font.GetWidth(_core.Typing.Replace("\n", "\\n").Substring(0, _core.cursorPosition) + new string('n', _core.Typing.Substring(0, _core.cursorPosition).Count(x => x == '\n'))) + 4f * _tray.scale.x + 1f,
                            height + 6f * _tray.scale.y);
                        Graphics.DrawLine(p1, p1 + new Vec2(0.0f, 4f * _tray.scale.x), Color.White, 2f, 1f);
                    }
                    
                    if (DGRSettings.UseDuckShell && DGRSettings.DuckShellAutoCompletion)
                    {
                        int length = Math.Min(CommandSuggestionLimit, LatestPredictionSuggestions.Length);
                        for (int i = 0; i < length; i++)
                        {
                            string suggestion = LatestPredictionSuggestions[i];

                            const float fontSize = 1f;
                            Vec2 stringSize = Extensions.GetStringSize(suggestion, fontSize);
                            Vec2 drawPos = p1 + new Vec2(4, -4 - stringSize.y - ((stringSize.y + 2) * i));
                            Rectangle textBounds = new(drawPos.x, drawPos.y, stringSize.x, stringSize.y);
                                
                            Graphics.DrawString(suggestion, drawPos, s_HighlightedSuggestionIndex == i ? Color.Aqua : Color.Yellow, 1.3f, scale: fontSize);
                            Graphics.DrawRect(textBounds, Color.Black, 1.2f);
                        }
                    }

                    float vOffset = 0.0f;
                    _core.font.scale = new Vec2((float)Math.Max(Math.Round(_tray.scale.x / 4f), 1f));
                    float mul = _core.font.scale.x / 2f;
                    float lineHeight = 18f * mul;
                    float lineNumWidth = 20f * (_core.font.scale.x * 2f) + core.font.GetWidth("HH:mm:ss ");
                    if (_raster != null)
                    {
                        lineHeight = (_raster.characterHeight - 2) * _raster.scale.y;
                        vOffset = lineHeight;
                        lineNumWidth = _raster.GetWidth("HH:mm:ss 0000  ");
                    }

                    vOffset -= lineHeight * core.viewOffset;

                    int maxDrawnLines = (int) ((height - 2f * _tray.scale.y) / lineHeight);
                    for (int itemDrawIndex = 0, itemTrueIndex = (_core.lines.Count - 1), skipNext = 0; itemTrueIndex >= 0; ++itemDrawIndex, --itemTrueIndex)
                    {
                        if (itemDrawIndex - _core.viewOffset >= maxDrawnLines - skipNext)
                            break;
                        
                        DCLine dcLine = _core.lines.ElementAt(itemTrueIndex);

                        string lineNumber = itemTrueIndex.ToString().PadLeft(4, '0');
                        string timeString = $"{dcLine.timestamp:HH:mm:ss} ";
                        lineNumber = timeString + lineNumber;
                        float posX = 4f * _tray.scale.x;
                        Color lineNumColor = itemTrueIndex % 2 > 0 ? Color.Gray * 0.4f : Color.Gray * 0.6f;

                        string originalLineText = dcLine.SectionString() + dcLine.line;
                        
                        if (_raster != null)
                        {
                            int maxCharsPerLine = (int)((width - posX - lineNumWidth - 32f) / _raster.GetWidth("M"));
                            string[] lineParts = originalLineText.SplitByLength(maxCharsPerLine + (originalLineText.Length - Extensions.CleanStringFormatting(originalLineText).Length), breakAtWordEnding: false);
                            int dcLineSize = lineParts.Length;
                            float blockHeight = lineHeight * dcLineSize;
                            float posY = height + lineHeight - blockHeight - vOffset + 2f;
                            
                            _raster.maxWidth = (int)(width - 35f * _tray.scale.x);
                            _raster.singleLine = true;
                            _raster.enforceWidthByWord = false;

                            if (posY <= height)
                            {
                                _raster.Draw(lineNumber, posX, posY, lineNumColor, 0.9f);
                                for (int i = 0; i < lineParts.Length; i++)
                                {
                                    float partPosY = posY + (i * lineHeight);
                                    if (partPosY <= height)
                                    {
                                        _raster.Draw(lineParts[i], posX + lineNumWidth, partPosY, dcLine.color * 0.8f, 0.9f);
                                        if (i != 0)
                                        {
                                            skipNext++;
                                        }
                                    }
                                }
                            }

                            vOffset += blockHeight;
                        }
                        else
                        {
                            int maxCharsPerLine = (int)((width - posX - lineNumWidth - 32f) / _core.font.GetWidth("M"));
                            string[] lineParts = originalLineText.SplitByLength(maxCharsPerLine + (originalLineText.Length - Extensions.CleanStringFormatting(originalLineText).Length), breakAtWordEnding: false);
                            int dcLineSize = lineParts.Length;
                            float blockHeight = lineHeight * dcLineSize;
                            float posY = height - blockHeight - vOffset + 2f;
                            
                            _core.font.maxWidth = (int)(width - 35f * _tray.scale.x);
                            _core.font.singleLine = true;
                            _core.font.enforceWidthByWord = false;

                            if (posY <= height)
                            {
                                _core.font.Draw(lineNumber, posX, posY, lineNumColor, 0.9f);
                                for (int i = 0; i < lineParts.Length; i++)
                                {
                                    float partPosY = posY + (i * lineHeight);
                                    if (partPosY <= height)
                                    {
                                        _core.font.Draw(lineParts[i], posX + lineNumWidth, partPosY, dcLine.color * 0.8f, 0.9f);
                                    }
                                }
                            }
                            
                            vOffset += blockHeight;
                        }
                    }
                }

                _core.font.scale = new Vec2(2f);
            }
            if (DevConsole.showFPS)
            {
                _core.fpsfont.DrawOutline(Convert.ToString(FPSCounter.GetFPS(1), CultureInfo.InvariantCulture) + " FPS", new Vec2(50f, 8f), Color.White, Color.Black, 1.1f);
                _core.fpsfont.DrawOutline(Convert.ToString(FPSCounter.GetFPS(0), CultureInfo.InvariantCulture) + " UPS", new Vec2(8f, 8f), Color.White, Color.Black, 1.1f);
            }

        }

        [Marker.AutoConfig]
        public static int CommandSuggestionLimit = 8;

        public static Vec2 dimensions => new(Options.Data.consoleWidth / 100f, Options.Data.consoleHeight / 100f);

        public static int consoleScale => Options.Data.consoleScale;

        public static int fontPoints => Options.Data.consoleFontSize;

        public static string fontName => Options.Data.consoleFont;
        public static int usedfornonsense = 0;
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
            try
            {
                GetCommands(pCommand.keyword).Add(pCommand);

                if (pCommand.aliases != null)
                {
                    foreach (string alias in pCommand.aliases)
                        GetCommands(alias).Add(pCommand);
                }

                if (pCommand.noDsh || !DGRSettings.ConvertModdedCommands)
                    return;

                // support DSH too
                ShellCommand shellCommand;

                if (pCommand.arguments != null)
                {
                    CMD.Argument[] arguments = pCommand.arguments;
                    ShellCommand.Parameter[] parameters = new ShellCommand.Parameter[arguments.Length];

                    for (int i = 0; i < arguments.Length; i++)
                    {
                        CMD.Argument argument = arguments[i];

                        parameters[i] = new ShellCommand.Parameter()
                        {
                            Name = argument.name ?? "<NULL>",
                            ParameterType = argument.type,
                            IsOptional = argument.optional,
                            DefaultValue = argument.defaultValue
                        };
                    }

                    shellCommand = new(pCommand.keyword, parameters, args =>
                    {
                        for (int i = 0; i < pCommand.arguments.Length; i++)
                        {
                            pCommand.arguments[i].value = args[i];
                        }

                        pCommand.action(pCommand);
                        return null;
                    });
                } else
                {
                    shellCommand = new(pCommand.keyword, null, args =>
                    {
                        pCommand.alternateAction();
                        
                        return null;
                    });
                }
                

                DevConsoleDSHWrapper.AttributeCommands.Add(new Marker.DevConsoleCommandAttribute()
                {
                    Name = pCommand.keyword,
                    Description = pCommand.description,
                    IsCheat = pCommand.cheat,
                    Command = shellCommand
                });
            } catch (Exception e)
            {
                DevConsole.Log(e);
            }
            
        }

        public static List<CMD> GetCommands(string pKeyword)
        {
            List<CMD> commands;
            if (!DevConsole.commands.TryGetValue(pKeyword, out commands))
                DevConsole.commands[pKeyword] = commands = new List<CMD>();
            return commands;
        }
        
        private static Regex s_getInviteLinkRegex = new(@"^(?:\[?steam:\/\/joinlobby\/312530\/|https:\/\/dgr-join\.github\.io\/\?lobby=)(\d+)(?:\/|&user=)(\d+)", RegexOptions.Compiled);
        public static bool HandleInviteLinkCommand(string command)
        {
            Match match = s_getInviteLinkRegex.Match(command.Trim());
            
            if (!match.Success)
                return false;

            Main.connectID = Convert.ToUInt64(match.Groups[1].Value);
            NCSteam.inviteLobbyID = Main.connectID;
            Level.current = new JoinServer(Main.connectID);
            // Level.current = new JoinServer(ulong.Parse(match.Groups[1].Value));
            //Level.current = new DisconnectFromGame(ulong.Parse(match.Groups[1].Value));
            // DuckNetwork.Join(match.Groups[1].Value);
            return true;
        }

        public static bool RunAsUser = false;

        public static void RunCommand(string command)
        {
            if (DG.buildExpired)
                return;

            if (HandleInviteLinkCommand(command))
                return;

            if (DGRSettings.UseDuckShell)
            {
                Commands.console.Run(command, RunAsUser);
                RunAsUser = false;
                return;
            }
            
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

                if (_core.writeExecutedCommand)
                {
                    _core.lines.Enqueue(new DCLine
                    {
                        line = command,
                        color = Color.White
                    });
                }
                _core.writeExecutedCommand = true;
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
                    if (cmd.cheat && CheckCheats())
                    {
                        _core.lines.Enqueue(new DCLine
                        {
                            line = "You can't do that here!",
                            color = Color.Red
                        });
                        return;
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
                                    Main.SpecialCode = "used `crash` command";
                                    usedfornonsense = 1 / usedfornonsense;
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

        /// <returns>True if cheats shan't be used</returns>
        internal static bool CheckCheats()
        {
            // sole online player
            if (Network.isActive && !Network.connections.Any())
                return false;

            // network debug
            if (NetworkDebugger.enabled)
                return false;

            ulong[] specialUsers =
            {
                76561197996786074UL,    // landon
                76561198885030822UL,    // landon alt
                76561198416200652UL,    // landon alt
                76561198104352795UL,    // dord
                76561198114791325UL,    // collin
            };

            if (Steam.user is null
                || specialUsers.Contains(Steam.user.id)                   // landon exemption
                || DGRDevs.CoreTeam.Any(x => x.SteamID == Steam.user.id)) // tater exemption
                return false;
            
            return Network.isActive || Level.current is ChallengeLevel or ArcadeLevel;
        }

        public static void LogComplexMessage(string text, Color c, float scale = 2f, int index = -1)
        {
            if (text.Contains('\n'))
            {
                string str = text;
                char[] chArray = new char[1] { '\n' };
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
        public static void Log(Exception e) => Log(e.ToString(), Colors.DGRed);
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
        
        /// only logs if currently in DEBUG mode
        [Conditional("DEBUG")]
        public static void DebugLog(object? obj)
        {
            Log(obj);
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
                    //Console.WriteLine(text);
                    debuggerLines.Add(dcLine);
                }
            }
            else
            {
                lock (_core.pendingLines)
                {
                    //Console.WriteLine(text);
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


        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void InitializeCommands()
        {
            /*
               all ported to the new system :D
               
               well, except the terrible ones i got rid of, oops.                            
               
               anyways, keeping this method so harmony stuff
               doesn't break if anyone's referencing this

                                                    ~firebreak */
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

                if (!DGRSettings.NoConsoleLineLimit && _core.lines.Count > 3000)
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
        public static string[] LatestPredictionSuggestions = Array.Empty<string>();
        private static int s_HighlightedSuggestionIndex = -1;
        private static bool s_canUsePrediction;
        private static bool s_suppressAutocompletionForNow;

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

            if (core.pendingSends.Count > 0)
            {
                NetMessage msg = core.pendingSends.Dequeue();
                Send.Message(msg, msg.connection);
            }
            bool open = _core.open;

            //jank workaround -Lucky

            _core.open = false;
            bool pressedTrigger = Input.Pressed(Triggers.DevConsoleTrigger);
            _core.open = open;
            if (pressedTrigger && !UIMenu.globalUILock && !DuckNetwork.core.enteringText && LockMovementQueue.Empty && NetworkDebugger.hoveringInstance)
            {
                if (_tray == null)
                {
                    _tray = new Sprite("devTray");
                    _scan = new Sprite("devScan");
                }

                _core.open = !_core.open;
                Keyboard.keyString = "";
                _core.cursorPosition = _core.Typing.Length;
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
                    {
                        RunAsUser = true;
                        RunCommand(queuedCommand.command);
                    }
                }
            }

            if (_core.open && NetworkDebugger.hoveringInstance)
            {
                Input._imeAllowed = true;
                if (_core.cursorPosition > _core.Typing.Length)
                    _core.cursorPosition = _core.Typing.Length;
                if (Keyboard.keyString.Length > 0 && Keyboard.keyString != "`")
                {
                    _core.Typing = _core.Typing.Insert(_core.cursorPosition, Keyboard.keyString);
                }
                if (_core.Typing != "" && _pendingCommandQueue.Count > 0)
                {
                    _pendingCommandQueue.Clear();
                    _core.lines.Enqueue(new DCLine
                    {
                        line = "Pending commands cleared!",
                        color = Colors.DGOrange
                    });
                }

                if (Keyboard.KeyString.Length > 0)
                {
                    _core.cursorPosition += Keyboard.KeyString.Length;
                    _core.lastCommandIndex = -1;
                }

                Keyboard.KeyString = "";
                if (Keyboard.control)
                {
                    if (Keyboard.Pressed(Keys.C))
                    {
                        if (!string.IsNullOrWhiteSpace(_core.Typing))
                        {
                            Thread thread = new(() => FNAPlatform.SetClipboardText(_core.Typing));
                            thread.SetApartmentState(ApartmentState.STA);
                            thread.Start();
                            thread.Join();
                            HUD.AddPlayerChangeDisplay("@CLIPCOPY@Copied!");
                        }
                    }
                    else if (Keyboard.Pressed(Keys.V))
                    {
                        string paste = "";
                        Thread thread = new(() => paste = FNAPlatform.GetClipboardText());
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
                            _core.Typing = _core.Typing.Insert(Math.Min(_core.Typing.Length, _core.cursorPosition), value);
                            _core.cursorPosition = _core.cursorPosition + value.Length;
                        }
                        else
                        {
                            _core.Typing = "";
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

                if (Keyboard.Pressed(Keys.Enter))
                {
                    if (Keyboard.shift)
                    {
                        _core.Typing = _core.Typing.Insert(_core.cursorPosition++, "\n");
                    }
                    else if (!string.IsNullOrWhiteSpace(_core.Typing))
                    {
                        s_suppressAutocompletionForNow = true;
                        ImplementCompletionSuggestion(_core.Typing, _core.cursorPosition);
                        
                        s_canUsePrediction = false;
                        LatestPredictionSuggestions = Array.Empty<string>();
                        
                        RunAsUser = true;
                        RunCommand(_core.Typing);

                        for (int i = _core.previousLines.Count - 1; i >= 0; i--)
                        {
                            if (_core.previousLines[i] == _core.Typing)
                            {
                                _core.previousLines.RemoveAt(i);
                            }
                        }

                        _core.previousLines.Add(_core.Typing);
                        _core.Typing = "";
                        Keyboard.keyString = "";
                        _core.lastCommandIndex = -1;
                        _core.viewOffset = 0;
                    }
                }
                else if (Keyboard.Pressed(Keys.Back))
                {
                    int length = Keyboard.control
                        ? WordBoundary.GetNextRange(_core.Typing, _core.cursorPosition, HorizontalDirection.Left).LengthAbs
                        : 1;

                    HitBackspace(length);
                }
                else if (Keyboard.Pressed(Keys.Delete))
                {
                    int length = Keyboard.control
                        ? WordBoundary.GetNextRange(_core.Typing, _core.cursorPosition).Length
                        : 1;

                    for (int i = 0; i < length; i++)
                    {
                        if (_core.Typing.Length > 0 && _core.cursorPosition < _core.Typing.Length)
                            _core.Typing = _core.Typing.Remove(_core.cursorPosition, 1);
                        _core.lastCommandIndex = -1;
                    }
                }
                else if (Keyboard.Pressed(Keys.Left))
                {
                    int length = Keyboard.control
                        ? WordBoundary.GetNextRange(_core.Typing, _core.cursorPosition, HorizontalDirection.Left).LengthAbs
                        : 1;

                    for (int i = 0; i < length; i++)
                    {
                        _core.cursorPosition = Math.Max(0, _core.cursorPosition - 1);
                    }
                    
                    UpdateAutocomplete(_core.Typing, _core.Typing);
                }
                else if (Keyboard.Pressed(Keys.Right))
                {
                    int length = Keyboard.control
                        ? WordBoundary.GetNextRange(_core.Typing, _core.cursorPosition).Length
                        : 1;

                    for (int i = 0; i < length; i++)
                    {
                        _core.cursorPosition = Math.Min(_core.Typing.Length, _core.cursorPosition + 1);
                    }
                    
                    UpdateAutocomplete(_core.Typing, _core.Typing);
                }
                else if (Keyboard.Pressed(Keys.Home))
                {
                    if (Keyboard.shift)
                        _core.viewOffset = core.lines.Count - 1;
                    else
                        _core.cursorPosition = 0;
                    
                    UpdateAutocomplete(_core.Typing, _core.Typing);
                }
                else if (Keyboard.Pressed(Keys.End))
                {
                    if (Keyboard.shift)
                        _core.viewOffset = 0;
                    else
                        _core.cursorPosition = _core.Typing.Length;
                    
                    UpdateAutocomplete(_core.Typing, _core.Typing);
                }
                else if (Keyboard.Pressed(Keys.Tab) && s_canUsePrediction)
                {
                    int practicalLimit = Math.Min(CommandSuggestionLimit, LatestPredictionSuggestions.Length);

                    if (Keyboard.control)
                    {
                        if (s_HighlightedSuggestionIndex == 0)
                            s_HighlightedSuggestionIndex = practicalLimit;
                        
                        s_HighlightedSuggestionIndex--;
                    }
                    else
                    {
                        s_HighlightedSuggestionIndex++;
                        s_HighlightedSuggestionIndex %= practicalLimit;
                    }
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
                    _core.viewOffset -= (Keyboard.shift ? 10 : 1) * (int) (Mouse.scroll / 120f);
                    if (_core.viewOffset < 0) _core.viewOffset = 0;
                    if (_core.viewOffset > core.lines.Count - 1) _core.viewOffset = core.lines.Count - 1;
                }

                if (Keyboard.Pressed(Keys.Up) && _core.previousLines.Count > 0)
                {
                    ++_core.lastCommandIndex;
                    if (_core.lastCommandIndex >= _core.previousLines.Count)
                        _core.lastCommandIndex = _core.previousLines.Count - 1;
                    _core.Typing = _core.previousLines[_core.previousLines.Count - 1 - _core.lastCommandIndex];
                    _core.cursorPosition = _core.Typing.Length;
                    
                    UpdateAutocomplete(_core.Typing, _core.Typing);
                }

                if (!Keyboard.Pressed(Keys.Down))
                    return;
                if (_core.previousLines.Count > 0 && _core.lastCommandIndex > 0)
                {
                    --_core.lastCommandIndex;
                    _core.Typing =_core.previousLines[_core.previousLines.Count - 1 - _core.lastCommandIndex];
                    _core.cursorPosition = _core.Typing.Length;
                    
                    UpdateAutocomplete(_core.Typing, _core.Typing);
                }
                else if (_core.lastCommandIndex == 0)
                {
                    --_core.lastCommandIndex;
                    _core.cursorPosition = 0;
                    _core.Typing = "";
                }
                else
                {
                    if (_core.lastCommandIndex != -1)
                        return;
                    _core.cursorPosition = 0;
                    _core.Typing = "";
                }
            }
            else
                lastCommand = null;
        }

        private static void ImplementCompletionSuggestion(string currentCommand, int caretPosition)
        {
            if (s_HighlightedSuggestionIndex == -1 || !s_canUsePrediction)
                return;

            bool editCurrentWord = caretPosition - 1 > -1 && !char.IsWhiteSpace(currentCommand[caretPosition - 1]);
            string newWord = LatestPredictionSuggestions[s_HighlightedSuggestionIndex];

            if (editCurrentWord)
            {
                IntRange deletionRange = WordBoundary.GetNextRange_Hard(currentCommand, caretPosition, HorizontalDirection.Left);

                _core.typing = _core.Typing.Remove(deletionRange.Start, deletionRange.Length).Insert(deletionRange.Start, newWord);
                _core.cursorPosition = deletionRange.Start + newWord.Length;
            }
            else
            {
                _core.typing = _core.Typing.Insert(caretPosition, newWord);
                _core.cursorPosition += newWord.Length;
            }

            UpdateAutocomplete(currentCommand, _core.Typing);
            s_suppressAutocompletionForNow = true;
        }

        private static void HitBackspace(int times)
        {
            for (int i = 0; i < times; i++)
            {
                if (_core.Typing.Length > 0 && _core.cursorPosition > 0)
                {
                    _core.cursorPosition = Maths.Clamp(_core.cursorPosition, 0, _core.Typing.Length);
                    _core.Typing = _core.Typing.Remove(_core.cursorPosition - 1, 1);
                    --_core.cursorPosition;
                }
            }

            _core.lastCommandIndex = -1;
        }

        private class QueuedCommand
        {
            public Func<bool> waitCommand;
            public string command;
            public int wait;
        }
    }
}
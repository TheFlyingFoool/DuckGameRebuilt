using AddedContent.Firebreak.DebuggerTools.Manager.Interface.Console;
using DuckGame.ConsoleEngine;
using DuckGame.MMConfig;
using Humanizer;
using SDL2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DuckGame.ConsoleInterface.Panes
{
    public class MMConsolePane : MallardManagerPane
    {
        public override bool Borderless { get; } = false;

        // console buffer text
        public List<MMConsoleLine> Lines = new();
        private int _cameraYOffset;

        // shell
        public CommandRunner ShellInstance;
        
        // user input / caret navigation
        public int CaretIndex = 0;
        private StringBuilder _userInput = new();
        private string _userInputString = "";
        
        // caret smooth movement
        private Vec2 _prevCaretPos = Vec2.MinValue;
        private Vec2 _nextCaretPos = Vec2.MinValue;
        private Vec2 _projectedCaretPos;
        private bool _caretMoving;
        private ProgressValue _caretMovementProgress = 1f;
        
        // caret blink
        private ProgressValue _caretBlinkProgress = 1f;
        private double _caretBlinkProgressSpeed = 0.025D;

        // traverse command history
        private List<string> _commandHistory = new();
        private int _currentCommandIndex = 0;
        
        // autocomplete
        private IEnumerable<string>? _autocompleteSuggestions = null;

        /// <summary>
        /// The current command-executing instance of <see cref="MMConsolePane"/>
        /// </summary>
        /// <remarks>
        /// Useful for statically-typed commands to reference the console
        /// </remarks>
        public static MMConsolePane CurrentExecutor = null!;

        public MMConsolePane()
        {
            ShellInstance = new CommandRunner();

            // TODO: optimize to only do one reflection search and reuse the list
            ShellInstance.AddTypeInterpretters();
            ShellInstance.AddCommandsUsingAttribute();
            
            ExecuteCommand(MallardManager.Config.Console.ShellStartUpCommand);
        }

        public void Print(object? obj, MMConsoleLine.Significance significance = 0)
        {
            string str = obj?.ToString() ?? "";
            
            if (!str.Contains("\n"))
                Lines.Add(new MMConsoleLine(str, significance));
            else
            {
                string[] lines = str.Split('\n');

                foreach (string line in lines)
                {
                    Lines.Add(new MMConsoleLine(line, significance));
                }
            }
            
            float zoom = MallardManager.Config.Zoom;
            int yOf = (int)((Layer.Console.height - (Bounds.tl.y + ((RebuiltMono.HEIGHT - 4) * zoom) + zoom * 8)) / (RebuiltMono.HEIGHT * zoom)) - 1;
            if (yOf < Lines.Count)
                _cameraYOffset = Lines.Count - yOf;
        }

        private void ApplyCaretMovement()
        {
            float zoom = MallardManager.Config.Zoom;
            float lineYOffset = zoom * 4 + (RebuiltMono.HEIGHT - 4) * zoom;
            Vec2 lineDrawStartPos = Bounds.tl + (zoom * 2, lineYOffset + zoom * 4);
            Vec2 userTextPosition = lineDrawStartPos + (0, RebuiltMono.HEIGHT * zoom * (Lines.Count - _cameraYOffset));
            _nextCaretPos = (userTextPosition.x + RebuiltMono.WIDTH * 3.5f * zoom + (RebuiltMono.WIDTH * CaretIndex * zoom), userTextPosition.y + zoom);
            
            _projectedCaretPos = Vec2.Lerp(_prevCaretPos, _nextCaretPos, (float) Ease.Out.Quint(_caretMovementProgress).NormalizedValue);
        }
        
        public override void Update()
        {
            HandleCaretSmoothMovement();
            HandleCaretBlink();
            
            HandleKeyboardInput();
        }

        private void HandleCaretBlink()
        {
            if (_caretBlinkProgress.Completed || (!_caretBlinkProgress).Completed)
                _caretBlinkProgressSpeed *= -1;

            _caretBlinkProgress += _caretBlinkProgressSpeed;

            if (_caretMoving)
                _caretBlinkProgress = 1;
        }

        private void HandleCaretSmoothMovement()
        {
            if (!_caretMovementProgress.Completed && _caretMoving)
            {
                float caretMovementSmoothness = MallardManager.Config.Console.Caret.MovementSmoothness;
                if (caretMovementSmoothness == 0)
                    _caretMovementProgress = 1;
                else _caretMovementProgress += 1 / caretMovementSmoothness;
                
                ApplyCaretMovement();
            }
            else if (_caretMoving)
            {
                _prevCaretPos = _nextCaretPos;
                _caretMoving = false;
            }
        }

        private void HandleKeyboardInput()
        {
            Microsoft.Xna.Framework.Input.Keys[] pressedKeys = Keyboard.KeyState.GetPressedKeys();
            foreach (Microsoft.Xna.Framework.Input.Keys key in pressedKeys)
            {
                if (!Keyboard.Pressed((Keys)key))
                    continue;

                if (Keyboard.control)
                {
                    CtrlPlus(key);
                    return;
                }
                else if (Keyboard.alt)
                {
                    AltPlus(key);
                    return;
                }

                switch (key)
                {
                    case Microsoft.Xna.Framework.Input.Keys.Escape:
                        break;
                    case Microsoft.Xna.Framework.Input.Keys.Space:
                        {
                            _userInput.Insert(CaretIndex++, ' ');
                            OnUserInputChange(_userInput.ToString());
                            break;
                        }
                    case Microsoft.Xna.Framework.Input.Keys.Tab:
                        {
                            int tabWidth = MallardManager.Config.Console.TabWidth;

                            _userInput.Insert(CaretIndex, new string(' ', tabWidth));
                            CaretIndex += tabWidth;
                            
                            OnUserInputChange(_userInput.ToString());
                            break;
                        }
                    case Microsoft.Xna.Framework.Input.Keys.Enter:
                        {
                            InputEnter();
                            break;
                        }
                    case Microsoft.Xna.Framework.Input.Keys.Back:
                        {
                            InputBackspace();
                            break;
                        }
                    case Microsoft.Xna.Framework.Input.Keys.Delete:
                        {
                            InputDelete();
                            break;
                        }
                    case Microsoft.Xna.Framework.Input.Keys.Up:
                        {
                            if (_commandHistory.Count == 0)
                                break;

                            _currentCommandIndex = Maths.Clamp(_currentCommandIndex, 1, _commandHistory.Count);
                            
                            _userInput = new StringBuilder(_commandHistory[--_currentCommandIndex]);
                            OnUserInputChange(_userInput.ToString());
                            
                            SetCaretPosition(_userInput.Length);
                            break;
                        }
                    case Microsoft.Xna.Framework.Input.Keys.Down:
                        {
                            if (_commandHistory.Count == 0)
                                break;
                            
                            _currentCommandIndex = Maths.Clamp(_currentCommandIndex, 0, _commandHistory.Count);

                            if (_currentCommandIndex >= _commandHistory.Count - 1)
                            {
                                if (_userInput.Length == 0)
                                    break;

                                _userInput.Clear();
                                _currentCommandIndex++;
                            }
                            else
                            {
                                _userInput = new StringBuilder(_commandHistory[++_currentCommandIndex]);
                            }
                            
                            OnUserInputChange(_userInput.ToString());
                            SetCaretPosition(_userInput.Length);
                            break;
                        }
                    case Microsoft.Xna.Framework.Input.Keys.PageUp:
                        {
                            if (_cameraYOffset > 0)
                                _cameraYOffset--;
                            break;
                        }
                    case Microsoft.Xna.Framework.Input.Keys.PageDown:
                        {
                            if (_cameraYOffset < Lines.Count - 1)
                                _cameraYOffset++;
                            break;
                        }
                    case Microsoft.Xna.Framework.Input.Keys.Left:
                        {
                            MoveCaretLeft();
                            break;
                        }
                    case Microsoft.Xna.Framework.Input.Keys.Right:
                        {
                            MoveCaretRight();
                            break;
                        }
                    default:
                        {
                            char charFromKey = Keyboard.GetCharFromKey((Keys)key);
                            
                            if (charFromKey != ' ')
                            {
                                _userInput.Insert(CaretIndex++, charFromKey);
                                OnUserInputChange(_userInput.ToString());
                            }
                            
                            break;
                        }
                }
            }
        }

        private void InputEnter()
        {
            string command = _userInput.ToString();
            if (string.IsNullOrWhiteSpace(command))
                return;

            Print(command, MMConsoleLine.Significance.User);
            ClearUserInput(); // clears user input
            OnUserInputChange("");

            ExecuteCommand(command);
            
            if (_commandHistory.Count == 0 || _commandHistory[_commandHistory.Count - 1] != command)
            {
                _commandHistory.Add(command);
                _currentCommandIndex = _commandHistory.Count;
            }
        }

        private void InputDelete(int times = 1)
        {
            if (times != 1)
            {
                for (int i = 0; i < times; i++)
                    InputDelete(1);
                return;
            }

            if (_userInput.Length <= 0 || CaretIndex >= _userInput.Length)
                return;

            _userInput.Remove(CaretIndex, 1);
            OnUserInputChange(_userInput.ToString());
        }

        private void InputBackspace(int times = 1)
        {
            if (times != 1)
            {
                for (int i = 0; i < times; i++)
                    InputBackspace(1);
                return;
            }

            if (_userInput.Length <= 0)
                return;

            _userInput.Remove(CaretIndex - 1, 1);
            OnUserInputChange(_userInput.ToString());
            MoveCaretLeft();
        }

        private void SetCaretPosition(int index)
        {
            CaretIndex = Maths.Clamp(index, 0, _userInputString.Length);

            if (_caretMoving)
                _prevCaretPos = _nextCaretPos;
            _caretMovementProgress = 0;
            _caretMoving = true;
        }

        private void MoveCaretRight(int times = 1)
        {
            if (times != 1)
            {
                for (int i = 0; i < times; i++)
                    MoveCaretRight(1);
                return;
            }
            
            SetCaretPosition(++CaretIndex);
            // if (CaretIndex < _userInput.Length)
            //     CaretIndex++;
            //
            // if (_caretMoving)
            //     _prevCaretPos = _nextCaretPos;
            // _caretMovementProgress = 0;
            // _caretMoving = true;
        }

        private void MoveCaretLeft(int times = 1)
        {
            if (times != 1)
            {
                for (int i = 0; i < times; i++)
                    MoveCaretLeft(1);
                return;
            }
            
            SetCaretPosition(--CaretIndex);
            // if (CaretIndex > 0)
            //     CaretIndex--;
            //
            // if (_caretMoving)
            //     _prevCaretPos = _nextCaretPos;
            // _caretMovementProgress = 0;
            // _caretMoving = true;
        }

        private void AltPlus(Microsoft.Xna.Framework.Input.Keys key)
        {
            
        }

        private void CtrlPlus(Microsoft.Xna.Framework.Input.Keys key)
        {
            switch (key)
            {
                case Microsoft.Xna.Framework.Input.Keys.V:
                {
                    string clipboardText = SDL.SDL_GetClipboardText();
                    
                    _userInput.Insert(CaretIndex, clipboardText);
                    CaretIndex += clipboardText.Length;
                            
                    OnUserInputChange(_userInput.ToString());
                    break;
                }
                case Microsoft.Xna.Framework.Input.Keys.C:
                {
                    SDL.SDL_SetClipboardText(_userInputString);
                    break;
                }
                case Microsoft.Xna.Framework.Input.Keys.Back:
                {
                    InputBackspace(BiDirectionalWordBoundaryMovement.GetNextWord(_userInputString, CaretIndex, HorizontalDirection.Left).Length);
                    break;
                }
                case Microsoft.Xna.Framework.Input.Keys.Delete:
                {
                    InputDelete(BiDirectionalWordBoundaryMovement.GetNextWord(_userInputString, CaretIndex, HorizontalDirection.Right).Length);
                    break;
                }
                case Microsoft.Xna.Framework.Input.Keys.Left:
                {
                    MoveCaretLeft(BiDirectionalWordBoundaryMovement.GetNextWord(_userInputString, CaretIndex, HorizontalDirection.Left).Length);
                    break;
                }
                case Microsoft.Xna.Framework.Input.Keys.Right:
                {
                    MoveCaretRight(BiDirectionalWordBoundaryMovement.GetNextWord(_userInputString, CaretIndex, HorizontalDirection.Right).Length);
                    break;
                }
                case Microsoft.Xna.Framework.Input.Keys.Up:
                {
                    SetCaretPosition(0);
                    break;
                }
                case Microsoft.Xna.Framework.Input.Keys.Down:
                {
                    SetCaretPosition(_userInputString.Length);
                    break;
                }
            }
        }

        private void OnUserInputChange(string newText)
        {
            _userInputString = newText;
            float zoom = MallardManager.Config.Zoom;
            int yOf = (int)((Layer.Console.height - (Bounds.tl.y + ((RebuiltMono.HEIGHT - 4) * zoom) + zoom * 8)) / (RebuiltMono.HEIGHT * zoom)) - 1;
            if (yOf < Lines.Count)
                _cameraYOffset = Lines.Count - yOf;
            
            if (_caretMoving)
                _prevCaretPos = _nextCaretPos;
            _caretMovementProgress = 0;
            _caretMoving = true;

            if (_currentCommandIndex < _commandHistory.Count
                && _currentCommandIndex >= 0
                && _commandHistory[_currentCommandIndex] != newText)
                _currentCommandIndex = _commandHistory.Count;

            if (newText == "") // autocompletion
                return;
            
            
        }

        public void ExecuteCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return;
            
            CurrentExecutor = this;
            ShellInstance.Run(command)?.TryUse(
                x =>
                {
                    if (x is not null)
                        Print(x, MMConsoleLine.Significance.Response);
                },
                x => Print(x.Message, MMConsoleLine.Significance.Error)
            );
        }

        public override void DrawRaw(Depth depth, float deltaUnit)
        {
            if (deltaUnit <= 0)
                return;
            
            Color backgroundColor = MallardManager.Colors.PrimaryBackground;
            Color layoutColor = MallardManager.Colors.SecondaryBackground;

            float doubleZoom = deltaUnit * 2;
            Vec2 marginOffset = new(doubleZoom);
            float lineYOffset = doubleZoom * 2 + (RebuiltMono.HEIGHT - 4) * deltaUnit;

            Graphics.DrawRect(Bounds, backgroundColor, depth);
            RebuiltMono.Draw("DuckShell", Bounds.tl + marginOffset, layoutColor, depth + 0.05f, deltaUnit);
            Graphics.DrawLine(
                Bounds.tl + (doubleZoom, lineYOffset),
                Bounds.tr + (-doubleZoom, lineYOffset),
                layoutColor, deltaUnit, depth + 0.05f);

            for (int i = _cameraYOffset; i < Lines.Count; i++)
            {
                if (i >= Lines.Count)
                    break;
                
                MMConsoleLine line = Lines[i];

                Vec2 textPosition = Bounds.tl + (deltaUnit * 2, lineYOffset + doubleZoom * 2 + (RebuiltMono.HEIGHT * deltaUnit * (i - _cameraYOffset)));
                DrawConsoleLine(line, textPosition, depth + 0.1f, deltaUnit);
            }
            
            Vec2 lineDrawStartPos = Bounds.tl + (deltaUnit * 2, lineYOffset + doubleZoom * 2);
            Vec2 userTextPosition = lineDrawStartPos + (0, RebuiltMono.HEIGHT * deltaUnit * (Lines.Count - _cameraYOffset));
            
            DrawUserText(userTextPosition, depth, deltaUnit);
        }

        public void ClearUserInput()
        {
            _userInput.Clear();
            _userInputString = "";
            CaretIndex = 0;
            _cameraYOffset = 0;
        }

        public override void OnFocus()
        {
            ClearUserInput();
            _caretBlinkProgress = 1f;

            // don't question why there's two of this,
            // only god knows, but that makes it work.
            // - firebreak
            
            _caretMovementProgress = 1;
            _caretMoving = true;
            HandleCaretSmoothMovement();
            ApplyCaretMovement();
            
            _caretMovementProgress = 1;
            _caretMoving = true;
            HandleCaretSmoothMovement();
            ApplyCaretMovement();
        }

        public void Clear()
        {
            Lines.Clear();
            ClearUserInput();
            
            float zoom = MallardManager.Config.Zoom;
            int yOf = (int)((Layer.Console.height - (Bounds.tl.y + ((RebuiltMono.HEIGHT - 4) * zoom) + zoom * 8)) / (RebuiltMono.HEIGHT * zoom)) - 1;
            if (yOf < Lines.Count)
                _cameraYOffset = Lines.Count - yOf;
        }

        private void DrawUserText(Vec2 position, Depth depth, float zoom)
        {
            RebuiltMono.Draw("DSH", position, MallardManager.Colors.SecondaryBackground, depth, zoom);
            RebuiltMono.Draw(_userInputString, position + (RebuiltMono.WIDTH * 3.5f * zoom, 0), MallardManager.Colors.UserText, depth, zoom);

            if (_autocompleteSuggestions is not null)
            {
                int i = 0;
                foreach (string suggestion in _autocompleteSuggestions)
                {
                    RebuiltMono.Draw(suggestion, (zoom, zoom + ((zoom * RebuiltMono.HEIGHT + 2) * i)), Color.White, 2f, zoom);
                    i++;
                }
            }

            DrawCaret(depth, zoom);
        }

        private void DrawCaret(Depth depth, float zoom)
        {
            MMCaretConfig config = MallardManager.Config.Console.Caret;
            Rectangle caretBounds = new(_projectedCaretPos.x, _projectedCaretPos.y, (RebuiltMono.WIDTH - 1) * zoom * config.ThicknessPercentage, (RebuiltMono.HEIGHT - 4) * zoom);

            Graphics.DrawRect(caretBounds, MallardManager.Colors.UserOverlay * (float)Ease.InOut.Quint( _caretBlinkProgress).NormalizedValue, depth, true);
        }

        private static void DrawConsoleLine(MMConsoleLine line, Vec2 position, Depth depth, float zoom)
        {
            Color linePrefixColor = MallardManager.Colors.SecondaryBackground;

            bool isUserLine = line.LineSignificance == MMConsoleLine.Significance.User;
            bool shouldHighlight = line.LineSignificance == MMConsoleLine.Significance.Highlight;
            float userInputPrefixOffset = isUserLine
                ? RebuiltMono.WIDTH * 3.5f * zoom
                : 0;

            if (isUserLine)
                RebuiltMono.Draw("DSH", position, linePrefixColor, depth, zoom);
            else if (shouldHighlight)
            {
                SizeF dimensions = RebuiltMono.GetDimensions(line.Text, zoom);
                Rectangle highlightBox = new(position - (zoom, zoom), dimensions + new SizeF(zoom, 0));
                Graphics.DrawRect(highlightBox, MallardManager.Colors.Secondary, depth - 0.01f);
            }
            
            RebuiltMono.Draw(line.Text, position + (userInputPrefixOffset, 0), line.LineSignificance switch
            {
                MMConsoleLine.Significance.Neutral => MallardManager.Colors.Primary,
                MMConsoleLine.Significance.User => MallardManager.Colors.UserText,
                MMConsoleLine.Significance.Response => MallardManager.Colors.PrimarySystemText,
                MMConsoleLine.Significance.Highlight => MallardManager.Colors.PrimarySystemText,
                MMConsoleLine.Significance.Error => MallardManager.Colors.SecondarySystemText,
                MMConsoleLine.Significance.VeryFuckingImportant => MallardManager.Colors.SecondarySystemText,
                _ => throw new ArgumentOutOfRangeException()
            }, depth, zoom);
        }
    }
}
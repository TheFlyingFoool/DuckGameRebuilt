using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RawInput;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class RawKeyboard : InputDevice
    {
        protected PadState _repeatState;
        protected KeyboardState _keyboardState;
        protected KeyboardState _keyboardStatePrev;
        protected PadState _state;
        protected PadState _statePrev;
        protected Queue<PadState> _delayBuffer = new Queue<PadState>();
        //private const int numStates = 256;
        private bool _startPressed;
        public bool delay;
        public int playerindex;
        protected static Array _xboxButtons = Enum.GetValues(typeof(PadButton));
        //public static bool inputDelay = true;
        //private int _disableDelay;

        public bool startWasPressed { get; set; }

        public virtual float leftTrigger => Maths.NormalizeSection(_state.triggers.left, 0.1f, 1f);

        public virtual float rightTrigger => Maths.NormalizeSection(_state.triggers.right, 0.1f, 1f);

        public virtual Vec2 leftStick => new Vec2(_state.sticks.left.x, _state.sticks.left.y);

        public virtual Vec2 rightStick => new Vec2(_state.sticks.right.x, _state.sticks.right.y);
        public IntPtr devicehandle;
        public RawKeyboard(int idx, IntPtr _devicehandle, int keyboardnumber)
          : base(idx)
        {
            _productName = "RAW KEYBOARD " + keyboardnumber.ToString();
            _productGUID = "";
            devicehandle = _devicehandle;
            playerindex = idx;
        }

        protected virtual PadState GetState(int index)
        {
            return new PadState();
        }

        public override void Rumble(float leftIntensity = 0f, float rightIntensity = 0f)
        {
            if (!isConnected)
                return;
        }


        public override void Update()
        {
            base.Update();
            //if (isConnected)
            //{
            //}
            //startWasPressed = _startPressed;
            //_startPressed = false;
            //_statePrev = _state;
            //_state = GetState(index);
            _keyboardStatePrev = _keyboardState;
            _keyboardState = RawInputHandle.GetState(devicehandle);
        }


        public override bool MapPressed(int mapping, bool any = false)
        {
            //PadButton butt = (PadButton)mapping;

            //if (butt == PadButton.Start && _startPressed)
            //    return true;

            Microsoft.Xna.Framework.Input.Keys key = (Microsoft.Xna.Framework.Input.Keys)mapping;
            if (any) //  return any ? (_state.buttons & ~_statePrev.buttons) != 0 : _state.IsButtonDown(butt) && !_statePrev.IsButtonDown(butt);
            {
                return _keyboardState.GetPressedKeys().Length > 0 && _keyboardStatePrev.GetPressedKeys().Length == 0;
            }
            else
            {
                return _keyboardState.IsKeyDown(key) && !_keyboardStatePrev.IsKeyDown(key);
            }
        }

        public override bool MapReleased(int mapping)
        {
            Microsoft.Xna.Framework.Input.Keys key = (Microsoft.Xna.Framework.Input.Keys)mapping;
            return !_keyboardState.IsKeyDown(key) && _keyboardStatePrev.IsKeyDown(key);
        }

        public override bool MapDown(int mapping, bool any = false)
        {
            Microsoft.Xna.Framework.Input.Keys key = (Microsoft.Xna.Framework.Input.Keys)mapping;
            if (any)
            {
                return _keyboardState.GetPressedKeys().Length > 0;
            }
            return _keyboardState.IsKeyDown(key);
        }
        private Dictionary<int, string> _triggerNames;
        public override Dictionary<int, string> GetTriggerNames()
        {
            if (_triggerNames == null)
            {
                _triggerNames = new Dictionary<int, string>();
                foreach (Keys key in Enum.GetValues(typeof(Keys)).Cast<Keys>())
                {
                    char ch = KeyToChar(key);
                    if (ch == ' ')
                    {
                        switch (key)
                        {
                            case Keys.Back:
                                _triggerNames[(int)key] = "BACK";
                                continue;
                            case Keys.Tab:
                                _triggerNames[(int)key] = "TAB";
                                continue;
                            case Keys.Enter:
                                _triggerNames[(int)key] = "ENTER";
                                continue;
                            case Keys.Escape:
                                _triggerNames[(int)key] = "ESC";
                                continue;
                            case Keys.Space:
                                _triggerNames[(int)key] = "SPACE";
                                continue;
                            case Keys.PageUp:
                                _triggerNames[(int)key] = "PGUP";
                                continue;
                            case Keys.PageDown:
                                _triggerNames[(int)key] = "PGDN";
                                continue;
                            case Keys.End:
                                _triggerNames[(int)key] = "END";
                                continue;
                            case Keys.Home:
                                _triggerNames[(int)key] = "HOME";
                                continue;
                            case Keys.Left:
                                _triggerNames[(int)key] = Triggers.Left;
                                continue;
                            case Keys.Up:
                                _triggerNames[(int)key] = Triggers.Up;
                                continue;
                            case Keys.Right:
                                _triggerNames[(int)key] = Triggers.Right;
                                continue;
                            case Keys.Down:
                                _triggerNames[(int)key] = Triggers.Down;
                                continue;
                            case Keys.Insert:
                                _triggerNames[(int)key] = "INSRT";
                                continue;
                            case Keys.F1:
                                _triggerNames[(int)key] = "F1";
                                continue;
                            case Keys.F2:
                                _triggerNames[(int)key] = "F2";
                                continue;
                            case Keys.F3:
                                _triggerNames[(int)key] = "F3";
                                continue;
                            case Keys.F4:
                                _triggerNames[(int)key] = "F4";
                                continue;
                            case Keys.F5:
                                _triggerNames[(int)key] = "F5";
                                continue;
                            case Keys.F6:
                                _triggerNames[(int)key] = "F6";
                                continue;
                            case Keys.F7:
                                _triggerNames[(int)key] = "F7";
                                continue;
                            case Keys.F8:
                                _triggerNames[(int)key] = "F8";
                                continue;
                            case Keys.F9:
                                _triggerNames[(int)key] = "F9";
                                continue;
                            case Keys.F10:
                                _triggerNames[(int)key] = "F10";
                                continue;
                            case Keys.F11:
                                _triggerNames[(int)key] = "F11";
                                continue;
                            case Keys.F12:
                                _triggerNames[(int)key] = "F12";
                                continue;
                            case Keys.LeftShift:
                                _triggerNames[(int)key] = "LSHFT";
                                continue;
                            case Keys.RightShift:
                                _triggerNames[(int)key] = "RSHFT";
                                continue;
                            case Keys.LeftControl:
                                _triggerNames[(int)key] = "LCTRL";
                                continue;
                            case Keys.RightControl:
                                _triggerNames[(int)key] = "RCTRL";
                                continue;
                            case Keys.LeftAlt:
                                _triggerNames[(int)key] = "LALT";
                                continue;
                            case Keys.RightAlt:
                                _triggerNames[(int)key] = "RALT";
                                continue;
                            case Keys.MouseLeft:
                                _triggerNames[(int)key] = "MB L";
                                continue;
                            case Keys.MouseMiddle:
                                _triggerNames[(int)key] = "MB M";
                                continue;
                            case Keys.MouseRight:
                                _triggerNames[(int)key] = "MB R";
                                continue;
                            default:
                                continue;
                        }
                    }
                    else
                        _triggerNames[(int)key] = ch.ToString() ?? "";
                }
            }
            return _triggerNames;
        }
        public static char KeyToChar(Keys key, bool caps = true, bool shift = false)
        {
            if (caps)
            {
                switch (key)
                {
                    case Keys.D0:
                        return '0';
                    case Keys.D1:
                        return '1';
                    case Keys.D2:
                        return '2';
                    case Keys.D3:
                        return '3';
                    case Keys.D4:
                        return '4';
                    case Keys.D5:
                        return '5';
                    case Keys.D6:
                        return '6';
                    case Keys.D7:
                        return '7';
                    case Keys.D8:
                        return '8';
                    case Keys.D9:
                        return '9';
                    case Keys.A:
                        return 'A';
                    case Keys.B:
                        return 'B';
                    case Keys.C:
                        return 'C';
                    case Keys.D:
                        return 'D';
                    case Keys.E:
                        return 'E';
                    case Keys.F:
                        return 'F';
                    case Keys.G:
                        return 'G';
                    case Keys.H:
                        return 'H';
                    case Keys.I:
                        return 'I';
                    case Keys.J:
                        return 'J';
                    case Keys.K:
                        return 'K';
                    case Keys.L:
                        return 'L';
                    case Keys.M:
                        return 'M';
                    case Keys.N:
                        return 'N';
                    case Keys.O:
                        return 'O';
                    case Keys.P:
                        return 'P';
                    case Keys.Q:
                        return 'Q';
                    case Keys.R:
                        return 'R';
                    case Keys.S:
                        return 'S';
                    case Keys.T:
                        return 'T';
                    case Keys.U:
                        return 'U';
                    case Keys.V:
                        return 'V';
                    case Keys.W:
                        return 'W';
                    case Keys.X:
                        return 'X';
                    case Keys.Y:
                        return 'Y';
                    case Keys.Z:
                        return 'Z';
                    case Keys.NumPad0:
                        return '0';
                    case Keys.NumPad1:
                        return '1';
                    case Keys.NumPad2:
                        return '2';
                    case Keys.NumPad3:
                        return '3';
                    case Keys.NumPad4:
                        return '4';
                    case Keys.NumPad5:
                        return '5';
                    case Keys.NumPad6:
                        return '6';
                    case Keys.NumPad7:
                        return '7';
                    case Keys.NumPad8:
                        return '8';
                    case Keys.NumPad9:
                        return '9';
                    case Keys.OemSemicolon:
                        return ';';
                    case Keys.OemPlus:
                        return '=';
                    case Keys.OemComma:
                        return ',';
                    case Keys.OemMinus:
                        return '-';
                    case Keys.OemPeriod:
                        return '.';
                    case Keys.OemQuestion:
                        return '/';
                    case Keys.OemTilde:
                        return '~';
                    case Keys.OemOpenBrackets:
                        return '[';
                    case Keys.OemPipe:
                        return '\\';
                    case Keys.OemCloseBrackets:
                        return ']';
                    case Keys.OemQuotes:
                        return '\'';
                    case Keys.OemBackslash:
                        return '\\';
                }
            }
            else if (shift)
            {
                switch (key)
                {
                    case Keys.D0:
                        return ')';
                    case Keys.D1:
                        return '!';
                    case Keys.D2:
                        return '@';
                    case Keys.D3:
                        return '#';
                    case Keys.D4:
                        return '$';
                    case Keys.D5:
                        return '%';
                    case Keys.D6:
                        return '^';
                    case Keys.D7:
                        return '&';
                    case Keys.D8:
                        return '*';
                    case Keys.D9:
                        return '(';
                    case Keys.A:
                        return 'A';
                    case Keys.B:
                        return 'B';
                    case Keys.C:
                        return 'C';
                    case Keys.D:
                        return 'D';
                    case Keys.E:
                        return 'E';
                    case Keys.F:
                        return 'F';
                    case Keys.G:
                        return 'G';
                    case Keys.H:
                        return 'H';
                    case Keys.I:
                        return 'I';
                    case Keys.J:
                        return 'J';
                    case Keys.K:
                        return 'K';
                    case Keys.L:
                        return 'L';
                    case Keys.M:
                        return 'M';
                    case Keys.N:
                        return 'N';
                    case Keys.O:
                        return 'O';
                    case Keys.P:
                        return 'P';
                    case Keys.Q:
                        return 'Q';
                    case Keys.R:
                        return 'R';
                    case Keys.S:
                        return 'S';
                    case Keys.T:
                        return 'T';
                    case Keys.U:
                        return 'U';
                    case Keys.V:
                        return 'V';
                    case Keys.W:
                        return 'W';
                    case Keys.X:
                        return 'X';
                    case Keys.Y:
                        return 'Y';
                    case Keys.Z:
                        return 'Z';
                    case Keys.NumPad0:
                        return '0';
                    case Keys.NumPad1:
                        return '1';
                    case Keys.NumPad2:
                        return '2';
                    case Keys.NumPad3:
                        return '3';
                    case Keys.NumPad4:
                        return '4';
                    case Keys.NumPad5:
                        return '5';
                    case Keys.NumPad6:
                        return '6';
                    case Keys.NumPad7:
                        return '7';
                    case Keys.NumPad8:
                        return '8';
                    case Keys.NumPad9:
                        return '9';
                    case Keys.OemSemicolon:
                        return ':';
                    case Keys.OemPlus:
                        return '+';
                    case Keys.OemComma:
                        return '<';
                    case Keys.OemMinus:
                        return '_';
                    case Keys.OemPeriod:
                        return '>';
                    case Keys.OemQuestion:
                        return '?';
                    case Keys.OemTilde:
                        return '~';
                    case Keys.OemOpenBrackets:
                        return '{';
                    case Keys.OemPipe:
                        return '|';
                    case Keys.OemCloseBrackets:
                        return '}';
                    case Keys.OemQuotes:
                        return '"';
                    case Keys.OemBackslash:
                        return '|';
                }
            }
            else
            {
                switch (key)
                {
                    case Keys.D0:
                        return '0';
                    case Keys.D1:
                        return '1';
                    case Keys.D2:
                        return '2';
                    case Keys.D3:
                        return '3';
                    case Keys.D4:
                        return '4';
                    case Keys.D5:
                        return '5';
                    case Keys.D6:
                        return '6';
                    case Keys.D7:
                        return '7';
                    case Keys.D8:
                        return '8';
                    case Keys.D9:
                        return '9';
                    case Keys.A:
                        return 'a';
                    case Keys.B:
                        return 'b';
                    case Keys.C:
                        return 'c';
                    case Keys.D:
                        return 'd';
                    case Keys.E:
                        return 'e';
                    case Keys.F:
                        return 'f';
                    case Keys.G:
                        return 'g';
                    case Keys.H:
                        return 'h';
                    case Keys.I:
                        return 'i';
                    case Keys.J:
                        return 'j';
                    case Keys.K:
                        return 'k';
                    case Keys.L:
                        return 'l';
                    case Keys.M:
                        return 'm';
                    case Keys.N:
                        return 'n';
                    case Keys.O:
                        return 'o';
                    case Keys.P:
                        return 'p';
                    case Keys.Q:
                        return 'q';
                    case Keys.R:
                        return 'r';
                    case Keys.S:
                        return 's';
                    case Keys.T:
                        return 't';
                    case Keys.U:
                        return 'u';
                    case Keys.V:
                        return 'v';
                    case Keys.W:
                        return 'w';
                    case Keys.X:
                        return 'x';
                    case Keys.Y:
                        return 'y';
                    case Keys.Z:
                        return 'z';
                    case Keys.NumPad0:
                        return '0';
                    case Keys.NumPad1:
                        return '1';
                    case Keys.NumPad2:
                        return '2';
                    case Keys.NumPad3:
                        return '3';
                    case Keys.NumPad4:
                        return '4';
                    case Keys.NumPad5:
                        return '5';
                    case Keys.NumPad6:
                        return '6';
                    case Keys.NumPad7:
                        return '7';
                    case Keys.NumPad8:
                        return '8';
                    case Keys.NumPad9:
                        return '9';
                    case Keys.OemSemicolon:
                        return ';';
                    case Keys.OemPlus:
                        return '=';
                    case Keys.OemComma:
                        return ',';
                    case Keys.OemMinus:
                        return '-';
                    case Keys.OemPeriod:
                        return '.';
                    case Keys.OemQuestion:
                        return '/';
                    case Keys.OemTilde:
                        return '~';
                    case Keys.OemOpenBrackets:
                        return '[';
                    case Keys.OemPipe:
                        return '\\';
                    case Keys.OemCloseBrackets:
                        return ']';
                    case Keys.OemQuotes:
                        return '\'';
                    case Keys.OemBackslash:
                        return '\\';
                }
            }
            return ' ';
        }
    }
}

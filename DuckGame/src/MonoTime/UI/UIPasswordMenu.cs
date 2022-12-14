// Decompiled with JetBrains decompiler
// Type: DuckGame.UIStringEntryMenu
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Text.RegularExpressions;

namespace DuckGame
{
    public class UIStringEntryMenu : UIMenu
    {
        public string password = "";
        public bool _directional;
        private FieldBinding _binding;
        private int _maxLength = 24;
        private int _minNumber;
        private int _maxNumber;
        private bool _numeric;
        private bool _cancelled = true;
        private string _originalValue = "";
        private float blink;
        private bool wasOpen;

        public UIStringEntryMenu(
          bool directional,
          string title,
          FieldBinding pBinding,
          int pMaxLength = 24,
          bool pNumeric = false,
          int pMinNumber = -2147483648,
          int pMaxNumber = 2147483647)
          : base(title, Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, directional ? 160f : 220f, 60f, directional ? "@WASD@SET @SELECT@ACCEPT" : "@ENTERKEY@ACCEPT @ESCAPEKEY@")
        {
            Add(new UIBox(0f, 0f, 100f, 16f, isVisible: false), true);
            _binding = pBinding;
            _directional = directional;
            _numeric = pNumeric;
            _maxLength = pMaxLength;
            _minNumber = pMinNumber;
            _maxNumber = pMaxNumber;
        }

        public void SetValue(string pValue) => password = pValue;

        public override void Open()
        {
            if (_directional)
                password = "";
            _originalValue = password;
            Keyboard.keyString = password;
            _cancelled = true;
            base.Open();
        }

        public override void OnClose()
        {
            Keyboard.repeat = false;
            if (wasOpen && _cancelled)
                _binding.value = _directional ? "" : (object)_originalValue;
            wasOpen = false;
            base.OnClose();
        }

        public override void Update()
        {
            if (open)
            {
                Input._imeAllowed = true;
                Keyboard.repeat = true;
                wasOpen = true;
                blink += 0.02f;
                if (_directional)
                {
                    if (password.Length < 6)
                    {
                        if (Input.Pressed("LEFT"))
                            password += "L";
                        else if (Input.Pressed("RIGHT"))
                            password += "R";
                        else if (Input.Pressed("UP"))
                            password += "U";
                        else if (Input.Pressed("DOWN"))
                            password += "D";
                    }
                    if (Input.Pressed("SELECT"))
                    {
                        _binding.value = password;
                        _cancelled = false;
                        _backFunction.Activate();
                    }
                }
                else
                {
                    globalUILock = true;
                    if (Keyboard.keyString.Length > _maxLength)
                        Keyboard.keyString = Keyboard.keyString.Substring(0, _maxLength);
                    if (_numeric)
                        Keyboard.keyString = Regex.Replace(Keyboard.keyString, "[^0-9]", "");
                    InputProfile.ignoreKeyboard = true;
                    password = Keyboard.keyString;
                    if (Keyboard.Pressed(Keys.Enter))
                    {
                        bool flag = false;
                        if (_numeric)
                        {
                            try
                            {
                                int num = Convert.ToInt32(Keyboard.keyString);
                                if (num < _minNumber)
                                {
                                    num = _minNumber;
                                    flag = true;
                                }
                                else if (num > _maxNumber)
                                {
                                    num = _maxNumber;
                                    flag = true;
                                }
                                Keyboard.keyString = num.ToString();
                            }
                            catch (Exception)
                            {
                                Keyboard.keyString = "";
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            globalUILock = false;
                            _binding.value = password;
                            _cancelled = false;
                            _backFunction.Activate();
                        }
                    }
                    else if (Keyboard.Pressed(Keys.Escape) || Input.Pressed("CANCEL"))
                    {
                        globalUILock = false;
                        _cancelled = true;
                        _backFunction.Activate();
                    }
                }
                InputProfile.ignoreKeyboard = false;
            }
            base.Update();
        }

        public override void Draw()
        {
            if (_directional)
                Graphics.DrawPassword(password, new Vec2(x - password.Length * 8 / 2, y - 6f), Color.White, depth + 10);
            else
                Graphics.DrawString(password + (blink % 1.0 > 0.5 ? "_" : ""), new Vec2(x - password.Length * 8 / 2, y - 6f), Color.White, depth + 10);
            base.Draw();
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.UIStringEntryMenu
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.Add(new UIBox(0f, 0f, 100f, 16f, isVisible: false), true);
            this._binding = pBinding;
            this._directional = directional;
            this._numeric = pNumeric;
            this._maxLength = pMaxLength;
            this._minNumber = pMinNumber;
            this._maxNumber = pMaxNumber;
        }

        public void SetValue(string pValue) => this.password = pValue;

        public override void Open()
        {
            if (this._directional)
                this.password = "";
            this._originalValue = this.password;
            Keyboard.keyString = this.password;
            this._cancelled = true;
            base.Open();
        }

        public override void OnClose()
        {
            Keyboard.repeat = false;
            if (this.wasOpen && this._cancelled)
                this._binding.value = this._directional ? "" : (object)this._originalValue;
            this.wasOpen = false;
            base.OnClose();
        }

        public override void Update()
        {
            if (this.open)
            {
                Input._imeAllowed = true;
                Keyboard.repeat = true;
                this.wasOpen = true;
                this.blink += 0.02f;
                if (this._directional)
                {
                    if (this.password.Length < 6)
                    {
                        if (Input.Pressed("LEFT"))
                            this.password += "L";
                        else if (Input.Pressed("RIGHT"))
                            this.password += "R";
                        else if (Input.Pressed("UP"))
                            this.password += "U";
                        else if (Input.Pressed("DOWN"))
                            this.password += "D";
                    }
                    if (Input.Pressed("SELECT"))
                    {
                        this._binding.value = password;
                        this._cancelled = false;
                        this._backFunction.Activate();
                    }
                }
                else
                {
                    UIMenu.globalUILock = true;
                    if (Keyboard.keyString.Length > this._maxLength)
                        Keyboard.keyString = Keyboard.keyString.Substring(0, this._maxLength);
                    if (this._numeric)
                        Keyboard.keyString = Regex.Replace(Keyboard.keyString, "[^0-9]", "");
                    InputProfile.ignoreKeyboard = true;
                    this.password = Keyboard.keyString;
                    if (Keyboard.Pressed(Keys.Enter))
                    {
                        bool flag = false;
                        if (this._numeric)
                        {
                            try
                            {
                                int num = Convert.ToInt32(Keyboard.keyString);
                                if (num < this._minNumber)
                                {
                                    num = this._minNumber;
                                    flag = true;
                                }
                                else if (num > this._maxNumber)
                                {
                                    num = this._maxNumber;
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
                            UIMenu.globalUILock = false;
                            this._binding.value = password;
                            this._cancelled = false;
                            this._backFunction.Activate();
                        }
                    }
                    else if (Keyboard.Pressed(Keys.Escape) || Input.Pressed("CANCEL"))
                    {
                        UIMenu.globalUILock = false;
                        this._cancelled = true;
                        this._backFunction.Activate();
                    }
                }
                InputProfile.ignoreKeyboard = false;
            }
            base.Update();
        }

        public override void Draw()
        {
            if (this._directional)
                Graphics.DrawPassword(this.password, new Vec2(this.x - this.password.Length * 8 / 2, this.y - 6f), Color.White, this.depth + 10);
            else
                Graphics.DrawString(this.password + (blink % 1.0 > 0.5 ? "_" : ""), new Vec2(this.x - this.password.Length * 8 / 2, this.y - 6f), Color.White, this.depth + 10);
            base.Draw();
        }
    }
}

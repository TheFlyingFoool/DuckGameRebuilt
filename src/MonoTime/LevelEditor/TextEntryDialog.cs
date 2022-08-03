// Decompiled with JetBrains decompiler
// Type: DuckGame.TextEntryDialog
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.IO;

namespace DuckGame
{
    public class TextEntryDialog : ContextMenu
    {
        private new string _text = "";
        public string result;
        public FancyBitmapFont _fancyFont;
        public float _cursorFlash;
        public bool filename;
        private MysteryTextbox _textbox;
        private string _startingText = "";
        private bool _usingOnscreenKeyboard;
        private char[] invalidPathChars;
        private int _maxChars = 30;
        private string _default = "";

        public TextEntryDialog()
          : base(null)
        {
        }

        //private void DoStuff(IAsyncResult r)
        //{
        //    this.opened = false;
        //    Editor.PopFocus();
        //}

        public override void Initialize()
        {
            if (Level.current is Editor)
                layer = Editor.objectMenuLayer;
            else
                layer = Layer.HUD;
            depth = (Depth)0.95f;
            float num1 = 300f;
            float num2 = 40f;
            Vec2 vec2_1 = new Vec2((float)(layer.width / 2.0 - num1 / 2.0), (float)(layer.height / 2.0 - num2 / 2.0));
            Vec2 vec2_2 = new Vec2((float)(layer.width / 2.0 + num1 / 2.0), (float)(layer.height / 2.0 + num2 / 2.0));
            position = vec2_1 + new Vec2(4f, 20f);
            itemSize = new Vec2(490f, 16f);
            _root = true;
            invalidPathChars = Path.GetInvalidPathChars();
            _fancyFont = new FancyBitmapFont("smallFont");
            _textbox = new MysteryTextbox(vec2_1.x + 4f, vec2_1.y + 4f, num1 - 20f, num2 - 10f)
            {
                enterConfirms = true,
                filename = true
            };
        }

        private void TextEntryComplete(string pResult)
        {
            Steam.TextEntryComplete -= new Steam.TextEntryCompleteDelegate(TextEntryComplete);
            result = pResult;
            if (result == null || result == "")
                result = _startingText;
            opened = false;
            Editor.skipFrame = true;
            Editor.PopFocus();
            Editor.enteringText = false;
        }

        public void Open(string text, string startingText = "", int maxChars = 30)
        {
            _usingOnscreenKeyboard = false;
            _startingText = startingText;
            result = null;
            opened = true;
            if (Steam.ShowOnscreenKeyboard(false, text, startingText, maxChars))
            {
                Steam.TextEntryComplete += new Steam.TextEntryCompleteDelegate(TextEntryComplete);
                _usingOnscreenKeyboard = true;
                Editor.enteringText = true;
                Editor.PushFocus(this);
                SFX.Play("openClick", 0.4f);
            }
            else
            {
                _text = text;
                _default = startingText;
                Keyboard.keyString = "";
                Editor.enteringText = true;
                _maxChars = maxChars;
                Editor.PushFocus(this);
                SFX.Play("openClick", 0.4f);
                _textbox.text = _default;
                _textbox._cursorPosition = _textbox.text.Length;
                _textbox.color = Color.White;
                _textbox.cursorColor = Color.White;
            }
        }

        public void Close()
        {
            Editor.enteringText = false;
            Editor.PopFocus();
            opened = false;
        }

        public override void Selected(ContextMenu item)
        {
        }

        public override void Update()
        {
            if (!opened || _usingOnscreenKeyboard)
                return;
            _textbox.Update();
            if (_textbox.confirmed)
            {
                _textbox.confirmed = false;
                result = _textbox.text;
                opened = false;
                Editor.skipFrame = true;
                Editor.PopFocus();
                Editor.enteringText = false;
            }
            if (!Keyboard.Pressed(Keys.Escape) && Mouse.right != InputState.Pressed && !Input.Pressed("CANCEL"))
                return;
            result = _default;
            opened = false;
            Editor.PopFocus();
            Editor.skipFrame = true;
            Editor.enteringText = false;
        }

        public override void Draw()
        {
            if (!opened || _usingOnscreenKeyboard)
                return;
            base.Draw();
            float num1 = 300f;
            float num2 = 72f;
            Vec2 p1 = new Vec2((float)(layer.width / 2.0 - num1 / 2.0), (float)(layer.height / 2.0 - num2 / 2.0));
            Vec2 p2 = new Vec2((float)(layer.width / 2.0 + num1 / 2.0), (float)(layer.height / 2.0 + num2 / 2.0));
            Graphics.DrawRect(p1, p2, new Color(70, 70, 70), depth, false, 0.95f);
            Graphics.DrawRect(p1, p2, new Color(30, 30, 30), depth - 1);
            Graphics.DrawRect(p1 + new Vec2(4f, 20f), p2 + new Vec2(-4f, -4f), new Color(10, 10, 10), depth + 1);
            Graphics.DrawRect(p1 + new Vec2(2f, 2f), new Vec2(p2.x - 2f, p1.y + 16f), new Color(70, 70, 70), depth + 1);
            _textbox.depth = depth + 20;
            _textbox.Draw();
            Graphics.DrawString(_text, p1 + new Vec2(5f, 5f), Color.White, depth + 2);
            Graphics.DrawRect(new Vec2(p2.x - 145f, p2.y), new Vec2(p2.x, p2.y + 10f), new Color(70, 70, 70), depth + 8);
            if (!(InputProfile.FirstProfileWithDevice.lastActiveDevice is Keyboard))
                Graphics.DrawString("@SELECT@ACCEPT  @CANCEL@CANCEL", p2 + new Vec2(-147f, 1f), Color.White, depth + 10);
            else
                Graphics.DrawString("@ENTERKEY@ACCEPT  @ESCAPE@CANCEL", p2 + new Vec2(-147f, 1f), Color.White, depth + 10);
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.Textbox
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Threading;
using System.Windows.Forms;

namespace DuckGame
{
    public class Textbox
    {
        private FancyBitmapFont _font;
        public string text = "";
        public int _cursorPosition;
        private Vec2 _position;
        private Vec2 _size;
        protected bool _inFocus;
        private float _blink;
        public int _maxLines;
        private string _emptyText;
        public Depth depth;
        public int maxLength = 1000;
        private string _drawText = "";
        private Vec2 _cursorPos;
        private bool _highlightDrag;
        private string _clipboardText = "";

        public Vec2 position
        {
            get => this._position;
            set => this._position = value;
        }

        public Vec2 size
        {
            get => this._size;
            set
            {
                this._size = value;
                this._font.maxWidth = (int)value.x;
            }
        }

        public Textbox(
          float x,
          float y,
          float width,
          float height,
          float scale = 1f,
          int maxLines = 2147483647,
          string emptyText = "")
        {
            this._font = new FancyBitmapFont("smallFont")
            {
                scale = new Vec2(scale),
                maxWidth = (int)width
            };
            this._position = new Vec2(x, y);
            this._size = new Vec2(width, height);
            this._maxLines = maxLines;
            this._emptyText = emptyText;
        }

        private void ConstrainSelection()
        {
            if (this._font._highlightEnd < 0)
                this._font._highlightEnd = 0;
            if (this._font._highlightStart < 0)
                this._font._highlightStart = 0;
            if (this._font._highlightEnd > this.text.Length)
                this._font._highlightEnd = this.text.Length;
            if (this._font._highlightStart <= this.text.Length)
                return;
            this._font._highlightStart = this.text.Length;
        }

        private void DeleteHighlight()
        {
            this.ConstrainSelection();
            if (this._font._highlightStart < this._font._highlightEnd)
            {
                this.text = this.text.Remove(this._font._highlightStart, this._font._highlightEnd - this._font._highlightStart);
                this._cursorPosition = this._font._highlightStart;
                this._font._highlightEnd = this._cursorPosition;
            }
            else
            {
                this.text = this.text.Remove(this._font._highlightEnd, this._font._highlightStart - this._font._highlightEnd);
                this._cursorPosition = this._font._highlightEnd;
                this._font._highlightStart = this._cursorPosition;
            }
        }

        public void ReadClipboardText()
        {
            this._clipboardText = "";
            if (!Clipboard.ContainsText())
                return;
            this._clipboardText = Clipboard.GetText();
        }

        public void LoseFocus()
        {
            this._inFocus = false;
            Editor.PopFocusNow();
        }

        public void GainFocus()
        {
            this._inFocus = true;
            Keyboard.keyString = "";
            Editor.PushFocus(this);
        }

        public void Update()
        {
            bool flag = false;
            if (Mouse.x > _position.x && Mouse.y > _position.y && Mouse.x < _position.x + this._size.x && Mouse.y < _position.y + this._size.y)
            {
                flag = true;
                Editor.hoverTextBox = true;
                if (Mouse.left == InputState.Pressed)
                {
                    if (Editor.PeekFocus() is Textbox)
                    {
                        (Editor.PeekFocus() as Textbox)._inFocus = false;
                        Editor.PopFocusNow();
                    }
                    this._inFocus = true;
                    Keyboard.keyString = "";
                    Editor.PushFocus(this);
                }
            }
            Vec2 position = this._position;
            if (this._inFocus)
            {
                Input._imeAllowed = true;
                Keyboard.repeat = true;
                int length1 = this.text.Length;
                if (Keyboard.Down(Keys.LeftControl) || Keyboard.Down(Keys.RightControl))
                {
                    if (Keyboard.Pressed(Keys.V))
                    {
                        Thread thread = new Thread(() => this.ReadClipboardText());
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                        thread.Join();
                        if (this._clipboardText != "")
                        {
                            if (this._font._highlightStart != this._font._highlightEnd)
                                this.DeleteHighlight();
                            this.text = this.text.Insert(this._cursorPosition, this._clipboardText);
                            this._cursorPosition += this._clipboardText.Length;
                        }
                    }
                    else if ((Keyboard.Pressed(Keys.C) || Keyboard.Pressed(Keys.X)) && this._font._highlightStart != this._font._highlightEnd)
                    {
                        string copyText = "";
                        copyText = this._font._highlightStart >= this._font._highlightEnd ? this.text.Substring(this._font._highlightEnd, this._font._highlightStart - this._font._highlightEnd) : this.text.Substring(this._font._highlightStart, this._font._highlightEnd - this._font._highlightStart);
                        if (copyText != "")
                        {
                            Thread thread = new Thread(() => Clipboard.SetText(copyText));
                            thread.SetApartmentState(ApartmentState.STA);
                            thread.Start();
                            thread.Join();
                        }
                        if (Keyboard.Pressed(Keys.X))
                            this.DeleteHighlight();
                    }
                    Keyboard.keyString = "";
                }
                if (Keyboard.keyString.Length > 0 && this._font._highlightStart != this._font._highlightEnd)
                    this.DeleteHighlight();
                this.text = this.text.Insert(this._cursorPosition, Keyboard.keyString);
                if (Keyboard.Pressed(Keys.Back) && this.text.Length > 0)
                {
                    if (this._font._highlightStart != this._font._highlightEnd)
                        this.DeleteHighlight();
                    else if (this._cursorPosition > 0)
                    {
                        this.text = this.text.Remove(this._cursorPosition - 1, 1);
                        --this._cursorPosition;
                    }
                }
                if (Keyboard.Pressed(Keys.Delete) && this.text.Length > 0)
                {
                    if (this._font._highlightStart != this._font._highlightEnd)
                        this.DeleteHighlight();
                    else if (this._cursorPosition > 0 && this._cursorPosition < this.text.Length)
                        this.text = this.text.Remove(this._cursorPosition, 1);
                }
                if (Keyboard.Pressed(Keys.Enter))
                {
                    if (this._font._highlightStart != this._font._highlightEnd)
                        this.DeleteHighlight();
                    this.text = this.text.Insert(this._cursorPosition, "\n");
                    ++this._cursorPosition;
                }
                int length2 = this.text.Length;
                this._cursorPosition += Keyboard.keyString.Length;
                Keyboard.keyString = "";
                if (Keyboard.Pressed(Keys.Left))
                {
                    --this._cursorPosition;
                    this._font._highlightStart = this._cursorPosition;
                    this._font._highlightEnd = this._cursorPosition;
                    this._blink = 0.5f;
                }
                if (Keyboard.Pressed(Keys.Right))
                {
                    ++this._cursorPosition;
                    this._font._highlightStart = this._cursorPosition;
                    this._font._highlightEnd = this._cursorPosition;
                    this._blink = 0.5f;
                }
                if (Keyboard.Pressed(Keys.Up))
                {
                    this._cursorPosition = this._font.GetCharacterIndex(this._drawText, this._cursorPos.x + 4f * this._font.scale.x, this._cursorPos.y - _font.characterHeight * this._font.scale.y);
                    this._font._highlightStart = this._cursorPosition;
                    this._font._highlightEnd = this._cursorPosition;
                    this._blink = 0.5f;
                }
                if (Keyboard.Pressed(Keys.Down))
                {
                    this._cursorPosition = this._font.GetCharacterIndex(this._drawText, this._cursorPos.x + 4f * this._font.scale.x, this._cursorPos.y + _font.characterHeight * this._font.scale.y);
                    this._font._highlightStart = this._cursorPosition;
                    this._font._highlightEnd = this._cursorPosition;
                    this._blink = 0.5f;
                }
                this.ConstrainSelection();
                this.text = this.text.Substring(0, this._font.GetCharacterIndex(this.text, 99999f, 99999f, this._maxLines));
            }
            else
                this._font._highlightStart = this._font._highlightEnd = 0;
            this._drawText = this.text;
            if (flag && Mouse.left == InputState.Pressed)
            {
                int characterIndex = this._font.GetCharacterIndex(this._drawText, Mouse.x + 4f * this._font.scale.x - position.x, Mouse.y - position.y);
                this._cursorPosition = characterIndex;
                this._font._highlightStart = characterIndex;
                this._font._highlightEnd = characterIndex;
                this._highlightDrag = true;
                this._blink = 0.5f;
            }
            if (this._highlightDrag)
            {
                this._font._highlightEnd = this._font.GetCharacterIndex(this._drawText, Mouse.x + 4f * this._font.scale.x - position.x, Mouse.y - position.y);
                this._blink = 0.5f;
            }
            if (this.text.Length > this.maxLength)
                this.text = this.text.Substring(0, this.maxLength);
            this.ConstrainSelection();
            if (Mouse.left != InputState.Pressed && Mouse.left != InputState.Down)
                this._highlightDrag = false;
            if (this._cursorPosition > this.text.Length)
                this._cursorPosition = this.text.Length;
            if (this._cursorPosition < 0)
                this._cursorPosition = 0;
            this._cursorPos = this._font.GetCharacterPosition(this._drawText, this._cursorPosition);
            this._drawText = this.text;
            if (this.text.Length == 0 && !this._inFocus)
                this._drawText = this._emptyText;
            this._blink = (float)((_blink + 0.0199999995529652) % 1.0);
        }

        public void Draw()
        {
            this._font.Draw(this._drawText, this._position, this.text.Length == 0 ? Colors.Silver * 0.8f : Color.White, this.depth);
            if (!this._inFocus || _blink < 0.5)
                return;
            Vec2 cursorPos = this._cursorPos;
            cursorPos.x += 1f * this._font.scale.x;
            Graphics.DrawLine(this._position + cursorPos, this._position + cursorPos + new Vec2(0f, 8f * this._font.scale.y), Color.White, 0.5f, this.depth);
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.MysteryTextbox
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace DuckGame
{
    public class MysteryTextbox
    {
        //private bool allowFocusStealing = true;
        public FancyBitmapFont _font;
        public string text = "";
        public int _cursorPosition;
        private Vec2 _position;
        private Vec2 _size;
        private float _blink;
        public int _maxLines;
        private string _emptyText;
        public Depth depth;
        public bool autoSizeVertically;
        public bool allowRightClick;
        public bool highlightKeywords;
        public int maxLength = 100000;
        public bool filename;
        private char[] invalidPathChars;
        private string _drawText = "";
        private Vec2 _cursorPos;
        private bool _highlightDrag;
        private string _clipboardText = "";
        //private int prevMaxIndex;
        //private string prevMaxString;
        public bool confirmed;
        public bool enterConfirms;
        //private string _prevCalcString;
        public int numPages;
        public int currentPage;
        public Color color = Color.Black;
        public Color cursorColor = Color.Black;

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

        public string emptyText => this._emptyText;

        public MysteryTextbox(
          float x,
          float y,
          float width,
          float height,
          float scale = 1f,
          int maxLines = 2147483647,
          string emptyText = "",
          string font = "smallFont")
        {
            this._font = new FancyBitmapFont(font)
            {
                scale = new Vec2(scale),
                maxWidth = (int)width
            };
            this._position = new Vec2(x, y);
            this._size = new Vec2(width, height);
            this._maxLines = maxLines;
            this._emptyText = emptyText;
            Keyboard.keyString = "";
            this.invalidPathChars = Path.GetInvalidPathChars();
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

        public void Update(int page = 0, int rowsPerPage = -1, int firstPageRows = 0)
        {
            bool flag = false;
            Vec2 position1 = Mouse.position;
            if (position1.x > this._position.x && position1.y > this._position.y && position1.x < _position.x + this._size.x && position1.y < _position.y + this._size.y)
            {
                flag = true;
                Editor.hoverTextBox = true;
                if (Mouse.left == InputState.Pressed)
                    Keyboard.keyString = "";
            }
            //this.allowFocusStealing = true;
            Vec2 position2 = this._position;
            Keyboard.repeat = true;
            Input._imeAllowed = true;
            int length1 = this.text.Length;
            string text = this.text;
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
            if (this._cursorPosition >= this.text.Length)
                this._cursorPosition = this.text.Length;
            if (this.filename)
                Keyboard.keyString = DuckFile.FixInvalidPath(Keyboard.keyString, true);
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
                else if (this._cursorPosition > -1 && this._cursorPosition < this.text.Length)
                    this.text = this.text.Remove(this._cursorPosition, 1);
            }
            if (Keyboard.Pressed(Keys.Enter) || Input.Pressed("JUMP"))
            {
                if (this.enterConfirms)
                {
                    this.confirmed = true;
                }
                else
                {
                    if (this._font._highlightStart != this._font._highlightEnd)
                        this.DeleteHighlight();
                    this.text = this.text.Insert(this._cursorPosition, "\n");
                    ++this._cursorPosition;
                }
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
            if (this._cursorPosition > this.text.Length)
                this._cursorPosition = this.text.Length;
            if (this._cursorPosition < 0)
                this._cursorPosition = 0;
            this._drawText = this.text;
            if (flag && Mouse.left == InputState.Pressed)
            {
                int characterIndex = this._font.GetCharacterIndex(this._drawText, position1.x + 4f * this._font.scale.x - position2.x, position1.y - position2.y);
                this._cursorPosition = characterIndex;
                this._font._highlightStart = characterIndex;
                this._font._highlightEnd = characterIndex;
                this._highlightDrag = true;
                this._blink = 0.5f;
            }
            if (this._highlightDrag)
            {
                //this.allowFocusStealing = false;
                this._font._highlightEnd = this._font.GetCharacterIndex(this._drawText, position1.x + 4f * this._font.scale.x - position2.x, position1.y - position2.y);
                this._blink = 0.5f;
            }
            if (this.text.Length > this.maxLength)
                this.text = this.text.Substring(0, this.maxLength);
            this.ConstrainSelection();
            if (Mouse.left != InputState.Pressed && Mouse.left != InputState.Down)
                this._highlightDrag = false;
            this._cursorPos = this._font.GetCharacterPosition(this._drawText, this._cursorPosition);
            this._drawText = this.text;
            this._blink = (float)((_blink + 0.02f) % 1f);
        }

        public float textWidth => this._font.GetWidth(this._drawText);

        public void Draw(int page = 0, int rowsPerPage = -1, int firstPageRows = 0)
        {
            this._font.Draw(this._drawText, this._position.x, this._position.y, this.text.Length == 0 ? Colors.BlueGray * 0.8f : this.color, this.depth);
            if (_blink < 0.5)
                return;
            Vec2 cursorPos = this._cursorPos;
            cursorPos.x += 1f * this._font.scale.x;
            Graphics.DrawLine(this._position + cursorPos, this._position + cursorPos + new Vec2(0f, 8f * this._font.scale.y), this.cursorColor, 0.5f, this.depth);
        }
    }
}

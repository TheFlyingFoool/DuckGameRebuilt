// Decompiled with JetBrains decompiler
// Type: DuckGame.MysteryTextbox
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using SDL2;
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
            get => _position;
            set => _position = value;
        }

        public Vec2 size
        {
            get => _size;
            set
            {
                _size = value;
                _font.maxWidth = (int)value.x;
            }
        }

        public string emptyText => _emptyText;

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
            _font = new FancyBitmapFont(font)
            {
                scale = new Vec2(scale),
                maxWidth = (int)width
            };
            _position = new Vec2(x, y);
            _size = new Vec2(width, height);
            _maxLines = maxLines;
            _emptyText = emptyText;
            Keyboard.keyString = "";
            invalidPathChars = Path.GetInvalidPathChars();
        }

        private void ConstrainSelection()
        {
            if (_font._highlightEnd < 0)
                _font._highlightEnd = 0;
            if (_font._highlightStart < 0)
                _font._highlightStart = 0;
            if (_font._highlightEnd > text.Length)
                _font._highlightEnd = text.Length;
            if (_font._highlightStart <= text.Length)
                return;
            _font._highlightStart = text.Length;
        }

        private void DeleteHighlight()
        {
            ConstrainSelection();
            if (_font._highlightStart < _font._highlightEnd)
            {
                text = text.Remove(_font._highlightStart, _font._highlightEnd - _font._highlightStart);
                _cursorPosition = _font._highlightStart;
                _font._highlightEnd = _cursorPosition;
            }
            else
            {
                text = text.Remove(_font._highlightEnd, _font._highlightStart - _font._highlightEnd);
                _cursorPosition = _font._highlightEnd;
                _font._highlightStart = _cursorPosition;
            }
        }

        public void ReadClipboardText()
        {
            _clipboardText = "";
            if (!(SDL.SDL_HasClipboardText() == SDL.SDL_bool.SDL_TRUE))
                return;
            _clipboardText = SDL.SDL_GetClipboardText();
        }

        public void Update(int page = 0, int rowsPerPage = -1, int firstPageRows = 0)
        {
            bool flag = false;
            Vec2 position1 = Mouse.position;
            if (position1.x > _position.x && position1.y > _position.y && position1.x < _position.x + _size.x && position1.y < _position.y + _size.y)
            {
                flag = true;
                Editor.hoverTextBox = true;
                if (Mouse.left == InputState.Pressed)
                    Keyboard.keyString = "";
            }
            //this.allowFocusStealing = true;
            Vec2 position2 = _position;
            Keyboard.repeat = true;
            Input._imeAllowed = true;
            int length1 = this.text.Length;
            string text = this.text;
            if (Keyboard.Down(Keys.LeftControl) || Keyboard.Down(Keys.RightControl))
            {
                if (Keyboard.Pressed(Keys.V))
                {
                    Thread thread = new Thread(() => ReadClipboardText());
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join();
                    if (_clipboardText != "")
                    {
                        if (_font._highlightStart != _font._highlightEnd)
                            DeleteHighlight();
                        this.text = this.text.Insert(_cursorPosition, _clipboardText);
                        _cursorPosition += _clipboardText.Length;
                    }
                }
                else if ((Keyboard.Pressed(Keys.C) || Keyboard.Pressed(Keys.X)) && _font._highlightStart != _font._highlightEnd)
                {
                    string copyText = "";
                    copyText = _font._highlightStart >= _font._highlightEnd ? this.text.Substring(_font._highlightEnd, _font._highlightStart - _font._highlightEnd) : this.text.Substring(_font._highlightStart, _font._highlightEnd - _font._highlightStart);
                    if (copyText != "")
                    {
                        Thread thread = new Thread(() => SDL.SDL_SetClipboardText(copyText));
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                        thread.Join();
                    }
                    if (Keyboard.Pressed(Keys.X))
                        DeleteHighlight();
                }
                Keyboard.keyString = "";
            }
            if (Keyboard.keyString.Length > 0 && _font._highlightStart != _font._highlightEnd)
                DeleteHighlight();
            if (_cursorPosition >= this.text.Length)
                _cursorPosition = this.text.Length;
            if (filename)
                Keyboard.keyString = DuckFile.FixInvalidPath(Keyboard.keyString, true);
            this.text = this.text.Insert(_cursorPosition, Keyboard.keyString);
            if (Keyboard.Pressed(Keys.Back) && this.text.Length > 0)
            {
                if (_font._highlightStart != _font._highlightEnd)
                    DeleteHighlight();
                else if (_cursorPosition > 0)
                {
                    this.text = this.text.Remove(_cursorPosition - 1, 1);
                    --_cursorPosition;
                }
            }
            if (Keyboard.Pressed(Keys.Delete) && this.text.Length > 0)
            {
                if (_font._highlightStart != _font._highlightEnd)
                    DeleteHighlight();
                else if (_cursorPosition > -1 && _cursorPosition < this.text.Length)
                    this.text = this.text.Remove(_cursorPosition, 1);
            }
            if (Keyboard.Pressed(Keys.Enter) || Input.Pressed("JUMP"))
            {
                if (enterConfirms)
                {
                    confirmed = true;
                }
                else
                {
                    if (_font._highlightStart != _font._highlightEnd)
                        DeleteHighlight();
                    this.text = this.text.Insert(_cursorPosition, "\n");
                    ++_cursorPosition;
                }
            }
            int length2 = this.text.Length;
            _cursorPosition += Keyboard.keyString.Length;
            Keyboard.keyString = "";
            if (Keyboard.Pressed(Keys.Left))
            {
                --_cursorPosition;
                _font._highlightStart = _cursorPosition;
                _font._highlightEnd = _cursorPosition;
                _blink = 0.5f;
            }
            if (Keyboard.Pressed(Keys.Right))
            {
                ++_cursorPosition;
                _font._highlightStart = _cursorPosition;
                _font._highlightEnd = _cursorPosition;
                _blink = 0.5f;
            }
            if (Keyboard.Pressed(Keys.Up))
            {
                _cursorPosition = _font.GetCharacterIndex(_drawText, _cursorPos.x + 4f * _font.scale.x, _cursorPos.y - _font.characterHeight * _font.scale.y);
                _font._highlightStart = _cursorPosition;
                _font._highlightEnd = _cursorPosition;
                _blink = 0.5f;
            }
            if (Keyboard.Pressed(Keys.Down))
            {
                _cursorPosition = _font.GetCharacterIndex(_drawText, _cursorPos.x + 4f * _font.scale.x, _cursorPos.y + _font.characterHeight * _font.scale.y);
                _font._highlightStart = _cursorPosition;
                _font._highlightEnd = _cursorPosition;
                _blink = 0.5f;
            }
            ConstrainSelection();
            if (_cursorPosition > this.text.Length)
                _cursorPosition = this.text.Length;
            if (_cursorPosition < 0)
                _cursorPosition = 0;
            _drawText = this.text;
            if (flag && Mouse.left == InputState.Pressed)
            {
                int characterIndex = _font.GetCharacterIndex(_drawText, position1.x + 4f * _font.scale.x - position2.x, position1.y - position2.y);
                _cursorPosition = characterIndex;
                _font._highlightStart = characterIndex;
                _font._highlightEnd = characterIndex;
                _highlightDrag = true;
                _blink = 0.5f;
            }
            if (_highlightDrag)
            {
                //this.allowFocusStealing = false;
                _font._highlightEnd = _font.GetCharacterIndex(_drawText, position1.x + 4f * _font.scale.x - position2.x, position1.y - position2.y);
                _blink = 0.5f;
            }
            if (this.text.Length > maxLength)
                this.text = this.text.Substring(0, maxLength);
            ConstrainSelection();
            if (Mouse.left != InputState.Pressed && Mouse.left != InputState.Down)
                _highlightDrag = false;
            _cursorPos = _font.GetCharacterPosition(_drawText, _cursorPosition);
            _drawText = this.text;
            _blink = (float)((_blink + 0.02f) % 1f);
        }

        public float textWidth => _font.GetWidth(_drawText);

        public void Draw(int page = 0, int rowsPerPage = -1, int firstPageRows = 0)
        {
            _font.Draw(_drawText, _position.x, _position.y, text.Length == 0 ? Colors.BlueGray * 0.8f : color, depth);
            if (_blink < 0.5)
                return;
            Vec2 cursorPos = _cursorPos;
            cursorPos.x += 1f * _font.scale.x;
            Graphics.DrawLine(_position + cursorPos, _position + cursorPos + new Vec2(0f, 8f * _font.scale.y), cursorColor, 0.5f, depth);
        }
    }
}

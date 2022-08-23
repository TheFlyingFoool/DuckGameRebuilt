// Decompiled with JetBrains decompiler
// Type: DuckGame.Textbox
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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

        public Textbox(
          float x,
          float y,
          float width,
          float height,
          float scale = 1f,
          int maxLines = 2147483647,
          string emptyText = "")
        {
            _font = new FancyBitmapFont("smallFont")
            {
                scale = new Vec2(scale),
                maxWidth = (int)width
            };
            _position = new Vec2(x, y);
            _size = new Vec2(width, height);
            _maxLines = maxLines;
            _emptyText = emptyText;
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
            if (!Clipboard.ContainsText())
                return;
            _clipboardText = Clipboard.GetText();
        }

        public void LoseFocus()
        {
            _inFocus = false;
            Editor.PopFocusNow();
        }

        public void GainFocus()
        {
            _inFocus = true;
            Keyboard.KeyString = "";
            Editor.PushFocus(this);
        }

        public void Update()
        {
            bool flag = false;
            if (Mouse.x > _position.x && Mouse.y > _position.y && Mouse.x < _position.x + _size.x && Mouse.y < _position.y + _size.y)
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
                    _inFocus = true;
                    Keyboard.KeyString = "";
                    Editor.PushFocus(this);
                }
            }
            Vec2 position = _position;
            if (_inFocus)
            {
                Input._imeAllowed = true;
                Keyboard.repeat = true;
                int length1 = text.Length;
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
                            text = text.Insert(_cursorPosition, _clipboardText);
                            _cursorPosition += _clipboardText.Length;
                        }
                    }
                    else if ((Keyboard.Pressed(Keys.C) || Keyboard.Pressed(Keys.X)) && _font._highlightStart != _font._highlightEnd)
                    {
                        string copyText = "";
                        copyText = _font._highlightStart >= _font._highlightEnd ? text.Substring(_font._highlightEnd, _font._highlightStart - _font._highlightEnd) : text.Substring(_font._highlightStart, _font._highlightEnd - _font._highlightStart);
                        if (copyText != "")
                        {
                            Thread thread = new Thread(() => Clipboard.SetText(copyText));
                            thread.SetApartmentState(ApartmentState.STA);
                            thread.Start();
                            thread.Join();
                        }
                        if (Keyboard.Pressed(Keys.X))
                            DeleteHighlight();
                    }
                    Keyboard.KeyString = "";
                }
                if (Keyboard.KeyString.Length > 0 && _font._highlightStart != _font._highlightEnd)
                    DeleteHighlight();
                text = text.Insert(_cursorPosition, Keyboard.KeyString);
                if (Keyboard.Pressed(Keys.Back) && text.Length > 0)
                {
                    if (_font._highlightStart != _font._highlightEnd)
                        DeleteHighlight();
                    else if (_cursorPosition > 0)
                    {
                        text = text.Remove(_cursorPosition - 1, 1);
                        --_cursorPosition;
                    }
                }
                if (Keyboard.Pressed(Keys.Delete) && text.Length > 0)
                {
                    if (_font._highlightStart != _font._highlightEnd)
                        DeleteHighlight();
                    else if (_cursorPosition > 0 && _cursorPosition < text.Length)
                        text = text.Remove(_cursorPosition, 1);
                }
                if (Keyboard.Pressed(Keys.Enter))
                {
                    if (_font._highlightStart != _font._highlightEnd)
                        DeleteHighlight();
                    text = text.Insert(_cursorPosition, "\n");
                    ++_cursorPosition;
                }
                int length2 = text.Length;
                _cursorPosition += Keyboard.KeyString.Length;
                Keyboard.KeyString = "";
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
                text = text.Substring(0, _font.GetCharacterIndex(text, 99999f, 99999f, _maxLines));
            }
            else
                _font._highlightStart = _font._highlightEnd = 0;
            _drawText = text;
            if (flag && Mouse.left == InputState.Pressed)
            {
                int characterIndex = _font.GetCharacterIndex(_drawText, Mouse.x + 4f * _font.scale.x - position.x, Mouse.y - position.y);
                _cursorPosition = characterIndex;
                _font._highlightStart = characterIndex;
                _font._highlightEnd = characterIndex;
                _highlightDrag = true;
                _blink = 0.5f;
            }
            if (_highlightDrag)
            {
                _font._highlightEnd = _font.GetCharacterIndex(_drawText, Mouse.x + 4f * _font.scale.x - position.x, Mouse.y - position.y);
                _blink = 0.5f;
            }
            if (text.Length > maxLength)
                text = text.Substring(0, maxLength);
            ConstrainSelection();
            if (Mouse.left != InputState.Pressed && Mouse.left != InputState.Down)
                _highlightDrag = false;
            if (_cursorPosition > text.Length)
                _cursorPosition = text.Length;
            if (_cursorPosition < 0)
                _cursorPosition = 0;
            _cursorPos = _font.GetCharacterPosition(_drawText, _cursorPosition);
            _drawText = text;
            if (text.Length == 0 && !_inFocus)
                _drawText = _emptyText;
            _blink = (float)((_blink + 0.02f) % 1.0);
        }

        public void Draw()
        {
            _font.Draw(_drawText, _position, text.Length == 0 ? Colors.Silver * 0.8f : Color.White, depth);
            if (!_inFocus || _blink < 0.5)
                return;
            Vec2 cursorPos = _cursorPos;
            cursorPos.x += 1f * _font.scale.x;
            Graphics.DrawLine(_position + cursorPos, _position + cursorPos + new Vec2(0f, 8f * _font.scale.y), Color.White, 0.5f, depth);
        }
    }
}

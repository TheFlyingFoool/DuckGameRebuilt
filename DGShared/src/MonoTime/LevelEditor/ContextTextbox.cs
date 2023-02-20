// Decompiled with JetBrains decompiler
// Type: DuckGame.ContextTextbox
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ContextTextbox : ContextMenu
    {
        private FieldBinding _field;
        public string path = "";
        private float _blink;
        private TextEntryDialog _dialog;
        public FancyBitmapFont _fancyFont;

        public ContextTextbox(
          string text,
          IContextListener owner,
          FieldBinding field,
          string valTooltip)
          : base(owner)
        {
            itemSize.x = 150f;
            itemSize.y = 16f;
            _text = text;
            _field = field;
            if (field == null)
                _field = new FieldBinding(this, "isChecked");
            _fancyFont = new FancyBitmapFont("smallFont");
            tooltip = valTooltip;
        }

        public ContextTextbox(string text, IContextListener owner, FieldBinding field = null)
          : base(owner)
        {
            itemSize.x = 150f;
            itemSize.y = 16f;
            _text = text;
            _field = field;
            if (field == null)
                _field = new FieldBinding(this, "isChecked");
            _fancyFont = new FancyBitmapFont("smallFont");
        }

        public override void Initialize()
        {
            _dialog = new TextEntryDialog();
            Level.Add(_dialog);
        }

        public override void Terminate() => Level.Remove(_dialog);

        public override void Selected()
        {
            string startingText = "";
            if (_field != null && _field.value is string)
                startingText = _field.value as string;
            SFX.Play("highClick", 0.3f, 0.2f);
            if (Level.current is Editor)
            {
                _dialog.Open(_text, startingText, 999999);
            }
            else
            {
                if (_owner == null)
                    return;
                _owner.Selected(this);
            }
        }

        public override void Update()
        {
            if (_dialog.opened)
                return;
            _blink += 0.04f;
            if (_blink >= 1f)
                _blink = 0f;
            if (_dialog.result != null)
            {
                _field.value = _dialog.result;
                _dialog.result = null;
                Editor.hasUnsavedChanges = true;
            }
            base.Update();
        }

        public override void Draw()
        {
            string text = "";
            if (_field != null && _field.value is string)
                text = _field.value as string;
            if (_hover)
            {
                Graphics.DrawRect(position, position + itemSize, new Color(70, 70, 70), (Depth)0.82f);
                if (text.Length > 12)
                {
                    Vec2 p1 = new Vec2(this.x, this.y);
                    p1.x += itemSize.x + 4f;
                    p1.y -= 2f;
                    float x = 200f;
                    float y = 100f;
                    Graphics.DrawString(_text, position + new Vec2(2f, 5f), Color.White, (Depth)0.88f);
                    Graphics.DrawRect(p1, p1 + new Vec2(x, y), new Color(70, 70, 70), (Depth)0.83f);
                    Graphics.DrawRect(p1 + new Vec2(1f, 1f), p1 + new Vec2(x - 1f, y - 1f), new Color(30, 30, 30), (Depth)0.84f);
                    _fancyFont.depth = (Depth)0.8f;
                    _fancyFont.maxWidth = 200;
                    _fancyFont.Draw(text, p1 + new Vec2(4f, 4f), Color.White, (Depth)0.86f);
                }
                else
                {
                    if (_blink >= 0.5)
                        text += "_";
                    _fancyFont.maxWidth = 200;
                    _fancyFont.Draw(text, position + new Vec2(2f, 5f), Color.White, (Depth)0.86f);
                }
            }
            else
            {
                Graphics.DrawString(_text, position + new Vec2(2f, 5f), Color.White, (Depth)0.84f);
                if (text.Length > 12)
                    text = text.Substring(0, 12) + "..";
                _fancyFont.depth = (Depth)0.81f;
                _fancyFont.Draw(text, position + new Vec2(itemSize.x - 4f - _fancyFont.GetWidth(text), 5f), Color.White, (Depth)0.84f);
            }
        }
    }
}

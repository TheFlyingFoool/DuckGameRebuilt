// Decompiled with JetBrains decompiler
// Type: DuckGame.ContextTextbox
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.itemSize.x = 150f;
            this.itemSize.y = 16f;
            this._text = text;
            this._field = field;
            if (field == null)
                this._field = new FieldBinding((object)this, "isChecked");
            this._fancyFont = new FancyBitmapFont("smallFont");
            this.tooltip = valTooltip;
        }

        public ContextTextbox(string text, IContextListener owner, FieldBinding field = null)
          : base(owner)
        {
            this.itemSize.x = 150f;
            this.itemSize.y = 16f;
            this._text = text;
            this._field = field;
            if (field == null)
                this._field = new FieldBinding((object)this, "isChecked");
            this._fancyFont = new FancyBitmapFont("smallFont");
        }

        public override void Initialize()
        {
            this._dialog = new TextEntryDialog();
            Level.Add((Thing)this._dialog);
        }

        public override void Terminate() => Level.Remove((Thing)this._dialog);

        public override void Selected()
        {
            string startingText = "";
            if (this._field != null && this._field.value is string)
                startingText = this._field.value as string;
            SFX.Play("highClick", 0.3f, 0.2f);
            if (Level.current is Editor)
            {
                this._dialog.Open(this._text, startingText, 999999);
            }
            else
            {
                if (this._owner == null)
                    return;
                this._owner.Selected((ContextMenu)this);
            }
        }

        public override void Update()
        {
            if (this._dialog.opened)
                return;
            this._blink += 0.04f;
            if ((double)this._blink >= 1.0)
                this._blink = 0.0f;
            if (this._dialog.result != null)
            {
                this._field.value = (object)this._dialog.result;
                this._dialog.result = (string)null;
                Editor.hasUnsavedChanges = true;
            }
            base.Update();
        }

        public override void Draw()
        {
            string text = "";
            if (this._field != null && this._field.value is string)
                text = this._field.value as string;
            if (this._hover)
            {
                Graphics.DrawRect(this.position, this.position + this.itemSize, new Color(70, 70, 70), (Depth)0.82f);
                if (text.Length > 12)
                {
                    Vec2 p1 = new Vec2(this.x, this.y);
                    p1.x += this.itemSize.x + 4f;
                    p1.y -= 2f;
                    float x = 200f;
                    float y = 100f;
                    Graphics.DrawString(this._text, this.position + new Vec2(2f, 5f), Color.White, (Depth)0.88f);
                    Graphics.DrawRect(p1, p1 + new Vec2(x, y), new Color(70, 70, 70), (Depth)0.83f);
                    Graphics.DrawRect(p1 + new Vec2(1f, 1f), p1 + new Vec2(x - 1f, y - 1f), new Color(30, 30, 30), (Depth)0.84f);
                    this._fancyFont.depth = (Depth)0.8f;
                    this._fancyFont.maxWidth = 200;
                    this._fancyFont.Draw(text, p1 + new Vec2(4f, 4f), Color.White, (Depth)0.86f);
                }
                else
                {
                    if ((double)this._blink >= 0.5)
                        text += "_";
                    this._fancyFont.maxWidth = 200;
                    this._fancyFont.Draw(text, this.position + new Vec2(2f, 5f), Color.White, (Depth)0.86f);
                }
            }
            else
            {
                Graphics.DrawString(this._text, this.position + new Vec2(2f, 5f), Color.White, (Depth)0.84f);
                if (text.Length > 12)
                    text = text.Substring(0, 12) + "..";
                this._fancyFont.depth = (Depth)0.81f;
                this._fancyFont.Draw(text, this.position + new Vec2(this.itemSize.x - 4f - this._fancyFont.GetWidth(text), 5f), Color.White, (Depth)0.84f);
            }
        }
    }
}

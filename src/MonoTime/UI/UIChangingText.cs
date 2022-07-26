// Decompiled with JetBrains decompiler
// Type: DuckGame.UIChangingText
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UIChangingText : UIText
    {
        private FieldBinding _field;
        private FieldBinding _filterBinding;
        public string defaultSizeString = "ON OFF  ";

        public UIChangingText(float wide, float high, FieldBinding field, FieldBinding filterBinding)
          : base("ON OFF  ", Color.White)
        {
            this._field = field;
            this._filterBinding = filterBinding;
        }

        public override string text
        {
            get => this._text;
            set
            {
                this._text = value;
                if (this.minLength <= 0)
                    return;
                while (this._text.Length < this.minLength)
                    this._text = " " + this._text;
            }
        }

        public override void Draw()
        {
            this._font.scale = this.scale;
            this._font.alpha = this.alpha;
            float width = this._font.GetWidth(this.defaultSizeString);
            float num1 = (this.align & UIAlign.Left) <= UIAlign.Center ? ((this.align & UIAlign.Right) <= UIAlign.Center ? (float)(-(double)width / 2.0) : this.width / 2f - width) : (float)-((double)this.width / 2.0);
            float num2 = (this.align & UIAlign.Top) <= UIAlign.Center ? ((this.align & UIAlign.Bottom) <= UIAlign.Center ? (float)(-(double)this._font.height / 2.0) : this.height / 2f - this._font.height) : (float)-((double)this.height / 2.0);
            string text = this.text;
            while (text.Length < 8)
                text = " " + text;
            this._font.colorOverride = UIMenu.disabledDraw ? Colors.BlueGray : new Color();
            this._font.Draw(text, this.x + num1, this.y + num2, Color.White, this.depth);
        }
    }
}

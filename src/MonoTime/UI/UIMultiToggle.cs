// Decompiled with JetBrains decompiler
// Type: DuckGame.UIMultiToggle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class UIMultiToggle : UIText
    {
        private FieldBinding _field;
        private List<string> _captions;
        private bool _compressed;

        public void SetFieldBinding(FieldBinding f) => this._field = f;

        public UIMultiToggle(
          float wide,
          float high,
          FieldBinding field,
          List<string> captions,
          bool compressed = false)
          : base("AAAAAAAAA", Color.White)
        {
            this._field = field;
            this._captions = captions;
            this._compressed = compressed;
        }

        public override void Draw()
        {
            this._font.scale = this.scale;
            this._font.alpha = this.alpha;
            int index = (int)this._field.value;
            string text = "";
            if (this._compressed && index < this._captions.Count)
            {
                text = this._captions[index];
            }
            else
            {
                int num = 0;
                foreach (string caption in this._captions)
                {
                    if (num != 0)
                        text += " ";
                    text = num != index ? text + "|GRAY|" : text + "|WHITE|";
                    text += caption;
                    ++num;
                }
            }
            Vec2 scale = this._font.scale;
            if ((double)this.specialScale != 0.0)
                this._font.scale = new Vec2(this.specialScale);
            float width = this._font.GetWidth(text);
            float num1 = (this.align & UIAlign.Left) <= UIAlign.Center ? ((this.align & UIAlign.Right) <= UIAlign.Center ? (float)(-(double)width / 2.0) : this.width / 2f - width) : (float)-((double)this.width / 2.0);
            float num2 = (this.align & UIAlign.Top) <= UIAlign.Center ? ((this.align & UIAlign.Bottom) <= UIAlign.Center ? (float)(-(double)this._font.height / 2.0) : this.height / 2f - this._font.height) : (float)-((double)this.height / 2.0);
            this._font.Draw(text, this.x + num1, this.y + num2, Color.White, this.depth);
            this._font.scale = scale;
        }
    }
}

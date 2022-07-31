// Decompiled with JetBrains decompiler
// Type: DuckGame.UIText
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class UIText : UIComponent
    {
        protected Color _color;
        public BitmapFont _font;
        protected string _text;
        protected Func<string> _textFunc;
        public int minLength;
        private float _heightAdd;
        private InputProfile _controlProfile;
        public float specialScale;

        public virtual string text
        {
            get
            {
                if (this._textFunc != null)
                    this.text = this._textFunc();
                return this._text;
            }
            set
            {
                this._text = value;
                if (this.minLength > 0)
                {
                    while (this._text.Length < this.minLength)
                        this._text = " " + this._text;
                }
                this._collisionSize = new Vec2(this._font.GetWidth(this._text), this._font.height + this._heightAdd);
            }
        }

        public void SetFont(BitmapFont f)
        {
            if (f != null)
                this._font = f;
            this._collisionSize = new Vec2(this._font.GetWidth(this.text), this._font.height + this._heightAdd);
        }

        public float scaleVal
        {
            set => this._font.scale = new Vec2(value);
        }

        public UIText(
          string textVal,
          Color c,
          UIAlign al = UIAlign.Center,
          float heightAdd = 0f,
          InputProfile controlProfile = null)
          : base(0f, 0f, 0f, 0f)
        {
            this._heightAdd = heightAdd;
            this._font = new BitmapFont("biosFontUI", 8, 7);
            this.text = textVal;
            this._color = c;
            this.align = al;
            this._controlProfile = controlProfile;
        }

        public UIText(
          Func<string> textFunc,
          Color c,
          UIAlign al = UIAlign.Center,
          float heightAdd = 0f,
          InputProfile controlProfile = null)
          : base(0f, 0f, 0f, 0f)
        {
            this._heightAdd = heightAdd;
            this._font = new BitmapFont("biosFontUI", 8, 7);
            this._textFunc = textFunc;
            this.text = this._textFunc();
            this._color = c;
            this.align = al;
            this._controlProfile = controlProfile;
        }

        public override void Draw()
        {
            this._font.scale = this.scale;
            this._font.alpha = this.alpha;
            float width = this._font.GetWidth(this.text);
            float num1 = (this.align & UIAlign.Left) <= UIAlign.Center ? ((this.align & UIAlign.Right) <= UIAlign.Center ? (float)(-(double)width / 2.0) : this.width / 2f - width) : (float)-((double)this.width / 2.0);
            float num2 = (this.align & UIAlign.Top) <= UIAlign.Center ? ((this.align & UIAlign.Bottom) <= UIAlign.Center ? (float)(-(double)this._font.height / 2.0) : this.height / 2f - this._font.height) : (float)-((double)this.height / 2.0);
            if (specialScale != 0.0)
            {
                Vec2 scale = this._font.scale;
                this._font.scale = new Vec2(this.specialScale);
                this._font.Draw(this.text, this.x + num1, this.y + num2, UIMenu.disabledDraw ? Colors.BlueGray : this._color, this.depth, this._controlProfile);
                this._font.scale = scale;
            }
            else
                this._font.Draw(this.text, this.x + num1, this.y + num2, UIMenu.disabledDraw ? Colors.BlueGray : this._color, this.depth, this._controlProfile);
            base.Draw();
        }
    }
}

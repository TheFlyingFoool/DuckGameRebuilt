// Decompiled with JetBrains decompiler
// Type: DuckGame.UIProgressBar
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class UIProgressBar : UIComponent
    {
        private FieldBinding _field;
        private Vec2 _barSize;
        private float _step;

        public UIProgressBar(float wide, float high, FieldBinding field, float increment, Color c = default(Color))
          : base(0f, 0f, 0f, 0f)
        {
            this._field = field;
            this._barSize = new Vec2(wide, high);
            this._collisionSize = this._barSize;
            this._step = increment;
        }

        public override void Draw()
        {
            float num1 = this._barSize.x * this.scale.x;
            float num2 = this._barSize.y * this.scale.y;
            int num3 = (int)Math.Ceiling(((double)this._field.max - (double)this._field.min) / _step);
            for (int index = 0; index < num3; ++index)
            {
                Vec2 p1 = this.position - new Vec2(this.halfWidth, num2 / 2f) + new Vec2(index * (int)Math.Round((double)num1 / num3), 0f);
                Vec2 p2 = this.position - new Vec2(this.halfWidth, (float)(-(double)num2 / 2.0)) + new Vec2((index + 1) * (int)Math.Round((double)num1 / num3) - 1f, 0f);
                if ((this.align & UIAlign.Center) > UIAlign.Center)
                {
                    p1.x += this.halfWidth - num1 / 2f;
                    p2.x += this.halfWidth - num1 / 2f;
                }
                else if ((this.align & UIAlign.Right) > UIAlign.Center)
                {
                    p1.x += this.width - num1;
                    p2.x += this.width - num1;
                }
                if (p1.x == (double)p2.x)
                    ++p2.x;
                float num4 = (float)this._field.value;
                Graphics.DrawRect(p1, p2, (double)num4 > index * (double)this._step ? Color.White : new Color(70, 70, 70), this.depth);
            }
        }
    }
}

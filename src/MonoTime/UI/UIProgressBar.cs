// Decompiled with JetBrains decompiler
// Type: DuckGame.UIProgressBar
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _field = field;
            _barSize = new Vec2(wide, high);
            _collisionSize = _barSize;
            _step = increment;
        }

        public override void Draw()
        {
            float num1 = _barSize.x * scale.x;
            float num2 = _barSize.y * scale.y;
            int num3 = (int)Math.Ceiling((_field.max - _field.min) / _step);
            for (int index = 0; index < num3; ++index)
            {
                Vec2 p1 = position - new Vec2(halfWidth, num2 / 2f) + new Vec2(index * (int)Math.Round(num1 / num3), 0f);
                Vec2 p2 = position - new Vec2(halfWidth, (float)(-num2 / 2.0)) + new Vec2((index + 1) * (int)Math.Round(num1 / num3) - 1f, 0f);
                if ((align & UIAlign.Center) > UIAlign.Center)
                {
                    p1.x += halfWidth - num1 / 2f;
                    p2.x += halfWidth - num1 / 2f;
                }
                else if ((align & UIAlign.Right) > UIAlign.Center)
                {
                    p1.x += width - num1;
                    p2.x += width - num1;
                }
                if (p1.x == p2.x)
                    ++p2.x;
                float num4 = (float)_field.value;
                Graphics.DrawRect(p1, p2, num4 > index * _step ? Color.White : new Color(70, 70, 70), depth);
            }
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.BigTitleShrink
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class BigTitleShrink : Thing
    {
        private static float _dept = 0.5f;
        private float _size;
        private bool _black;
        private Sprite _sprite;

        public BigTitleShrink(float vx, float vy, float vscale, Color vfade)
          : base()
        {
            alpha = vfade.a / (float)byte.MaxValue;
            vfade.a = byte.MaxValue;
            _sprite = new Sprite("duckGameTitleOutline");
            graphic = _sprite;
            x = vx;
            y = vy;
            scale = new Vec2(vscale, vscale);
            depth = (Depth)_dept;
            layer = Layer.HUD;
            centerx = _sprite.w / 2;
            centery = _sprite.h;
            graphic.color = vfade;
            _black = vfade == Color.Black;
            _dept -= 0.0001f;
            _size = vscale;
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
            _size += (0.98f - _size) * 0.078f;
            xscale = _size;
            yscale = _size;
            if (xscale < 1.1f)
                alpha *= 0.8f;
            if (xscale < 1.05f && _black)
                Level.Remove(this);
            if (alpha > 0f)
                return;
            Level.Remove(this);
        }
    }
}

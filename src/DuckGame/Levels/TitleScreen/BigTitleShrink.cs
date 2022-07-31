// Decompiled with JetBrains decompiler
// Type: DuckGame.BigTitleShrink
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.alpha = vfade.a / (float)byte.MaxValue;
            vfade.a = byte.MaxValue;
            this._sprite = new Sprite("duckGameTitleOutline");
            this.graphic = this._sprite;
            this.x = vx;
            this.y = vy;
            this.scale = new Vec2(vscale, vscale);
            this.depth = (Depth)BigTitleShrink._dept;
            this.layer = Layer.HUD;
            this.centerx = this._sprite.w / 2;
            this.centery = _sprite.h;
            this.graphic.color = vfade;
            this._black = vfade == Color.Black;
            BigTitleShrink._dept -= 0.0001f;
            this._size = vscale;
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
            this._size += (0.98f - _size) * 0.078f;
            this.xscale = this._size;
            this.yscale = this._size;
            if (this.xscale < 1.1f)
                this.alpha *= 0.8f;
            if (this.xscale < 1.05f && this._black)
                Level.Remove(this);
            if ((double)this.alpha > 0f)
                return;
            Level.Remove(this);
        }
    }
}

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
            this.alpha = (float)vfade.a / (float)byte.MaxValue;
            vfade.a = byte.MaxValue;
            this._sprite = new Sprite("duckGameTitleOutline");
            this.graphic = this._sprite;
            this.x = vx;
            this.y = vy;
            this.scale = new Vec2(vscale, vscale);
            this.depth = (Depth)BigTitleShrink._dept;
            this.layer = Layer.HUD;
            this.centerx = (float)(this._sprite.w / 2);
            this.centery = (float)this._sprite.h;
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
            this._size += (float)((0.980000019073486 - (double)this._size) * 0.0799999982118607);
            this.xscale = this._size;
            this.yscale = this._size;
            if ((double)this.xscale < 1.10000002384186)
                this.alpha *= 0.8f;
            if ((double)this.xscale < 1.04999995231628 && this._black)
                Level.Remove((Thing)this);
            if ((double)this.alpha > 0.0)
                return;
            Level.Remove((Thing)this);
        }
    }
}

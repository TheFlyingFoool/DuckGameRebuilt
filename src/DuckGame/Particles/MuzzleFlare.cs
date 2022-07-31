// Decompiled with JetBrains decompiler
// Type: DuckGame.MuzzleFlare
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class MuzzleFlare : Thing
    {
        private SpriteMap _sprite;

        public MuzzleFlare(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("smallFlare", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(0f, 8f);
        }

        public override void Update()
        {
            this.alpha -= 0.1f;
            if ((double)this.alpha >= 0.0)
                return;
            Level.Remove(this);
        }
    }
}

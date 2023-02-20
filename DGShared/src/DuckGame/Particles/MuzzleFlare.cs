// Decompiled with JetBrains decompiler
// Type: DuckGame.MuzzleFlare
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _sprite = new SpriteMap("smallFlare", 16, 16);
            graphic = _sprite;
            center = new Vec2(0f, 8f);
        }

        public override void Update()
        {
            alpha -= 0.1f;
            if (alpha >= 0.0)
                return;
            Level.Remove(this);
        }
    }
}

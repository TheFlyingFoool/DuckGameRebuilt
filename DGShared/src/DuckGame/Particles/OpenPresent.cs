// Decompiled with JetBrains decompiler
// Type: DuckGame.OpenPresent
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class OpenPresent : PhysicsParticle
    {
        private SpriteMap _sprite;

        public OpenPresent(float xpos, float ypos, int frame)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("presents", 16, 16)
            {
                frame = frame + 8
            };
            graphic = _sprite;
            center = new Vec2(8f, 13f);
            hSpeed = 0f;
            vSpeed = 0f;
            _bounceEfficiency = 0f;
            depth = (Depth)0.9f;
            _life = 5f;
        }

        public override void Update() => base.Update();
    }
}

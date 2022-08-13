// Decompiled with JetBrains decompiler
// Type: DuckGame.ChainLink
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ChainLink : PhysicsObject
    {
        public ChainLink(float xpos, float ypos)
        {
            graphic = new Sprite("chainLink");
            center = new Vec2(3f, 2f);
            _collisionOffset = new Vec2(-2f, -2f);
            _collisionSize = new Vec2(4f, 4f);
            _skipPlatforms = true;
            weight = 0.1f;
            _impactThreshold = 999f;
        }
    }
}

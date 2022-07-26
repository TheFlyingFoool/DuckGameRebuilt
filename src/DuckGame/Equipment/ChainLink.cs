// Decompiled with JetBrains decompiler
// Type: DuckGame.ChainLink
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ChainLink : PhysicsObject
    {
        public ChainLink(float xpos, float ypos)
        {
            this.graphic = new Sprite("chainLink");
            this.center = new Vec2(3f, 2f);
            this._collisionOffset = new Vec2(-2f, -2f);
            this._collisionSize = new Vec2(4f, 4f);
            this._skipPlatforms = true;
            this.weight = 0.1f;
            this._impactThreshold = 999f;
        }
    }
}

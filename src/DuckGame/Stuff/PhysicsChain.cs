// Decompiled with JetBrains decompiler
// Type: DuckGame.PhysicsChain
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Ropes")]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class PhysicsChain : PhysicsRope
    {
        public PhysicsChain(float xpos, float ypos, PhysicsRope next = null)
          : base(xpos, ypos)
        {
            this.chain = true;
            this._vine = new SpriteMap("chain", 16, 16);
            this.graphic = this._vine;
            this.center = new Vec2(8f, 8f);
            this._vineEnd = new Sprite("chainStretchEnd")
            {
                center = new Vec2(8f, 0.0f)
            };
            this.collisionOffset = new Vec2(-5f, -4f);
            this.collisionSize = new Vec2(11f, 7f);
            this.graphic = this._vine;
            this._beam = new Sprite("chainStretch");
            this._editorName = "Chain";
            this.editorTooltip = "It's like a metal rope! Great for swinging through a factory.";
        }

        public override Vine GetSection(float x, float y, int div) => new ChainPart(x, y, div);
    }
}

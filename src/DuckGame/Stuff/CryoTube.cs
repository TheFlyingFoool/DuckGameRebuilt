// Decompiled with JetBrains decompiler
// Type: DuckGame.CryoTube
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("survival")]
    [BaggedProperty("isOnlineCapable", false)]
    public class CryoTube : Thing
    {
        private CryoPlug _plug;

        public CryoTube(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("survival/cryoTube");
            this.center = new Vec2(16f, 15f);
            this._collisionSize = new Vec2(18f, 32f);
            this._collisionOffset = new Vec2(-9f, -16f);
            this.depth = (Depth)0.9f;
            this.hugWalls = WallHug.Floor;
        }

        public override void Initialize()
        {
            this._plug = new CryoPlug(this.x - 20f, this.y);
            Level.Add((Thing)this._plug);
            this._plug.AttachTo((Thing)this);
        }

        public override void Terminate() => Level.Remove((Thing)this._plug);
    }
}

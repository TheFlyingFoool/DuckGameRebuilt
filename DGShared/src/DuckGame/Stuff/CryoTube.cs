// Decompiled with JetBrains decompiler
// Type: DuckGame.CryoTube
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            graphic = new Sprite("survival/cryoTube");
            center = new Vec2(16f, 15f);
            _collisionSize = new Vec2(18f, 32f);
            _collisionOffset = new Vec2(-9f, -16f);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Floor;
        }

        public override void Initialize()
        {
            _plug = new CryoPlug(x - 20f, y);
            Level.Add(_plug);
            _plug.AttachTo(this);
        }

        public override void Terminate() => Level.Remove(_plug);
    }
}

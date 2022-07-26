// Decompiled with JetBrains decompiler
// Type: DuckGame.CryoBackground
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("survival")]
    [BaggedProperty("isOnlineCapable", false)]
    public class CryoBackground : Thing
    {
        public CryoBackground(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("survival/cryoBackground");
            this.center = new Vec2((float)(this.graphic.w / 2), (float)(this.graphic.h / 2));
            this._collisionSize = new Vec2(32f, 32f);
            this._collisionOffset = new Vec2(-16f, -16f);
            this.depth = (Depth)0.9f;
            this.layer = Layer.Background;
        }
    }
}

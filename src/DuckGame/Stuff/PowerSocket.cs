// Decompiled with JetBrains decompiler
// Type: DuckGame.PowerSocket
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("survival")]
    [BaggedProperty("isOnlineCapable", false)]
    public class PowerSocket : Thing
    {
        public PowerSocket(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("survival/cryoSocket");
            this.center = new Vec2(8f, 8f);
            this._collisionSize = new Vec2(14f, 14f);
            this._collisionOffset = new Vec2(-7f, -7f);
            this.depth = - 0.9f;
        }
    }
}

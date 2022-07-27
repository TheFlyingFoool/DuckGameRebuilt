// Decompiled with JetBrains decompiler
// Type: DuckGame.Tombstone
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Props")]
    public class Tombstone : Holdable, IPlatform
    {
        private SpriteMap _sprite;

        public Tombstone(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("grave", 15, 16);
            this.graphic = _sprite;
            this.center = new Vec2(7f, 8f);
            this.collisionOffset = new Vec2(-7f, -8f);
            this.collisionSize = new Vec2(15f, 15f);
            this.depth = - 0.5f;
            this.thickness = 4f;
            this.weight = 7f;
            this.flammable = 0.0f;
            this.collideSounds.Add("rockHitGround2");
            this.editorTooltip = "The saddest rock.";
        }
    }
}

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
            _sprite = new SpriteMap("grave", 15, 16);
            graphic = _sprite;
            center = new Vec2(7f, 8f);
            collisionOffset = new Vec2(-7f, -8f);
            collisionSize = new Vec2(15f, 15f);
            depth = -0.5f;
            thickness = 4f;
            weight = 7f;
            flammable = 0f;
            collideSounds.Add("rockHitGround2");
            editorTooltip = "The saddest rock.";
        }
    }
}

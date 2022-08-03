// Decompiled with JetBrains decompiler
// Type: DuckGame.Key
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Doors")]
    [BaggedProperty("previewPriority", true)]
    public class Key : Holdable
    {
        private SpriteMap _sprite;

        public Key(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("key", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-7f, -4f);
            collisionSize = new Vec2(14f, 8f);
            depth = -0.5f;
            thickness = 1f;
            weight = 3f;
            flammable = 0f;
            collideSounds.Add("metalRebound");
            physicsMaterial = PhysicsMaterial.Metal;
            editorTooltip = "For opening locked doors. You've heard of keys before, right?";
            holsterAngle = 90f;
            coolingFactor = 1f / 1000f;
        }

        public override void Update()
        {
            _sprite.flipH = offDir < 0;
            if (owner != null)
                Level.CheckLine<Door>(position + new Vec2(-10f, 0f), position + new Vec2(10f, 0f))?.UnlockDoor(this);
            base.Update();
        }

        public override void Terminate()
        {
        }
    }
}

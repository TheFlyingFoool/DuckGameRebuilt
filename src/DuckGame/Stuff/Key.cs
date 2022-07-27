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
            this._sprite = new SpriteMap("key", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-7f, -4f);
            this.collisionSize = new Vec2(14f, 8f);
            this.depth = -0.5f;
            this.thickness = 1f;
            this.weight = 3f;
            this.flammable = 0.0f;
            this.collideSounds.Add("metalRebound");
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.editorTooltip = "For opening locked doors. You've heard of keys before, right?";
            this.holsterAngle = 90f;
            this.coolingFactor = 1f / 1000f;
        }

        public override void Update()
        {
            this._sprite.flipH = this.offDir < 0;
            if (this.owner != null)
                Level.CheckLine<Door>(this.position + new Vec2(-10f, 0.0f), this.position + new Vec2(10f, 0.0f))?.UnlockDoor(this);
            base.Update();
        }

        public override void Terminate()
        {
        }
    }
}

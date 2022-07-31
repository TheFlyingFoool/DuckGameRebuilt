// Decompiled with JetBrains decompiler
// Type: DuckGame.SpikesDown
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Spikes")]
    [BaggedProperty("isInDemo", false)]
    [BaggedProperty("previewPriority", false)]
    public class SpikesDown : Spikes
    {
        private SpriteMap _sprite;

        public SpikesDown(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("spikes", 16, 19)
            {
                speed = 0.1f
            };
            this.graphic = _sprite;
            this.center = new Vec2(8f, 14f);
            this.collisionOffset = new Vec2(-7f, -2f);
            this.collisionSize = new Vec2(13f, 5f);
            this._editorName = "Spikes Down";
            this.editorTooltip = "Pointy and dangerous.";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.editorCycleType = typeof(SpikesLeft);
            this.angle = 3.141593f;
            this.up = false;
            this.editorOffset = new Vec2(0f, -6f);
            this.hugWalls = WallHug.Ceiling;
            this._killImpact = ImpactedFrom.Bottom;
        }

        public override void Update() => base.Update();

        public override void Draw() => base.Draw();
    }
}

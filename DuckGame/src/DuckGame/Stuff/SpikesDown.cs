using System;

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
            _sprite = new SpriteMap("spikes", 16, 19)
            {
                speed = 0.1f
            };
            graphic = _sprite;
            center = new Vec2(8f, 14f);
            collisionOffset = new Vec2(-7f, -2f);
            collisionSize = new Vec2(13f, 5f);
            _editorName = "Spikes Down";
            editorTooltip = "Pointy and dangerous.";
            physicsMaterial = PhysicsMaterial.Metal;
            editorCycleType = typeof(SpikesLeft);
            angle = (float)Math.PI;
            up = false;
            editorOffset = new Vec2(0f, -6f);
            hugWalls = WallHug.Ceiling;
            _killImpact = ImpactedFrom.Bottom;
        }

        public override void Update() => base.Update();

        public override void Draw() => base.Draw();
    }
}

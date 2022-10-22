// Decompiled with JetBrains decompiler
// Type: DuckGame.SpikesDown
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            angle = 3.141593f;
            up = false;
            editorOffset = new Vec2(0f, -6f);
            hugWalls = WallHug.Ceiling;
            _killImpact = ImpactedFrom.Bottom;
        }

        public override void Update() => base.Update();

        public override void Draw() => base.Draw();
    }
}

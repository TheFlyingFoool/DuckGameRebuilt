// Decompiled with JetBrains decompiler
// Type: DuckGame.SpikesLeft
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Spikes")]
    [BaggedProperty("previewPriority", false)]
    public class SpikesLeft : Spikes
    {
        private SpriteMap _sprite;

        public SpikesLeft(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("spikes", 16, 19)
            {
                speed = 0.1f
            };
            graphic = _sprite;
            center = new Vec2(8f, 14f);
            collisionOffset = new Vec2(-3f, -7f);
            collisionSize = new Vec2(5f, 13f);
            _editorName = "Spikes Left";
            editorTooltip = "Pointy and dangerous.";
            physicsMaterial = PhysicsMaterial.Metal;
            editorCycleType = typeof(Spikes);
            angle = -1.570796f;
            up = false;
            editorOffset = new Vec2(6f, 0f);
            hugWalls = WallHug.Right;
            _killImpact = ImpactedFrom.Left;
        }

        public override void Update() => base.Update();

        public override void Draw() => base.Draw();
    }
}

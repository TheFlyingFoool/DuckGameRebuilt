// Decompiled with JetBrains decompiler
// Type: DuckGame.SpikesRight
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Stuff|Spikes")]
    [BaggedProperty("isInDemo", false)]
    [BaggedProperty("previewPriority", false)]
    public class SpikesRight : Spikes
    {
        private SpriteMap _sprite;

        public SpikesRight(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("spikes", 16, 19)
            {
                speed = 0.1f
            };
            graphic = _sprite;
            center = new Vec2(8f, 14f);
            collisionOffset = new Vec2(-2f, -6f);
            collisionSize = new Vec2(5f, 13f);
            _editorName = "Spikes Right";
            editorTooltip = "Pointy and dangerous.";
            physicsMaterial = PhysicsMaterial.Metal;
            editorCycleType = typeof(SpikesDown);
            angle = (float)Math.PI / 2.0f;
            up = false;
            editorOffset = new Vec2(-6f, 0f);
            hugWalls = WallHug.Left;
            _killImpact = ImpactedFrom.Right;
        }

        public override void Update() => base.Update();

        public override void Draw() => base.Draw();
    }
}

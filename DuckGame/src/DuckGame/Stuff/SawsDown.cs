using System;

namespace DuckGame
{
    [EditorGroup("Stuff|Spikes")]
    public class SawsDown : Saws
    {
        private SpriteMap _sprite;
        public new bool up = true;

        public SawsDown(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("movingSpikes", 16, 21)
            {
                speed = 0.3f
            };
            graphic = _sprite;
            center = new Vec2(8f, 14f);
            collisionOffset = new Vec2(-6f, -2f);
            collisionSize = new Vec2(12f, 4f);
            _editorName = "Saws Down";
            editorTooltip = "Deadly hazards, able to cut through even the strongest of boots";
            physicsMaterial = PhysicsMaterial.Metal;
            editorCycleType = typeof(SawsLeft);
            angle = (float)Math.PI;
            editorOffset = new Vec2(0f, -6f);
            hugWalls = WallHug.Ceiling;
            _editorImageCenter = true;
            impactThreshold = 0.01f;
        }

        public override void Touch(MaterialThing with)
        {
            if (with.destroyed)
                return;
            with.Destroy(new DTImpale(this));
            with.vSpeed = 3f;
        }

        public override void Update() => base.Update();

        public override void Draw() => base.Draw();
    }
}

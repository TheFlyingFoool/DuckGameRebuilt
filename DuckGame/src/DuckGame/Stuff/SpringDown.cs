namespace DuckGame
{
    [EditorGroup("Stuff|Springs")]
    [BaggedProperty("isInDemo", false)]
    [BaggedProperty("previewPriority", false)]
    public class SpringDown : Spring
    {
        public SpringDown(float xpos, float ypos)
          : base(xpos, ypos)
        {
            UpdateSprite();
            center = new Vec2(8f, 7f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(16f, 8f);
            depth = -0.5f;
            _editorName = "Spring Down";
            editorTooltip = "Can't reach a high platform or want to get somewhere fast? That's why we built springs.";
            physicsMaterial = PhysicsMaterial.Metal;
            editorCycleType = typeof(SpringDownLeft);
            angle = 3.14159f;
            hugWalls = WallHug.Ceiling;
        }

        public override void Touch(MaterialThing with)
        {
            if (with.isServerForObject && with.Sprung(this))
            {
                if (with.vSpeed < 12f * _mult)
                    with.vSpeed = 12f * _mult;
                if (with is Gun)
                    (with as Gun).PressAction();
                if (with is Duck)
                {
                    (with as Duck).jumping = false;
                    DoRumble(with as Duck);
                }
                with.lastHSpeed = with._hSpeed;
                with.lastVSpeed = with._vSpeed;
            }
            SpringUp();
        }

        public override void Draw() => base.Draw();
    }
}

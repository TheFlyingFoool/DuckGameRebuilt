namespace DuckGame
{
    [EditorGroup("Stuff|Springs")]
    [BaggedProperty("previewPriority", false)]
    public class SpringDownRight : SpringUpRight
    {
        public SpringDownRight(float xpos, float ypos)
          : base(xpos, ypos)
        {
            UpdateSprite();
            center = new Vec2(8f, 7f);
            collisionOffset = new Vec2(-8f, 0f);
            collisionSize = new Vec2(16f, 8f);
            depth = -0.5f;
            _editorName = "Spring DownRight";
            editorTooltip = "Can't reach a low platform or want to get falling fast? That's why we built (down) springs.";
            physicsMaterial = PhysicsMaterial.Metal;
            editorCycleType = typeof(SpringDown);
            angle = 2.35619f;
        }

        public override void Touch(MaterialThing with)
        {
            if (with.isServerForObject && with.Sprung(this))
            {
                if (with.vSpeed < 22f * _mult)
                    with.vSpeed = 22f * _mult;
                if (flipHorizontal)
                {
                    if (purple)
                    {
                        if (with.hSpeed > -7f)
                            with.hSpeed = -7f;
                    }
                    else if (with.hSpeed > -10f)
                        with.hSpeed = -10f;
                }
                else if (purple)
                {
                    if (with.hSpeed < 7f)
                        with.hSpeed = 7f;
                }
                else if (with.hSpeed < 10f)
                    with.hSpeed = 10f;
                if (with is Gun)
                    (with as Gun).PressAction();
                if (with is Duck)
                    (with as Duck).jumping = false;
                with.lastHSpeed = with._hSpeed;
                with.lastVSpeed = with._vSpeed;
            }
            SpringUp();
        }

        public override void UpdateAngle()
        {
            if (flipHorizontal)
                angle = 3.92699f;
            else
                angle = 2.35619f;
        }

        public override void Draw() => base.Draw();
    }
}

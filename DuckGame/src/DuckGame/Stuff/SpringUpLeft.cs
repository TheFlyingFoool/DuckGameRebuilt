namespace DuckGame
{
    [EditorGroup("Stuff|Springs")]
    [BaggedProperty("previewPriority", false)]
    public class SpringUpLeft : SpringUpRight
    {
        public SpringUpLeft(float xpos, float ypos)
          : base(xpos, ypos)
        {
            UpdateSprite();
            center = new Vec2(8f, 7f);
            collisionOffset = new Vec2(-8f, 0f);
            collisionSize = new Vec2(16f, 8f);
            depth = -0.5f;
            _editorName = "Spring UpLeft";
            editorTooltip = "Can't reach a high platform or want to get somewhere fast? That's why we built springs.";
            physicsMaterial = PhysicsMaterial.Metal;
            editorCycleType = typeof(Spring);
            angle = -0.785398f;
        }

        public override void Touch(MaterialThing with)
        {
            if (with.isServerForObject && with.Sprung(this))
            {
                if (with.vSpeed > -22f * _mult)
                    with.vSpeed = -22f * _mult;
                if (!flipHorizontal)
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
                {
                    (with as Duck).jumping = false;
                    DoRumble(with as Duck);
                }
                with.lastHSpeed = with._hSpeed;
                with.lastVSpeed = with._vSpeed;
            }
            SpringUp();
        }

        public override void UpdateAngle()
        {
            if (flipHorizontal)
                angle = 0.785398f;
            else
                angle = -0.785398f;
        }

        public override void Draw() => base.Draw();
    }
}

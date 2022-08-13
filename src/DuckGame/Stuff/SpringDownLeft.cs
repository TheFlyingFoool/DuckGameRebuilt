// Decompiled with JetBrains decompiler
// Type: DuckGame.SpringDownLeft
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Springs")]
    [BaggedProperty("previewPriority", false)]
    public class SpringDownLeft : SpringUpRight
    {
        public SpringDownLeft(float xpos, float ypos)
          : base(xpos, ypos)
        {
            UpdateSprite();
            center = new Vec2(8f, 7f);
            collisionOffset = new Vec2(-8f, 0f);
            collisionSize = new Vec2(16f, 8f);
            depth = -0.5f;
            _editorName = "Spring DownLeft";
            editorTooltip = "Can't reach a low platform or want to get falling fast? That's why we built (down) springs.";
            physicsMaterial = PhysicsMaterial.Metal;
            editorCycleType = typeof(SpringLeft);
            angleDegrees = 225f;
        }

        public override void Touch(MaterialThing with)
        {
            if (with.isServerForObject && with.Sprung(this))
            {
                if (with.vSpeed < 22.0 * _mult)
                    with.vSpeed = 22f * _mult;
                if (!flipHorizontal)
                {
                    if (purple)
                    {
                        if (with.hSpeed > -7.0)
                            with.hSpeed = -7f;
                    }
                    else if (with.hSpeed > -10.0)
                        with.hSpeed = -10f;
                }
                else if (purple)
                {
                    if (with.hSpeed < 7.0)
                        with.hSpeed = 7f;
                }
                else if (with.hSpeed < 10.0)
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
                angleDegrees = 135f;
            else
                angleDegrees = 225f;
        }

        public override void Draw() => base.Draw();
    }
}

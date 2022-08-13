// Decompiled with JetBrains decompiler
// Type: DuckGame.SpringUpRight
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Springs")]
    [BaggedProperty("previewPriority", false)]
    public class SpringUpRight : Spring
    {
        public SpringUpRight(float xpos, float ypos)
          : base(xpos, ypos)
        {
            UpdateSprite();
            center = new Vec2(8f, 7f);
            collisionOffset = new Vec2(-8f, 0f);
            collisionSize = new Vec2(16f, 8f);
            depth = -0.5f;
            _editorName = "Spring UpRight";
            editorTooltip = "Can't reach a high platform or want to get somewhere fast? That's why we built springs.";
            physicsMaterial = PhysicsMaterial.Metal;
            editorCycleType = typeof(SpringRight);
            angleDegrees = 45f;
        }

        protected override void UpdateSprite()
        {
            if (purple)
            {
                _sprite = new SpriteMap("springAnglePurple", 16, 20);
                _sprite.ClearAnimations();
                _sprite.AddAnimation("idle", 1f, false, new int[1]);
                _sprite.AddAnimation("spring", 4f, false, 1, 2, 1, 0);
                _sprite.SetAnimation("idle");
                _sprite.speed = 0.1f;
                graphic = _sprite;
            }
            else
            {
                _sprite = new SpriteMap("springAngle", 16, 20);
                _sprite.ClearAnimations();
                _sprite.AddAnimation("idle", 1f, false, new int[1]);
                _sprite.AddAnimation("spring", 4f, false, 1, 2, 1, 0);
                _sprite.SetAnimation("idle");
                _sprite.speed = 0.1f;
                graphic = _sprite;
            }
        }

        public override void Touch(MaterialThing with)
        {
            if (with.isServerForObject && with.Sprung(this))
            {
                if (with.vSpeed > -22.0 * _mult)
                    with.vSpeed = -22f * _mult;
                if (flipHorizontal)
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
                angleDegrees = -45f;
            else
                angleDegrees = 45f;
        }

        public override void Draw() => base.Draw();
    }
}

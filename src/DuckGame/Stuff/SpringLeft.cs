// Decompiled with JetBrains decompiler
// Type: DuckGame.SpringLeft
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Springs")]
    [BaggedProperty("isInDemo", false)]
    [BaggedProperty("previewPriority", false)]
    public class SpringLeft : Spring
    {
        public override bool flipHorizontal
        {
            get => _flipHorizontal;
            set
            {
                _flipHorizontal = value;
                offDir = _flipHorizontal ? (sbyte)-1 : (sbyte)1;
                if (!_flipHorizontal)
                {
                    center = new Vec2(8f, 7f);
                    collisionOffset = new Vec2(0f, -8f);
                    collisionSize = new Vec2(8f, 16f);
                    angleDegrees = -90f;
                    hugWalls = WallHug.Right;
                }
                else
                {
                    center = new Vec2(8f, 7f);
                    collisionOffset = new Vec2(-8f, -8f);
                    collisionSize = new Vec2(8f, 16f);
                    angleDegrees = 90f;
                    hugWalls = WallHug.Left;
                }
            }
        }

        public SpringLeft(float xpos, float ypos)
          : base(xpos, ypos)
        {
            UpdateSprite();
            center = new Vec2(8f, 7f);
            collisionOffset = new Vec2(0f, -8f);
            collisionSize = new Vec2(8f, 16f);
            depth = -0.5f;
            _editorName = "Spring Left";
            editorTooltip = "Can't reach a high platform or want to get somewhere fast? That's why we built springs.";
            physicsMaterial = PhysicsMaterial.Metal;
            editorCycleType = typeof(SpringUpLeft);
            angleDegrees = -90f;
            hugWalls = WallHug.Right;
        }

        public override void Touch(MaterialThing with)
        {
            if (with.isServerForObject && with.Sprung(this))
            {
                if (!_flipHorizontal)
                {
                    if (with.hSpeed > -12.0 * _mult)
                        with.hSpeed = -12f * _mult;
                }
                else if (with.hSpeed < 12.0 * _mult)
                    with.hSpeed = 12f * _mult;
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

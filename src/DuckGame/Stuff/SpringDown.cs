// Decompiled with JetBrains decompiler
// Type: DuckGame.SpringDown
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            this.UpdateSprite();
            this.center = new Vec2(8f, 7f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 8f);
            this.depth = -0.5f;
            this._editorName = "Spring Down";
            this.editorTooltip = "Can't reach a high platform or want to get somewhere fast? That's why we built springs.";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.editorCycleType = typeof(SpringDownLeft);
            this.angleDegrees = 180f;
            this.hugWalls = WallHug.Ceiling;
        }

        public override void Touch(MaterialThing with)
        {
            if (with.isServerForObject && with.Sprung(this))
            {
                if (with.vSpeed < 12.0 * _mult)
                    with.vSpeed = 12f * this._mult;
                if (with is Gun)
                    (with as Gun).PressAction();
                if (with is Duck)
                {
                    (with as Duck).jumping = false;
                    this.DoRumble(with as Duck);
                }
                with.lastHSpeed = with._hSpeed;
                with.lastVSpeed = with._vSpeed;
            }
            this.SpringUp();
        }

        public override void Draw() => base.Draw();
    }
}

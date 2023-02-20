// Decompiled with JetBrains decompiler
// Type: DuckGame.WireButtonTop
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [BaggedProperty("isOnlineCapable", true)]
    public class WireButtonTop : MaterialThing
    {
        private WireButton _button;
        private int _orientation;

        public WireButtonTop(float xpos, float ypos, WireButton b, int orientation)
          : base(xpos, ypos)
        {
            _button = b;
            _orientation = orientation;
            switch (orientation)
            {
                case 0:
                    collisionSize = new Vec2(12f, 4f);
                    collisionOffset = new Vec2(-6f, -2f);
                    break;
                case 1:
                    collisionSize = new Vec2(4f, 12f);
                    collisionOffset = new Vec2(-2f, -6f);
                    break;
                case 2:
                    collisionSize = new Vec2(12f, 4f);
                    collisionOffset = new Vec2(-6f, -2f);
                    break;
                case 3:
                    collisionSize = new Vec2(4f, 12f);
                    collisionOffset = new Vec2(-2f, -6f);
                    break;
            }
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is PhysicsObject && !(with is TeamHat))
            {
                if (_orientation == 0 && with.vSpeed > -0.1f)
                    _button.ButtonPressed(with as PhysicsObject);
                else if (_orientation == 1 && with.hSpeed < 0.1f)
                    _button.ButtonPressed(with as PhysicsObject);
                else if (_orientation == 2 && with.vSpeed < 0.1f)
                    _button.ButtonPressed(with as PhysicsObject);
                else if (_orientation == 3 && with.hSpeed > -0.1f)
                    _button.ButtonPressed(with as PhysicsObject);
            }
            base.OnSoftImpact(with, from);
        }
    }
    [ClientOnly]
    public class WireButtonTop2 : MaterialThing
    {
        private CodeButton _button;
        private int _orientation;

        public WireButtonTop2(float xpos, float ypos, CodeButton b, int orientation)
          : base(xpos, ypos)
        {
            _button = b;
            _orientation = orientation;
            switch (orientation)
            {
                case 0:
                    collisionSize = new Vec2(12f, 4f);
                    collisionOffset = new Vec2(-6f, -2f);
                    break;
                case 1:
                    collisionSize = new Vec2(4f, 12f);
                    collisionOffset = new Vec2(-2f, -6f);
                    break;
                case 2:
                    collisionSize = new Vec2(12f, 4f);
                    collisionOffset = new Vec2(-6f, -2f);
                    break;
                case 3:
                    collisionSize = new Vec2(4f, 12f);
                    collisionOffset = new Vec2(-2f, -6f);
                    break;
            }
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is PhysicsObject && !(with is TeamHat))
            {
                if (_orientation == 0 && with.vSpeed > -0.1f)
                    _button.ButtonPressed(with as PhysicsObject);
                else if (_orientation == 1 && with.hSpeed < 0.1f)
                    _button.ButtonPressed(with as PhysicsObject);
                else if (_orientation == 2 && with.vSpeed < 0.1f)
                    _button.ButtonPressed(with as PhysicsObject);
                else if (_orientation == 3 && with.hSpeed > -0.1f)
                    _button.ButtonPressed(with as PhysicsObject);
            }
            base.OnSoftImpact(with, from);
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.WireButtonTop
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._button = b;
            this._orientation = orientation;
            switch (orientation)
            {
                case 0:
                    this.collisionSize = new Vec2(12f, 4f);
                    this.collisionOffset = new Vec2(-6f, -2f);
                    break;
                case 1:
                    this.collisionSize = new Vec2(4f, 12f);
                    this.collisionOffset = new Vec2(-2f, -6f);
                    break;
                case 2:
                    this.collisionSize = new Vec2(12f, 4f);
                    this.collisionOffset = new Vec2(-6f, -2f);
                    break;
                case 3:
                    this.collisionSize = new Vec2(4f, 12f);
                    this.collisionOffset = new Vec2(-2f, -6f);
                    break;
            }
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is PhysicsObject && !(with is TeamHat))
            {
                if (this._orientation == 0 && with.vSpeed > -0.100000001490116)
                    this._button.ButtonPressed(with as PhysicsObject);
                else if (this._orientation == 1 && with.hSpeed < 0.100000001490116)
                    this._button.ButtonPressed(with as PhysicsObject);
                else if (this._orientation == 2 && with.vSpeed < 0.100000001490116)
                    this._button.ButtonPressed(with as PhysicsObject);
                else if (this._orientation == 3 && with.hSpeed > -0.100000001490116)
                    this._button.ButtonPressed(with as PhysicsObject);
            }
            base.OnSoftImpact(with, from);
        }
    }
}

namespace DuckGame
{
    public class TapedSword : Sword
    {
        public TapedSword(float xval, float yval)
          : base(xval, yval)
        {
            graphic = new Sprite("tapedSword");
            center = new Vec2(4f, 44f);
            collisionOffset = new Vec2(-2f, -16f);
            collisionSize = new Vec2(4f, 18f);
            centerHeld = new Vec2(4f, 44f);
            centerUnheld = new Vec2(4f, 22f);
        }

        public override bool CanTapeTo(Thing pThing)
        {
            switch (pThing)
            {
                case Sword _:
                case EnergyScimitar _:
                    return false;
                default:
                    return true;
            }
        }
    }
}

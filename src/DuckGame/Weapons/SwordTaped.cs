// Decompiled with JetBrains decompiler
// Type: DuckGame.TapedSword
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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

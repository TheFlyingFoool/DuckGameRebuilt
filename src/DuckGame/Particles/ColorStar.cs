// Decompiled with JetBrains decompiler
// Type: DuckGame.ColorStar
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ColorStar : PhysicsParticle, IFactory
    {
        private float maxSize;

        public ColorStar(float xpos, float ypos, Vec2 dir, Color pColor)
          : base(xpos, ypos)
        {
            graphic = new Sprite("colorStar");
            graphic.CenterOrigin();
            center = new Vec2(graphic.width / 2, graphic.height / 2);
            xscale = yscale = 0.9f;
            hSpeed = dir.x;
            vSpeed = dir.y;
            maxSize = 0.1f;
            graphic.color = pColor;
            _gravMult = 3f;
        }

        public override void Update()
        {
            xscale = Lerp.Float(xscale, maxSize, 0.04f);
            yscale = xscale;
            if (xscale <= maxSize)
                Level.Remove(this);
            base.Update();
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.DizzyStar
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DizzyStar : PhysicsParticle, IFactory
    {
        private float maxSize;

        public DizzyStar(float xpos, float ypos, Vec2 dir)
          : base(xpos, ypos)
        {
            graphic = new Sprite("dizzyStar");
            graphic.CenterOrigin();
            xscale = yscale = Rando.Float(0.7f, 1.3f);
            hSpeed = dir.x;
            vSpeed = dir.y;
            maxSize = 0.1f;
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

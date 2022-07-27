// Decompiled with JetBrains decompiler
// Type: DuckGame.DizzyStar
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.graphic = new Sprite("dizzyStar");
            this.graphic.CenterOrigin();
            this.xscale = this.yscale = Rando.Float(0.7f, 1.3f);
            this.hSpeed = dir.x;
            this.vSpeed = dir.y;
            this.maxSize = 0.1f;
        }

        public override void Update()
        {
            this.xscale = Lerp.Float(this.xscale, this.maxSize, 0.04f);
            this.yscale = this.xscale;
            if ((double)this.xscale <= maxSize)
                Level.Remove(this);
            base.Update();
        }
    }
}

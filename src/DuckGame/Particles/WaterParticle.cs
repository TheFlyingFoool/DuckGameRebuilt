// Decompiled with JetBrains decompiler
// Type: DuckGame.WaterParticle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class WaterParticle : Thing
    {
        public WaterParticle(float xpos, float ypos, Vec2 hitAngle)
          : base(xpos, ypos)
        {
            this.hSpeed = (float)(-(double)hitAngle.x * 2.0 * ((double)Rando.Float(1f) + 0.300000011920929));
        }

        public override void Update()
        {
            this.vSpeed += 0.1f;
            this.hSpeed *= 0.9f;
            this.x += this.hSpeed;
            this.y += this.vSpeed;
            this.alpha -= 0.06f;
            if ((double)this.alpha < 0.0)
                Level.Remove((Thing)this);
            base.Update();
        }

        public override void Draw() => Graphics.DrawRect(this.position, this.position + new Vec2(1f, 1f), Color.LightBlue * this.alpha, this.depth);
    }
}

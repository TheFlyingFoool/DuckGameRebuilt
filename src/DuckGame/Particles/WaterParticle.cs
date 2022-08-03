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
            hSpeed = (float)(-hitAngle.x * 2.0 * (Rando.Float(1f) + 0.300000011920929));
        }

        public override void Update()
        {
            vSpeed += 0.1f;
            hSpeed *= 0.9f;
            x += hSpeed;
            y += vSpeed;
            alpha -= 0.06f;
            if (alpha < 0.0)
                Level.Remove(this);
            base.Update();
        }

        public override void Draw() => Graphics.DrawRect(position, position + new Vec2(1f, 1f), Color.LightBlue * alpha, depth);
    }
}

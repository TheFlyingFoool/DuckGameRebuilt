// Decompiled with JetBrains decompiler
// Type: DuckGame.SnowParticle
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class SnowParticle : WeatherParticle
    {
        public SnowParticle(Vec2 pos)
          : base(pos)
        {
            velocity = new Vec2(Rando.Float(-0.5f, 0.5f), Rando.Float(0.5f, 1f));
            zSpeed = Rando.Float(-0.1f, 0.1f);
        }

        public override void Draw()
        {
            Vec2 position = this.position;
            Vec3 vec3 = (Vec3)new Viewport(0, 0, (int)Layer.HUD.width, (int)Layer.HUD.height).Project((Vector3)new Vec3(position.x, z, position.y), (Microsoft.Xna.Framework.Matrix)Layer.Game.projection, (Microsoft.Xna.Framework.Matrix)Layer.Game.view, (Microsoft.Xna.Framework.Matrix)Matrix.Identity);
            this.position = new Vec2(vec3.x, vec3.y);
            float num1 = z / 200f;
            float num2 = (float)(0.3f + num1 * 0.3f);
            Graphics.DrawRect(this.position + new Vec2(-num2, -num2), this.position + new Vec2(num2, num2), Color.White * alpha, (Depth)(float)(num1 * 0.1f - 0.02f));
            this.position = position;
        }

        public override void Update()
        {
            if (!die)
            {
                position += velocity;
                z += zSpeed;
            }
            else
                alpha -= 0.04f;
        }
    }
}

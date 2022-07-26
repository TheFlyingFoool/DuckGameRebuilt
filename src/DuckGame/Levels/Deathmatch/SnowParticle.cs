// Decompiled with JetBrains decompiler
// Type: DuckGame.SnowParticle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.velocity = new Vec2(Rando.Float(-0.5f, 0.5f), Rando.Float(0.5f, 1f));
            this.zSpeed = Rando.Float(-0.1f, 0.1f);
        }

        public override void Draw()
        {
            Vec2 position = this.position;
            Vec3 vec3 = (Vec3)new Viewport(0, 0, (int)Layer.HUD.width, (int)Layer.HUD.height).Project((Vector3)new Vec3(position.x, this.z, position.y), (Microsoft.Xna.Framework.Matrix)Layer.Game.projection, (Microsoft.Xna.Framework.Matrix)Layer.Game.view, (Microsoft.Xna.Framework.Matrix)Matrix.Identity);
            this.position = new Vec2(vec3.x, vec3.y);
            float num1 = this.z / 200f;
            float num2 = (float)(0.300000011920929 + (double)num1 * 0.300000011920929);
            DuckGame.Graphics.DrawRect(this.position + new Vec2(-num2, -num2), this.position + new Vec2(num2, num2), Color.White * this.alpha, (Depth)(float)((double)num1 * 0.100000001490116 - 0.0199999995529652));
            this.position = position;
        }

        public override void Update()
        {
            if (!this.die)
            {
                this.position = this.position + this.velocity;
                this.z += this.zSpeed;
            }
            else
                this.alpha -= 0.04f;
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.RainParticle
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class RainParticle : WeatherParticle
    {
        public static SpriteMap splash;
        private Vec2 _prevPos;
        private float _frame;

        public RainParticle(Vec2 pos)
          : base(pos)
        {
            velocity = new Vec2(Rando.Float(-1.2f, -1.4f), Rando.Float(3.3f, 4f));
            zSpeed = Rando.Float(-0.1f, 0.1f);
        }

        public override void Draw()
        {
            if (die)
            {
                int num1 = (int)_frame;
                if (num1 > 3)
                    num1 = 3;
                splash.frame = num1;
                _frame += 0.3f;
                if (_frame >= 3.9f)
                    alpha = 0f;
                Vec2 position = this.position;
                Vec3 vec3 = new Viewport(0, 0, (int)Layer.HUD.width, (int)Layer.HUD.height).Project((Vector3)new Vec3(position.x, z, position.y), (Microsoft.Xna.Framework.Matrix)Layer.Game.projection, (Microsoft.Xna.Framework.Matrix)Layer.Game.view, (Microsoft.Xna.Framework.Matrix)Matrix.Identity);
                this.position = new Vec2(vec3.x, vec3.y);
                float num2 = z / 200f;
                splash.depth = -0.02f + num2 * 0.1f;
                splash.color = Color.White * 0.8f;
                Graphics.Draw(splash, this.position.x - 6f, this.position.y - 6f);
                this.position = position;
            }
            else
            {
                Vec2 position = this.position;
                Vec3 vec3 = (Vec3)new Viewport(0, 0, (int)Layer.HUD.width, (int)Layer.HUD.height).Project((Vector3)new Vec3(position.x, z, position.y), (Microsoft.Xna.Framework.Matrix)Layer.Game.projection, (Microsoft.Xna.Framework.Matrix)Layer.Game.view, (Microsoft.Xna.Framework.Matrix)Matrix.Identity);
                this.position = new Vec2(vec3.x, vec3.y);
                float num = z / 200f;
                Graphics.DrawLine(this.position, this._prevPos, Color.White * 0.8f, 1f, -0.02f + num * 0.1f);
                _prevPos = this.position;
                this.position = position;
            }
        }

        public override void Update()
        {
            if (!die)
            {
                position += velocity;
                z += zSpeed;
            }
            else
                alpha -= 0.01f;
        }
    }
}

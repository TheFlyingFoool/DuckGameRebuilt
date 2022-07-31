// Decompiled with JetBrains decompiler
// Type: DuckGame.RainParticle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.velocity = new Vec2(Rando.Float(-1.2f, -1.4f), Rando.Float(3.3f, 4f));
            this.zSpeed = Rando.Float(-0.1f, 0.1f);
        }

        public override void Draw()
        {
            if (this.die)
            {
                int num1 = (int)this._frame;
                if (num1 > 3)
                    num1 = 3;
                RainParticle.splash.frame = num1;
                this._frame += 0.3f;
                if (_frame >= 3.90000009536743)
                    this.alpha = 0f;
                Vec2 position = this.position;
                Vec3 vec3 = (Vec3)new Viewport(0, 0, (int)Layer.HUD.width, (int)Layer.HUD.height).Project((Vector3)new Vec3(position.x, this.z, position.y), (Microsoft.Xna.Framework.Matrix)Layer.Game.projection, (Microsoft.Xna.Framework.Matrix)Layer.Game.view, (Microsoft.Xna.Framework.Matrix)Matrix.Identity);
                this.position = new Vec2(vec3.x, vec3.y);
                float num2 = this.z / 200f;
                RainParticle.splash.depth = (Depth)(float)(num2 * 0.100000001490116 - 0.0199999995529652);
                RainParticle.splash.color = Color.White * 0.8f;
                DuckGame.Graphics.Draw(splash, this.position.x - 6f, this.position.y - 6f);
                this.position = position;
            }
            else
            {
                Vec2 position = this.position;
                Vec3 vec3 = (Vec3)new Viewport(0, 0, (int)Layer.HUD.width, (int)Layer.HUD.height).Project((Vector3)new Vec3(position.x, this.z, position.y), (Microsoft.Xna.Framework.Matrix)Layer.Game.projection, (Microsoft.Xna.Framework.Matrix)Layer.Game.view, (Microsoft.Xna.Framework.Matrix)Matrix.Identity);
                this.position = new Vec2(vec3.x, vec3.y);
                float num = this.z / 200f;
                DuckGame.Graphics.DrawLine(this.position, this._prevPos, Color.White * 0.8f, depth: ((Depth)(float)(num * 0.100000001490116 - 0.0199999995529652)));
                this._prevPos = this.position;
                this.position = position;
            }
        }

        public override void Update()
        {
            if (!this.die)
            {
                this.position += this.velocity;
                this.z += this.zSpeed;
            }
            else
                this.alpha -= 0.01f;
        }
    }
}

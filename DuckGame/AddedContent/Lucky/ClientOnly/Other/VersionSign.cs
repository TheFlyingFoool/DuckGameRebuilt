using System.Diagnostics;
using System.Web.Configuration;

namespace DuckGame
{
    public class VersionSign : Thing
    {
        public VersionSign(float xpos, float ypos) : base(xpos, ypos)
        {
            lined = Content.Load<Tex2D>("vLine");
            vend = new Sprite("vEnd");
            sprite = new SpriteMap("vMachiner", 45, 43);
            graphic = sprite;
            depth = 0.9f;
        }
        public SpriteMap sprite;
        public Tex2D lined;
        public Sprite vend;
        public int vis;
        public int fadeTime = -1;
        public bool go;
        public bool broke;
        public float angspeed;
        public bool hit;
        public override void Update()
        {//53
            if (broke)
            {
                vSpeed += 0.2f;
                y += vSpeed;
                if (y > 44)
                {
                    if (!hit)
                    {
                        vSpeed += 1;
                        hit = true;
                        angspeed += Rando.Float(1, 1.4f) * Rando.ChooseInt(-1, 1);
                    }
                    y = 44;
                    vSpeed *= -0.7f;
                }
                angle += angspeed / 10;
                if (angle > 0.1f) angspeed -= 0.1f;
                else if (angle < -0.1f) angspeed += 0.1f;
                else
                {
                    angspeed = Lerp.Float(angspeed, 0, 0.03f);
                    angle = Lerp.Float(angle, 0, 0.01f);
                }
            }
            if (Network.isActive) Level.Remove(this);
            if (fadeTime > 0)
            {
                fadeTime--;
                if (fadeTime == 0)
                {
                    go = true;
                }
            }
            if (go)
            {
                y--;
                fadeTime--;
                if (fadeTime < -30) Level.Remove(this);
            }
            if (Debugger.IsAttached)
            {
                if (DGRSettings.ActualParticleMultiplier == 0) return;
                if (Rando.Float(20 / DGRSettings.ActualParticleMultiplier) <= 1)
                {
                    Vec2 v = new Vec2(x + 24, y + 28) - center;
                    v = v.Rotate(angle, position);
                    Level.Add(Spark.New(v.x, v.y, new Vec2(Rando.Float(-1, 1), 0)));
                }
            }
        }
        public override void Draw()
        {
            if (y > 36 && broke)
            {
                Graphics.DrawLine(position, Offset(new Vec2(0,9)), Color.Red, 1);
                Graphics.DrawLine(position - new Vec2(1,0), Offset(new Vec2(-1,9)), Color.Green, 1);
                Graphics.DrawLine(position + new Vec2(1,0), Offset(new Vec2(1,9)), Color.Blue, 1);
            }
            Color c = Color.White;
            string s = Program.CURRENT_VERSION_ID.Replace("-beta","b");
            if (broke)
            {
                s = "";
                sprite.imageIndex = 1;
            }
            else if (Debugger.IsAttached)
            {
                s = "DEBUG";
                c = Color.Green * Rando.Float(0.3f, 1f);
                Graphics.DrawString(Program.CURRENT_VERSION_ID, position + new Vec2(9, 38f) - center, c, depth + 3, null, 0.35f);
            }
            else if (Program.IS_DEV_BUILD)
            {
                s = "DEV♥VER";
                c = Color.DeepPink;
                Graphics.DrawString(Program.CURRENT_VERSION_ID, position + new Vec2(9, 38) - center, c, depth + 3, null, 0.35f);
            }
            Vec2 v = new Vec2(x + 8, y + 35) - center;

            float xs = s.Length * 6f;
            if (broke) xs = 35;

            vend.angle = angle;
            if (broke) Graphics.DrawTexturedLine(lined, v.Rotate(angle, position), (v + new Vec2(xs, 0)).Rotate(angle, position), Color.White, 1, depth - 1);
            else Graphics.DrawTexturedLine(lined, v.Rotate(angle, position), (v + new Vec2(xs, 0)).Rotate(angle, position), Color.White, 1, depth + 1);

            Vec2 v2 = (new Vec2(v.x + xs, y + 27 - center.y)).Rotate(angle, position);
            Graphics.Draw(vend, v2.x, v2.y, depth + 2);

            Graphics.DrawString(s, position + new Vec2(9, 32.5f) - center, c, depth + 3, null, 0.7f);
            base.Draw();
        }
    }
}

using System.Diagnostics;

namespace DuckGame
{
    public class VersionSign : Thing
    {
        public VersionSign(float xpos, float ypos) : base(xpos, ypos)
        {
            lined = Content.Load<Tex2D>("vLine");
            vend = new Sprite("vEnd");
            graphic = new Sprite("vMachiner");
            depth = 0.9f;
        }
        public Tex2D lined;
        public Sprite vend;
        public int vis;
        public int fadeTime = -1;
        public bool go;
        public override void Update()
        {
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
                    Level.Add(Spark.New(x + 24, y + 28, new Vec2(Rando.Float(-1, 1), 0)));
                }
            }
        }
        public override void Draw()
        {
            Color c = Color.White;
            string s = Program.currentversion.Replace("-beta","b");
            if (Debugger.IsAttached)
            {
                s = "DEBUG";
                c = Color.Green * Rando.Float(0.3f, 1f);
            }
            else if (Program.currentversion == "")
            {
                s = "DEV♥VER";
                c = Color.DeepPink;
            }
            Vec2 v = new Vec2(x + 8, y + 35);

            float xs = s.Length * 6f;

            Graphics.DrawTexturedLine(lined, v, v + new Vec2(xs, 0), Color.White, 1, depth + 1);
            Graphics.Draw(vend, v.x + xs, y + 27, depth + 2);

            Graphics.DrawString(s, position + new Vec2(9, 32.5f), c, depth + 3, null, 0.7f);
            base.Draw();
        }
    }
}

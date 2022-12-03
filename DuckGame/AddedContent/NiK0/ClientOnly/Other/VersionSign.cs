using Microsoft.Xna.Framework.Graphics;
using NAudio.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public class VersionSign : Thing
    {
        public VersionSign(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new Sprite("vMachiner");
            depth = 0.9f;
        }
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
            if (Debugger.IsAttached)
            {
                Graphics.DrawString("DEBUG", position + new Vec2(9, 32), Color.Green * Rando.Float(0.3f, 1f), depth + 1, null, 0.85f);
            }
            else if (Program.currentversion == "")
            {
                Graphics.DrawString("DEV♥VER", position + new Vec2(9, 33), Color.DeepPink, depth + 1, null, 0.6f);
            }
            else
            {
                Graphics.DrawString("v" + Program.currentversion, position + new Vec2(9, 32), Color.White, depth + 1, null, 0.7f);
            }
            base.Draw();
        }
    }
}

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DuckGame
{
    public class RainParticel : Thing
    {
        public RainParticel(Vec2 v)
        {
            lPos = v;
            pos = v;
            rY = Rando.Float(5, 7);
            rX = Rando.Float(1, 2);
            c = new Color(0, 112, 168);
            c.a = (byte)Rando.Int(127, 255);
        }
        public override void Update()
        {
            pos.x += rX;
            pos.y += rY;
            Block b = Level.CheckPoint<Block>(pos);
            if (b != null)
            {
                Level.Add(new WaterSplash(pos.x, b.top, Fluid.Water));
                Level.Remove(this);
            }
            else if (pos.y > Level.current.bottomRight.y + 200) Level.Remove(this);
        }
        public override void Draw()
        {
            Graphics.DrawLine(lPos, pos, c, 1.6f, 1.1f);
            lPos = pos;
        }
        public Color c;
        public float rY;
        public float rX;
        public Vec2 lPos;
        public Vec2 pos;
    }
}

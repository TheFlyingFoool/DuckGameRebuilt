using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public class WarpBeam : Thing
    {
        public WarpBeam(float xpos, float ypos) : base(xpos, ypos)
        {

        }
        public Vec2 end;
        public float wwidth;
        public override void Draw()
        {
            if (MonoMain.UpdateLerpState) wwidth -= 0.5f;
            Graphics.DrawLine(position, end, new Color(83, 14, 144) * alpha, wwidth, -1);
        }
        public override void Update()
        {
            alpha -= 0.05f;
            if (alpha <= 0 || wwidth <= 0)
            {
                Level.Remove(this);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public class WarpSlash : Thing
    {
        public WarpSlash()
        {
            graphic = new Sprite("warpFade");
            center = new Vec2(10.5f, 24);
            layer = Layer.Foreground;
            mater = new MaterialEnergy(this);
            mater.timeMulti = 2.5f;
        }
        public MaterialEnergy mater;
        public int killFrames;
        public Color color;
        public float alphMult = 1;
        public Duck ignore;
        public override void Update()
        {
            if (isServerForObject)
            {
                position += velocity;
                velocity = Lerp.Vec2(velocity, Vec2.Zero, 0.03f);
                alpha -= 0.02f * alphMult;
                if (killFrames > 0)
                {
                    killFrames--;
                    foreach (MaterialThing mt in Level.CheckLineAll<MaterialThing>(position, position + (Maths.AngleToVec(angle - 1.7f) * 24 * yscale) * new Vec2(1, -1)))
                    {
                        if (Duck.GetAssociatedDuck(mt) == ignore) continue;
                        if (mt is Block) BlockBehind = true;
                        Fondle(mt);
                        mt.Destroy(new DTIncinerate(this));
                    }
                    killFrames = 0;
                }
                if (BlockBehind)
                {
                    alpha += 0.01f;
                    mater.Update();
                    mater.glow = 1 * alpha;
                }
                if (alpha <= 0)
                {
                    if (BlockBehind && alphMult == 1)
                    {
                        SFX.Play("swipe", 0.5f, -1.4f);
                    }
                    Level.Remove(this);
                }
            }
            base.Update();
        }
        public bool BlockBehind;
        public override void Draw()
        {
            if (DevConsole.showCollision)
            {
                Graphics.DrawLine(position, position + (Maths.AngleToVec(angle - 1.7f) * 24 * yscale) * new Vec2(1, -1), Color.Red, 4, 1);
            }
            graphic.color = color;
            graphic.flipH = offDir < 0;
            if (BlockBehind) Graphics.material = mater;
            base.Draw();
            Graphics.material = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    [ClientOnly]
    public class WarpSlash : Thing
    {
        public StateBinding _unspeedMultBinding = new StateBinding("unspeedMult");//unspeedMult
        public StateBinding _positionBinding = new StateBinding("position");
        public StateBinding _offDirBinding = new StateBinding("offDir");
        public StateBinding _alphaBinding = new StateBinding("alpha");
        public StateBinding _scaleBinding = new StateBinding("scale");
        public WarpSlash()
        {
            
            graphic = new Sprite("warpFade");
            center = new Vec2(10.5f, 24);
            layer = Layer.Foreground;
            mater = new MaterialEnergy(this);
            mater.timeMulti = 2.5f;
            color = new Color(147, 64, 221);
        }
        public MaterialEnergy mater;
        public int killFrames = 2;
        public Color color;
        public float alphMult = 1;
        public Duck ignore;
        public float unspeedMult = 1;
        public override void Terminate()
        {
            if (BlockBehind && alphMult == 1) SFX.Play("swipe", 0.5f, -1.4f);
            base.Terminate();
        }
        public override void Update()
        {
            if (isServerForObject)
            {
                position += velocity;
                velocity = Lerp.Vec2(velocity, Vec2.Zero, 0.03f * unspeedMult);
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
                }
                if (unspeedMult != 1) BlockBehind = false;
                if (BlockBehind)
                {
                    alpha += 0.01f;
                    mater.Update();
                    mater.glow = 1 * alpha;
                }
                if (alpha <= 0)
                {
                    Level.Remove(this);
                }
            }
            else
            {
                if (killFrames > 0)
                {
                    killFrames--;
                    foreach (MaterialThing mt in Level.CheckLineAll<MaterialThing>(position, position + (Maths.AngleToVec(angle - 1.7f) * 24 * yscale) * new Vec2(1, -1)))
                    {
                        if (mt is Block)
                        {
                            BlockBehind = true;
                            break;
                        }
                    }
                }
                if (unspeedMult != 1) BlockBehind = false;
                if (BlockBehind)
                {
                    alpha += 0.01f;
                    mater.Update();
                    mater.glow = 1 * alpha;
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

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public class GlassPlatform : Block
    {
        public SpriteMap sprite;
        public GlassPlatform(float xpos, float ypos) : base(xpos, ypos)
        {
            sprite = new SpriteMap("glassPlatform", 320, 240);
            graphic = sprite;
            collisionSize = new Vec2(115, 13);
            _collisionOffset = new Vec2(-58, 47);
            center = new Vec2(160, 120);
            mg = new MaterialSpace();
            mg.siz = 0.4f;
            mg.sizMult = 0.75f;
            mg.xS = 0.1f;
            mg.yS = 0.1f;
        }
        public MaterialSpace mg;
        public float addX;
        public float addY;
        public float ys;
        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            base.OnSoftImpact(with, from);
        }
        public override void Draw()
        {
            sprite.imageIndex = 0;
            base.Draw();
            Graphics.material = mg;
            sprite.imageIndex = 1;
            base.Draw();
            Graphics.material = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public class BGLightning : Thing, IDrawToDifferentLayers
    {
        public SpriteMap sprite;
        public BGLightning(float xpos, float ypos) : base(xpos, ypos)
        {
            startX = xpos;
            sprite = new SpriteMap("lightning", 80, 240);
            graphic = sprite;
            depth = 1;
            frame = Rando.Int(7);
            layer = Layer.Parallax;
            bg = Level.First<ParallaxBackground>();
        }
        public override void Update()
        {
            alpha -= 0.01f;
            if (alpha > 0.9f)
            {
                frame = Rando.Int(7);
            }
            if (alpha < 0) Level.Remove(this);
        }
        public float startX;
        public ParallaxBackground bg;
        public void OnDrawLayer(Layer l)
        {
            if (l == Layer.Parallax)
            {
                if (bg != null)
                {
                    y = bg.y;
                }
                base.Draw();
            }
        }
        public override void Draw()
        {
            
        }
    }
}

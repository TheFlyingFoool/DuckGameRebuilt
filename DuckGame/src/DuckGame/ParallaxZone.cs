using System.Collections.Generic;

namespace DuckGame
{
    public class ParallaxZone
    {
        public float distance;
        public float speed;
        public float scroll;
        public float wrapMul = 1f;
        public bool moving;
        public bool visible = true;
        private List<Sprite> _sprites = new List<Sprite>();
        private List<Thing> _things = new List<Thing>();
        private int _ypos;

        public ParallaxZone(float d, float s, bool m, bool vis = true, int ypos = 0)
        {
            distance = d;
            speed = s;
            moving = m;
            visible = vis;
            _ypos = 0;
        }

        public void Update(float mul)
        {
            if (moving)
                mul = 1f;
            scroll += (1f - distance) * speed * mul;
        }

        public void RenderSprites(Vec2 position)
        {
            float num = (0.4f + _ypos * 0.01f);
            foreach (Sprite sprite1 in _sprites)
            {
                Sprite sprite2 = sprite1;
                sprite2.position += position;
                sprite1.position.x += scroll;
                if (sprite1.position.x < -200f * wrapMul)
                    sprite1.position.x += 500f * wrapMul;
                if (sprite1.position.x > 450f * wrapMul)
                    sprite1.position.x -= 500f * wrapMul;
                sprite1.depth = (Depth)num;
                Graphics.Draw(sprite1, sprite1.x, sprite1.y);
                num += 1f / 1000f;
                sprite1.position.x -= scroll;
                Sprite sprite3 = sprite1;
                sprite3.position -= position;
            }
            foreach (Thing thing1 in _things)
            {
                Thing thing2 = thing1;
                thing2.position += position;
                thing1.position.x += scroll;
                if (thing1.position.x < -200f)
                    thing1.position.x += 500f;
                if (thing1.position.x > 450f)
                    thing1.position.x -= 500f;
                thing1.depth = (Depth)num;
                thing1.Update();
                thing1.Draw();
                thing1.position.x -= scroll;
                Thing thing3 = thing1;
                thing3.position -= position;
            }
        }

        public void AddSprite(Sprite s) => _sprites.Add(s);

        public void AddThing(Thing s) => _things.Add(s);
    }
}

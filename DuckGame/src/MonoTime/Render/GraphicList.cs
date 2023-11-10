using System.Collections.Generic;

namespace DuckGame
{
    public class GraphicList : Sprite
    {
        private List<Sprite> _objects;

        public override int width
        {
            get
            {
                float num1 = 0f;
                float num2 = 0f;
                foreach (Sprite sprite in _objects)
                {
                    if (sprite.x - sprite.centerx < num1)
                        num1 = sprite.x - sprite.centerx;
                    if (sprite.x - sprite.centerx + sprite.width > num2)
                        num2 = sprite.x - sprite.centerx + sprite.width;
                }
                return (int)(num2 - num1 + 0.5);
            }
        }

        public override int w => width;

        public override int height
        {
            get
            {
                float num1 = 0f;
                float num2 = 0f;
                foreach (Sprite sprite in _objects)
                {
                    if (sprite.y - sprite.centery < num1)
                        num1 = sprite.x - sprite.centery;
                    if (sprite.y - sprite.centery + sprite.height > num2)
                        num2 = sprite.y - sprite.centery + sprite.width;
                }
                return (int)(num2 - num1 + 0.5);
            }
        }

        public override int h => height;

        public GraphicList(List<Sprite> list) => _objects = list;

        public GraphicList() => _objects = new List<Sprite>();

        public void Add(Sprite graphic) => _objects.Add(graphic);

        public void Remove(Sprite graphic) => _objects.Remove(graphic);

        public override void Draw()
        {
            foreach (Sprite sprite1 in _objects)
            {
                Vec2 vec2_1 = new Vec2(sprite1.position);
                Sprite sprite2 = sprite1;
                sprite2.position -= center;
                sprite1.position.x *= xscale;
                sprite1.position.y *= yscale;
                Sprite sprite3 = sprite1;
                sprite3.position += position;
                float alpha = sprite1.alpha;
                sprite1.alpha *= this.alpha;
                Vec2 vec2_2 = new Vec2(sprite1.scale);
                sprite1.xscale *= xscale;
                sprite1.yscale *= yscale;
                float angle = sprite1.angle;
                sprite1.angle *= this.angle;
                bool flipH = sprite1.flipH;
                sprite1.flipH = this.flipH;
                sprite1.Draw();
                sprite1.angle = angle;
                sprite1.scale = vec2_2;
                sprite1.alpha = alpha;
                sprite1.position = vec2_1;
                sprite1.flipH = flipH;
            }
        }
    }
}

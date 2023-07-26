using System;
namespace DuckGame
{
    //most of the comments here were made by othello7 im just here to fix stuff up and improve upon it -NiK0
    public class Firework : Thing
    {
        private Color _color; // watch?v=FZFZl0ZaFSU
        private int _lifespan;
        private float _speed = 1;

        private static Color[] FireworkColors = new Color[] { Color.Violet, Color.SkyBlue, Color.Wheat, Color.GreenYellow, Color.Pink};
        public Firework(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("firework");
            layer = Layer.Console;
            depth = 1;
            _color = FireworkColors[Rando.Int(FireworkColors.Length - 1)];
            _lifespan = Rando.Int(20);
            scale = new Vec2(1.2f, 1.4f);
        }

        public override void Update()
        {
            y -= _speed;
            _speed += 0.2f; //accelerating upwards

            if (_lifespan > 52) //explode now
            {
                int sg = 2;
                if (Rando.Int(3) == 0) 
                    sg++;
                if (Rando.Int(9) == 0) 
                    sg++;

                if (Rando.Int(20) == 0)
                {
                    _color = Color.Pink;
                    for (int i = 1; i < sg; i++)
                    {
                        if (i == 2) 
                            _color = Color.Red;
                        else if (i == 3) 
                            _color = Color.Orange;
                        float radius = 6 + Rando.Float(6) * i + i * 4;

                        //i do not care. -NiK0
                        Level.Add(new FireworkCharm(x, y, _color, new Vec2(x - 3 * radius, y)));
                        Level.Add(new FireworkCharm(x, y, _color, new Vec2(x - 3 * radius, y - 1 * radius)));
                        Level.Add(new FireworkCharm(x, y, _color, new Vec2(x + 3 * radius, y)));
                        Level.Add(new FireworkCharm(x, y, _color, new Vec2(x + 3 * radius, y - 1 * radius)));

                        Level.Add(new FireworkCharm(x, y, _color, new Vec2(x + 2 * radius, y - 2 * radius)));
                        Level.Add(new FireworkCharm(x, y, _color, new Vec2(x + 1 * radius, y - 2 * radius)));
                        Level.Add(new FireworkCharm(x, y, _color, new Vec2(x - 2 * radius, y - 2 * radius)));
                        Level.Add(new FireworkCharm(x, y, _color, new Vec2(x - 1 * radius, y - 2 * radius)));
                        Level.Add(new FireworkCharm(x, y, _color, new Vec2(x, y - 1 * radius)));

                        Level.Add(new FireworkCharm(x, y, _color, new Vec2(x - 2 * radius, y + 1 * radius)));
                        Level.Add(new FireworkCharm(x, y, _color, new Vec2(x + 2 * radius, y + 1 * radius)));
                        Level.Add(new FireworkCharm(x, y, _color, new Vec2(x - 1 * radius, y + 2 * radius)));
                        Level.Add(new FireworkCharm(x, y, _color, new Vec2(x + 1 * radius, y + 2 * radius)));
                        Level.Add(new FireworkCharm(x, y, _color, new Vec2(x, y + 3 * radius)));
                    }
                }
                else
                {
                    for (int i = 1; i < sg; i++)
                    {
                        float radius = 1.4f + Rando.Float(0.8f) * i + (i);

                        for (int l = 0; l < 12; l++)
                        { //add all the different FireworkCharms evenly spaced (magic)
                            //othello for the love of god use fucking "for" statements not "while" -NiK0
                            float flyangle = (float)l / 12 * 6.28f + i;
                            Level.Add(new FireworkCharm(x, y, _color, flyangle, radius + Rando.Float(-0.05f, 0.1f)));
                        }
                    }
                }
                Level.Remove(this);
                DoTerminate();
            }
            _lifespan++;
        }
        private class FireworkCharm : Thing
        {
            private Color _color;
            private float _flyangle;
            private float _speed;

            public SpriteMap sprite;
            public Vec2 lerpPos;
            public FireworkCharm(float xpos, float ypos, Color color, float flyangle, float speed)
              : base(xpos, ypos)
            {
                sprite = new SpriteMap("cspark", 5, 5);
                sprite.AddAnimation("go", Rando.Float(0.05f, 0.1f), false, 0, 1, 2, 3);
                sprite.SetAnimation("go");
                graphic = sprite;
                graphic.color = color;
                center = new Vec2(2.5f);
                collisionSize = new Vec2(5);
                _collisionOffset = new Vec2(-2.5f);
                _color = color;
                layer = Layer.Console;
                _flyangle = flyangle;
                _speed = speed;
                scale = new Vec2(Rando.Float(1.3f, 1.6f));
            }
            public FireworkCharm(float xpos, float ypos, Color color, Vec2 lerp)
              : base(xpos, ypos)
            {
                lerpPos = lerp;
                sprite = new SpriteMap("cspark", 5, 5);
                sprite.AddAnimation("go", Rando.Float(0.05f, 0.08f), false, 0, 0, 1, 2, 3);
                sprite.SetAnimation("go");
                graphic = sprite;
                graphic.color = color;
                center = new Vec2(2.5f);
                collisionSize = new Vec2(5);
                _collisionOffset = new Vec2(-2.5f);
                _color = color;
                layer = Layer.Console;
                scale = new Vec2(Rando.Float(1.6f, 1.8f));
            }

            public override void Update()
            {
                y += 0.1f;

                if (lerpPos != Vec2.Zero) position = Lerp.Vec2Smooth(position, lerpPos, 0.1f);
                else
                {
                    if (_speed > 0f)
                    {
                        position.x += _speed * (float)Math.Cos(_flyangle);
                        position.y += _speed * (float)Math.Sin(_flyangle);
                    }
                    if (_speed > 0) _speed -= 0.1f;
                }

                if (sprite.finished)
                {
                    Level.Remove(this);
                    DoTerminate();
                }

                if (_speed <= 0) //nooo dont look at it
                {
                    subtractColor((255 / 15));
                    void subtractColor(byte val)
                    {//trust me it needs this to fade properly
                        if (_color.a - val >= 0) _color.a -= val;
                        else _color.a = 0;

                        if (_color.r - val >= 0) _color.r -= val;
                        else _color.r = 0;

                        if (_color.g - val >= 0) _color.g -= val;
                        else _color.g = 0;

                        if (_color.b - val >= 0) _color.b -= val;
                        else _color.b = 0;
                    }
                }
            }
        }
    }
}

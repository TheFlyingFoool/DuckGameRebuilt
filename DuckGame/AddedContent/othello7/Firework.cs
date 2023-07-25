using System;
namespace DuckGame
{
    public class Firework : Thing
    {
        private Color _color; // watch?v=FZFZl0ZaFSU
        private int _lifespan;
        private float _speed = 1;


        public Firework(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.layer = Layer.Foreground; //trolled
            int num = Rando.ChooseInt(0, 1, 2, 3); //add more colors if you want
            if (num == 0)
                _color = Color.Violet;
            if (num == 1)
                _color = Color.SkyBlue;
            if (num == 2)
                _color = Color.Wheat;
            if (num == 3)
                _color = Color.GreenYellow;
        }

        public override void Update()
        {
            y -= _speed;
            _speed += 0.2f; //accelerating upwards

            if (_lifespan > 20) //explode now
            {
                int l = 0;
                const int max = 12; //I think 12 looks nice because it's like a clock
                float radius = 1.4f + Rando.Float(0.8f);
                while (l < max)
                { //add all the different FireworkCharms evenly spaced (magic)
                    float flyangle = (float)l / max * 6.28f;
                    Level.Add(new FireworkCharm(x, y, _color, flyangle, radius));
                    l++;
                }
                Level.Remove(this);
                this.DoTerminate();
            }
            _lifespan++;
        }

        public override void Draw()
        {
            Color drawColor = Color.Red;
            drawColor.r = (byte)(255 / 20 * _lifespan); //why does it turn white without this?
            drawColor.a = (byte)(255 / 20 * _lifespan);
            Graphics.DrawRect(position, position + new Vec2(2f, 4f), drawColor, depth);
        }

        private class FireworkCharm : Thing
        {
            private Color _color;
            private int _lifespan = 0;
            private float _flyangle;
            private float _speed;


            public FireworkCharm(float xpos, float ypos, Color color, float flyangle, float speed)
              : base(xpos, ypos)
            {
                _color = color;
                this.layer = Layer.Foreground;
                _flyangle = flyangle;
                _speed = speed;
            }

            public override void Update()
            {
                y += 0.1f;

                if (_speed > 0f)
                {
                    position.x += _speed * (float)Math.Cos(_flyangle);
                    position.y += _speed * (float)Math.Sin(_flyangle);
                }
                if (_speed > 0)
                    _speed -= 0.1f;

                if (_lifespan > 15)
                {
                    Level.Remove(this);
                    this.DoTerminate();
                }

                if (_speed <= 0) //nooo dont look at it
                {
                    _lifespan++;
                    subtractColor((255 / 15));
                    void subtractColor(byte val)
                    {//trust me it needs this to fade properly
                        if (_color.a - val >= 0)
                            _color.a -= val;
                        else
                            _color.a = 0;

                        if (_color.r - val >= 0)
                            _color.r -= val;
                        else
                            _color.r = 0;

                        if (_color.g - val >= 0)
                            _color.g -= val;
                        else
                            _color.g = 0;

                        if (_color.b - val >= 0)
                            _color.b -= val;
                        else
                            _color.b = 0;
                    }
                }
            }

            public override void Draw()
            {
                Graphics.DrawRect(position, position + new Vec2(2f, 2f), _color, depth);
            }

        }
    }
}

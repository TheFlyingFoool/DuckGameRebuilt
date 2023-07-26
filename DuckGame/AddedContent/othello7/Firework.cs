using System;
namespace DuckGame
{
    //most of the comments here were made by othello7 im just here to fix stuff up and improve upon it -NiK0
    public class Firework : Thing
    {
        private Color _color; // watch?v=FZFZl0ZaFSU
        private float _deathHeight;
        private float _speed = 1;
        private static Color[] s_fireworkColors = {
            Color.Violet,
            Color.SkyBlue,
            Color.Wheat,
            Color.GreenYellow,
            Color.Pink
        };

        private const int TRAIL_PARTICLE_FRAME_INTERVAL = 3;
        private float _framesSinceTrailParticle = Rando.Int(TRAIL_PARTICLE_FRAME_INTERVAL);

        private static int[][,] s_explosionPatterns = { // for some god forsaken reason even-number
            new[,] // heart                             // widths/heights cause wacky alignment
            {                                           // issues that drive me insane   -firebreak
                {0, 1, 1, 0, 1, 1, 0},
                {1, 0, 0, 1, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 1},
                {1, 0, 0, 0, 0, 0, 1},
                {0, 1, 0, 0, 0, 1, 0},
                {0, 0, 1, 0, 1, 0, 0},
                {0, 0, 0, 1, 0, 0, 0},
            },
        };
        
        public Firework(float xpos, float ypos, float deathHeight)
          : base(xpos, ypos)
        {
            graphic = new Sprite("firework");
            layer = Layer.Console;
            depth = 1;
            center = new Vec2(graphic.w / 2, graphic.h / 2);
            // scale = new Vec2(1.2f, 1.4f);
            _deathHeight = deathHeight;
            
            _color = s_fireworkColors.ChooseRandom();
        }

        public override void Draw()
        {
            Graphics.DrawRect(new Rectangle(x, y, w, h), Color.Purple, 2f, false, 0.5f);
            base.Draw();
        }

        public override void Update()
        {
            y -= _speed;
            _speed += 0.2f; //accelerating upwards

            if (_framesSinceTrailParticle++ > TRAIL_PARTICLE_FRAME_INTERVAL)
            {
                Level.Add(new FireworkCharm(x + width, y + height, Color.White));
                _framesSinceTrailParticle = 0;
            }

            if (y < _deathHeight) //explode now
            {
                int sg = 2;
                if (Rando.Int(3) == 0) 
                    sg++;
                if (Rando.Int(9) == 0) 
                    sg++;

                if (Rando.Int(20) == 0)
                {
                    _color = Color.Pink;
                    int[,] explosionPattern = s_explosionPatterns.ChooseRandom();
                    
                    for (int i = 1; i < sg; i++)
                    {
                        if (i == 2) 
                            _color = Color.Red;
                        else if (i == 3) 
                            _color = Color.Orange;
                        float radius = 
                            6 +
                            i * Rando.Float(6) +
                            i * 4;

                        int explosionWidth = explosionPattern.GetLength(1);
                        int explosionHeight = explosionPattern.GetLength(0);
                        
                        int halfExplosionWidth = explosionWidth / 2;
                        int halfExplosionHeight = explosionHeight / 2;
                        
                        for (int yi = 0; yi < explosionHeight; yi++)
                        {
                            for (int xi = 0; xi < explosionWidth; xi++)
                            {
                                if (explosionPattern[yi, xi] == 0)
                                    continue;
                                
                                Vec2 posMultiplier = new(xi - halfExplosionWidth, yi - halfExplosionHeight);

                                Level.Add(new FireworkCharm(x, y, _color, position + new Vec2(radius, radius) * posMultiplier));
                            }
                        }

                        // //i do not care. -NiK0 // I DO -firebreak
                        // float tripleRadius = 3 * singleRadius;
                        // float doubleRadius = 2 * singleRadius;
                        //
                        // Level.Add(new FireworkCharm(x, y, _color, new Vec2(x - tripleRadius, y)));
                        // Level.Add(new FireworkCharm(x, y, _color, new Vec2(x - tripleRadius, y - singleRadius)));
                        // Level.Add(new FireworkCharm(x, y, _color, new Vec2(x + tripleRadius, y)));
                        // Level.Add(new FireworkCharm(x, y, _color, new Vec2(x + tripleRadius, y - singleRadius)));
                        //
                        // Level.Add(new FireworkCharm(x, y, _color, new Vec2(x + doubleRadius, y - doubleRadius)));
                        // Level.Add(new FireworkCharm(x, y, _color, new Vec2(x + singleRadius, y - doubleRadius)));
                        // Level.Add(new FireworkCharm(x, y, _color, new Vec2(x - doubleRadius, y - doubleRadius)));
                        // Level.Add(new FireworkCharm(x, y, _color, new Vec2(x - singleRadius, y - doubleRadius)));
                        // Level.Add(new FireworkCharm(x, y, _color, new Vec2(x, y - singleRadius)));
                        //
                        // Level.Add(new FireworkCharm(x, y, _color, new Vec2(x - doubleRadius, y + singleRadius)));
                        // Level.Add(new FireworkCharm(x, y, _color, new Vec2(x + doubleRadius, y + singleRadius)));
                        // Level.Add(new FireworkCharm(x, y, _color, new Vec2(x - singleRadius, y + doubleRadius)));
                        // Level.Add(new FireworkCharm(x, y, _color, new Vec2(x + singleRadius, y + doubleRadius)));
                        // Level.Add(new FireworkCharm(x, y, _color, new Vec2(x, y + tripleRadius)));
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
            public FireworkCharm(float xpos, float ypos, Color color)
              : base(xpos, ypos)
            {
                sprite = new SpriteMap("cspark", 5, 5);
                sprite.AddAnimation("go", Rando.Float(0.2f, 0.4f), false, 0, 1, 2, 3);
                sprite.SetAnimation("go");
                graphic = sprite;
                graphic.color = color;
                center = new Vec2(2.5f);
                collisionSize = new Vec2(5);
                _collisionOffset = new Vec2(-2.5f);
                _color = color;
                layer = Layer.Console;
                _flyangle = 0;
                _speed = 0;
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

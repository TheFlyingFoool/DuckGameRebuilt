using System;

namespace DuckGame
{
    public class SelectCircleParticle : Thing
    {
        public SelectCircleParticle(float xpos, float ypos, float spd, bool inverse, Color c) : base(xpos, ypos, null)
        {
            depth = 0.6f;
            vSpeed = Rando.Float(-0.5f, -1.5f);
            _inverse = inverse;
            _size = 0.5f + Rando.Float(0.8f);
            _color = c;
            rand = Rando.Float(6, 22);
            add = spd * 0.5f;
            _wave = Rando.Float(-80, 80);
        }
        public float rRand;
        public override void Update()
        {
            rRand = Lerp.Float(rRand, rand, 1f);
            if (_inverse) _wave -= add;
            else _wave += add;

            if (Math.Abs(_wave) > 160)
            {
                Level.Remove(this);
            }
            base.Update();
        }

        public override void Draw()
        {
            Vec2 value = position + new Vec2((float)Math.Sin(_wave / rand) * rRand, (float)Math.Cos(_wave / rand) * rRand);
            Graphics.DrawRect(value - new Vec2(_size, _size), value + new Vec2(_size, _size), _color * 0.4f, depth);
            base.Draw();
        }
        public float add;
        public float rand;
        public float _wave;

        public float _sinVal;

        private bool _inverse;

        private float _size;

        private Color _color;
    }
}

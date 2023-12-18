using System;

namespace DuckGame
{
    public class BeamParticle : Thing
    {
        public static int colorindex;
        public float _wave;
        public float _sinVal;
        private bool _inverse;
        private float _size;
        private Color _color;
        protected Interp ParticleLerp = new Interp(true);

        public BeamParticle(float xpos, float ypos, float spd, bool inverse, Color c)
          : base(xpos, ypos)
        {
            if (Program.gay)
            {
                c = Colors.Rainbow[colorindex];
                colorindex += 1;
                if (colorindex >= Colors.Rainbow.Length)
                {
                    colorindex = 0;
                }
            }
            depth = (Depth)0.9f;
            vSpeed = Rando.Float(-0.5f, -1.5f);
            y += Rando.Float(10f);
            _inverse = inverse;
            _size = 0.5f + Rando.Float(0.8f);
            _color = c;
        }

        public override void Update()
        {
            _wave += 0.1f;
            _sinVal = (float)Math.Sin(_wave);
            y += vSpeed;
            if (_sinVal < -0.8f && depth > 0f)
                depth = -0.8f;
            else if (_sinVal > 0.8f && depth < 0f)
                depth = (Depth)0.8f;
            if (y < -20f)
                Level.Remove(this);
            base.Update();
        }

        public override void Draw()
        {
            Vec2 vec2 = position + new Vec2((float)(16f * _sinVal * (_inverse ? -1f : 1f)), 0f);
            ParticleLerp.UpdateLerpState(vec2, MonoMain.IntraTick, MonoMain.UpdateLerpState);

            Graphics.DrawRect(ParticleLerp.Position - new Vec2(_size, _size), ParticleLerp.Position + new Vec2(_size, _size), _color * 0.4f, depth);

            base.Draw();
        }
    }
}

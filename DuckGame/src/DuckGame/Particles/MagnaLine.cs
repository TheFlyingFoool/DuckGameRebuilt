using System;

namespace DuckGame
{
    public class MagnaLine : Thing
    {
        private Gun _attach;
        private float _length;
        private float _startLength;
        private float _move = (float)(Math.PI / 2);
        public bool show;
        public float dist;
        public float _alphaFade;
        private Interp LineLerp = new Interp(true);
        public MagnaLine(float xpos, float ypos, Gun attach, float length, float percent)
          : base(xpos, ypos)
        {
            _attach = attach;
            _length = length;
            _startLength = length;
            _move = (float)(Math.PI / 2) * percent;
            alpha = 0f;
        }

        public override void Update()
        {
        }

        public override void Draw()
        {
            if(MonoMain.UpdateLerpState)
                _move = Lerp.Float(_move, 0f, 0.04f);
            if (_move <= 0.01f)
                _move += (float)(Math.PI / 2);
            if (_length > dist)
                show = false;
            _alphaFade = Lerp.Float(_alphaFade, show ? 1f : 0f, 0.1f);
            _length = _startLength * (float)Math.Sin(_move);
            alpha = (1f - _length / _startLength) * _alphaFade;
            if (alpha < 0.01f)
                return;
            position = _attach.barrelPosition + _attach.barrelVector * _length;
            Vec2 vec2 = _attach.barrelVector.Rotate(Maths.DegToRad(90f), Vec2.Zero);
            LineLerp.UpdateLerpState(position, SkipIntratick > 0 ? 1:MonoMain.IntraTick, MonoMain.UpdateLerpState);
            Graphics.DrawLine(LineLerp.Position + vec2 * 7f, LineLerp.Position - vec2 * 7f, Color.Blue * alpha, (1f + (1f - _length / _startLength) * 4f), (Depth)0.9f);
        }
    }
}

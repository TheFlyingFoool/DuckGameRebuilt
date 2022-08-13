// Decompiled with JetBrains decompiler
// Type: DuckGame.MagnaLine
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class MagnaLine : Thing
    {
        private Gun _attach;
        private float _length;
        private float _startLength;
        private float _move = 1.570796f;
        public bool show;
        public float dist;
        public float _alphaFade;

        public MagnaLine(float xpos, float ypos, Gun attach, float length, float percent)
          : base(xpos, ypos)
        {
            _attach = attach;
            _length = length;
            _startLength = length;
            _move = 1.570796f * percent;
            alpha = 0f;
        }

        public override void Update()
        {
        }

        public override void Draw()
        {
            _move = Lerp.Float(_move, 0f, 0.04f);
            if (_move <= 0.01f)
                _move += 1.570796f;
            if (_length > dist)
                show = false;
            _alphaFade = Lerp.Float(_alphaFade, show ? 1f : 0f, 0.1f);
            _length = _startLength * (float)Math.Sin(_move);
            alpha = (1f - _length / _startLength) * _alphaFade;
            if (alpha < 0.01f)
                return;
            position = _attach.barrelPosition + _attach.barrelVector * _length;
            Vec2 vec2 = _attach.barrelVector.Rotate(Maths.DegToRad(90f), Vec2.Zero);
            Graphics.DrawLine(position + vec2 * 7f, position - vec2 * 7f, Color.Blue * alpha, (1f + (1f - _length / _startLength) * 4f), (Depth)0.9f);
        }
    }
}

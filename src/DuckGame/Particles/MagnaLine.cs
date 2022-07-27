// Decompiled with JetBrains decompiler
// Type: DuckGame.MagnaLine
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._attach = attach;
            this._length = length;
            this._startLength = length;
            this._move = 1.570796f * percent;
            this.alpha = 0.0f;
        }

        public override void Update()
        {
        }

        public override void Draw()
        {
            this._move = Lerp.Float(this._move, 0.0f, 0.04f);
            if (_move <= 0.00999999977648258)
                this._move += 1.570796f;
            if (_length > (double)this.dist)
                this.show = false;
            this._alphaFade = Lerp.Float(this._alphaFade, this.show ? 1f : 0.0f, 0.1f);
            this._length = this._startLength * (float)Math.Sin(_move);
            this.alpha = (float)(1.0 - _length / (double)this._startLength) * this._alphaFade;
            if ((double)this.alpha < 0.00999999977648258)
                return;
            this.position = this._attach.barrelPosition + this._attach.barrelVector * this._length;
            Vec2 vec2 = this._attach.barrelVector.Rotate(Maths.DegToRad(90f), Vec2.Zero);
            Graphics.DrawLine(this.position + vec2 * 7f, this.position - vec2 * 7f, Color.Blue * this.alpha, (float)(1.0 + (1.0 - _length / (double)this._startLength) * 4.0), (Depth)0.9f);
        }
    }
}

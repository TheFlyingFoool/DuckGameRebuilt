// Decompiled with JetBrains decompiler
// Type: DuckGame.BeamParticle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class BeamParticle : Thing
    {
        public float _wave;
        public float _sinVal;
        private bool _inverse;
        private float _size;
        private Color _color;

        public BeamParticle(float xpos, float ypos, float spd, bool inverse, Color c)
          : base(xpos, ypos)
        {
            this.depth = (Depth)0.9f;
            this.vSpeed = Rando.Float(-0.5f, -1.5f);
            this.y += Rando.Float(10f);
            this._inverse = inverse;
            this._size = 0.5f + Rando.Float(0.8f);
            this._color = c;
        }

        public override void Update()
        {
            this._wave += 0.1f;
            this._sinVal = (float)Math.Sin(_wave);
            this.y += this.vSpeed;
            if (_sinVal < -0.800000011920929 && this.depth > 0.0f)
                this.depth = -0.8f;
            else if (_sinVal > 0.800000011920929 && this.depth < 0.0f)
                this.depth = (Depth)0.8f;
            if ((double)this.y < -20.0)
                Level.Remove(this);
            base.Update();
        }

        public override void Draw()
        {
            Vec2 vec2 = this.position + new Vec2((float)(16.0 * _sinVal * (this._inverse ? -1.0 : 1.0)), 0.0f);
            Graphics.DrawRect(vec2 - new Vec2(this._size, this._size), vec2 + new Vec2(this._size, this._size), this._color * 0.4f, this.depth);
            base.Draw();
        }
    }
}

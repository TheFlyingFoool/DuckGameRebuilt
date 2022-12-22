// Decompiled with JetBrains decompiler
// Type: DuckGame.MultiBeamParticle
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class MultiBeamParticle : Thing
    {
        public static int colorindex;
        public float _wave;
        public float _sinVal;
        private bool _inverse;
        private float _size;
        private Color _color;

        public MultiBeamParticle(float xpos, float ypos, float spd, bool inverse, Color c)
          : base(xpos, ypos)
        {
            if (Program.gay)
            {
                colorindex += 1;
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
            layer = Layer.Background;
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
            if (y < 0f)
                Level.Remove(this);
            base.Update();
        }

        public override void Draw()
        {
            Vec2 vec2 = position + new Vec2((16f * _sinVal * (_inverse ? -1f : 1f)), 0f);
            Graphics.DrawRect(vec2 - new Vec2(_size, _size), vec2 + new Vec2(_size, _size), _color * 0.4f, depth);
            base.Draw();
        }
    }
}

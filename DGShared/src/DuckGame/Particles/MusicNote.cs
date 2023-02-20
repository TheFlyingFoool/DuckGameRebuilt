// Decompiled with JetBrains decompiler
// Type: DuckGame.MusicNote
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class MusicNote : Thing
    {
        private Color _color;
        private SinWaveManualUpdate _sin;
        private float _size;
        private float _speed;
        private SpriteMap _sprite;
        private Vec2 _dir;

        public MusicNote(float xpos, float ypos, Vec2 dir)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("notes", 8, 8)
            {
                frame = Rando.Int(1)
            };
            _sprite.CenterOrigin();
            int val = Rando.ChooseInt(0, 1, 2, 3);
            if (val == 0)
                _color = Color.Violet;
            else if (val == 1)
                _color = Color.SkyBlue;
            else if(val == 2)
                _color = Color.Wheat;
            else if(val == 4)
                _color = Color.GreenYellow;
            _dir = dir;
            float mul = 1f;
            if (Rando.Float(1f) <= 0.5f)
                mul = -1f;
            _sin = new SinWaveManualUpdate(0.03f + (Rando.Float(0.1f)), mul * ((float)Math.PI * 2.0f));
            _size = 3f + Rando.Float(6f);
            _speed = 0.8f + Rando.Float(1.4f);
            depth = (Depth)0.95f;
            scale = new Vec2(0.1f, 0.1f);
        }

        public override void Update()
        {
            _sin.Update();
            x += _dir.x;
            Vec2 scale = this.scale;
            scale.x = scale.y = Lerp.Float(scale.x, 1f, 0.05f);
            this.scale = scale;
            if (this.scale.x <= 0.9f)
                return;
            alpha -= 0.01f;
            if (alpha > 0f)
                return;
            Level.Remove(this);
        }

        public override void Draw()
        {
            Vec2 position = this.position;
            position.y += _sin.value * _size;
            _sprite.alpha = alpha;
            _sprite.scale = scale;
            Graphics.Draw(_sprite, position.x, position.y);
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.SpawnerBall
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class SpawnerBall : Thing
    {
        public System.Type contains;
        protected SpriteMap _sprite;
        private bool _secondBall;
        private float _wave;
        private bool _grow = true;
        private float _wave2;
        public float orbitDistance = 3f;
        public float desiredOrbitDistance = 3f;
        public float orbitHeight = 1f;
        public float desiredOrbitHeight = 1f;

        public SpawnerBall(float xpos, float ypos, bool secondBall)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("spawnerBall", 4, 4)
            {
                frame = 1
            };
            graphic = _sprite;
            center = new Vec2(2f, 2f);
            _sprite.center = new Vec2(2f, 2f);
            depth = (Depth)0.5f;
            _secondBall = secondBall;
        }

        public override void Update()
        {
            orbitDistance = MathHelper.Lerp(orbitDistance, desiredOrbitDistance, 0.05f);
            orbitHeight = MathHelper.Lerp(orbitHeight, desiredOrbitHeight, 0.05f);
            _wave += 0.08f;
            if (_wave > 6.28000020980835)
            {
                _wave -= 6.28f;
                _grow = !_grow;
            }
            _wave2 += 0.05f;
        }

        public override void Draw()
        {
            float num = (float)((Math.Sin(_wave + 1.57000005245209) + 1.0) / 2.0 * 0.5);
            if (!_secondBall)
            {
                _sprite.scale = new Vec2(num + 0.6f, num + 0.6f);
                _sprite.depth = (Depth)(_sprite.scale.x > 0.800000011920929 ? 0.4f : -0.8f);
                Graphics.Draw(_sprite, x + (float)Math.Sin(_wave) * orbitDistance, y - orbitHeight);
            }
            else
            {
                _sprite.scale = new Vec2((float)(0.5 - num + 0.600000023841858), (float)(0.5 - num + 0.600000023841858));
                _sprite.depth = (Depth)(_sprite.scale.x > 0.800000011920929 ? 0.4f : -0.8f);
                Graphics.Draw(_sprite, x - (float)Math.Sin(_wave) * orbitDistance, y - orbitHeight);
            }
        }
    }
}

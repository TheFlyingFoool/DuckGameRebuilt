// Decompiled with JetBrains decompiler
// Type: DuckGame.LevelUpArrowAnimation
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class LevelUpArrowAnimation : EffectAnimation
    {
        private float _startWait;
        private float _alph = 2f;
        private float _vel;

        public LevelUpArrowAnimation(Vec2 pos)
          : base(pos, new SpriteMap("levelUpArrow", 16, 16), 0.9f)
        {
            layer = Layer.HUD;
            alpha = 0f;
            _startWait = Rando.Float(2.5f);
            _sprite.depth = (Depth)1f;
        }

        public override void Update()
        {
            if (_startWait > 0.0)
            {
                _startWait -= 0.1f;
            }
            else
            {
                _vel -= 0.1f;
                y += _vel;
                _alph -= 0.1f;
                alpha = Math.Min(_alph, 1f);
                if (alpha <= 0.0)
                    Level.Remove(this);
            }
            base.Update();
        }
    }
}

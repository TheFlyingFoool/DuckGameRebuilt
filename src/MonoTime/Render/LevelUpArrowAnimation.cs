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
            this.layer = Layer.HUD;
            this.alpha = 0.0f;
            this._startWait = Rando.Float(2.5f);
            this._sprite.depth = (Depth)1f;
        }

        public override void Update()
        {
            if (_startWait > 0.0)
            {
                this._startWait -= 0.1f;
            }
            else
            {
                this._vel -= 0.1f;
                this.y += this._vel;
                this._alph -= 0.1f;
                this.alpha = Math.Min(this._alph, 1f);
                if ((double)this.alpha <= 0.0)
                    Level.Remove(this);
            }
            base.Update();
        }
    }
}

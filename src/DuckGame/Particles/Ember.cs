// Decompiled with JetBrains decompiler
// Type: DuckGame.Ember
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Ember : PhysicsParticle
    {
        private SinWaveManualUpdate _wave = new SinWaveManualUpdate(0.1f + Rando.Float(0.1f));
        private Color _col;
        private float _initialLife = 1f;

        public Ember(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.vSpeed = -(0.2f + Rando.Float(0.7f));
            this.hSpeed = Rando.Float(0.4f) - 0.2f;
            this._col = Rando.Float(1f) >= 0.4f ? (Rando.Float(1f) >= 0.4f ? Color.Gray : Color.Orange) : Color.Yellow;
            if (Rando.Float(1f) < 0.2f)
                this._initialLife += Rando.Float(10f);
            this.alpha = 0.7f;
        }

        public override void Update()
        {
            this._wave.Update();
            this.position.x += this._wave.value * 0.2f;
            this.position.x += this.hSpeed;
            this.position.y += this.vSpeed;
            this._initialLife -= 0.1f;
            if (_initialLife >= 0f)
                return;
            this.alpha -= 0.025f;
            if ((double)this.alpha >= 0f)
                return;
            Level.Remove(this);
        }

        public override void Draw() => Graphics.DrawRect(this.position, this.position + new Vec2(1f, 1f), this._col * this.alpha, this.depth);
    }
}

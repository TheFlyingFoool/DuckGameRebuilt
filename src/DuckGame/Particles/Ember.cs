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
            vSpeed = -(0.2f + Rando.Float(0.7f));
            hSpeed = Rando.Float(0.4f) - 0.2f;
            _col = Rando.Float(1f) >= 0.4f ? (Rando.Float(1f) >= 0.4f ? Color.Gray : Color.Orange) : Color.Yellow;
            if (Rando.Float(1f) < 0.2f)
                _initialLife += Rando.Float(10f);
            alpha = 0.7f;
        }

        public override void Update()
        {
            _wave.Update();
            position.x += _wave.value * 0.2f;
            position.x += hSpeed;
            position.y += vSpeed;
            _initialLife -= 0.1f;
            if (_initialLife >= 0f)
                return;
            alpha -= 0.025f;
            if (alpha >= 0f)
                return;
            Level.Remove(this);
        }

        public override void Draw() => Graphics.DrawRect(position, position + new Vec2(1f, 1f), _col * alpha, depth);
    }
}

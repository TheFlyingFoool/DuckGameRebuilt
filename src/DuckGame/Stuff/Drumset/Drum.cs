// Decompiled with JetBrains decompiler
// Type: DuckGame.Drum
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Drum : Thing
    {
        protected string _sound = "";
        protected string _alternateSound = "";
        protected float _shake;
        private SinWave _shakeWave = (SinWave)1.1f;

        public Drum(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.layer = Layer.Blocks;
        }

        public void Hit()
        {
            SFX.Play(this._sound, 0.9f + Rando.Float(0.1f), Rando.Float(-0.05f, 0.05f));
            this._shake = 1f;
        }

        public void AlternateHit()
        {
            SFX.Play(this._alternateSound, 0.9f + Rando.Float(0.1f), Rando.Float(-0.05f, 0.05f));
            this._shake = 1f;
        }

        public override void Update()
        {
            this._shake = Lerp.Float(this._shake, 0f, 0.08f);
            base.Update();
        }

        public override void Draw()
        {
            this.position.x += (float)this._shakeWave * this._shake;
            base.Draw();
            this.position.x -= (float)this._shakeWave * this._shake;
        }
    }
}

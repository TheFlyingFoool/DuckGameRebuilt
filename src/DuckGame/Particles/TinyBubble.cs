// Decompiled with JetBrains decompiler
// Type: DuckGame.TinyBubble
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class TinyBubble : PhysicsParticle
    {
        private SinWaveManualUpdate _wave = new SinWaveManualUpdate(0.1f + Rando.Float(0.1f), Rando.Float(3f));
        private float _minY;
        private float _waveSize = 1f;

        public TinyBubble(float xpos, float ypos, float startHSpeed, float minY, bool blue = false)
          : base(xpos, ypos)
        {
            this.alpha = 0.7f;
            this._minY = minY;
            this._gravMult = 0f;
            this.vSpeed = -Rando.Float(0.5f, 1f);
            this.hSpeed = startHSpeed;
            this.depth = (Depth)0.3f;
            SpriteMap spriteMap = new SpriteMap("tinyBubbles", 8, 8);
            if (blue)
                spriteMap = new SpriteMap("tinyBlueBubbles", 8, 8);
            spriteMap.frame = Rando.Int(0, 1);
            this.graphic = spriteMap;
            this.center = new Vec2(4f, 4f);
            this._waveSize = Rando.Float(0.1f, 0.3f);
            this.xscale = this.yscale = 0.1f;
        }

        public override void Update()
        {
            this._wave.Update();
            this.position.x += this._wave.value * this._waveSize;
            this.position.x += this.hSpeed;
            this.position.y += this.vSpeed;
            this.hSpeed = Lerp.Float(this.hSpeed, 0f, 0.1f);
            this.xscale = this.yscale = Lerp.Float(this.xscale, 1f, 0.1f);
            if (this.y < _minY - 4.0)
                this.alpha -= 0.025f;
            if (this.y < _minY - 8.0)
                this.alpha = 0f;
            if (this.y >= _minY)
                return;
            this.alpha -= 0.025f;
            if (this.alpha >= 0.0)
                return;
            Level.Remove(this);
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.GlassParticle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class GlassParticle : PhysicsParticle
    {
        private int _tint;

        public GlassParticle(float xpos, float ypos, Vec2 hitAngle, int tint = -1)
          : base(xpos, ypos)
        {
            this.hSpeed = -hitAngle.x * 2f * (Rando.Float(1f) + 0.3f);
            this.vSpeed = (-hitAngle.y * 2f * (Rando.Float(1f) + 0.3f)) - Rando.Float(2f);
            this._bounceEfficiency = 0.6f;
            this._tint = tint;
        }

        public override void Update()
        {
            this.alpha -= 0.01f;
            if (this.alpha < 0f)
                Level.Remove(this);
            base.Update();
        }

        public override void Draw() => Graphics.DrawRect(this.position, this.position + new Vec2(1f, 1f), (this._tint > 0 ? Window.windowColors[this._tint] : Color.LightBlue) * this.alpha, this.depth);
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.RCControlBolt
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class RCControlBolt : Thing
    {
        private bool _fade;
        private RCCar _control;

        public RCCar control => this._control;

        public RCControlBolt(float xval, float yval, RCCar c)
          : base(xval, yval)
        {
            this._control = c;
            this.graphic = new Sprite("rcBolt");
            this.center = new Vec2(8f, 8f);
            this.scale = new Vec2(0.3f, 0.3f);
            this.alpha = 1f;
        }

        public override void Update()
        {
            Vec2 vec2 = this._control.position - this.position;
            double length = vec2.length;
            vec2.Normalize();
            this.angleDegrees = (float)(-Maths.PointDirection(this.position, this._control.position) + 90.0);
            this.position += vec2 * 8f;
            this.xscale = this.yscale = Lerp.Float(this.xscale, 1f, 0.1f);
            if (length < 48.0 || this._control.destroyed || !this._control.receivingSignal)
                this._fade = true;
            this.alpha = Lerp.Float(this.alpha, this._fade ? 0f : 1f, 0.1f);
            if (this.alpha < 0.00999999977648258 && this._fade)
                Level.Remove(this);
            base.Update();
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.RCControlBolt
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class RCControlBolt : Thing
    {
        private bool _fade;
        private RCCar _control;

        public RCCar control => _control;

        public RCControlBolt(float xval, float yval, RCCar c)
          : base(xval, yval)
        {
            _control = c;
            graphic = new Sprite("rcBolt");
            center = new Vec2(8f, 8f);
            scale = new Vec2(0.3f, 0.3f);
            alpha = 1f;
        }

        public override void Update()
        {
            Vec2 vec2 = _control.position - position;
            double length = vec2.length;
            vec2.Normalize();
            angleDegrees = (float)(-Maths.PointDirection(position, _control.position) + 90.0);
            position += vec2 * 8f;
            xscale = yscale = Lerp.Float(xscale, 1f, 0.1f);
            if (length < 48.0 || _control.destroyed || !_control.receivingSignal)
                _fade = true;
            alpha = Lerp.Float(alpha, _fade ? 0f : 1f, 0.1f);
            if (alpha <  0.01f && _fade)
                Level.Remove(this);
            base.Update();
        }
    }
}

namespace DuckGame
{
    public class MindControlBolt : Thing
    {
        private bool _fade;
        private Duck _controlledDuck;

        public Duck controlledDuck => _controlledDuck;

        public MindControlBolt(float xval, float yval, Duck control)
          : base(xval, yval)
        {
            _controlledDuck = control;
            graphic = new Sprite("mindBolt");
            center = new Vec2(8f, 8f);
            scale = new Vec2(0.1f, 0.1f);
            alpha = 0f;
        }

        public override void Update()
        {
            Vec2 position = _controlledDuck.position;
            if (_controlledDuck.ragdoll != null)
                position = _controlledDuck.ragdoll.part3.position;
            Vec2 vec2 = position - this.position;
            double length = vec2.length;
            vec2.Normalize();
            angleDegrees = (-Maths.PointDirection(this.position, position) + 90f);
            this.position += vec2 * 4f;
            xscale = yscale = Lerp.Float(xscale, 1f, 0.05f);
            if (length < 48f || _controlledDuck.mindControl == null)
                _fade = true;
            alpha = Lerp.Float(alpha, _fade ? 0f : 1f, 0.1f);
            if (alpha < 0.01f && _fade)
                Level.Remove(this);
            base.Update();
        }
    }
}

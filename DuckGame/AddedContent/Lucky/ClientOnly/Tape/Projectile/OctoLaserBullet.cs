namespace DuckGame
{
    [ClientOnly]
    public class OctoLaserBullet : QuadLaserBullet
    {
        public StateBinding _quadScaleBinding = new StateBinding("quadScale");
        public OctoLaserBullet(float xpos, float ypos, Vec2 travel, float qscal = 2)
          : base(xpos, ypos, travel)
        {
            quadScale = qscal;
            collisionOffset = new Vec2(-3f, -3f) * qscal;
            _collisionSize = new Vec2(6f, 6f) * qscal;
        }
        public float quadScale;
        public override void Update()
        {
            _wave.Update();
            _wave2.Update();
            timeAlive += 0.016f;
            position += _travel * 0.5f;
            if (isServerForObject && (x > Level.current.ExtendedRight || x < Level.current.ExtendedLeft || y < Level.current.ExtendedTop || y > Level.current.ExtendedBottom)) Level.Remove(this);
            foreach (MaterialThing materialThing in Level.CheckRectAll<MaterialThing>(topLeft, bottomRight))
            {
                if ((safeFrames <= 0 || materialThing != safeDuck) && materialThing.isServerForObject)
                {
                    bool destroyed = materialThing.destroyed;
                    materialThing.Destroy(new DTIncinerate(this));
                    if (materialThing.destroyed && !destroyed)
                    {
                        if (Recorder.currentRecording != null)
                            Recorder.currentRecording.LogAction(2);
                        if (materialThing is Duck && !(materialThing as Duck).dead)
                            Recorder.currentRecording.LogBonus();
                    }
                }
            }
            if (safeFrames > 0) --safeFrames;
        }

        public override void Draw()
        {
            Graphics.DrawRect(position + new Vec2(-4f, -4f) * quadScale, position + new Vec2(4f, 4f) * quadScale, new Color(byte.MaxValue - (int)(_wave.normalized * 90f), 137 + (int)(_wave.normalized * 50f), 31 + (int)(_wave.normalized * 30f)), depth);
            Graphics.DrawRect(position + new Vec2(-4f, -4f) * quadScale, position + new Vec2(4f, 4f) * quadScale, new Color(byte.MaxValue, 224 - (int)(_wave2.normalized * 150f), 90 + (int)(_wave2.normalized * 50f)), depth + 1, false, quadScale);
        }
    }
}

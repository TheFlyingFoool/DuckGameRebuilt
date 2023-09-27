namespace DuckGame
{
    [EditorGroup("Guns|Pistols")]
    public class SuicidePistol : Gun
    {
        public SuicidePistol(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 6;
            _ammoType = new AT9mm
            {
                barrelAngleDegrees = 180f,
                immediatelyDeadly = true
            };
            _type = "gun";
            graphic = new Sprite("suicidePistol");
            center = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -5f);
            collisionSize = new Vec2(16f, 10f);
            _barrelOffsetTL = new Vec2(8f, 13f);
            _fireSound = "magnum";
            _kickForce = -3.5f;
            _fireRumble = RumbleIntensity.Kick;
            handOffset = new Vec2(6f, 0f);
            _holdOffset = new Vec2(6f, 0f);
            loseAccuracy = 0.1f;
            maxAccuracyLost = 0.6f;
            editorTooltip = "There's something odd about this gun but I can't quite put my finger on it.";
        }

        public override void Update()
        {
            if (_raised)
            {
                handOffset = new Vec2(0f, 0f);
                _holdOffset = new Vec2(0f, 0f);
                collisionOffset = new Vec2(-8f, -5f);
                collisionSize = new Vec2(16f, 10f);
            }
            else
            {
                handOffset = new Vec2(7f, 0f);
                _holdOffset = new Vec2(4f, -1f);
                collisionOffset = new Vec2(-8f, -5f);
                collisionSize = new Vec2(8f, 10f);
            }
            base.Update();
        }
    }
}

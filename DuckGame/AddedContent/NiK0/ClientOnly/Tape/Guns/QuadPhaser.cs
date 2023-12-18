namespace DuckGame
{
    [ClientOnly]
    public class QuadPhaser : Phaser
    {
        public QuadPhaser(float xpos, float ypos) : base(xpos, ypos) 
        {
            graphic = new Sprite("quadphaser");
            _barrelOffsetTL = new Vec2(16.5f, 6.5f);
            ammo = 4;
            _holdOffset = new Vec2(-1, -3);
            _phaserCharge.center = new Vec2(-2, -3f);
            _phaserCharge.color = Color.Orange;
            _phaserCharge.scale = new Vec2(2);
        }
        public OctoLaserBullet b;
        public override bool CanTapeTo(Thing pThing)
        {
            switch (pThing)
            {
                case QuadLaser _:
                case Phaser _:
                    return false;
                default:
                    return true;
            }
        }
        public bool supressed;
        public override void Update()
        {
            if (isServerForObject)
            {
                bool doubleUp = false;
                if (tapedCompatriot is QuadPhaser qr && tape.gun1 == this)
                {
                    qr.supressed = true;
                    doubleUp = true;
                }
                if (b != null)
                {
                    if (doubleUp)
                    {
                        _barrelOffsetTL = new Vec2(16.5f, 7.5f);
                        b.position = barrelPosition + (barrelVector * b.quadScale * 3);
                        b.travel = barrelVector * (_charge  * 6.5f + 1);
                        b.quadScale = Lerp.Float(b.quadScale, (_chargeFade * 4) + 0.5f, 0.1f);
                    }
                    else
                    {
                        b.travel = barrelVector * (_charge * 3 +1);
                        b.quadScale = Lerp.Float(b.quadScale, (_chargeFade * 1.5f) + 1, 0.05f);
                        b.position = barrelPosition + (barrelVector * b.quadScale * 3);
                    }
                }
            }
            base.Update();
        }
        public override void OnPressAction()
        {
            if (ammo > 0 && isServerForObject)
            {
                if (!supressed)
                {
                    b = new OctoLaserBullet(barrelPosition.x, barrelPosition.y, Vec2.Zero, 0);
                    b.owner = this;
                    Level.Add(b);
                }
            }
        }
        public override void OnHoldAction()
        {
            if (b != null)
            {
                _charge += 0.01f;
                if (_charge > 1f)
                    _charge = 1f;
                if (_chargeLevel == 0)
                    _chargeLevel = 1;
                else if (_charge > 0.4f && _chargeLevel == 1)
                {
                    _chargeLevel = 2;
                    SFX.PlaySynchronized("phaserCharge02", 0.5f, -0.5f);
                }
                else
                {
                    if (_charge <= 0.8f || _chargeLevel != 2)
                        return;
                    _chargeLevel = 3;
                    SFX.PlaySynchronized("phaserCharge03", 0.6f, -0.5f);
                }
            }
        }
        public override void OnReleaseAction()
        {
            if (b != null)
            {
                float f = (_chargeFade * 1.5f) + 1;
                if (b.quadScale < f)
                {
                    b.quadScale = f;
                }
                SFX.PlaySynchronized("laserBlast");
                b.owner = null;
                b = null;
                _kickForce = _charge * 4 + 3; 
                ApplyKick();
                _charge = 0f;
                _chargeLevel = 0;
                ammo--;
            }
        }
    }
}

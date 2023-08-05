namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt|Wump|Lasers")]
    public class WumpQuadLaser : Gun
    {
        public WumpQuadLaser(float xval, float yval) : base(xval, yval)
        {
            ammo = 1;
            _ammoType = new AT9mm();
            graphic = new Sprite("wumpquadlaser");
            center = new Vec2(9.5f, 6.5f);
            collisionOffset = new Vec2(-9.5f, -6.5f);
            collisionSize = new Vec2(19, 13);
            _barrelOffsetTL = new Vec2(28f, 8f);
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(2f, -2f);
            _editorName = "Quad Laser";
            editorTooltip = "Shoots a FAST-MOVING supernatural block of DOOM that passes through dimensions.";
        }
        public override void Fire()
        {
        }
        public override void OnPressAction()
        {
            if (ammo > 0)
            {
                Vec2 vec = Offset(barrelOffset);
                if (isServerForObject)
                {
                    WumpQuadLaserBullet quadLaserBullet = new WumpQuadLaserBullet(vec.x, vec.y, barrelVector * 16)
                    {
                        killThingType = GetType()
                    };
                    if (infiniteAmmoVal) quadLaserBullet.theholysee = true;
                    Level.Add(quadLaserBullet);
                    if (duck != null)
                    {
                        RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None, RumbleType.Gameplay));
                        duck.hSpeed = -barrelVector.x * 20f;
                        duck.vSpeed = -barrelVector.y * 18f - 2f;
                        duck.hMax = 24;
                        duck.vMax = 16;
                        quadLaserBullet.responsibleProfile = duck.profile;
                    }
                }
                Level.Remove(this);
                SFX.Play("laserBlast", 1f, -0.5f);
                SFX.Play("laserBlast", 1f, -0.6f);
                SFX.Play("laserBlast", 1f, -0.7f);
            }
        }
    }
}

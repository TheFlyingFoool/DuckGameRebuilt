namespace DuckGame
{
    [ClientOnly]
    public class OctoLaser : Gun
    {
        public OctoLaser(float xpos, float ypos) : base(xpos, ypos)
        {
            ammo = 3;
            graphic = new Sprite("octolaser");
            _ammoType = new AT9mm();

            center = new Vec2(9, 8.5f);
            _collisionSize = new Vec2(19, 18);
            _collisionOffset = new Vec2(-9, -8.5f);

            _barrelOffsetTL = new Vec2(21, 8.5f);
        }
        public override bool CanTapeTo(Thing pThing)
        {
            switch (pThing)
            {
                case OctoLaser _:
                case QuadLaser _:
                    return false;
                default:
                    return true;
            }
        }
        public override void Update()
        {
            base.Update();
        }
        public override void Fire()
        {
            base.Fire();
        }
        public override void OnPressAction()
        {
            if (ammo <= 0) return;
            if (isServerForObject)
            {
                OctoLaserBullet quadLaserBullet = new OctoLaserBullet(barrelPosition.x, barrelPosition.y, barrelVector * 2)
                {
                    killThingType = GetType()
                };
                Level.Add(quadLaserBullet);
                if (duck != null)
                {
                    RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                    duck.hSpeed = (float)(-barrelVector.x * 9);
                    duck.vSpeed = (float)(-barrelVector.y * 5 - 2);
                    quadLaserBullet.responsibleProfile = duck.profile;
                }
            }
            ammo--;
            SFX.Play("laserBlast");
        }
    }
}

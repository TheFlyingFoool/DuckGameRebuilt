namespace DuckGame
{
    [EditorGroup("Guns|Lasers")]
    [BaggedProperty("isSuperWeapon", true)]
    public class PlasmaBlaster : Gun
    {
        private SpriteMap _bigFlare;
        private bool _flared;

        public PlasmaBlaster(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 99;
            _ammoType = new ATPlasmaBlaster();
            _type = "gun";
            graphic = new Sprite("plasmaBlaster");
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -3f);
            collisionSize = new Vec2(16f, 8f);
            _barrelOffsetTL = new Vec2(18f, 6f);
            _fireSound = "plasmaFire";
            _kickForce = 1f;
            _fireRumble = RumbleIntensity.Kick;
            loseAccuracy = 0.14f;
            maxAccuracyLost = 0.9f;
            _bigFlare = new SpriteMap("plasmaFlare", 32, 32);
            _bigFlare.AddAnimation("idle", 1f, false, 0, 1, 2);
            _bigFlare.center = new Vec2(0f, 16f);
            _fullAuto = true;
            _bulletColor = Color.Orange;
            _bio = "Originally found in a crater next to a burnt power suit. It's origin and mechanism of action are unknown, but tests indicate that it is seriously badass.";
            _editorName = "Plasma Blaster";
            editorTooltip = "Out of control rapid-fire blaster. Seriously, be careful with this one.";

            //this is here because an omega edge case where a plasmablaster can fire at lightning speed if being graphic culled
            //because there is functionality in the draw function. i could of moved that into update but it might change the way
            //it works ever so slightly so just gotta deal with the mess that is duck game by duct taping stuff on top -Lucky
            shouldbegraphicculled = false;
        }

        public override void Update()
        {
            ammo = 99;
            if (_fireWait > 6)
                _fireWait = 6f;
            _fireWait = Maths.LerpTowards(_fireWait, 0.3f, 0.01f);
            base.Update();
        }

        public override void Draw()
        {
            _barrelHeat = 0f;
            if (_flareAlpha > 0 && !_flared)
            {
                _flared = true;
                _bigFlare.SetAnimation("idle");
                _bigFlare.frame = 0;
                _fireWait += 0.1f;
            }
            if (_flared)
            {
                Draw(ref _bigFlare, barrelOffset + new Vec2(-8f, -1f));
                if (_bigFlare.finished)
                {
                    _flared = false;
                    _flareAlpha = 0f;
                }
            }
            base.Draw();
        }
    }
}

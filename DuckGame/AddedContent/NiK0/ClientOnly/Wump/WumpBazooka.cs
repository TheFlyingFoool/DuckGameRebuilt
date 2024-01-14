namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Guns|Wump @DGR@")]
    public class WumpBazooka : TampingWeapon
    {
        public WumpBazooka(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 99;
            _ammoType = new ATWumpMissile();
            _type = "gun";
            graphic = new Sprite("wumpbazooka");
            center = new Vec2(15f, 9.5f);
            collisionOffset = new Vec2(-15f, -4f);
            collisionSize = new Vec2(30f, 10f);
            _barrelOffsetTL = new Vec2(29f, 9f);
            _fireSound = "missile";
            _kickForce = 6f;
            _fireRumble = RumbleIntensity.Light;
            _holdOffset = new Vec2(-2f, -3.5f);
            loseAccuracy = 0.1f;
            maxAccuracyLost = 0.6f;
            _editorName = "Bazooka";
            editorTooltip = "Funnier name, the missiles this gun are greatly explosive and volatile. Wouldn't recommend.";
            physicsMaterial = PhysicsMaterial.Metal;
            _numBulletsPerFire = 2;
            _timeToTamp = 1.5f;
        }

        public override void OnPressAction()
        {
            if (_tamped)
            {
                base.OnPressAction();
                int num = 0;
                for (int index = 0; index < 6 * Maths.Clamp(DGRSettings.ActualParticleMultiplier, 1, 100000); ++index)
                {
                    MusketSmoke musketSmoke = new MusketSmoke(barrelPosition.x - 16f + Rando.Float(32f), barrelPosition.y - 16f + Rando.Float(32f))
                    {
                        depth = (Depth)(float)(0.9f + index * (1f / 1000f))
                    };
                    if (num < 6)
                        musketSmoke.move -= barrelVector * Rando.Float(0.1f);
                    if (num > 5 && num < 10)
                        musketSmoke.fly += barrelVector * (2f + Rando.Float(7.8f));
                    Level.Add(musketSmoke);
                    ++num;
                }
                _tampInc = 0f;
                if (infinite.value)
                    _tampTime = 1f;
                else
                    _tampTime = 0.3f;
                _tamped = false;
            }
            else
            {
                if (_raised || !(this.owner is Duck owner) || !owner.grounded)
                    return;
                owner.immobilized = true;
                owner.sliding = false;
                _rotating = true;
            }
        }
    }
}

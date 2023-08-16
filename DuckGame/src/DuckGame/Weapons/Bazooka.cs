namespace DuckGame
{
    [EditorGroup("Guns|Explosives")]
    [BaggedProperty("isInDemo", true)]
    public class Bazooka : TampingWeapon
    {
        public Bazooka(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 99;
            _ammoType = new ATMissile();
            _type = "gun";
            graphic = new Sprite("bazooka");
            center = new Vec2(15f, 5f);
            collisionOffset = new Vec2(-15f, -4f);
            collisionSize = new Vec2(30f, 10f);
            _barrelOffsetTL = new Vec2(29f, 4f);
            _fireSound = "missile";
            _kickForce = 4f;
            _fireRumble = RumbleIntensity.Light;
            _holdOffset = new Vec2(-2f, -2f);
            loseAccuracy = 0.1f;
            maxAccuracyLost = 0.6f;
            _bio = "Old faithful, the 9MM pistol.";
            _editorName = nameof(Bazooka);
            editorTooltip = "Funny name, serious firepower. Launches an explosive missile that can destroy terrain.";
            physicsMaterial = PhysicsMaterial.Metal;
        }
        public override Holdable BecomeTapedMonster(TapedGun pTaped)
        {
            if (Editor.clientonlycontent)
            {
                return pTaped.gun1 is Bazooka && pTaped.gun2 is Sniper ? new SniperZooka(x, y) : null;
            }
            return base.BecomeTapedMonster(pTaped);
        }
        public override void OnPressAction()
        {
            if (_tamped)
            {
                base.OnPressAction();
                int num = 0;
                for (int index = 0; index < 14 * Maths.Clamp(DGRSettings.ActualParticleMultiplier, 1, 100000); ++index)
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
                    _tampTime = 0.8f;
                else
                    _tampTime = 0.5f;
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

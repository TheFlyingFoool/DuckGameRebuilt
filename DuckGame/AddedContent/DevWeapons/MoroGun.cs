namespace DuckGame
{
    [ClientOnly]
    #if DEBUG
    [EditorGroup("Guns|DEV")]
    [BaggedProperty("canSpawn", false)]
    #endif
    public class MoroGun : Gun
    {
        public SpriteMap sprite;
        public MoroGun(float xpos, float ypos) : base(xpos, ypos)
        {
            ammo = 3;
            weight = 9;

            sprite = new SpriteMap("MoroGun", 60, 35);
            graphic = sprite;
            center = new Vec2(30, 17.5f);
            _kickForce = 12f;
            wobble = new aWobbleMaterial(this, 0.2f);
            spawn = new MaterialDev(this, new Color(255, 255, 0));
            collisionSize = new Vec2(49, 13.5f);
            _collisionOffset = new Vec2(-21, -2);
            _holdOffset = new Vec2(0, -10);
            loseAccuracy = 1;
            maxAccuracyLost = 1;
            _fireWait = 3;
            _barrelOffsetTL = new Vec2(60, 21.5f);
        }
        public MaterialDev spawn;
        public float spawnSc;
        public aWobbleMaterial wobble;
        public override void Fire()
        {
            if (ammo > 0 && _wait == 0)
            {
                ApplyKick();
                //my grasp on the strings of reality here is fuzzy so im putting this "isServerForObject" here just to be safe -NiK0
                if (isServerForObject)
                {
                    Level.Add(new BFGBall(barrelPosition.x, barrelPosition.y) { travel = barrelVector * 7 });
                }
                ammo--;

                firing = true;
                _accuracyLost += loseAccuracy;
                if (_accuracyLost <= maxAccuracyLost)
                    return;
                _accuracyLost = maxAccuracyLost;
            }
        }
        public Sound fire;
        public override void OnPressAction()
        {
            if (isServerForObject)
            {
                if (ammo > 0)
                {
                    fire = SFX.PlaySynchronized("bfgChargeFire");
                    tilFire = 45;
                }
                else DoAmmoClick();
            }
        }
        public int tilFire = -1;
        public override void Terminate()
        {
            if (fire != null) fire.Kill();
            base.Terminate();
        }
        public override void Update()
        {
            if (owner == null && fire != null && fire.State == Microsoft.Xna.Framework.Audio.SoundState.Playing) fire.Stop();
            if (tilFire > 0 && isServerForObject && owner != null)
            { 
                tilFire--;
                if (tilFire == 0)
                {
                    tilFire = -1;
                    Fire();
                }
            }
            base.Update();
        }
        public override void Draw()
        {
            if (!spawn.finished) { Graphics.material = spawn; spawn.Update(); }

            sprite.imageIndex = 0;
            base.Draw();
            if (spawn.finished && level != null)  
            {
                if (spawnSc == 0) SFX.Play("laserChargeTeeny", 0.8f, -0.1f);
                spawnSc = Lerp.FloatSmooth(spawnSc, 1, 0.06f);
                sprite.imageIndex = 1;
                alpha = spawnSc;
                Graphics.material = wobble;
                depth -= 1;
                base.Draw();
                depth += 1;
                alpha = 1;
                Graphics.material = null;
            }
        }
    }
}

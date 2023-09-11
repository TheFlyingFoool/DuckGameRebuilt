using System;
using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    public class CollinGun : Gun
    {
        public SpriteMap sprite;
        public StateBinding _rechargeBinding = new StateBinding("recharge");
        public StateBinding _coinBinding = new StateBinding("coin");
        public CollinGun(float xpos, float ypos) : base(xpos, ypos)
        {
            sprite = new SpriteMap("CollinGun", 23, 17);
            graphic = sprite;
            center = new Vec2(11.5f, 8.5f);
            collisionSize = new Vec2(19, 12);
            _collisionOffset = new Vec2(-9.5f, -5.5f);
            _ammoType = new ATHighCalSniper();
            _kickForce = 2.55f;
            _fireWait = 4f;
            _fullAuto = true;
            ammo = 100;
            _barrelOffsetTL = new Vec2(20, 5.5f);
            wobble = new aWobbleMaterial(this, 0.2f);
            coin = true;
        }
        public aWobbleMaterial wobble;
        protected override void PlayFireSound()
        {
        }
        public int recharge;
        public bool coin;
        public override void OnPressAction()
        {
            loaded = true;
            base.OnPressAction();
        }
        public override void Fire()
        {
            if (_wait == 0)
            {
                if (coin)
                {
                    SFX.Play("coin");
                    if (isServerForObject)
                    {
                        Coin c = new Coin(x, y);
                        Vec2 extra = velocity;
                        if (owner != null) extra = owner.velocity;
                        c.velocity += barrelVector + extra + new Vec2(1 * -offDir, -6);
                        if (Math.Abs(c.hSpeed) < 3) c.hSpeed += offDir * 3;
                        Level.Add(c);

                        coin = false;
                        loaded = false;
                    }
                }
                else if (loaded)
                {
                    ApplyKick();
                    _wait = _fireWait;
                    if (isServerForObject)
                    {
                        SFX.PlaySynchronized("collinShoot", 1, Rando.Float(0.2f));
                        Vec2 v1 = barrelPosition;
                        Vec2 v2 = barrelPosition + barrelVector * 4000;
                        HitscanBullet hsb = new HitscanBullet(v1.x, v1.y, v2) { isLocal = true };
                        hsb.ignore = duck;
                        hsb.spawnLogic();
                        Level.Add(hsb);

                        IEnumerable<MaterialThing> mts = Level.CheckLineAll<MaterialThing>(v1, hsb.pos2);
                        foreach (MaterialThing mt in mts)
                        {
                            if (mt is IAmADuck && Duck.GetAssociatedDuck(mt) != owner)
                            {
                                SuperFondle(mt, DuckNetwork.localConnection);
                                mt.Destroy(new DTShot(null));
                            }
                            else
                            {
                                Fondle(mt);
                                mt.Hurt(0.1f);
                            }
                        }
                    }
                }
            }
        }
        public override void Draw()
        {
            sprite.imageIndex = 0;
            base.Draw();
            sprite.imageIndex = 1;
            Graphics.material = wobble;
            depth -= 1;
            base.Draw();
            depth += 1;
            Graphics.material = null;

            if (coin)
            {
                float al = alpha;
                sprite.imageIndex = 2;
                depth += 1;
                alpha = 0.3f;
                base.Draw();
                depth -= 1;
                alpha = al;
            }
        }
        public override void Update()
        {
            if (!coin && isServerForObject)
            {
                recharge++;
                if (recharge > 240)
                {
                    coin = true;
                    recharge = 0;
                }
            }
            ammo = 100;
            base.Update();
        }
    }
}

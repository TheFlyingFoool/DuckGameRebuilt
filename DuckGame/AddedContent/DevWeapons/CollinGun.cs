using System;
using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    public class CollinGun : Gun
    {
        public SpriteMap sprite;
        public StateBinding _rechargeBinding = new StateBinding("recharge");
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
            recharge = 400;
        }
        public aWobbleMaterial wobble;
        protected override void PlayFireSound()
        {
        }
        public int recharge;
        public override void OnPressAction()
        {
            loaded = true;
            base.OnPressAction();
        }
        public override void Fire()
        {
            if (_wait == 0)
            {
                if (recharge > 300)
                {
                    SFX.Play("coin");
                    if (isServerForObject)
                    {
                        recharge -= 300;
                        _wait = 1f;
                        Coin c = new Coin(x, y);
                        Vec2 extra = velocity;
                        if (owner != null) extra = owner.velocity;
                        c.velocity += barrelVector + extra + new Vec2(1 * -offDir, -6);
                        if (Math.Abs(c.hSpeed) < 3) c.hSpeed += offDir * 3;
                        Level.Add(c);

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

            if (recharge > 300)
            {
                float al = alpha;
                sprite.imageIndex = 2;
                depth += 1;
                alpha = 0.3f;
                base.Draw();
                depth -= 1;
                alpha = al;

            }
            if (owner != null)
            {
                if (holsterDraw)
                {
                    Graphics.DrawString((recharge / 300f).ToString("0.0"), Offset(new Vec2(-16, 8 * offDir)), Color.Yellow, 1, null, 0.7f);
                }
                else
                {
                    Graphics.DrawString(Math.Floor(recharge / 300f).ToString("0"), Offset(new Vec2(-18 + (offDir < 0 ? 5 : 0), -2)), Color.Yellow, 1, null, 0.7f);

                }
            }
        }
        public bool holsterDraw;
        public override void HolsterUpdate(Holster pHolster)
        {
            if (pHolster is Holster)
            {
                holsterDraw = true;
                if (recharge < 1200 && pHolster is PowerHolster) recharge += 2;
            }
            base.HolsterUpdate(pHolster);
        }
        public override void Update()
        {
            holsterDraw = false;
            if (recharge < 1200 && isServerForObject)
            {
                recharge++;
            }
            ammo = 100;
            base.Update();
        }
    }
}

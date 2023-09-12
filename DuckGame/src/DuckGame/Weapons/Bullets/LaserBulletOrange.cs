// Decompiled with JetBrains decompiler
// Type: DuckGame.LaserBulletOrange
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class LaserBulletOrange : LaserBullet
    {
        private int _travels;
        private bool _exploded;

        public LaserBulletOrange(
          float xval,
          float yval,
          AmmoType type,
          float ang = -1f,
          Thing owner = null,
          bool rbound = false,
          float distance = -1f,
          bool tracer = false,
          bool network = false)
          : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
            _thickness = type.bulletThickness;
            _beem = Content.Load<Texture2D>("laserBeemOrange");
        }
        protected override void Rebound(Vec2 pos, float dir, float rng)
        {
            if (specialRebound)
            {
                ++reboundBulletsCreated;
                LaserBulletOrange bullet = new LaserBulletOrange(pos.x, pos.y, ammo, dir, null, false, rng);
                bullet._teleporter = _teleporter;
                bullet.timesRebounded = timesRebounded + 1;
                bullet.lastReboundSource = lastReboundSource;
                bullet.isLocal = isLocal;
                bullet._thickness = _thickness;
                _reboundedBullet = bullet;
                reboundCalled = true;
                Level.Add(bullet);
                return;
            }
            base.Rebound(pos, dir, rng);
        }
        protected override void CheckTravelPath(Vec2 pStart, Vec2 pEnd)
        {
            if (_thickness > 1f && _travels > 0)
            {
                for (int index = 0; index < 10; ++index)
                {
                    Vec2 vec2 = pStart + (pEnd - pStart) * (index / 10f);
                    if (ATMissile.DestroyRadius(vec2, 16f, this, true) > 0 && !_exploded)
                    {
                        _exploded = true;
                        SFX.Play("explode");
                    }
                    foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(vec2, 16f))
                    {
                        switch (physicsObject)
                        {
                            case Gun _:
                            case Equipment _:
                                continue;
                            default:
                                physicsObject.Destroy(new DTIncinerate(this));
                                continue;
                        }
                    }
                }
            }
            ++_travels;
        }
    }
}

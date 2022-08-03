// Decompiled with JetBrains decompiler
// Type: DuckGame.PelletBullet
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class PelletBullet : Bullet
    {
        private float _isVolatile = 1f;

        public PelletBullet(
          float xval,
          float yval,
          AmmoType type,
          float ang = -1f,
          Thing owner = null,
          bool rbound = false,
          float distance = -1f,
          bool tracer = false,
          bool network = true)
          : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
        }

        protected override void Rebound(Vec2 pos, float dir, float rng)
        {
            PelletBullet bullet = ammo.GetBullet(pos.x, pos.y, angle: (-dir), firedFrom: firedFrom, distance: rng, tracer: _tracer) as PelletBullet;
            bullet._teleporter = _teleporter;
            bullet._isVolatile = _isVolatile;
            bullet.isLocal = isLocal;
            bullet.lastReboundSource = lastReboundSource;
            bullet.connection = connection;
            reboundCalled = true;
            Level.Add(bullet);
            SFX.Play("littleRic", 0.8f, Rando.Float(-0.15f, 0.15f));
        }

        public override void Update()
        {
            _isVolatile -= 0.06f;
            if (_isVolatile <= 0.0)
                rebound = false;
            base.Update();
        }
    }
}

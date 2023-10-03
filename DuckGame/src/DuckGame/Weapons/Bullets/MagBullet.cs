// Decompiled with JetBrains decompiler
// Type: DuckGame.MagBullet
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MagBullet : Bullet
    {
        private Texture2D _beem;
        private float _thickness;
        private Color color = Color.White;
        private static int colorindex;

        public MagBullet(
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
            _beem = Content.Load<Texture2D>("magBeam");
            if (Program.gay)
            {
                color = Colors.Rainbow[colorindex];
                colorindex += 1;
                if (colorindex >= Colors.Rainbow.Length)
                {
                    colorindex = 0;
                }
            }
        }

        public override void Draw()
        {
            if (_tracer || _bulletDistance <= 0.1f)
                return;

            BulletStart.UpdateLerpState(this.drawStart, MonoMain.IntraTick, MonoMain.UpdateLerpState);
            BulletEnd.UpdateLerpState(this.drawEnd, MonoMain.IntraTick, MonoMain.UpdateLerpState);
            Vec2 drawStart = BulletStart.Position;
            Vec2 drawEnd = BulletEnd.Position;

            float length = (drawStart - drawEnd).length;
            float val = 0f;
            float num1 = (1f / (length / 8f));
            float num2 = 0f;
            float num3 = 8f;
            if (Program.nikogay)
            {
                color = Colors.Rainbow[colorindex];
            }
            while (true)
            {
                bool flag = false;
                if (val + num3 > length)
                {
                    num3 = length - Maths.Clamp(val, 0f, 99f);
                    flag = true;
                }
                num2 += num1;
                Graphics.DrawTexturedLine((Tex2D)_beem, drawStart + travelDirNormalized * val, drawStart + travelDirNormalized * (val + num3), color * num2, _thickness, (Depth)0.6f);
                if (!flag)
                    val += 8f;
                else
                    break;
            }
        }

        protected override void OnHit(bool destroyed)
        {
            if (!destroyed)
                return;
            if (DGRSettings.ActualParticleMultiplier > 0)
            {
                ExplosionPart explosionPart = new ExplosionPart(x, y);
                explosionPart.xscale *= 0.7f;
                explosionPart.yscale *= 0.7f;
                Level.Add(explosionPart);
            }
            SFX.Play("magPop", 0.7f, Rando.Float(-0.5f, -0.3f));
            if (!isLocal)
                return;
            Thing owner = this.owner;
            foreach (MaterialThing t in Level.CheckCircleAll<MaterialThing>(position, 14f))
            {
                if (t != owner)
                {
                    SuperFondle(t, DuckNetwork.localConnection);
                    t.Destroy(new DTShot(this));
                }
            }
        }

        protected override void Rebound(Vec2 pos, float dir, float rng)
        {
            MagBullet t = new MagBullet(pos.x, pos.y, ammo, dir, rbound: rebound, distance: rng)
            {
                _teleporter = _teleporter,
                firedFrom = firedFrom,
                lastReboundSource = lastReboundSource,
                connection = connection,
                isLocal = isLocal
            };
            reboundCalled = true;
            Level.current.AddThing(t);
            Level.current.AddThing(new LaserRebound(pos.x, pos.y));
        }
    }
}

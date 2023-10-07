// Decompiled with JetBrains decompiler
// Type: DuckGame.LaserBullet
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace DuckGame
{
    public class LaserBullet : Bullet
    {
        protected Texture2D _beem;
        protected float _thickness;

        public LaserBullet(
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
            _beem = Content.Load<Texture2D>("laserBeam");
        }

        public override void Draw()
        {
            if (_tracer || _bulletDistance <= 0.1f)
                return;

            BulletStart.UpdateLerpState(this.drawStart, MonoMain.IntraTick, MonoMain.UpdateLerpState);
            BulletEnd.UpdateLerpState(this.drawEnd, MonoMain.IntraTick, MonoMain.UpdateLerpState);
            Vec2 drawStart = BulletStart.Position;
            Vec2 drawEnd = BulletEnd.Position;

            if (gravityAffected)
            {
                if (prev.Count < 1)
                    return;
                int num = (int)Math.Ceiling((drawdist - startpoint) / 8f);
                Vec2 p2 = prev.Last();
                for (int index = 0; index < num; ++index)
                {
                    Vec2 pointOnArc = GetPointOnArc(index * 8);
                    Graphics.DrawTexturedLine((Tex2D)_beem, pointOnArc, p2, color * (1f - index / num) * alpha, ammo.bulletThickness, (Depth)0.9f);
                    if (pointOnArc == prev[0])
                        break;
                    p2 = pointOnArc;
                    if (index == 0 && ammo.sprite != null && !doneTravelling)
                    {
                        ammo.sprite.depth = (Depth)1f;
                        ammo.sprite.angleDegrees = -Maths.PointDirection(Vec2.Zero, travelDirNormalized);
                        Graphics.Draw(ammo.sprite, p2.x, p2.y);
                    }
                }
            }
            else
            {
                float length = (drawStart - drawEnd).length;
                float dist = 0f;
                float incs = (1f / (length / 8f));
                float alph = 1f;
                float drawLength = 8f;
                while (true)
                {
                    bool bulletDrawn = false;
                    if (dist + drawLength > length)
                    {
                        drawLength = length - Maths.Clamp(dist, 0f, 99f);
                        bulletDrawn = true;
                    }
                    alph -= incs;
                    Graphics.DrawTexturedLine((Tex2D)_beem, drawStart + travelDirNormalized * length - travelDirNormalized * dist, drawStart + travelDirNormalized * length - travelDirNormalized * (dist + drawLength), Color.White * alph, _thickness, (Depth)0.6f);
                    if (!bulletDrawn)
                        dist += 8f;
                    else
                        break;
                }
            }
        }

        protected override void Rebound(Vec2 pos, float dir, float rng)
        {
            if (DGRSettings.ActualParticleMultiplier > 0) Level.current.AddThing(new LaserRebound(pos.x, pos.y));
            if (Recorderator.Playing) return;
            ++reboundBulletsCreated;
            isRebound = true;
            LaserBullet t = new LaserBullet(pos.x, pos.y, ammo, dir, rbound: rebound, distance: rng);
            isRebound = false;
            t._teleporter = _teleporter;
            t.firedFrom = firedFrom;
            t.timesRebounded = timesRebounded + 1;
            t.lastReboundSource = lastReboundSource;
            t.isLocal = isLocal;
            t.connection = connection;
            reboundCalled = true;
            Level.current.AddThing(t);
        }
    }
}

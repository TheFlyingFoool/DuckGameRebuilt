// Decompiled with JetBrains decompiler
// Type: DuckGame.LaserBullet
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._thickness = type.bulletThickness;
            this._beem = Content.Load<Texture2D>("laserBeam");
        }

        public override void Draw()
        {
            if (this._tracer || _bulletDistance <= 0.100000001490116)
                return;
            if (this.gravityAffected)
            {
                if (this.prev.Count < 1)
                    return;
                int num = (int)Math.Ceiling((drawdist - (double)this.startpoint) / 8.0);
                Vec2 p2 = this.prev.Last<Vec2>();
                for (int index = 0; index < num; ++index)
                {
                    Vec2 pointOnArc = this.GetPointOnArc(index * 8);
                    DuckGame.Graphics.DrawTexturedLine((Tex2D)this._beem, pointOnArc, p2, this.color * (float)(1.0 - index / (double)num) * this.alpha, this.ammo.bulletThickness, (Depth)0.9f);
                    if (pointOnArc == this.prev.First<Vec2>())
                        break;
                    p2 = pointOnArc;
                    if (index == 0 && this.ammo.sprite != null && !this.doneTravelling)
                    {
                        this.ammo.sprite.depth = (Depth)1f;
                        this.ammo.sprite.angleDegrees = -Maths.PointDirection(Vec2.Zero, this.travelDirNormalized);
                        DuckGame.Graphics.Draw(this.ammo.sprite, p2.x, p2.y);
                    }
                }
            }
            else
            {
                float length = (this.drawStart - this.drawEnd).length;
                float val = 0.0f;
                float num1 = (float)(1.0 / ((double)length / 8.0));
                float num2 = 0.0f;
                float num3 = 8f;
                while (true)
                {
                    bool flag = false;
                    if ((double)val + (double)num3 > (double)length)
                    {
                        num3 = length - Maths.Clamp(val, 0.0f, 99f);
                        flag = true;
                    }
                    num2 += num1;
                    DuckGame.Graphics.DrawTexturedLine((Tex2D)this._beem, this.drawStart + this.travelDirNormalized * val, this.drawStart + this.travelDirNormalized * (val + num3), Color.White * num2, this._thickness, (Depth)0.6f);
                    if (!flag)
                        val += 8f;
                    else
                        break;
                }
            }
        }

        protected override void Rebound(Vec2 pos, float dir, float rng)
        {
            ++this.reboundBulletsCreated;
            Bullet.isRebound = true;
            LaserBullet t = new LaserBullet(pos.x, pos.y, this.ammo, dir, rbound: this.rebound, distance: rng);
            Bullet.isRebound = false;
            t._teleporter = this._teleporter;
            t.firedFrom = this.firedFrom;
            t.timesRebounded = this.timesRebounded + 1;
            t.lastReboundSource = this.lastReboundSource;
            t.isLocal = this.isLocal;
            t.connection = this.connection;
            this.reboundCalled = true;
            Level.current.AddThing(t);
            Level.current.AddThing(new LaserRebound(pos.x, pos.y));
        }
    }
}

// Decompiled with JetBrains decompiler
// Type: DuckGame.PortalBullet
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace DuckGame
{
    public class PortalBullet : Bullet
    {
        private Texture2D _beem;
        private float _thickness;

        public PortalBullet(
          float xval,
          float yval,
          AmmoType type,
          float ang = -1f,
          Thing owner = null,
          bool rbound = false,
          float distance = -1f,
          float thick = 0.3f)
          : base(xval, yval, type, ang, owner, rbound, distance)
        {
            _thickness = thick;
            _beem = Content.Load<Texture2D>("laserBeam");
        }

        public override void OnCollide(Vec2 pos, Thing t, bool willBeStopped)
        {
            if (!(t is Block & willBeStopped) || !(this.owner is PortalGun owner))
                return;
            if (!(Level.current.things[typeof(Portal)].FirstOrDefault<Thing>(p => (p as Portal).gun == this.owner) is Portal portal))
            {
                portal = new Portal(owner);
                Level.Add(portal);
            }
            Vec2 p1 = pos - travelDirNormalized;
            PortalDoor door = new PortalDoor
            {
                center = pos
            };
            if (Math.Abs(travelDirNormalized.y) < 0.5)
            {
                door.horizontal = false;
                door.point1 = new Vec2(pos + new Vec2(0f, -16f));
                door.point2 = new Vec2(pos + new Vec2(0f, 16f));
                AutoBlock autoBlock1 = Level.CheckLine<AutoBlock>(p1, p1 + new Vec2(0f, 16f));
                if (autoBlock1 != null && autoBlock1.top < door.point2.y)
                    door.point2.y = autoBlock1.top;
                AutoBlock autoBlock2 = Level.CheckLine<AutoBlock>(p1, p1 + new Vec2(0f, -16f));
                if (autoBlock2 != null && autoBlock2.bottom > door.point1.y)
                    door.point1.y = autoBlock2.bottom;
            }
            else
            {
                door.horizontal = true;
                door.point1 = new Vec2(pos + new Vec2(-16f, 0f));
                door.point2 = new Vec2(pos + new Vec2(16f, 0f));
                AutoBlock autoBlock3 = Level.CheckLine<AutoBlock>(p1, p1 + new Vec2(16f, 0f));
                if (autoBlock3 != null && autoBlock3.left < door.point2.x)
                    door.point2.x = autoBlock3.left;
                AutoBlock autoBlock4 = Level.CheckLine<AutoBlock>(p1, p1 + new Vec2(-16f, 0f));
                if (autoBlock4 != null && autoBlock4.right > door.point1.x)
                    door.point1.x = autoBlock4.right;
            }
            portal.AddPortalDoor(door);
        }

        public override void Draw()
        {
            if (_tracer || _bulletDistance <= 0.1f)
                return;
            float length = (drawStart - drawEnd).length;
            float val = 0f;
            float num1 = (float)(1.0 / (length / 8.0));
            float num2 = 0f;
            float num3 = 8f;
            while (true)
            {
                bool flag = false;
                if (val + num3 > length)
                {
                    num3 = length - Maths.Clamp(val, 0f, 99f);
                    flag = true;
                }
                num2 += num1;
                DuckGame.Graphics.DrawTexturedLine((Tex2D)_beem, drawStart + travelDirNormalized * val, drawStart + travelDirNormalized * (val + num3), Color.White * num2, _thickness, (Depth)0.6f);
                if (!flag)
                    val += 8f;
                else
                    break;
            }
        }
    }
}

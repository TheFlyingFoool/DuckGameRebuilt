// Decompiled with JetBrains decompiler
// Type: DuckGame.LaserBulletPurple
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class LaserBulletPurple : LaserBullet
    {
        public LaserBulletPurple(
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
            _beem = Content.Load<Texture2D>("laserBeamPurple");
        }
    }
}

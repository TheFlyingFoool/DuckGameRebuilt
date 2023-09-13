// Decompiled with JetBrains decompiler
// Type: DuckGame.GrenadeBullet
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class MagicalInkBullet : Bullet
    {
        public MagicalInkBullet(
          float xval,
          float yval,
          AmmoType type,
          float ang = -1f,
          Thing owner = null,
          bool rbound = false,
          float distance = -1f,
          bool tracer = true,
          bool network = true)
          : base(xval, yval, type, ang, owner, rbound, distance, tracer, network)
        {
        }

        protected override void OnHit(bool destroyed)
        {
            if (!destroyed || !isLocal)
                return;
            
            level.AddThing(new MagicalInkSplat(x, y));
        }
    }
}

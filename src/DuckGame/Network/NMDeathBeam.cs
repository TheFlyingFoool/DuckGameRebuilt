// Decompiled with JetBrains decompiler
// Type: DuckGame.NMDeathBeam
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMDeathBeam : NMEvent
    {
        public HugeLaser laser;
        public Vec2 position;
        public Vec2 target;

        public NMDeathBeam()
        {
        }

        public NMDeathBeam(HugeLaser pLaser, Vec2 pPosition, Vec2 pTarget)
        {
            position = pPosition;
            target = pTarget;
            laser = pLaser;
        }

        public override void Activate()
        {
            DeathBeam deathBeam = new DeathBeam(position, target)
            {
                isLocal = false
            };
            Level.Add(deathBeam);
            if (laser != null)
                laser.PostFireLogic();
            base.Activate();
        }
    }
}

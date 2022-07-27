// Decompiled with JetBrains decompiler
// Type: DuckGame.NMDeathBeam
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.position = pPosition;
            this.target = pTarget;
            this.laser = pLaser;
        }

        public override void Activate()
        {
            DeathBeam deathBeam = new DeathBeam(this.position, this.target)
            {
                isLocal = false
            };
            Level.Add(deathBeam);
            if (this.laser != null)
                this.laser.PostFireLogic();
            base.Activate();
        }
    }
}

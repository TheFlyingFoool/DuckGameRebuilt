using System;

namespace DuckGame
{
    [ClientOnly]
    public class NMDodgeBlockCurse : NMEvent
    {
        public NMDodgeBlockCurse(Duck d)
        {
            duck = d;
        }
        public NMDodgeBlockCurse()
        {
        }
        public Duck duck;

        public override void Activate()
        {
            if (duck.isServerForObject)
            {
                Vec2 v = duck.GetPos();
                DodgeblockCurse dc = new DodgeblockCurse((float)Math.Round(v.x / 16) * 16, (float)Math.Round(v.y / 16) * 16);
                dc.duck = duck;
                Level.Add(dc);
            }
        }
    }
}

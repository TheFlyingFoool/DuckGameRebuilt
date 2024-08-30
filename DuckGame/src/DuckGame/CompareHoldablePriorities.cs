using System.Collections.Generic;

namespace DuckGame
{
    public class CompareHoldablePriorities : IComparer<Holdable>
    {
        private Duck _duck;

        public  CompareHoldablePriorities(Duck d) => _duck = d;

        public int Compare(Holdable h1, Holdable h2)
        {
            if (h1 == h2)
                return 0;
            if (h1 is CTFPresent)
                return -1;
            if (h2 is CTFPresent)
                return 1;
            if (h1 is TrappedDuck)
                return -1;
            if (h2 is TrappedDuck || h1 is Equipment && _duck.HasEquipment(h1 as Equipment))
                return 1;
            if (h2 is Equipment && _duck.HasEquipment(h2 as Equipment))
                return -1;
            if (h1.PickupPriority() == h2.PickupPriority())
            {
                Vec2 vec2 = h1.position - _duck.position;
                double length1 = vec2.length;
                vec2 = h2.position - _duck.position;
                double length2 = vec2.length;
                return length1 - length2 < -2f ? -1 : 1;
            }
            return h1.PickupPriority() < h2.PickupPriority() ? -1 : 1;
        }
    }
}

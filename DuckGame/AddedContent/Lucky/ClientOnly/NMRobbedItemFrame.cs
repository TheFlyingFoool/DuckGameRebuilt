using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [ClientOnly]
    public class NMRobbedItemFrame : NMEvent
    {
        public NMRobbedItemFrame(ItemFrame f, Holdable h)
        {
            frame = f;
            holdable = h;
        }
        public NMRobbedItemFrame()
        {
        }
        public ItemFrame frame;
        public Holdable holdable;

        public override void Activate()
        {
            if (holdable != null)
            {
                holdable.solid = true;
                holdable.alpha = 1;
                holdable.enablePhysics = true;
                holdable = null;
            }
        }
    }
}

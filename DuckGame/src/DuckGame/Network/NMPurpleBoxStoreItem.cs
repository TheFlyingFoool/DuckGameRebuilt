namespace DuckGame
{
    public class NMPurpleBoxStoreItem : NMEvent
    {
        public Duck duck;
        public PhysicsObject thing;

        public NMPurpleBoxStoreItem()
        {
        }

        public NMPurpleBoxStoreItem(Duck pDuck, PhysicsObject pThing)
        {
            duck = pDuck;
            thing = pThing;
        }

        public override void Activate()
        {
            if (duck != null && thing != null && duck.profile != null)
                PurpleBlock.StoreItem(duck.profile, thing);
            base.Activate();
        }
    }
}

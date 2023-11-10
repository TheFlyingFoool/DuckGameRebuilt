namespace DuckGame
{
    public class NMUnequip : NMEvent
    {
        public Duck duck;
        public Equipment equipment;

        public NMUnequip()
        {
        }

        public NMUnequip(Duck pDuck, Equipment pEquipment)
        {
            duck = pDuck;
            equipment = pEquipment;
        }

        public override void Activate()
        {
            if (duck != null && equipment != null)
                duck.Unequip(equipment, true);
            base.Activate();
        }
    }
}

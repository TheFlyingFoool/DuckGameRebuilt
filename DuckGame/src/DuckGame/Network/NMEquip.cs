namespace DuckGame
{
    public class NMEquip : NMEvent
    {
        public Duck duck;
        public Equipment equipment;

        public NMEquip()
        {
        }

        public NMEquip(Duck pDuck, Equipment pEquipment)
        {
            duck = pDuck;
            equipment = pEquipment;
        }

        public override void Activate()
        {
            if (duck != null && equipment != null)
                duck.Equip(equipment, false, true);
            base.Activate();
        }
    }
}
